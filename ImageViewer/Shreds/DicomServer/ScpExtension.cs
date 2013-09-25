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

using System.Collections.Generic;
using Macro.Dicom;
using Macro.Dicom.Network;
using Macro.Dicom.Network.Scp;
using Macro.ImageViewer.Common.StudyManagement;

namespace Macro.ImageViewer.Shreds.DicomServer
{
	public interface IDicomServerContext
	{
		string AETitle { get; }
		string Host { get; }
		int Port { get; }
	    StorageConfiguration StorageConfiguration { get; }
	}

	public abstract class ScpExtension : IDicomScp<IDicomServerContext>
	{
		private IDicomServerContext _context;
		private readonly List<SupportedSop> _supportedSops;

		protected ScpExtension(IEnumerable<SupportedSop> supportedSops)
		{
			_supportedSops = new List<SupportedSop>(supportedSops);
		}

		protected IDicomServerContext Context
		{
			get { return _context; }
		}

		#region IDicomScp<ServerContext> Members

		public DicomPresContextResult VerifyAssociation(AssociationParameters association, byte pcid)
		{
			DicomRejectResult result;
			DicomRejectReason reason;
			if (!AssociationVerifier.VerifyAssociation(Context, association, out result, out reason))
				return DicomPresContextResult.RejectUser;

			return OnVerifyAssociation(association, pcid);
		}

		public virtual DicomPresContextResult OnVerifyAssociation(AssociationParameters association, byte pcid)
		{
			return DicomPresContextResult.Accept;
		}

		public abstract bool OnReceiveRequest(Macro.Dicom.Network.DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message);

		public IList<SupportedSop> GetSupportedSopClasses()
		{
			return _supportedSops;
		}

		public void SetContext(IDicomServerContext context)
		{
			_context = context;	
		}

	    public virtual void Cleanup()
	    {
	        
	    }

	    #endregion
	}
}
