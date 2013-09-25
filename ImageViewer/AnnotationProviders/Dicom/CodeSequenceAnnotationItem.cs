﻿#region License

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
using Macro.Common;
using Macro.Dicom;
using Macro.Dicom.Iod.Macros;
using Macro.ImageViewer.Annotations;
using Macro.ImageViewer.Annotations.Dicom;
using Macro.ImageViewer.StudyManagement;

namespace Macro.ImageViewer.AnnotationProviders.Dicom
{
	internal class CodeSequenceAnnotationItem : DicomAnnotationItem<string>
	{
		public CodeSequenceAnnotationItem(string identifier, IAnnotationResourceResolver resolver, uint codeSequenceTag)
			: this(identifier, resolver, codeSequenceTag, null) {}

		public CodeSequenceAnnotationItem(string identifier, IAnnotationResourceResolver resolver, uint codeSequenceTag, uint? descriptorTag)
			: base(identifier, resolver, new SopDataRetriever(codeSequenceTag, descriptorTag).RetrieveData, FormatResult) {}

		public CodeSequenceAnnotationItem(string identifier, string displayName, string label, uint codeSequenceTag)
			: this(identifier, displayName, label, codeSequenceTag, null) {}

		public CodeSequenceAnnotationItem(string identifier, string displayName, string label, uint codeSequenceTag, uint? descriptorTag)
			: base(identifier, displayName, label, new SopDataRetriever(codeSequenceTag, descriptorTag).RetrieveData, FormatResult) {}

		private static string FormatResult(string s)
		{
			return s;
		}

		private class SopDataRetriever
		{
			private readonly uint _codeSequenceTag;
			private readonly uint? _descriptorTag;

			public SopDataRetriever(uint codeSequenceTag, uint? descriptorTag)
			{
				_codeSequenceTag = codeSequenceTag;
				_descriptorTag = descriptorTag;
			}

			public string RetrieveData(Frame f)
			{
				try
				{
					var codeSequence = f.ParentImageSop[_codeSequenceTag] as DicomAttributeSQ;
					var codeSequenceItem = codeSequence != null && !codeSequence.IsEmpty && !codeSequence.IsNull && codeSequence.Count > 0 ? new CodeSequenceMacro(codeSequence[0]) : null;
					var descriptor = _descriptorTag.HasValue ? f.ParentImageSop[_descriptorTag.Value].ToString() : null;

					if (codeSequenceItem != null && !string.IsNullOrEmpty(codeSequenceItem.CodeMeaning))
						return codeSequenceItem.CodeMeaning;
					if (!string.IsNullOrEmpty(descriptor))
						return descriptor;
					if (codeSequenceItem != null && !string.IsNullOrEmpty(codeSequenceItem.CodeValue))
						return string.Format(SR.FormatCodeSequence, codeSequenceItem.CodeValue, codeSequenceItem.CodingSchemeDesignator);
					return string.Empty;
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Debug, ex, "Failed to parse code sequence attribute at tag ({0:X4},{1:X4})", (_codeSequenceTag >> 16) & 0x00FFFF, _codeSequenceTag & 0x00FFFF);
					return string.Empty;
				}
			}
		}
	}
}