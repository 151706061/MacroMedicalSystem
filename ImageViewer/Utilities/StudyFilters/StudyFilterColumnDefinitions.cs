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
using System.Reflection;
using Macro.Dicom;
using Macro.ImageViewer.Utilities.StudyFilters.Columns;

namespace Macro.ImageViewer.Utilities.StudyFilters
{
	partial class StudyFilterColumn
	{
		public abstract class ColumnDefinition
		{
			public readonly string Name;
			public readonly string Key;

			internal ColumnDefinition(string key, string name)
			{
				this.Key = key;
				this.Name = name;
			}

			public abstract StudyFilterColumn Create();

			public override bool Equals(object obj)
			{
				if (obj is ColumnDefinition)
					return this.Key == ((ColumnDefinition) obj).Key;
				return false;
			}

			public override int GetHashCode()
			{
				return 0x35522EF9 ^ this.Key.GetHashCode();
			}

			public override string ToString()
			{
				return this.Name;
			}
		}

		public static IEnumerable<ColumnDefinition> ColumnDefinitions
		{
			get
			{
				foreach (ColumnDefinition definition in SpecialColumnDefinitions)
					yield return definition;
				foreach (ColumnDefinition definition in DicomTagColumnDefinitions)
					yield return definition;
			}
		}

		public static ColumnDefinition GetColumnDefinition(string key)
		{
			// force initialize definition table
			StudyFilterColumn.SpecialColumnDefinitions.GetHashCode();

			if (_specialColumnDefinitions.ContainsKey(key))
				return _specialColumnDefinitions[key];

			// force initialize definition table
			StudyFilterColumn.DicomTagColumnDefinitions.GetHashCode();

			if (_dicomColumnDefinitions.ContainsKey(key))
				return _dicomColumnDefinitions[key];

			uint dicomTag;
			if (uint.TryParse(key, System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.InvariantCulture, out dicomTag))
				return StudyFilterColumn.GetColumnDefinition(dicomTag);

			return null;
		}

		public static StudyFilterColumn CreateColumn(string key)
		{
			ColumnDefinition definition = StudyFilterColumn.GetColumnDefinition(key);
			if (definition != null)
				return definition.Create();
			return null;
		}

		#region Special Columns

		private static Dictionary<string, ColumnDefinition> _specialColumnDefinitions;

		public static IEnumerable<ColumnDefinition> SpecialColumnDefinitions
		{
			get
			{
				if (_specialColumnDefinitions == null)
				{
					_specialColumnDefinitions = new Dictionary<string, ColumnDefinition>();

					SpecialColumnExtensionPoint xp = new SpecialColumnExtensionPoint();
					foreach (ISpecialColumn prototype in xp.CreateExtensions())
					{
						// not worried about the default constructor not existing, since it had to have one for CreateExtensions to work
						_specialColumnDefinitions.Add(prototype.Key, new SpecialColumnDefinition(prototype.Key, prototype.Name, prototype.GetType().GetConstructor(Type.EmptyTypes)));
					}
				}
				return _specialColumnDefinitions.Values;
			}
		}

		private class SpecialColumnDefinition : ColumnDefinition
		{
			private readonly ConstructorInfo _constructor;

			public SpecialColumnDefinition(string key, string name, ConstructorInfo constructor) : base(key, name)
			{
				_constructor = constructor;
			}

			public override StudyFilterColumn Create()
			{
				return (StudyFilterColumn) _constructor.Invoke(null);
			}
		}

		#endregion

		#region DICOM Tag Columns

		private static Dictionary<string, ColumnDefinition> _dicomColumnDefinitions;

		public static IEnumerable<ColumnDefinition> DicomTagColumnDefinitions
		{
			get
			{
				if (_dicomColumnDefinitions == null)
				{
					_dicomColumnDefinitions = new Dictionary<string, ColumnDefinition>();

					foreach (DicomTag dicomTag in DicomTagDictionary.GetDicomTagList())
					{
						if ((dicomTag.Group & 0xFFE1) != 0x6000)
						{
							ColumnDefinition definition = CreateDefinition(dicomTag);
							_dicomColumnDefinitions.Add(definition.Key, definition);
						}
						else
						{
							// the tag we're trying to add is in a repeating group (the overlays, group 6000)
							// so we replicate the tag for each of the other repeated groups (the even numbers from 6002-601E)
							for (uint n = 0; n <= 0x1E; n += 2)
							{
								DicomTag rDicomTag = new DicomTag(
									dicomTag.TagValue + n * 0x10000,
									string.Format(SR.FormatRepeatingDicomTagName, dicomTag.Name, 1 + n / 2), 
									string.Format("{0}{1:X2}", dicomTag.VariableName, n),
									dicomTag.VR, dicomTag.MultiVR, dicomTag.VMLow, dicomTag.VMHigh, dicomTag.Retired);
								ColumnDefinition rDefinition = CreateDefinition(rDicomTag);
								_dicomColumnDefinitions.Add(rDefinition.Key, rDefinition);
							}
						}
					}
				}
				return _dicomColumnDefinitions.Values;
			}
		}

