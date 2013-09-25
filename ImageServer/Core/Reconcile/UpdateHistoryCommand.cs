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

using Macro.Dicom.Utilities.Command;
using Macro.Enterprise.Core;
using Macro.ImageServer.Common.Command;
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Core.Data;
using Macro.ImageServer.Core.Reconcile.CreateStudy;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;

namespace Macro.ImageServer.Core.Reconcile
{
    
	/// <summary>
	/// Command to update the study history record
	/// </summary>
	public class UpdateHistorySeriesMappingCommand : ServerDatabaseCommand
    {
        private readonly UidMapper _map;
        private readonly StudyHistory _studyHistory;
	    private readonly StudyStorageLocation _destStudy;

	    public UpdateHistorySeriesMappingCommand(StudyHistory studyHistory, StudyStorageLocation destStudy, UidMapper map) 
            : base("Update Study History Series Mapping")
        {
            _map = map;
            _studyHistory = studyHistory;
            _destStudy = destStudy;
        }

        protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
        {
            IStudyHistoryEntityBroker historyUpdateBroker = updateContext.GetBroker<IStudyHistoryEntityBroker>();
        	StudyHistoryUpdateColumns parms = new StudyHistoryUpdateColumns {DestStudyStorageKey = _destStudy.Key};

        	if (_map != null)
            {
                // replace the mapping in the history
                StudyReconcileDescriptor changeLog = XmlUtils.Deserialize<StudyReconcileDescriptor>(_studyHistory.ChangeDescription);
                changeLog.SeriesMappings = new System.Collections.Generic.List<SeriesMapping>(_map.GetSeriesMappings());
                parms.ChangeDescription = XmlUtils.SerializeAsXmlDoc(changeLog);
            }

            historyUpdateBroker.Update(_studyHistory.Key, parms);
        }
    }
}