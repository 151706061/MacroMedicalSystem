#region License

//HL7 Support for Clear Canvas RIS
//Copyright (C)  2012 Archibald Archibaldovitch

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.OrderEntry;
using NHapi.Model.V25.Datatype;
using NHapi.Model.V25.Segment;

namespace ClearCanvas.HL7.In
{
    public interface IHL7MessageHandler
    {
        void Handle(HL7Message msg);
        IList<string> GetHandledMessageTypes();
    }

    public abstract class HL7MessageHandler : IHL7MessageHandler
    {
        // inbound order control codes
        public const string NewOrderOrderControlCode = "NW";
        public const string OrderAcceptedOrderControlCode = "OK";
        public const string CancelRequestOrderControlCode = "CA";
        public const string DiscontinueRequestOrderControlCode = "DC";
        public const string StatusChangedOrderControlCode = "SC";
        public const string NumberAssignedOrderControlCode = "NA";
        public const string ModifyRequestedOrderControlCode = "XO";

        protected HL7Message Message;
        protected Patient Patient;
        protected Visit Visit;
        private CachingPersistenceManager _persistenceManager;

        public abstract IList<string> GetHandledMessageTypes();

        public void Handle(HL7Message msg)
        {
            Message = msg;
            using (_persistenceManager = new CachingPersistenceManager(PersistenceScope.CurrentContext))
            {
                Handle();
            }
        }

        protected abstract void Handle();


        protected IPersistenceContext PersistenceContext
        {
            get { return _persistenceManager != null ? _persistenceManager.PersistenceContext : null; }
        }

        protected static Address GetAddress(PID pid)
        {
            var address = new Address
                              {
                                  Street = pid.GetPatientAddress(0).StreetAddress.StreetName.Value,
                                  City = pid.GetPatientAddress(0).City.Value,
                                  Province = pid.GetPatientAddress(0).StateOrProvince.Value,
                                  Country = pid.GetPatientAddress(0).Country.Value,
                                  PostalCode = pid.GetPatientAddress(0).ZipOrPostalCode.Value,
                                  Type =
                                      (AddressType)
                                      Enum.Parse(typeof (AddressType), pid.GetPatientAddress(0).AddressType.Value),
                                  ValidRange = new DateTimeRange(
                                      ParseNullableDateTimeFromDateField(
                                          pid.GetPatientAddress(0).AddressValidityRange.RangeStartDateTime.Time.Value),
                                      ParseNullableDateTimeFromDateField(
                                          pid.GetPatientAddress(0).AddressValidityRange.RangeEndDateTime.Time.Value))
                              };

            return CheckValid(address) ? address : null;
        }


        protected static bool CheckValid(Address address)
        {
            return address.Street != null && address.City != null && address.Province != null && address.Country != null &&
                   address.PostalCode != null;
        }

        protected static ExternalPractitioner ParseOrderingPhysicianFromVisit(Visit visit)
        {
            ExternalPractitioner attending = null;
            ExternalPractitioner admitting = null;
            ExternalPractitioner referring = null;
            foreach (var p in visit.Practitioners.Where(p => !p.EndTime.HasValue))
            {
                switch (p.Role)
                {
                    case VisitPractitionerRole.RF:
                        referring = p.Practitioner;
                        break;
                    case VisitPractitionerRole.AT:
                        attending = p.Practitioner;
                        break;
                    case VisitPractitionerRole.AD:
                        admitting = p.Practitioner;
                        break;
                }
            }
            var orderingPhysician = attending ?? admitting ?? referring;
            if (orderingPhysician == null)
                throw new Exception("Couldn't find ordering physician");
            return orderingPhysician;
        }

        protected ExternalPractitioner GetOrderingPhysician(XCN physician)
        {
            return !string.IsNullOrEmpty(physician.IDNumber.Value)
                       ? FindPractitioner(physician)
                       : ParseOrderingPhysicianFromVisit(Visit);
        }

        protected static OrderPriority ParseOrderPriority(string s)
        {
            if (string.IsNullOrEmpty(s))
                return OrderPriority.R;
            try
            {
                var priority = (OrderPriority) Enum.Parse(typeof (OrderPriority), s);
                return priority;
            }
            catch (ArgumentException)
            {
                return OrderPriority.R;
            }
        }