		public static ColumnDefinition GetColumnDefinition(uint dicomTag)
		{
			DicomTag tag = DicomTagDictionary.GetDicomTag(dicomTag);
			if (tag == null)
				tag = new DicomTag(dicomTag, string.Empty, string.Empty, DicomVr.UNvr, false, uint.MinValue, uint.MaxValue, false);
			return GetColumnDefinition(tag);
		}

		public static ColumnDefinition GetColumnDefinition(DicomTag dicomTag)
		{
			// initialize defintion table
			DicomTagColumnDefinitions.GetHashCode();

			string key = dicomTag.TagValue.ToString("x8");

			if (_dicomColumnDefinitions.ContainsKey(key))
				return _dicomColumnDefinitions[key];

			return CreateDefinition(dicomTag);
		}

		private static ColumnDefinition CreateDefinition(DicomTag dicomTag)
		{
			switch (dicomTag.VR.Name)
			{
				case "AE":
				case "CS":
				case "LO":
				case "PN":
				case "SH":
				case "UI":
					// multi-valued strings
					return new StringDicomColumnDefintion(dicomTag);
				case "LT":
				case "ST":
				case "UT":
					// single-valued strings
					return new TextDicomColumnDefintion(dicomTag);
				case "IS":
				case "SL":
				case "SS":
					// multi-valued integers
					return new IntegerDicomColumnDefintion(dicomTag);
				case "UL":
				case "US":
					// multi-valued unsigned integers
					return new UnsignedDicomColumnDefintion(dicomTag);
				case "DS":
				case "FL":
				case "FD":
					// multi-valued floating-point numbers
					return new FloatingPointDicomColumnDefintion(dicomTag);
				case "DA":
				case "DT":
				case "TM":
					// multi-valued dates/times
					return new DateTimeDicomColumnDefintion(dicomTag);
				case "AS":
					// multi-valued time spans
					return new AgeDicomColumnDefintion(dicomTag);
				case "AT":
					// multi-valued DICOM tags
					return new AttributeTagDicomColumnDefintion(dicomTag);
				case "SQ":
				case "OB":
				case "OF":
				case "OW":
				case "UN":
				default:
					// sequence, binary and unknown data
					return new BinaryDicomColumnDefintion(dicomTag);
			}
		}

		#region Definitions

		public interface IDicomColumnDefinition
		{
			DicomTag Tag { get; }
		}

		private abstract class DicomColumnDefinition : ColumnDefinition, IDicomColumnDefinition
		{
			private readonly DicomTag _tag;

			protected DicomColumnDefinition(DicomTag tag)
				: base(tag.TagValue.ToString("x8"), string.Format(SR.FormatDicomTag, tag.Group, tag.Element, tag.Name))
			{
				_tag = tag;
			}

			public DicomTag Tag
			{
				get { return _tag; }
			}
		}

		private class StringDicomColumnDefintion : DicomColumnDefinition
		{
			public StringDicomColumnDefintion(DicomTag tag) : base(tag) {}

			public override StudyFilterColumn Create()
			{
				return new StringDicomTagColumn(Tag);
			}
		}

		private class IntegerDicomColumnDefintion : DicomColumnDefinition
		{
			public IntegerDicomColumnDefintion(DicomTag tag) : base(tag) {}

			public override StudyFilterColumn Create()
			{
				return new IntegerDicomTagColumn(Tag);
			}
		}

		private class UnsignedDicomColumnDefintion : DicomColumnDefinition
		{
			public UnsignedDicomColumnDefintion(DicomTag tag) : base(tag) {}

			public override StudyFilterColumn Create()
			{
				return new UnsignedDicomTagColumn(Tag);
			}
		}

		private class FloatingPointDicomColumnDefintion : DicomColumnDefinition
		{
			public FloatingPointDicomColumnDefintion(DicomTag tag) : base(tag) {}

			public override StudyFilterColumn Create()
			{
				return new FloatingPointDicomTagColumn(Tag);
			}
		}

		private class AgeDicomColumnDefintion : DicomColumnDefinition
		{
			public AgeDicomColumnDefintion(DicomTag tag) : base(tag) {}

			public override StudyFilterColumn Create()
			{
				return new AgeDicomTagColumn(Tag);
			}
		}

		private class DateTimeDicomColumnDefintion : DicomColumnDefinition
		{
			public DateTimeDicomColumnDefintion(DicomTag tag) : base(tag) {}

			public override StudyFilterColumn Create()
			{
				return new DateTimeDicomTagColumn(Tag);
			}
		}

		private class TextDicomColumnDefintion : DicomColumnDefinition
		{
			public TextDicomColumnDefintion(DicomTag tag) : base(tag) {}

			public override StudyFilterColumn Create()
			{
				return new TextDicomTagColumn(Tag);
			}
		}

		private class AttributeTagDicomColumnDefintion : DicomColumnDefinition
		{
			public AttributeTagDicomColumnDefintion(DicomTag tag) : base(tag) {}

			public override StudyFilterColumn Create()
			{
				return new AttributeTagDicomTagColumn(Tag);
			}
		}

		private class BinaryDicomColumnDefintion : DicomColumnDefinition
		{
			public BinaryDicomColumnDefintion(DicomTag tag) : base(tag) {}

			public override StudyFilterColumn Create()
			{
				return new BinaryDicomTagColumn(Tag);
			}
		}

		#endregion

		#endregion
	}
}