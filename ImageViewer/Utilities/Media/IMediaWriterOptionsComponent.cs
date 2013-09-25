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
namespace Macro.ImageViewer.Utilities.Media
{
    using System.ComponentModel;

    public interface IMediaWriterOptionsComponent
    {
        event PropertyChangedEventHandler PropertyChanged;

        void Cancel();
        void Save();

        bool DeleteStagedFilesOnCompleted { get; set; }

        bool EjectMediaOnCompleted { get; set; }

        bool IncludeIdeographicNames { get; set; }

        bool IncludePhoneticNames { get; set; }

        bool IncludePortableWorkstation { get; set; }

        bool StageToTempFolder { get; set; }

        string UserStagingFolder { get; set; }

        bool VerifyMediaOnCompleted { get; set; }
    }
}