        protected static Facility FindFacility(string facilityName, IPersistenceContext context)
        {
            var searchCriteria = new FacilitySearchCriteria();
            searchCriteria.Code.EqualTo(facilityName);
            searchCriteria.Deactivated.EqualTo(false);
            try
            {
                return context.GetBroker<IFacilityBroker>().FindOne(searchCriteria);
            }
            catch (EntityNotFoundException)
            {
                throw new Exception(string.Format("Could not find facility {0}", facilityName));
            }
        }

        protected static DiagnosticService FindDiagnosticService(string serviceNumber, IPersistenceContext context)
        {
            var searchCriteria = new DiagnosticServiceSearchCriteria();
            searchCriteria.Id.EqualTo(serviceNumber);
            searchCriteria.Deactivated.EqualTo(false);
            DiagnosticService diagnosticService;
            try
            {
                diagnosticService = context.GetBroker<IDiagnosticServiceBroker>().FindOne(searchCriteria);
            }
            catch (EntityNotFoundException)
            {
                throw new Exception(serviceNumber);
            }

            return diagnosticService;
        }

        protected static Staff FindStaff(string id, IPersistenceContext context)
        {
            var searchCriteria = new StaffSearchCriteria();
            searchCriteria.Id.EqualTo(id);
            try
            {
                return context.GetBroker<IStaffBroker>().FindOne(searchCriteria);
            }
            catch (EntityNotFoundException)
            {
                throw new Exception(id);
            }
        }

        #region Order

        protected virtual void HandleOrders(ORC orc, OBR obr)
        {
            switch (orc.OrderControl.Value)
            {
                case NewOrderOrderControlCode:
                    HandleNewOrder(orc, obr);
                    break;
                case CancelRequestOrderControlCode:
                case DiscontinueRequestOrderControlCode:
                    HandleCancelDiscontinueOrder(obr);
                    break;
                case StatusChangedOrderControlCode:
                    HandleStatusChanged(obr);
                    break;
                case NumberAssignedOrderControlCode:
                    HandleNumberAssigned(obr);
                    break;
            }
        }

        protected virtual void HandleNewOrder(ORC orc, OBR obr)
        {
            string placerNumber = obr.PlacerOrderNumber.EntityIdentifier.Value;
            var order = _persistenceManager.GetOrderByPlacer(placerNumber);

            if (order != null)
                throw new Exception("Cannot create order which already exists");

            var accessionNumber = PersistenceContext.GetBroker<IAccessionNumberBroker>().GetNext();
            var diagnosticService = FindDiagnosticService(obr.UniversalServiceIdentifier.Identifier.Value,
                                                          PersistenceContext);
            var requestedTime = ParseNullableDateTimeFromDateField(obr.RequestedDateTime.Time.Value);
            var orderingPhysician = GetOrderingPhysician(obr.GetOrderingProvider(0));
            var orderingFacility = FindFacility(orc.GetOrderingFacilityName(0).OrganizationName.Value,
                                                PersistenceContext);
            var orderingStaff = FindStaff(orc.GetOrderingProvider(0).IDNumber.Value, PersistenceContext);
            var priority = ParseOrderPriority(obr.PriorityOBR.Value);

            order = Order.NewOrder(
                new OrderCreationArgs(Platform.Time, orderingStaff, "HL7", accessionNumber,
                                      Patient, Visit, diagnosticService, obr.GetReasonForStudy(0).Text.Value,
                                      priority, orderingFacility, orderingFacility, requestedTime,
                                      orderingPhysician,
                                      new List<ResultRecipient>()),
                PersistenceContext.GetBroker<IProcedureNumberBroker>(),
                PersistenceContext.GetBroker<IDicomUidBroker>()
                );

            order.PlacerNumber = placerNumber;
            var familyPhysician = GetFamilyPhysicianRecipients(GetProfileForFacility(orderingFacility));
            if (familyPhysician != null)
                order.ResultRecipients.Add(familyPhysician);

            _persistenceManager.AddOrder(order);

            foreach (var procedure in order.Procedures)
            {
                procedure.CreateProcedureSteps();
                procedure.Schedule(requestedTime);
                procedure.PerformingDepartment = GetPerformingDepartment(procedure.PerformingFacility,
                                                                         obr.ProcedureCode.Identifier.Value);
            }
        }

