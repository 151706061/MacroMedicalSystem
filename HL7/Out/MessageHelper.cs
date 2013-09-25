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
using System.Globalization;
using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using NHapi.Model.V25.Datatype;
using NHapi.Model.V25.Message;
using NHapi.Model.V25.Segment;

namespace ClearCanvas.HL7.Out
{

    public class MessageHelper
    {
        private readonly string _sendingFacility;
        private readonly string _sendingApplication;
        private readonly string _receivingFacility;
        private readonly string _receivingApplication;

        public MessageHelper(string sendingApplication, string sendingFacility, string receivingApplication,
                                string receivingFacility)
        {
            _sendingApplication = sendingApplication;
            _sendingFacility = sendingFacility;
            _receivingApplication = receivingApplication;
            _receivingFacility = receivingFacility;
        }


        public virtual void SetMSH(MSH header, string messageCode, string triggerEvent)
        {
            // Populate the MSH Segment
            header.FieldSeparator.Value = ("|");
            header.EncodingCharacters.Value = "^~\\&";
            header.MessageType.MessageCode.Value = messageCode;
            header.MessageType.TriggerEvent.Value = triggerEvent;
            header.SendingApplication.NamespaceID.Value = _sendingApplication;
            header.SendingFacility.NamespaceID.Value = _sendingFacility;
            header.ReceivingApplication.NamespaceID.Value = _receivingApplication;
            header.ReceivingFacility.NamespaceID.Value = _receivingFacility;
            header.DateTimeOfMessage.Time.SetLongDate(Platform.Time);
            header.MessageControlID.Value = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            header.ProcessingID.ProcessingID.Value = "P"; //production
        }

        public virtual void SetPID(PID pid, PatientProfile profile)
        {
            pid.PatientID.IDNumber.Value = profile.Mrn.Id;
            pid.PatientID.AssigningAuthority.NamespaceID.Value = profile.Mrn.AssigningAuthority.Value;
            pid.GetAlternatePatientIDPID(0).IDNumber.Value = profile.Healthcard.Id;
            pid.GetAlternatePatientIDPID(0).AssigningAuthority.NamespaceID.Value =
                profile.Healthcard.AssigningAuthority.Value;
            pid.GetAlternatePatientIDPID(0).CheckDigit.Value = profile.Healthcard.VersionCode;

            pid.GetPatientName(0).GivenName.Value = profile.Name.GivenName;
            pid.GetPatientName(0).SecondAndFurtherGivenNamesOrInitialsThereof.Value =
                profile.Name.MiddleName;
            pid.GetPatientName(0).FamilyName.Surname.Value = profile.Name.FamilyName;
            pid.AdministrativeSex.Value = profile.Sex.ToString();


            SetPhone(pid, profile.CurrentHomePhone);
            SetPhone(pid, profile.CurrentWorkPhone);

            if (profile.CurrentHomeAddress != null)
            {
                pid.GetPatientAddress(0).StreetAddress.StreetOrMailingAddress.Value =
                    profile.CurrentHomeAddress.Street;
                pid.GetPatientAddress(0).City.Value = profile.CurrentHomeAddress.City;
                pid.GetPatientAddress(0).StateOrProvince.Value = profile.CurrentHomeAddress.Province;
                pid.GetPatientAddress(0).Country.Value = profile.CurrentHomeAddress.Country;
                pid.GetPatientAddress(0).ZipOrPostalCode.Value = profile.CurrentHomeAddress.PostalCode;
                pid.GetPatientAddress(0).AddressType.Value = "R";
            }

            if (profile.DateOfBirth.HasValue)
                pid.DateTimeOfBirth.Time.Value = profile.DateOfBirth.Value.ToString("yyyyMMdd");

            if (profile.PrimaryLanguage != null)
                pid.PrimaryLanguage.Identifier.Value = profile.PrimaryLanguage.Value;

            if (profile.Religion != null)
                pid.Religion.Identifier.Value = profile.Religion.Value;

            if (profile.TimeOfDeath.HasValue)
                pid.DateTimeOfBirth.Time.Value = profile.TimeOfDeath.Value.ToString("yyyyMMdd");

            pid.PatientDeathIndicator.Value = BoolToString(profile.DeathIndicator);
        }

