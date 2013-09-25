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

using ClearCanvas.Common;
using ClearCanvas.HL7.Out;
using ClearCanvas.Healthcare;

namespace ClearCanvas.HL7.Peers.TestPeer.Out
{
    [ExtensionOf(typeof (LogicalHL7EventListenerExtensionPoint))]
    public class TestPeerLogicalHL7EventRouter : LogicalHL7EventRouter<ITestPeerEventHandler, TestPeerEventHandlerExtensionPoint>
    {
    }
}