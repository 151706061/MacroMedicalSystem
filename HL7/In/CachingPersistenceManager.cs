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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.HL7.In
{
    public class CachingPersistenceManager : IDisposable
    {
        private readonly IPersistenceContext _persistenceContext;
        private readonly IList<LockableEntity<Visit>> _visits;
        private readonly IList<LockableEntity<Order>> _orders;
        private readonly IList<LockableEntity<Patient>> _patients;

        public CachingPersistenceManager(IPersistenceContext persistanceContext)
        {
            _persistenceContext = persistanceContext;
            _patients = new List<LockableEntity<Patient>>();
            _visits = new List<LockableEntity<Visit>>();
            _orders = new List<LockableEntity<Order>>();
        }

        public IPersistenceContext PersistenceContext
        {
            get { return _persistenceContext; }
        }

        public void Dispose()
        {
            Persist(_patients);
            Persist(_visits);
            Persist(_orders);
        }

        private void Persist<T>(IEnumerable<LockableEntity<T>> entities) where T : Entity
        {
            foreach (var ent in entities)
            {
                _persistenceContext.Lock(ent.Entity, ent.LockState);
            }
        }

        public Patient GetPatient(string mrn, string assigningAuthority, bool createIfNotExist)
        {
            Platform.CheckForEmptyString(mrn, "mrn");
            Platform.CheckForEmptyString(assigningAuthority, "assigningAuthority");
            var infoAuth =
                _persistenceContext.GetBroker<IEnumBroker>().TryFind<InformationAuthorityEnum>(assigningAuthority);
            Patient patient = null;
            var criteria = new PatientProfileSearchCriteria();
            criteria.Mrn.Id.EqualTo(mrn);
            criteria.Mrn.AssigningAuthority.EqualTo(infoAuth);

            try
            {
                var profile =
                    _persistenceContext.GetBroker<IPatientProfileBroker>().FindOne(criteria);
                patient = profile.Patient;
                _patients.Add(new LockableEntity<Patient>(patient, DirtyState.Dirty));
            }
            catch (EntityNotFoundException)
            {
                if (createIfNotExist)
                {
                    patient = new Patient();
                    _patients.Add(new LockableEntity<Patient>(patient, DirtyState.New));
                    var profile = new PatientProfile {Mrn = {Id = mrn, AssigningAuthority = infoAuth}};
                    patient.AddProfile(profile);
                }
            }

            return patient;
        }
        public Visit GetVisit(string visitNumber, string assigningAuthority, bool createIfNotExist,
                              out bool doneCreatedVisit)
        {
            doneCreatedVisit = false;
            Platform.CheckForEmptyString(visitNumber, "visitNumber");
            Platform.CheckForEmptyString(assigningAuthority, "assigningAuthority");
            var infoAuth =
                _persistenceContext.GetBroker<IEnumBroker>().TryFind<InformationAuthorityEnum>(assigningAuthority);
            Visit visit = null;
            var criteria = new VisitSearchCriteria();
            criteria.VisitNumber.Id.EqualTo(visitNumber);
            criteria.VisitNumber.AssigningAuthority.EqualTo(infoAuth);
            try
            {
                visit = _persistenceContext.GetBroker<IVisitBroker>().FindOne(criteria);
                _visits.Add(new LockableEntity<Visit>(visit, DirtyState.Dirty));
                //todo check that visit.Patient matches patient in _patients
            }
            catch (EntityNotFoundException)
            {
                if (createIfNotExist)
                {
                    visit = new Visit();
                    _visits.Add(new LockableEntity<Visit>(visit, DirtyState.New));
                    visit.VisitNumber.Id = visitNumber;
                    visit.VisitNumber.AssigningAuthority = infoAuth;
                    // facility will be handled by location handler, and yet we can't leave facility null.
                    //so, we simply pick any old facility for now
                    visit.Facility =
                        _persistenceContext.GetBroker<IFacilityBroker>().FindOne(new FacilitySearchCriteria());
                    doneCreatedVisit = true;
                }
            }
            return visit;
        }
        public Order GetOrderByPlacer(string placerNumber)
        {
            Platform.CheckForEmptyString(placerNumber, "placerNumber");
            Order order = null;
            var criteria = new OrderSearchCriteria();
            criteria.PlacerNumber.EqualTo(placerNumber);
            try
            {
                order = _persistenceContext.GetBroker<IOrderBroker>().FindOne(criteria);
                _orders.Add(new LockableEntity<Order>(order, DirtyState.Dirty));
            }
            catch (EntityNotFoundException)
            {
            }
            return order;
        }

        public Order GetOrderByPlacer(string placerNumber, string errorMessage)
        {
            var order = GetOrderByPlacer(placerNumber);
            if (order == null)
                throw new Exception(errorMessage);
            return order;
        }

        public void AddOrder(Order order)
        {
            _orders.Add(new LockableEntity<Order>(order, DirtyState.New));
        }

        public Order GetOrderByAccession(string accessionNumber)
        {
            Platform.CheckForEmptyString(accessionNumber, "accessionNumber");
            Order order = null;
            var criteria = new OrderSearchCriteria();
            criteria.AccessionNumber.EqualTo(accessionNumber);
            try
            {
                order = _persistenceContext.GetBroker<IOrderBroker>().FindOne(criteria);
                _orders.Add(new LockableEntity<Order>(order, DirtyState.Dirty));
            }
            catch (EntityNotFoundException)
            {
            }
            return order;
        }

        public Order GetOrderByAccession(string accessionNumber, string errorMessage)
        {
            var order = GetOrderByAccession(accessionNumber);
            if (order == null)
                throw new Exception(errorMessage);
            return order;
        }
        private class LockableEntity<T> where T : Entity
        {
            private readonly T _entity;
            private readonly DirtyState _lockState;

            public LockableEntity(T entity, DirtyState lockState)
            {
                _entity = entity;
                _lockState = lockState;
            }

            public T Entity
            {
                get { return _entity; }
            }

            public DirtyState LockState
            {
                get { return _lockState; }
            }
        }
    }
}