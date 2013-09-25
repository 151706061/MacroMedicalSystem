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
using Macro.Dicom;
using Macro.Dicom.Utilities.Command;
using Macro.ImageServer.Core.Edit;

namespace Macro.ImageServer.Services.WorkQueue.ProcessDuplicate
{
    internal class UpdateDuplicateSopCommand : CommandBase
    {
        #region Private Members

        private readonly List<BaseImageLevelUpdateCommand> _commands;
        private readonly DicomFile _file;

        #endregion

        #region Constructors

        public UpdateDuplicateSopCommand(DicomFile file, List<BaseImageLevelUpdateCommand> commands)
            :base("Duplicate SOP demographic update command", true)
        {
            _file = file;
            _commands = commands;
        }

        #endregion

        #region Overridden Protected Methods

        protected override void OnExecute(CommandProcessor theProcessor)
        {
            if (_commands!=null)
            {
                foreach (BaseImageLevelUpdateCommand command in _commands)
                {
                    if (!command.Apply(_file))
                        throw new ApplicationException(
                            String.Format("Unable to update the duplicate sop. Command={0}", command));
                }
            }
            
        }

        
        protected override void OnUndo()
        {
        }

        #endregion

    }
}