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

using Macro.Enterprise.Core;

namespace Macro.ImageServer.Enterprise
{
    /// <summary>
    /// Abstract base class to store select criteria for a broker implementing
    /// the <see cref="IEntityBroker{TServerEntity,TSelectCriteria,TUpdateColumns}"/> interface.
    /// </summary>
    public abstract class EntitySelectCriteria : SearchCriteria
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entityName">The name of the <see cref="ServerEntity"/> the criteria selects against.</param>
        public EntitySelectCriteria(string entityName)
            : base(entityName)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public EntitySelectCriteria()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected EntitySelectCriteria(EntitySelectCriteria other)
            : base(other)
        {
        }

    	/// <summary>
    	/// 
    	/// </summary>
    	/// <returns></returns>
    	public override abstract object Clone();
    }
}
