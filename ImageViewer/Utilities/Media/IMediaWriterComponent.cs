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
using Macro.Common.Media.IMAPI2;

namespace Macro.ImageViewer.Utilities.Media
{
    using System.Collections.Generic;
    using Macro.Desktop.Trees;

    public interface IMediaWriterComponent 
    {
        void ClearStudies();
        void EjectMedia();
        void OpenOptions();
        void WriteMedia();
        void Cancel();
        void DetectMedia();
        void EreaseDisc();

        bool CanCancel { get; }

        bool CanWrite { get; }

        bool IsWriting { get; set; }

        string CurrentMediaDescription { get; }

        int CurrentMediaSpacePercent { get;  }

        string CurrentWriteStageName { get; set; }

        int CurrentWriteStagePercent { get; set; }

        bool EjectOnCompleted { get; }

        IList<IDiscRecorder2> MediaWriters { get; }

        string NumberOfStudies { get; }

        string RequiredMediaSpace { get; set; }

        IDiscRecorder2 SelectedMediaWriter { get; set; }

        string StagingFolderPath { get;}

        ITree Tree { get; set; }

        string VolumeName { get; set; }
    }
}

