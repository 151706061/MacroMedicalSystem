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
using Macro.Common;
using Macro.Common.Utilities;

namespace Macro.Dicom.Utilities
{
	public class DicomTagPath : IEquatable<DicomTagPath>, IEquatable<DicomTag>, IEquatable<string>, IEquatable<uint>
	{
		private static readonly string _exceptionFormatInvalidTagPath = "The specified Dicom Tag Path is invalid: {0}.";

		private static readonly char[] _pathSeparator = new char[] { '\\' };
		private static readonly char[] _tagSeparator = new char[] { ',' };

		private List<DicomTag> _tags;
		private string _path;

		protected DicomTagPath()
			: this(new DicomTag[] {})
		{
		}

		public DicomTagPath(string path)
			: this(GetTags(path))
		{
		}

		public DicomTagPath(uint tag)
			: this(new []{ tag })
		{
		}

		public DicomTagPath(IEnumerable<uint> tags)
			: this(GetTags(tags))
		{
		}

		public DicomTagPath(DicomTag tag)
			: this(new []{ tag })
		{
		}

		public DicomTagPath(params uint[] tags)
            : this((IEnumerable<uint>)tags)
		{
		}

        public DicomTagPath(params DicomTag[] tags)
            : this((IEnumerable<DicomTag>)tags)
        {
        }

	    public DicomTagPath(IEnumerable<DicomTag> tags)
		{
			BuildPath(tags);
		}

		public virtual string Path
		{
			get { return _path; }
			protected set 
			{
				BuildPath(GetTags(value));
			}
		}

		public IList<DicomTag> TagsInPath
		{
			get { return _tags.AsReadOnly(); }
			protected set
			{
				BuildPath(value);
			}
		}

		public DicomVr ValueRepresentation
		{
			get { return _tags[_tags.Count - 1].VR; }	
		}

        public DicomTagPath UpOne()
        {
            if (_tags.Count == 1)
                throw new InvalidOperationException();

            var lessOne = new DicomTag[_tags.Count - 1];
            _tags.CopyTo(0, lessOne, 0, _tags.Count - 1);
            return new DicomTagPath(lessOne);
        }

	    public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is DicomTagPath)
				return Equals(obj as DicomTagPath);
			if (obj is DicomTag)
				return Equals(obj as DicomTag);
			if (obj is string)
				return Equals(obj as string);
			if (obj is uint)
				return Equals((uint)obj);

			return false;
		}

		#region IEquatable<DicomTagPath> Members

		public bool Equals(DicomTagPath other)
		{
			if (other == null)
				return false;

			return other.Path.Equals(Path);
		}

		#endregion	

		#region IEquatable<DicomTag> Members

		public bool Equals(DicomTag other)
		{
			if (other == null)
				return false;

			if (_tags.Count != 1)
				return false;

			return _tags[0].Equals(other);
		}

		#endregion

		#region IEquatable<string> Members

		public bool Equals(string other)
		{
			return Path.Equals(other);
		}

		#endregion

		#region IEquatable<uint> Members

		public bool Equals(uint other)
		{
			if (_tags.Count != 1)
				return false;

			return _tags[0].TagValue.Equals(other);
		}

		#endregion

		public override int GetHashCode()
		{
			return _path.GetHashCode();
		}

		public override string ToString()
		{
			return _path;
		}

		public static DicomTagPath operator +(DicomTagPath left, DicomTagPath right)
		{
			List<DicomTag> tags = new List<DicomTag>(left.TagsInPath);
			tags.AddRange(right.TagsInPath);
			return new DicomTagPath(tags);
		}

		public static DicomTagPath operator +(DicomTagPath left, DicomTag right)
		{
			List<DicomTag> tags = new List<DicomTag>(left.TagsInPath);
			tags.Add(right);
			return new DicomTagPath(tags);
		}

		public static DicomTagPath operator +(DicomTagPath left, uint right)
		{
			List<DicomTag> tags = new List<DicomTag>(left.TagsInPath);
			tags.Add(DicomTagDictionary.GetDicomTag(right));
			return new DicomTagPath(tags);
		}
		
		public static implicit operator DicomTagPath(DicomTag tag)
		{
			return new DicomTagPath(tag);
		}

		public static implicit operator DicomTagPath(uint tag)
		{
			return new DicomTagPath(tag);
		}

		/// <summary>
		/// Implicit cast to a String object, for ease of use.
		/// </summary>
		public static implicit operator string(DicomTagPath path)
		{
			return path.ToString();
		}

		private void BuildPath(IEnumerable<DicomTag> dicomTags)
		{
			Platform.CheckForNullReference(dicomTags, "dicomTags");
			_tags = new List<DicomTag>(dicomTags);
			_path = StringUtilities.Combine(dicomTags, "\\", delegate(DicomTag tag) { return String.Format("({0:x4},{1:x4})", tag.Group, tag.Element); });
		}

		private static IEnumerable<DicomTag> GetTags(string path)
		{
			Platform.CheckForEmptyString(path, "path");

			List<DicomTag> dicomTags = new List<DicomTag>();

			string[] groupElementValues = path.Split(_pathSeparator);

			foreach (string groupElement in groupElementValues)
			{
				string[] values = groupElement.Split(_tagSeparator);
				if (values.Length != 2)
					throw new ArgumentException(String.Format(_exceptionFormatInvalidTagPath, path));

				string group = values[0];
				if (!group.StartsWith("(") || group.Length != 5)
					throw new ArgumentException(String.Format(_exceptionFormatInvalidTagPath, path));

				string element = values[1];
				if (!element.EndsWith(")") || element.Length != 5)
					throw new ArgumentException(String.Format(_exceptionFormatInvalidTagPath, path));

				try
				{
					ushort groupValue = System.Convert.ToUInt16(group.TrimStart('('), 16);
					ushort elementValue = System.Convert.ToUInt16(element.TrimEnd(')'), 16);

					dicomTags.Add(NewTag(DicomTag.GetTagValue(groupValue, elementValue)));

				}
				catch
				{
					throw new ArgumentException(String.Format(_exceptionFormatInvalidTagPath, path));
				}
			}

			ValidatePath(dicomTags);

			return dicomTags;
		}

		private static void ValidatePath(IList<DicomTag> dicomTags)
		{
			for (int i = 0; i < dicomTags.Count - 1; ++i)
			{
				if (dicomTags[i].VR != DicomVr.SQvr)
					throw new ArgumentException("All but the last item in the path must have VR = SQ.");
			}
		}

		private static IEnumerable<DicomTag> GetTags(IEnumerable<uint> tags)
		{
			foreach (uint tag in tags)
				yield return NewTag(tag);
		}

		private static DicomTag NewTag(uint tag)
		{
			DicomTag returnTag = DicomTagDictionary.GetDicomTag(tag);
			if (returnTag == null)
				returnTag = new DicomTag(tag, "Unknown Tag", "UnknownTag", DicomVr.UNvr, false, 1, uint.MaxValue, false);

			return returnTag;
		}
	}
}