        public virtual void SetPV1(PV1 pv1, Visit visit)
        {
            pv1.VisitNumber.IDNumber.Value = visit.VisitNumber.Id;
            pv1.ServicingFacility.Value = visit.Facility.Code;
            if (visit.PatientClass != null)
                pv1.PatientClass.Value = visit.PatientClass.Code;
            if (visit.AdmissionType != null)
                pv1.AdmissionType.Value = visit.AdmissionType.Code;
            if (visit.PatientType != null)
                pv1.PatientType.Value = visit.PatientType.Code;
            if (visit.CurrentLocation != null)
            {
                pv1.AssignedPatientLocation.PointOfCare.Value = visit.CurrentLocation.PointOfCare;
                pv1.AssignedPatientLocation.Room.Value = visit.CurrentRoom;
                pv1.AssignedPatientLocation.Bed.Value = visit.CurrentBed;
                pv1.AssignedPatientLocation.Facility.NamespaceID.Value = visit.CurrentLocation.Facility.Code;
                pv1.AssignedPatientLocation.Building.Value = visit.CurrentLocation.Building;
                pv1.AssignedPatientLocation.Floor.Value = visit.CurrentLocation.Floor;
            }

            foreach (var practitioner in visit.Practitioners)
            {
                SetPhysician(pv1, practitioner);
            }

            foreach (var property in visit.ExtendedProperties)
            {
                if (!property.Key.Contains("Physician")) continue;
                var role = property.Key.Substring(0, property.Key.IndexOf("Physician"));
                SetPhysician(pv1, role, property.Value);
            }

            pv1.VIPIndicator.Value = BoolToString(visit.VipIndicator);
            if (visit.AdmitTime.HasValue)
                pv1.AdmitDateTime.Time.Value = ExtractHL7DateTime(visit.AdmitTime);
            if (visit.DischargeTime.HasValue)
                pv1.GetDischargeDateTime(0).Time.Value = ExtractHL7DateTime(visit.DischargeTime);
            pv1.DischargeDisposition.Value = visit.DischargeDisposition;
            pv1.VisitIndicator.Value = "V";
        }


        private XCN ParseRole(string role, PV1 pv1)
        {
            XCN physician = null;
            switch (role.ToUpper())
            {
                case "RF":
                    physician = pv1.GetReferringDoctor(0);
                    break;
                case "AT":
                    physician = pv1.GetAttendingDoctor(0);
                    break;
                case "CN":
                    physician = pv1.GetConsultingDoctor(0);
                    break;
                case "AD":
                    physician = pv1.GetConsultingDoctor(0);
                    break;
            }
            return physician;
        }
        private string[] GetPhysicianDetails(ExternalPractitioner practitioner)
        {
            if (practitioner == null)
                return null;
            var details = new[]
                              {
                                  practitioner.LicenseNumber, practitioner.Name.FamilyName, practitioner.Name.GivenName,
                                  practitioner.Name.MiddleName
                              };
            return details;
        }

        private void SetPhysician(PV1 pv1, string role, ExternalPractitioner practitioner)
        {
            XCN physician = ParseRole(role, pv1);
            if (physician == null)
                return;
            SetPhysician(physician, GetPhysicianDetails(practitioner));
        }

        private void SetPhysician(PV1 pv1, VisitPractitioner practitioner)
        {
            SetPhysician(pv1, practitioner.Role.ToString(), practitioner.Practitioner);
        }
         private void SetPhysician(XCN physician,ExternalPractitioner practitioner)
         {
             SetPhysician(physician, GetPhysicianDetails(practitioner));
         }
        
        private void SetPhysician(XCN physician, string[] physicianDetails)
        {
            if (physicianDetails.Length == 4)
            {
                physician.IDNumber.Value = physicianDetails[0];
                physician.FamilyName.Surname.Value = physicianDetails[1];
                physician.GivenName.Value = physicianDetails[2];
                physician.SecondAndFurtherGivenNamesOrInitialsThereof.Value = physicianDetails[3];
            }
            else if (physicianDetails.Length == 3)
            {
                int id;
                if (Int32.TryParse(physicianDetails[0], out id))
                {
                    physician.IDNumber.Value = physicianDetails[0];
                    physician.FamilyName.Surname.Value = physicianDetails[1];
                    physician.GivenName.Value = physicianDetails[2];
                }
                else
                {
                    physician.FamilyName.Surname.Value = physicianDetails[0];
                    physician.GivenName.Value = physicianDetails[1];
                    physician.SecondAndFurtherGivenNamesOrInitialsThereof.Value = physicianDetails[2];
                }
            }
            else if (physicianDetails.Length == 2)
            {
                int id;
                if (Int32.TryParse(physicianDetails[0], out id))
                {
                    physician.IDNumber.Value = physicianDetails[0];
                    physician.FamilyName.Surname.Value = physicianDetails[1];
                }
                else
                {
                    physician.FamilyName.Surname.Value = physicianDetails[0];
                    physician.GivenName.Value = physicianDetails[1];
                }
            }
            else if (physicianDetails.Length == 1)
            {
                int id;
                if (Int32.TryParse(physicianDetails[0], out id))
                {
                    physician.IDNumber.Value = physicianDetails[0];
                }
                else
                {
                    physician.FamilyName.Surname.Value = physicianDetails[0];
                }
            }
        }

        private void SetPhysician(PV1 pv1, string role, string physicianInfo)
        {
            XCN physician = ParseRole(role, pv1);
            if (physician == null)
                return;

            SetPhysician(physician, physicianInfo.Split(new[] {','}));
        }

