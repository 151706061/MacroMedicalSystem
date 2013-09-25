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

using Macro.Common.Statistics;

namespace Macro.ImageServer.Core.Edit
{
	/// <summary>
	/// Stores statistics of a WorkQueue instance processing.
	/// </summary>
	public class UpdateStudyStatistics : StatisticsSet
	{
		#region Constructors

		public UpdateStudyStatistics(string studyInstanceUid)
			: this("UpdateStudy", studyInstanceUid)
		{ }

		public UpdateStudyStatistics(string name, string studyInstanceUid)
			: base(name)
		{
			AddField("StudyInstanceUid", studyInstanceUid);
		}

		#endregion Constructors

		#region Public Properties

        
		public TimeSpanStatistics ProcessTime
		{
			get
			{
				if (this["ProcessTime"] == null)
					this["ProcessTime"] = new TimeSpanStatistics("ProcessTime");

				return (this["ProcessTime"] as TimeSpanStatistics);
			}
			set { this["ProcessTime"] = value; }
		}

		public ulong StudySize
		{
			set
			{
				this["StudySize"] = new ByteCountStatistics("StudySize", value);
			}
			get
			{
				if (this["StudySize"] == null)
					this["StudySize"] = new ByteCountStatistics("StudySize");

				return ((ByteCountStatistics)this["StudySize"]).Value;
			}
		}

		public int InstanceCount
		{
			set
			{
				this["InstanceCount"] = new Statistics<int>("InstanceCount", value);
			}
			get
			{
				if (this["InstanceCount"] == null)
					this["InstanceCount"] = new Statistics<int>("InstanceCount");

				return ((Statistics<int>)this["InstanceCount"]).Value;
			}
		}

		#endregion Public Properties
	}
}