        protected Department GetPerformingDepartment(Facility facility, string department)
        {
            if (facility == null || String.IsNullOrEmpty(department))
                return null;
            var criteria = new DepartmentSearchCriteria();
            criteria.Facility.EqualTo(facility);
            criteria.Name.EqualTo(department);
            try
            {
                return PersistenceContext.GetBroker<IDepartmentBroker>().FindOne(criteria);
            }
            catch (EntityNotFoundException)
            {
                return null;
            }
        }

        protected void HandleNumberAssigned(OBR obr)
        {
            var order = _persistenceManager.GetOrderByAccession(obr.FillerOrderNumber.NamespaceID.Value,
                                                                "Unable to update order");
            var operation = new AssignPlacerOrderNumberOperation();
            operation.Execute(order, obr.PlacerOrderNumber.EntityIdentifier.Value);
        }

        protected void HandleCancelDiscontinueOrder(OBR obr)
        {
            var cancelDiscontinueOperation = new CancelOrDiscontinueOrderOperation();
            cancelDiscontinueOperation.Execute(
                _persistenceManager.GetOrderByPlacer(obr.PlacerOrderNumber.EntityIdentifier.Value, "Unable to update order"),
                new OrderCancelInfo()
                );
        }

        protected void HandleStatusChanged(OBR obr)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Visit

        protected virtual void HandleVisit(PV1 pv1)
        {
            bool doneCreatedVisit;

            var visit = _persistenceManager.GetVisit(pv1.VisitNumber.IDNumber.Value,
                                                     pv1.VisitNumber.AssigningAuthority.NamespaceID.Value,
                                                     true,
                                                     out doneCreatedVisit);

            if (visit == null) return;

            visit.Patient = Patient;
            visit.PatientClass = TryFind<PatientClassEnum>(pv1.PatientClass.Value);
            visit.AdmissionType = TryFind<AdmissionTypeEnum>(pv1.AdmissionType.Value);
            visit.VipIndicator = StringToBool(pv1.VIPIndicator.Value);
            visit.PatientType = TryFind<PatientTypeEnum>(pv1.PatientType.Value);

            visit.AdmitTime = ParseNullableDateTimeFromDateField(pv1.AdmitDateTime.Time.Value);
            visit.DischargeTime = ParseNullableDateTimeFromDateField(pv1.GetDischargeDateTime(0).Time.Value);
            visit.InferVisitStatus();

            for (int i = 0; i < pv1.AmbulatoryStatusRepetitionsUsed; ++i)
            {
                visit.AmbulatoryStatuses.Add(TryFind<AmbulatoryStatusEnum>(pv1.GetAmbulatoryStatus(i).Value));
            }

            for (int i = 0; i < pv1.AttendingDoctorRepetitionsUsed; ++i)
            {
                HandlePractitioner(visit, pv1.GetAttendingDoctor(i), VisitPractitionerRole.AT);
            }

            for (int i = 0; i < pv1.ReferringDoctorRepetitionsUsed; ++i)
            {
                HandlePractitioner(visit, pv1.GetReferringDoctor(i), VisitPractitionerRole.RF);
            }

            for (int i = 0; i < pv1.ConsultingDoctorRepetitionsUsed; ++i)
            {
                HandlePractitioner(visit, pv1.GetConsultingDoctor(i), VisitPractitionerRole.CN);
            }

            for (int i = 0; i < pv1.AdmittingDoctorRepetitionsUsed; ++i)
            {
                HandlePractitioner(visit, pv1.GetConsultingDoctor(i), VisitPractitionerRole.AD);
            }

            HandlePatientLocation(visit, pv1);
            Visit = visit;
        }


        protected virtual void HandlePatientLocation(Visit visit, PV1 pv1)
        {
            HandlePatientLocation(Visit, pv1.AssignedPatientLocation, true);
        }

