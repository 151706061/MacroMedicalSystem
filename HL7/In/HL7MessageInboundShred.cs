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
using ClearCanvas.Common.Shreds;

namespace ClearCanvas.HL7.In
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class HL7MessageInboundShred : QueueProcessorShred, IApplicationRoot
	{
        public HL7MessageInboundShred()
		{
		}

		public override string GetDisplayName()
		{
            return ShredSettings.Default.ShredName;
		}

		public override string GetDescription()
		{
			return ShredSettings.Default.ShredDescription;
		}

		protected override IList<QueueProcessor> GetProcessors()
		{
			var p = new HL7MessageInboundProcessor(
				ShredSettings.Default.BatchSize,
				TimeSpan.FromSeconds(ShredSettings.Default.EmptyQueueSleepTime));
			return new QueueProcessor[] { p };
		}

		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			Start();
		}

		#endregion
	}
}