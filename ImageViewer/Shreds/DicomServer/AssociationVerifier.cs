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

using Macro.Dicom.Network;
using Macro.ImageViewer.Common.ServerDirectory;
using LocalDicomServer = Macro.ImageViewer.Common.DicomServer.DicomServer;

namespace Macro.ImageViewer.Shreds.DicomServer
{
	internal static class AssociationVerifier
	{
		public static bool VerifyAssociation(IDicomServerContext context, AssociationParameters assocParms, out DicomRejectResult result, out DicomRejectReason reason)
		{
			string calledTitle = (assocParms.CalledAE ?? "").Trim();
			string callingAE = (assocParms.CallingAE ?? "").Trim();

			result = DicomRejectResult.Permanent;
			reason = DicomRejectReason.NoReasonGiven;

		    var extendedConfiguration = LocalDicomServer.GetExtendedConfiguration();
            if (!extendedConfiguration.AllowUnknownCaller && ServerDirectory.GetRemoteServersByAETitle(callingAE).Count == 0)
			{
				reason = DicomRejectReason.CallingAENotRecognized;
			}
			else if (calledTitle != context.AETitle)
			{
				reason = DicomRejectReason.CalledAENotRecognized;
			}
			else
			{
				return true;
			}

			return false;
		}
	}
}
