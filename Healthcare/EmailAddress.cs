#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
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
using System.Collections;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using System.Collections.Generic;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// EmailAddress component
    /// </summary>
	public partial class EmailAddress
	{
        /// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		public bool IsCurrent
		{
			get { return this.ValidRange == null || this.ValidRange.Includes(Platform.Time); }
		}

        /// <summary>
        /// Returns true if the objects represent the same email address, regardless of validity range.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public bool IsSameEmailAddress(EmailAddress that)
        {
            return that != null && this._address == that._address;
        }
	}
}