        protected void HandlePatientLocation(Visit visit, PL patientLocation, bool isCurrent)
        {
            var location = FindLocation(patientLocation);
            var visitLocation = new VisitLocation
                                    {
                                        Role = VisitLocationRole.CR,
                                        Location = location,
                                        Room = patientLocation.Room.Value,
                                        Bed = patientLocation.Bed.Value,
                                        StartTime = Platform.Time
                                    };

            if (isCurrent)
            {
                visit.Facility = location.Facility;
                visit.CurrentLocation = visitLocation.Location;
                visit.CurrentRoom = visitLocation.Room;
                visit.CurrentBed = visitLocation.Bed;
            }
            visit.Locations.Add(visitLocation);
            foreach (
                var existing in
                    visit.Locations.Where(
                        existing => existing != visitLocation && existing.Role == visitLocation.Role &&
                                    existing.EndTime.HasValue == false))
            {
                existing.EndTime = Platform.Time;
            }
        }

        protected virtual Location HandleLocationNotFound(PL patientLocation, EntityNotFoundException e)
        {
            string locationName = patientLocation.PointOfCare.Value;
            Facility facility;
            try
            {
                facility = FindFacility(patientLocation.Facility.NamespaceID.Value, PersistenceContext);
            }
            catch (Exception)
            {
                facility = null;
            }

            var location = new Location
                               {
                                   Id = locationName,
                                   Name = locationName,
                                   PointOfCare = locationName,
                                   Facility = facility,
                                   Deactivated = false,
                                   Description = "HL7"
                               };

            PersistenceContext.Lock(location, DirtyState.New);
            return location;
        }

        private Location FindLocation(PL patientLocation)
        {
            Location location;
            var locationName = patientLocation.PointOfCare.Value;
            var locationCriteria = new LocationSearchCriteria();
            if (!string.IsNullOrEmpty(locationName))
            {
                locationCriteria.Id.EqualTo(locationName);
            }
            locationCriteria.Deactivated.EqualTo(false);
            try
            {
                location = PersistenceContext.GetBroker<ILocationBroker>().FindOne(locationCriteria);
            }
            catch (EntityNotFoundException e)
            {
                location = HandleLocationNotFound(patientLocation, e);
            }
            return location;
        }

        public void HandlePractitioner(Visit visit, XCN physician, VisitPractitionerRole role)
        {
            string physicianIDNumber = physician.IDNumber.Value;
            string physicianFamilyName = physician.FamilyName.Surname.Value;
            string physicanGivenName = physician.GivenName.Value;
            string physicanMiddleName = physician.SecondAndFurtherGivenNamesOrInitialsThereof.Value;

            if (physicianIDNumber == null &&
                physicianFamilyName == null &&
                physicanGivenName == null &&
                physicanMiddleName == null)
            {
                return;
            }
            var vp = new VisitPractitioner
                         {
                             Role = role,
                             Practitioner = GetPractitioner(physicianIDNumber)
                                            ?? AddPractitioner(physicianIDNumber,
                                                               physicanGivenName,
                                                               physicianFamilyName),
                             StartTime = Platform.Time
                         };

            visit.Practitioners.Add(vp);
            foreach (var existing in visit.Practitioners)
            {
                if (existing != vp && existing.Role == vp.Role && existing.EndTime.HasValue == false)
                {
                    existing.EndTime = Platform.Time;
                }
            }
        }

        protected ExternalPractitioner FindPractitioner(XCN physician)
        {
            return GetPractitioner(physician.IDNumber.Value)
                   ?? AddPractitioner(physician.IDNumber.Value,
                                      physician.GivenName.Value,
                                      physician.FamilyName.Surname.Value);
        }

        protected ExternalPractitioner GetPractitioner(string practitionerId)
        {
            if (practitionerId == null)
                return null;
            ExternalPractitioner externalPractitioner = null;
            var searchCriteria = new ExternalPractitionerSearchCriteria();
            searchCriteria.LicenseNumber.EqualTo(practitionerId);
            searchCriteria.Deactivated.EqualTo(false);
            try
            {
                externalPractitioner = PersistenceContext.GetBroker<IExternalPractitionerBroker>().FindOne(searchCriteria);
            }
            catch (EntityNotFoundException)
            {
            }

            return externalPractitioner;
        }

