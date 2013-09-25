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
	public class FloatingPointDicomTagColumn : DicomTagColumn<DicomArray<double>>, INumericSortableColumn
	{
		public FloatingPointDicomTagColumn(DicomTag dicomTag) : base(dicomTag) {}

		public override DicomArray<double> GetTypedValue(IStudyItem item)
		{
			DicomAttribute attribute = item[base.Tag];

			if (attribute == null)
				return null;
			if (attribute.IsNull)
				return new DicomArray<double>();

			double?[] result;
			try
			{
				result = new double?[CountValues(attribute)];
				for (int n = 0; n < result.Length; n++)
				{
					double value;
					if (attribute.TryGetFloat64(n, out value))
						result[n] = value;
				}
			}
			catch (DicomException)
			{
				return null;
			}
			return new DicomArray<double>(result);
		}

		public override bool Parse(string input, out DicomArray<double> output)
		{
			return DicomArray<double>.TryParse(input, double.TryParse, out output);
		}

		public override int Compare(IStudyItem x, IStudyItem y)
		{
			return this.CompareNumerically(x, y);
		}

		public int CompareNumerically(IStudyItem x, IStudyItem y)
		{
			return DicomArray<double>.Compare(this.GetTypedValue(x), this.GetTypedValue(y));
		}
	}
}