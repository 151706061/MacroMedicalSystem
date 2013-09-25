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

using System.Xml.Serialization;
using Macro.Dicom;
using Macro.Dicom.Utilities.Command;
using Macro.ImageServer.Common.Helpers;

namespace Macro.ImageServer.Core.Edit
{
	/// <summary>
	/// Defines the interface of an <see cref="BaseImageLevelUpdateCommand"/> which modifies a tag in a Dicom file.
	/// </summary>
	public interface IUpdateImageTagCommand
	{
		/// <summary>
		/// Gets the <see cref="ImageLevelUpdateEntry"/> associated with the command
		/// </summary>
		ImageLevelUpdateEntry UpdateEntry { get; }

	}

	/// <summary>
	/// Encapsulates the tag update specification
	/// </summary>
	public class ImageLevelUpdateEntry
	{
		#region Private members

		public ImageLevelUpdateEntry()
		{
			TagPath = new DicomTagPath();
		}

		#endregion

		#region Public Properties

		public DicomTagPath TagPath { get; set; }

		/// <summary>
		/// Gets or sets the value of the tag to be updated
		/// </summary>
		public object Value { get; set; }

		/// <summary>
		/// Gets or sets the original value in the tag to be updated.
		/// </summary>
		public string OriginalValue { get; set; }


		/// <summary>
		/// Gets the value of the tag as a string 
		/// </summary>
		/// <returns></returns>
		public string GetStringValue()
	    {
	    	if (Value == null)
				return null;
	    	return Value.ToString();
	    }

		#endregion

	}

	public abstract class BaseImageLevelUpdateCommand : CommandBase, IUpdateImageTagCommand
	{
		protected BaseImageLevelUpdateCommand()
			: base("ImageLevelUpdateCommand", true)
		{
			UpdateEntry = new ImageLevelUpdateEntry();
		}

		protected BaseImageLevelUpdateCommand(string name)
			: base("ImageLevelUpdateCommand", true)
		{
			UpdateEntry = new ImageLevelUpdateEntry();
			CommandName = name;
		    Description = "Update Dicom Tag";
		}

	    #region IActionItem<DicomFile> Members

		public abstract bool Apply(DicomFile file);

		#endregion

		#region IImageLevelUpdateOperation Members

		[XmlIgnore]
		public string CommandName { get; set; }

		[XmlIgnore]
		public DicomFile File { private get; set; }

		/// <summary>
		/// Gets or sets the <see cref="ImageLevelUpdateEntry"/> for this command.
		/// </summary>
		[XmlIgnore]
		public ImageLevelUpdateEntry UpdateEntry { get; set; }

		#endregion

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			if (File != null)
				Apply(File);
		}

		protected override void OnUndo()
		{
			// NO-OP
		}
	}
}