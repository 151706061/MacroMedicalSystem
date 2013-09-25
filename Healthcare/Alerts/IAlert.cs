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

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Alerts
{
    public interface IAlert<TEntity>
    {
		/// <summary>
		/// Identifies this type of alert.
		/// </summary>
		string Id { get; }

        /// <summary>
        /// Test the entity for any alert conditions.  This method must be thread-safe
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="context"></param>
        /// <returns>NULL if the test does not trigger an alert </returns>
        AlertNotification Test(TEntity entity, IPersistenceContext context);
    }
}
