#region License

// Copyright (c) 2013, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This file is part of the Macro RIS/PACS open source project.
//
// The Macro RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The Macro RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the Macro RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Macro.Utilities.BuildTasks
{
    public class RelativeBuildDateTime : Microsoft.Build.Utilities.Task
    {
        //Outputs a Build Datetime that is a relative datetime from the date of Macro Inc incorporation
        //Build Datatime is the concatenation of <Number of days since November 3, 2005> and <Number of hours since midnight>

        public override bool Execute()
        {
            TimeSpan timeSince = DateTime.Now - _incorporationDate;
            int inHours = (int) timeSince.TotalHours;
            _buildNumber = inHours < UInt16.MaxValue ? inHours.ToString() : "0";

            return true;
        }

        [Output]
        public string BuildNumber
        {
            get { return _buildNumber; }
        }

        private static DateTime _incorporationDate = new DateTime(2005, 11, 3);
        private string _buildNumber;
    }
}
