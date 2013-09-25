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

namespace Macro.ImageViewer.StudyManagement
{
	/// <summary>
	/// An exception that is thrown when Sop validation fails.
	/// </summary>
	public class SopValidationException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SopValidationException"/>.
		/// </summary>
		public SopValidationException() {}

		/// <summary>
		/// Initializes a new instance of <see cref="SopValidationException"/> with the
		/// specified message.
		/// </summary>
		/// <param name="message"></param>
		public SopValidationException(string message) : base(message) {}

		/// <summary>
		/// Initializes a new instance of <see cref="SopValidationException"/> with the
		/// specified message and inner exception.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public SopValidationException(string message, Exception inner) : base(message, inner) { }
	}
}
