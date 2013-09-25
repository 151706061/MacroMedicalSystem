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
using Macro.Common;
using Macro.Dicom;
using Macro.ImageViewer.Annotations;
using Macro.ImageViewer.StudyManagement;

namespace Macro.ImageViewer.AnnotationProviders.Dicom
{
	internal class LateralityViewPositionAnnotationItem : AnnotationItem
	{
		private readonly bool _showLaterality;
		private readonly bool _showViewPosition;

		public LateralityViewPositionAnnotationItem(string identifier, bool showLaterality, bool showViewPosition)
			: base(identifier, new AnnotationResourceResolver(typeof(LateralityViewPositionAnnotationItem).Assembly))
		{
			Platform.CheckTrue(showViewPosition || showLaterality, "At least one of showLaterality and showViewPosition must be true.");

			_showLaterality = showLaterality;
			_showViewPosition = showViewPosition;
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			string nullString = SR.ValueNil;

			IImageSopProvider provider = presentationImage as IImageSopProvider;
			if (provider == null)
				return "";

			string laterality = null;
			if (_showLaterality)
			{
				laterality = provider.ImageSop.ImageLaterality;
				if (string.IsNullOrEmpty(laterality))
					laterality = provider.ImageSop.Laterality;
			}

			string viewPosition = null;
			if (_showViewPosition)
			{
				viewPosition = provider.ImageSop.ViewPosition;
				if (string.IsNullOrEmpty(viewPosition))
				{
					//TODO: later, we could translate to ACR MCQM equivalent, at least for mammo.
					DicomAttributeSQ codeSequence = provider.ImageSop[DicomTags.ViewCodeSequence] as DicomAttributeSQ;
					if (codeSequence != null && !codeSequence.IsNull && codeSequence.Count > 0)
						viewPosition = codeSequence[0][DicomTags.CodeMeaning].GetString(0, null);
				}
			}

			string str = "";
			if (_showLaterality && _showViewPosition)
			{
				if (string.IsNullOrEmpty(laterality))
					laterality = nullString;
				if (string.IsNullOrEmpty(viewPosition))
					viewPosition = nullString;

				if (laterality == nullString && viewPosition == nullString)
					str = ""; // if both parts are null then just show one hyphen (rather than -/-)
				else
					str = String.Format(SR.FormatLateralityViewPosition, laterality, viewPosition);
			}
			else if (_showLaterality)
			{
				str = laterality;
			}
			else if (_showViewPosition)
			{
				str = viewPosition;
			}

			return str;
		}
	}
}