        private ResultRecipient GetFamilyPhysicianRecipients(PatientProfile profile)
        {
            if (profile == null)
                return null;
            var familyPhysician = profile.FamilyPhysician;
            return familyPhysician != null
                       ? new ResultRecipient(familyPhysician.DefaultContactPoint, ResultCommunicationMode.ANY)
                       : null;
        }

        private PatientProfile GetProfileForFacility(Facility facility)
        {
            return Patient.Profiles.First(profile => profile.Mrn.AssigningAuthority == facility.InformationAuthority);
        }

        protected ExternalPractitioner AddPractitioner(string practitionerId, string givenName, string familyName)
        {
            var practitioner = GetPractitioner(practitionerId);
            if (practitioner != null) return practitioner;
            if (practitionerId == null || givenName == null || familyName == null)
            {
                throw new Exception("Practitioner is not valid");
            }

            var addedPractitioner = new ExternalPractitioner
                                        {
                                            LicenseNumber = practitionerId,
                                            Name = {FamilyName = familyName, GivenName = givenName}
                                        };
            addedPractitioner.ExtendedProperties.Add("Comment", "HL7");
            var contactPoint = new ExternalPractitionerContactPoint(addedPractitioner)
                                   {
                                       IsDefaultContactPoint = true,
                                       Name = "Default",
                                       Description = "HL7"
                                   };
            PersistenceContext.Lock(addedPractitioner, DirtyState.New);
            PersistenceContext.Lock(contactPoint, DirtyState.New);
            return addedPractitioner;
        }

        #endregion

        #region Patient

        protected virtual void HandlePatient(PID pid)
        {
            string mrn = pid.PatientID.IDNumber.Value;
            string assigningAuthority = pid.PatientID.AssigningAuthority.NamespaceID.Value;
            Patient = _persistenceManager.GetPatient(mrn, assigningAuthority, true);

            var patProfile = Patient.Profiles.FirstOrDefault(profile => profile.Mrn.Id == mrn);
            if (patProfile == null) return;

            patProfile.Mrn.AssigningAuthority =
                PersistenceContext.GetBroker<IEnumBroker>().Find<InformationAuthorityEnum>(assigningAuthority);

            patProfile.Healthcard.AssigningAuthority =
                PersistenceContext.GetBroker<IEnumBroker>().Find<InsuranceAuthorityEnum>(assigningAuthority);

            patProfile.Healthcard.Id = pid.GetAlternatePatientIDPID(0).IDNumber.Value;

            patProfile.Sex = (Sex) Enum.Parse(typeof (Sex), pid.AdministrativeSex.Value);
            patProfile.DateOfBirth = ParseDateTimeFromDateString(pid.DateTimeOfBirth.Time.Value);
            patProfile.DeathIndicator = StringToBool(pid.PatientDeathIndicator.Value);

            patProfile.Name.GivenName = pid.GetPatientName(0).GivenName.Value;
            patProfile.Name.FamilyName = pid.GetPatientName(0).FamilyName.Surname.Value;
            patProfile.Name.MiddleName = pid.GetPatientName(0).SecondAndFurtherGivenNamesOrInitialsThereof.Value;

            HandlePatientProfileAddress(pid, patProfile);
            HandlePatientProfileTelephoneNumber(GetTelephoneNumber(pid.GetPhoneNumberHome(0)), patProfile);
            HandlePatientProfileTelephoneNumber(GetTelephoneNumber(pid.GetPhoneNumberBusiness(0)), patProfile);
        }

        protected static void HandlePatientProfileTelephoneNumber(TelephoneNumber number, PatientProfile profile)
        {
            if (number == null) return;
            if (profile.TelephoneNumbers.Any(number.IsSameNumber))
                return;
            if (number.IsCurrent)
            {
                foreach (var existingNumber in profile.TelephoneNumbers)
                {
                    if (existingNumber.Equipment == number.Equipment &&
                        existingNumber.Use == number.Use &&
                        existingNumber.ValidRange.Until == null)
                    {
                        existingNumber.ValidRange.Until = number.ValidRange.From;
                    }
                }
            }
            profile.TelephoneNumbers.Add(number);
        }

