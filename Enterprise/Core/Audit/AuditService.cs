#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Macro.Common;
using Macro.Enterprise.Common.Audit;

namespace Macro.Enterprise.Core.Audit
{
	[ExtensionOf(typeof(CoreServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IAuditService))]
	public class AuditService : CoreServiceLayer, IAuditService
	{
		#region IAuditService Members

		[UpdateOperation(ChangeSetAuditable = false)]
		public WriteEntryResponse WriteEntry(WriteEntryRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.LogEntry, "LogEntry");

			AuditLogEntryAssembler assembler = new AuditLogEntryAssembler();
			AuditLogEntry logEntry = assembler.CreateAuditLogEntry(request.LogEntry);
			logEntry.ServerReceivedTimeStamp = Platform.Time;

			// save the log entry
			PersistenceContext.Lock(logEntry, DirtyState.New);

			return new WriteEntryResponse();
		}

		#endregion
	}
}
