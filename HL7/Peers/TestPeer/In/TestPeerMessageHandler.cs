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
using ClearCanvas.HL7.In;
using ClearCanvas.Healthcare;
using NHapi.Base.Model;
using NHapi.Base.Parser;
using NHapi.Model.V25.Message;

namespace ClearCanvas.HL7.Peers.TestPeer.In
{
    public interface ITestPeerMessageHandler : IHL7MessageHandler
    {
    }

    [ExtensionPoint]
    public class TestPeerMessageHandlerExtensionPoint : ExtensionPoint<ITestPeerMessageHandler>
    {
    }

    public class ADT_A01_A04_A10_A14 : HL7MessageHandler, ITestPeerMessageHandler
    {
        #region ITestPeerMessageHandler Members

        public override IList<string> GetHandledMessageTypes()
        {
            return new List<string> {"ADT^A01", "ADT^A04", "ADT^A10", "ADT^A14"};
        }

        #endregion

        protected override void Handle()
        {
            var parser = new PipeParser();
            var parsedMessage = parser.Parse(Message.Text);
            var adtA01 = parsedMessage as ADT_A01;
            if (adtA01 == null) return;

            HandlePatient(adtA01.PID);
            HandleVisit(adtA01.PV1);
        }
    }

    public class ADTA05 : HL7MessageHandler, ITestPeerMessageHandler
    {
        #region ITestPeerMessageHandler Members

        public override IList<string> GetHandledMessageTypes()
        {
            return new List<string> {"ADT^A05"};
        }

        #endregion

        protected override void Handle()
        {
            var parser = new PipeParser();
            var parsedMessage = parser.Parse(Message.Text);
            var adtA05 = parsedMessage as ADT_A05;
            if (adtA05 == null) return;

            HandlePatient((adtA05).PID);
            HandleVisit((adtA05).PV1);
            if (Visit != null)
            {
                Visit.PreadmitNumber = (adtA05).PV1.PreadmitNumber.IDNumber.Value;
                Visit.Status = VisitStatus.PA;
            }
        }
    }

    public class OMIO23 : HL7MessageHandler, ITestPeerMessageHandler
    {
        #region ITestPeerMessageHandler Members

        public override IList<string> GetHandledMessageTypes()
        {
            return new List<string> {"OMI^O23"};
        }

        #endregion

        protected override void Handle()
        {
            var parser = new PipeParser();
            var parsedMessage = parser.Parse(Message.Text);
            var omi = parsedMessage as OMI_O23;
            if (omi == null) return;

            HandlePatient((omi.PATIENT.PID));
            HandleVisit(omi.PATIENT.PATIENT_VISIT.PV1);
            HandleOrders(omi.GetORDER(0).ORC, omi.GetORDER(0).OBR);
        }
    }
}