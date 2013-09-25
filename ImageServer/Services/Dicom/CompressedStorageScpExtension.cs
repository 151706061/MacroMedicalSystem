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
using Macro.Common;
using Macro.Dicom;
using Macro.Dicom.Network;
using Macro.Dicom.Network.Scp;
using Macro.Enterprise.Core;
using Macro.ImageServer.Core;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.Brokers;
using Macro.ImageServer.Model.Parameters;

namespace Macro.ImageServer.Services.Dicom
{
    [ExtensionOf(typeof(DicomScpExtensionPoint<DicomScpContext>))]
    public class CompressedStorageScpExtension : StorageScp
    {
        #region Private Members
        private IList<SupportedSop> _sopList;
        private IList<PartitionTransferSyntax> _syntaxList;
        private readonly string _type = "Compressed C-STORE-RQ";

        public override string StorageScpType
        {
            get { return _type; }
        }

        #endregion
        
        #region Private Methods

        #endregion

        #region IDicomScp Members

        /// <summary>
        /// Returns a list of the services supported by this plugin.
        /// </summary>
        /// <returns></returns>
        public override IList<SupportedSop> GetSupportedSopClasses()
        {
            if (_sopList == null)
            {
                _sopList = new List<SupportedSop>();

                // Get the SOP Classes
                using (IReadContext read = _store.OpenReadContext())
                {
                    // Get the transfer syntaxes
                    _syntaxList = LoadTransferSyntaxes(read, Partition.GetKey(),true);
                }

                var partitionSopClassConfig = new PartitionSopClassConfiguration();
                var sopClasses = partitionSopClassConfig.GetAllPartitionSopClasses(Partition);
                if (sopClasses != null)
                {
                    // Now process the SOP Class List
                    foreach (PartitionSopClass partitionSopClass in sopClasses)
                    {
                        if (partitionSopClass.Enabled
                            && !partitionSopClass.NonImage)
                        {
                            SupportedSop sop = new SupportedSop();

                            sop.SopClass = SopClass.GetSopClass(partitionSopClass.SopClassUid);
                            foreach (PartitionTransferSyntax syntax in _syntaxList)
                            {
                                sop.SyntaxList.Add(TransferSyntax.GetTransferSyntax(syntax.Uid));
                            }
                            _sopList.Add(sop);
                        }
                    }
                }

            }
            return _sopList;
        }

        #endregion
    }
}
