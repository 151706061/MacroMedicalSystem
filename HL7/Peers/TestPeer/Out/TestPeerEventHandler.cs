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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.HL7.Out;
using ClearCanvas.Healthcare;
using NHapi.Model.V25.Message;

namespace ClearCanvas.HL7.Peers.TestPeer.Out
{
    public interface ITestPeerEventHandler : ILogicalHL7EventHandler
    {
    }

    [ExtensionPoint]
    public class TestPeerEventHandlerExtensionPoint : ExtensionPoint<ITestPeerEventHandler>
    {
    }

    abstract class TestPeerEventHandler : LogicalHL7EventHandler, ITestPeerEventHandler
    {
        protected TestPeerEventHandler()
            : base("InformationAuthority", "TestPeer")
        {
        }

        public override void HandleEvent(LogicalHL7EventArgs args)
        {
            Platform.Log(LogLevel.Info, "Handling Logical HL7 event {0} for {1}", args.EventType, Peer);
            var order = GetOrder(args);
            var patientProfile = GetPatientProfile(order);
            var visit = GetVisit(patientProfile, order);
            var applicableProcedures = args.IsProcedureEvent()
                ? CollectionUtils.Select(order.Procedures, orderProcedure => args.ProcedureOID.Equals(orderProcedure.OID))
                : (IEnumerable<Procedure>)order.Procedures;

            foreach (var procedure in applicableProcedures)
            {
                var message = new OMI_O23();
                var helper = new MessageHelper("CCRis", "CCRisFacility", "TestPeer", "TestPeerFacility");
                helper.SetMSH(message.MSH, "OMI", "023");
                helper.SetOBR(message.GetORDER().OBR, order, procedure);
                helper.SetORC(message.GetORDER().ORC, order, procedure, GetOrderControlCode(args.EventType));
                helper.SetPID(message.PATIENT.PID, patientProfile);
                helper.SetPV1(message.PATIENT.PATIENT_VISIT.PV1, visit);
                helper.SetTQ1(message.GetORDER().GetTIMING().TQ1, procedure);

                EnqueueMessage(message,"OMI^023");
            }
        }

        abstract public override string[] GetSupportedLogicalEventTypes();
    }


    [ExtensionOf(typeof(TestPeerEventHandlerExtensionPoint))]
    class OrderEventHandler : TestPeerEventHandler
    {
        //public, parameter-less constructor, needed for object instantiation
        public OrderEventHandler()
        {
            
        }
        public override string[] GetSupportedLogicalEventTypes()
        {
            return new [] { LogicalHL7Event.OrderCreatedEventType,
                            LogicalHL7Event.ProcedureCreatedEventType,
                            LogicalHL7Event.OrderModifiedEventType,
                            LogicalHL7Event.ProcedureModifiedEventType,
                            LogicalHL7Event.OrderCancelledEventType, 
                            LogicalHL7Event.ProcedureCancelledEventType
            
                         };
        }
    } 

    [ExtensionOf(typeof(TestPeerEventHandlerExtensionPoint))]
    class ReportPublishedEventHandler : TestPeerEventHandler
    {
        //public, parameter-less constructor, needed for object instantiation
        public ReportPublishedEventHandler()
        {
            
        }

        public override void HandleEvent(LogicalHL7EventArgs args)
        {
            Platform.Log(LogLevel.Info, "Handling Logical HL7 event {0} for {1}", args.EventType, Peer);
            var order = GetOrder(args);
            var patientProfile = GetPatientProfile(order);
            var visit = GetVisit(patientProfile, order);
            var report = args.IsReportEvent() ? GetReport(args) : null;
            if (report == null)
            {
                //should log this weird condtion
                return;
            }

            var applicableProcedures = args.IsProcedureEvent()
                ? CollectionUtils.Select(report.Procedures, orderProcedure => args.ProcedureOID.Equals(orderProcedure.OID))
                : (IEnumerable<Procedure>)order.Procedures;

            foreach (var procedure in applicableProcedures)
            {
                var message = new ORU_R01();
                var helper = new MessageHelper("CCRis", "CCRisFacility", "TestPeer", "TestPeerFacility");
                helper.SetMSH(message.MSH, "ORU", "R01");
                helper.SetOBR(message.GetPATIENT_RESULT().GetORDER_OBSERVATION().OBR, order, procedure);
                helper.SetORC(message.GetPATIENT_RESULT().GetORDER_OBSERVATION().ORC,
                                                           order, procedure, GetOrderControlCode(args.EventType));
                helper.SetPID(message.GetPATIENT_RESULT().PATIENT.PID, patientProfile);
                helper.SetPV1(message.GetPATIENT_RESULT().PATIENT.VISIT.PV1, visit);
                helper.SetTQ1(message.GetPATIENT_RESULT().GetORDER_OBSERVATION().GetTIMING_QTY().TQ1, procedure);
                helper.SetOrderNotes(message, order);
                helper.SetReport(message, order, report);
                EnqueueMessage(message, "ORU^R01");
            }
        }


        public override string[] GetSupportedLogicalEventTypes()
        {
            return new[] { LogicalHL7Event.ReportPublishedEventType };
        }
    }

  

}
