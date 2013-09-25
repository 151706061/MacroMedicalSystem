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

using Macro.Dicom;

namespace Macro.ImageViewer.Utilities.StudyFilters.Columns
{
	public class BinaryDicomTagColumn : DicomTagColumn<DicomObject>, IGenericSortableColumn
	{
		public BinaryDicomTagColumn(DicomTag dicomTag) : base(dicomTag) {}

		public override DicomObject GetTypedValue(IStudyItem item)
		{
			DicomAttribute attribute = item[base.Tag];
			if (attribute == null)
				return new DicomObject(-1, base.VR);
			if (attribute.IsNull)
				return new DicomObject(0, base.VR);
			return new DicomObject(attribute.Count, base.VR);
		}

		public override bool Parse(string input, out DicomObject output)
		{
			output = DicomObject.Empty;
			return false;
		}

		public override int Compare(IStudyItem x, IStudyItem y)
		{
			return this.GetTypedValue(x).CompareTo(this.GetTypedValue(y));
		}
	}
}