        protected static TelephoneNumber GetTelephoneNumber(XTN phoneNumber)
        {
            var number = new TelephoneNumber
                             {
                                 Number = phoneNumber.TelephoneNumber.Value,
                                 Use = (TelephoneUse) Enum.Parse(typeof (TelephoneUse),
                                                                 phoneNumber.TelecommunicationUseCode.
                                                                     Value),
                                 AreaCode = phoneNumber.AreaCityCode.Value,
                                 Extension = phoneNumber.Extension.Value,
                                 ValidRange = new DateTimeRange(Platform.Time, null)
                             };

            return string.IsNullOrEmpty(number.Number) ? null : number;
        }

        protected static void HandlePatientProfileAddress(PID pid, PatientProfile patProfile)
        {
            var address = GetAddress(pid);
            if (address == null)
                return;
            if (patProfile.Addresses.Any(address.IsSameAddress))
                return;
            if (address.IsCurrent)
            {
                foreach (
                    var addr in
                        patProfile.Addresses.Where(addr => addr.Type == address.Type && addr.ValidRange.Until == null))
                {
                    addr.ValidRange.Until = address.ValidRange.From;
                }
            }

            patProfile.Addresses.Add(address);
        }

        #endregion

        #region Utility Methods

        protected TEnumValue TryFind<TEnumValue>(string code) where TEnumValue : EnumValue
        {
            return PersistenceContext.GetBroker<IEnumBroker>().TryFind<TEnumValue>(code);
        }

        protected static bool StringToBool(string value)
        {
            return !string.IsNullOrEmpty(value) && value.ToUpper().StartsWith("Y", true, null);
        }

        protected static DateTime? ParseNullableDateTimeFromDateField(string date)
        {
            if (date == null)
                return null;

            switch (date.Length)
            {
                case 8:
                    return new DateTime(
                        int.Parse(date.Substring(0, 4)),
                        int.Parse(date.Substring(4, 2)),
                        int.Parse(date.Substring(6, 2)));

                case 12:
                    return new DateTime(
                        int.Parse(date.Substring(0, 4)),
                        int.Parse(date.Substring(4, 2)),
                        int.Parse(date.Substring(6, 2)),
                        int.Parse(date.Substring(8, 2)),
                        int.Parse(date.Substring(10, 2)),
                        0);

                case 14:
                    return new DateTime(
                        int.Parse(date.Substring(0, 4)),
                        int.Parse(date.Substring(4, 2)),
                        int.Parse(date.Substring(6, 2)),
                        int.Parse(date.Substring(8, 2)),
                        int.Parse(date.Substring(10, 2)),
                        int.Parse(date.Substring(12, 2)));

                default:
                    throw new Exception(String.Format("Unable to parse date {0}", date));
            }
        }

        protected static DateTime ParseDateTimeFromDateString(string date)
        {
            if (string.IsNullOrEmpty(date))
                throw new Exception(String.Format("Unable to parse date {0}", date));

            switch (date.Length)
            {
                case 8:
                    return new DateTime(
                        int.Parse(date.Substring(0, 4)),
                        int.Parse(date.Substring(4, 2)),
                        int.Parse(date.Substring(6, 2)));

                case 12:
                    return new DateTime(
                        int.Parse(date.Substring(0, 4)),
                        int.Parse(date.Substring(4, 2)),
                        int.Parse(date.Substring(6, 2)),
                        int.Parse(date.Substring(8, 2)),
                        int.Parse(date.Substring(10, 2)),
                        0);

                case 14:
                    return new DateTime(
                        int.Parse(date.Substring(0, 4)),
                        int.Parse(date.Substring(4, 2)),
                        int.Parse(date.Substring(6, 2)),
                        int.Parse(date.Substring(8, 2)),
                        int.Parse(date.Substring(10, 2)),
                        int.Parse(date.Substring(12, 2)));

                default:
                    throw new Exception(String.Format("Unable to parse date {0}", date));
            }
        }

        #endregion
    }
}