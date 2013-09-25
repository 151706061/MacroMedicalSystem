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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using NHapi.Base.Model;
using NHapi.Base.Parser;

namespace ClearCanvas.HL7.Out
{
    public interface ILogicalHL7EventHandler
    {
        void HandleEvent(LogicalHL7EventArgs args);
        string[] GetSupportedLogicalEventTypes();
    }
    public abstract class LogicalHL7EventHandler : ILogicalHL7EventHandler
    {
        private readonly string _authority;
        protected readonly string Peer;

        public const string NewOrderOrderControlCode = "NW";
        public const string OrderAcceptedOrderControlCode = "OK";
        public const string CancelOrderControlCode = "OC";
        public const string DiscontinueOrderControlCode = "OD";
        public const string StatusChangedOrderControlCode = "SC";
        public const string ModifyOrderControlCode = "XX";
        public const string ResultOrderControlCode = "RE";

        
        protected LogicalHL7EventHandler(string authority, string peer)
        {
            _authority = authority;
            Peer = peer;
        }

        #region ILogicalHL7EventHandler Members


        protected void EnqueueMessage(AbstractMessage message, string messageType)
        {
            var parsedMessageType = messageType.Split('^');
            string messageEvent = null;
            if (parsedMessageType.Length == 2)
                messageEvent = parsedMessageType[1];
            var queuedMessage = new HL7Message
            {
                CreationTime = Platform.Time,
                Direction = "O",
                Text = new PipeParser().Encode(message),
                Peer = Peer,
                Format = "ER7",
                Type = messageType,
                Event = messageEvent,
                HL7Version = message.Version,
                Status = "P"

            };
            PersistenceScope.CurrentContext.Lock(queuedMessage, DirtyState.New);
        }

        protected PatientProfile GetPatientProfile(Order order)
        {
            var profile = CollectionUtils.SelectFirst(order.Patient.Profiles, p => p.Mrn.AssigningAuthority.Code == _authority);
            if (profile == null)
                throw new Exception(string.Format("Unable to find patient profile in {0} for order accession number {1}",
                                                  _authority, order.AccessionNumber));
            return profile;
        }

        protected Visit GetVisit(PatientProfile patientProfile, Order order)
        {
            //if order originated from this authority, then just use visit in order
            if (_authority == order.OrderingFacility.InformationAuthority.Code)
                return order.Visit;

            //otherwise, find appropriate visit
            var criteria = new VisitSearchCriteria();
            criteria.Patient.EqualTo(order.Patient);
            criteria.AdmitTime.SortDesc(0);
            criteria.VisitNumber.AssigningAuthority.EqualTo(patientProfile.Mrn.AssigningAuthority);
            criteria.Status.In(new[] {VisitStatus.AA, VisitStatus.PD, VisitStatus.PA});
            return PersistenceScope.CurrentContext.GetBroker<IVisitBroker>().FindOne(criteria);

        }
        protected Order GetOrder(LogicalHL7EventArgs item)
        {
            var criteria = new OrderSearchCriteria();
            criteria.OID.EqualTo(item.OrderOID);
            return PersistenceScope.CurrentContext.GetBroker<IOrderBroker>().FindOne(criteria);
        }
        protected static Report GetReport(LogicalHL7EventArgs item)
        {
            var criteria = new ReportSearchCriteria();
            criteria.OID.EqualTo(item.ReportOID);
            return PersistenceScope.CurrentContext.GetBroker<IReportBroker>().FindOne(criteria);
        }

        #endregion

        public abstract void HandleEvent(LogicalHL7EventArgs args);
        public abstract string[] GetSupportedLogicalEventTypes();

        protected string GetOrderControlCode(string eventType)
        {
            switch (eventType)
            {
                case LogicalHL7Event.OrderCreatedEventType:
                case LogicalHL7Event.ProcedureCreatedEventType:
                    return NewOrderOrderControlCode;
                case LogicalHL7Event.OrderModifiedEventType:
                case LogicalHL7Event.ProcedureModifiedEventType:
                    return ModifyOrderControlCode;
                case LogicalHL7Event.OrderCancelledEventType:
                case LogicalHL7Event.ProcedureCancelledEventType:
                    return CancelOrderControlCode;
                case LogicalHL7Event.ReportPublishedEventType:
                    return ResultOrderControlCode;
            }
            return ModifyOrderControlCode;
        }
    }
}