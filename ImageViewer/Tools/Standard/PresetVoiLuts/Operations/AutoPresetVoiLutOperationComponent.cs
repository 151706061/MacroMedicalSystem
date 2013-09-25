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

namespace Macro.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
	public sealed class AutoPresetVoiLutOperationComponent : DefaultPresetVoiLutOperationComponent
	{
		public AutoPresetVoiLutOperationComponent() {}

		public override string Name
		{
			get { return SR.AutoPresetVoiLutOperationName; }
		}

		public override string Description
		{
			get { return SR.AutoPresetVoiLutOperationDescription; }
		}

		public override bool AppliesTo(IPresentationImage presentationImage)
		{
		    return AutoVoiLutApplicator.CanCreate(presentationImage);
		}

		public override void Apply(IPresentationImage presentationImage)
		{
            var applicator = AutoVoiLutApplicator.Create(presentationImage);
            if (applicator == null)
				throw new InvalidOperationException("The input presentation image is not supported.");

		    applicator.ApplyNextLut();
		}
	}
}