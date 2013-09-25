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
using System.Xml.Serialization;
using Macro.Common;
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Core.Edit;
using Macro.ImageServer.Model;

namespace Macro.ImageServer.Core.Data
{
    public class ProcessDuplicateChangeLog
    {
        #region Constructors

    	public ProcessDuplicateChangeLog()
    	{
    		TimeStamp = Platform.Time;
    	}

    	#endregion

        #region Public Properties

    	public DateTime TimeStamp { get; set; }

    	public ProcessDuplicateAction Action { get; set; }

    	public ImageSetDetails DuplicateDetails { get; set; }

    	public StudyInformation StudySnapShot { get; set; }

    	[XmlArray("StudyUpdateCommands")]
    	[XmlArrayItem("Command", Type = typeof (AbstractProperty<BaseImageLevelUpdateCommand>))]
    	public List<BaseImageLevelUpdateCommand> StudyUpdateCommands { get; set; }

        public string UserName { get; set; }

    	#endregion
    }
}