        public virtual void SetORC(ORC orc, Order order, Procedure procedure, string orderControlCode)
        {
            orc.OrderControl.Value = orderControlCode;
            orc.PlacerOrderNumber.EntityIdentifier.Value = procedure.Number;
            orc.PlacerGroupNumber.EntityIdentifier.Value = order.AccessionNumber;
            orc.OrderStatus.Value = order.Status.ToString();
            orc.DateTimeOfTransaction.Time.Value = ExtractHL7DateTime(DateTime.Now);
            SetPhysician(orc.GetOrderingProvider(0), order.OrderingPractitioner);
            orc.GetQuantityTiming(0).Priority.Value = order.Priority.ToString();
        }

        public virtual void SetTQ1(TQ1 tq1, Procedure procedure)
        {
            tq1.Quantity.Quantity.Value = "1"; //only one order per message
            tq1.StartDateTime.Time.Value = ExtractHL7DateTime(procedure.ScheduledStartTime);
        }

        public virtual void SetOBR(OBR obr, Order order, Procedure procedure)
        {
            obr.PlacerOrderNumber.EntityIdentifier.Value = procedure.Number;
            obr.UniversalServiceIdentifier.Identifier.Value = procedure.Type.Id;
            obr.UniversalServiceIdentifier.Text.Value = procedure.Type.Name;
            SetPhysician(obr.GetOrderingProvider(0), order.OrderingPractitioner);
            obr.RequestedDateTime.Time.Value = ExtractHL7DateTime(procedure.ScheduledStartTime);
        }


        private void SetPhone(PID pid, TelephoneNumber phone)
        {
            if (phone == null)
                return;

            pid.GetPhoneNumberHome(0).TelephoneNumber.Value = String.Format("({0}){1}", phone.AreaCode,
                                                                            phone.Number);
            pid.GetPhoneNumberHome(0).TelecommunicationUseCode.Value = phone.Use.ToString();
            pid.GetPhoneNumberHome(0).AreaCityCode.Value = phone.AreaCode;
            pid.GetPhoneNumberHome(0).Extension.Value = phone.Extension;
        }


        // REPORT   /////////////////////////////////////////////////////////////////
        public void SetReport(ORU_R01 oru, Order order, Report report)
        {
            if (report == null)
                return;

            var status = report.HasAddenda ? "C" : "F"; // corrected/final
            var time = report.HasAddenda ? ExtractHL7DateTime(report.CorrectedTime) : ExtractHL7DateTime(report.CompletedTime);
            oru.GetPATIENT_RESULT().GetORDER_OBSERVATION().OBR.ResultsRptStatusChngDateTime.Time.Value = time;
            oru.GetPATIENT_RESULT().GetORDER_OBSERVATION().OBR.ResultStatus.Value = status;

            var diagnosticServiceId = order.DiagnosticService.Id;
            var diagnosticServiceName = order.DiagnosticService.Name;
            SetOBX(oru,diagnosticServiceId, diagnosticServiceName, order.AccessionNumber, status, time);

            foreach (var part in report.Parts)
            {
                var label = part.IsAddendum ? "Addendum" : "Report";
                if (report.CompletedTime != null)
                    SetOBX(oru, diagnosticServiceId, diagnosticServiceName, label + report.Status + "--" + report.CompletedTime.Value.ToString("yyyy/MM/dd"), status, time);
                var reportContent = part.ExtendedProperties["ReportContent"];
                var reportContentSections = reportContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var section in reportContentSections)
                    SetOBX(oru,diagnosticServiceId, diagnosticServiceName, section, status, time);
            }
        }

        public void SetOrderNotes(ORU_R01 oru, Order order)
        {
            var i = 0;
            foreach (var orderNote in OrderNote.GetNotesForOrder(order, "General", false))
            {
                var nte = oru.GetPATIENT_RESULT().GetORDER_OBSERVATION().GetNTE(i);
                nte.SetIDNTE.Value = (i + 1).ToString(CultureInfo.InvariantCulture);
                nte.SourceOfComment.Value = "????";
                nte.GetComment(0).Value = orderNote.Body;
                i++;
            }
        }

        private void SetOBX(ORU_R01 oru, string identifier, string name, string observation, string status, string time)
        {
            var rep = oru.GetPATIENT_RESULT().GetORDER_OBSERVATION().OBSERVATIONRepetitionsUsed;
            var obx = oru.GetPATIENT_RESULT().GetORDER_OBSERVATION().GetOBSERVATION(rep).OBX;
            obx.SetIDOBX.Value = (rep + 1).ToString(CultureInfo.InvariantCulture);
            obx.ValueType.Value = "TX";
            obx.ObservationIdentifier.Identifier.Value = identifier;
            obx.ObservationIdentifier.Text.Value = name;
            //use text data type for OBX data
            var tx = new TX(oru);
            obx.GetObservationValue(0).Data = tx;
            tx.Value = observation;
            obx.ObservationResultStatus.Value = status;
            obx.DateTimeOfTheObservation.Time.Value = time;
        }
        ////////////////////////////////////////////////////////////////////////////////////////

        private string BoolToString(bool value)
        {
            return value ? "Y" : "N";
        }

        private string ExtractHL7DateTime(DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToString("yyyyMMddHHmm") : "";
        }
    
    
    }
}