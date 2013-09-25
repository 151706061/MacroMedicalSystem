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

namespace Macro.Desktop.Validation
{
    /// <summary>
    /// Implements a validation strategy that does not consider any nodes.
    /// </summary>
    /// <remarks>
	/// This is effectively equivalent to having no validation at all.  The 
	/// container is always considered valid, regardless of the validity
	/// of contained nodes.
	/// </remarks>
    public class NoComponentsValidationStrategy : IApplicationComponentContainerValidationStrategy
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		public NoComponentsValidationStrategy()
		{
		}

    	#region IApplicationComponentContainerValidationStrategy Members

		/// <summary>
		/// Returns false.
		/// </summary>
        public bool HasValidationErrors(IApplicationComponentContainer container)
        {
            return false;
        }

		/// <summary>
		/// Does nothing.
		/// </summary>
        public void ShowValidation(IApplicationComponentContainer container, bool show)
        {
            // do nothing
        }

        #endregion
    }
}
