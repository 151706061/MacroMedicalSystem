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

#pragma warning disable 1591

namespace Macro.Common.Statistics
{
    /// <summary>
    /// Statistics to store the number of messages.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class CountStatistics : Statistics<uint>
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="CountStatistics"/> with unit "?"
        /// </summary>
        /// <param name="name"></param>
        public CountStatistics(string name)
            : base(name)
        {
            Unit = "";
        }

        /// <summary>
        /// Creates an instance of <see cref="CountStatistics"/> with specified name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public CountStatistics(string name, uint value)
            : this(name)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a copy of the original <see cref="CountStatistics"/>
        /// </summary>
        /// <param name="copy"></param>
        public CountStatistics(CountStatistics copy)
            : base(copy)
        {
        }

        #endregion Constructors

        #region Overridden Public Methods

        public override object Clone()
        {
            return new CountStatistics(this);
        }

        public override IAverageStatistics NewAverageStatistics()
        {
            return new AverageCountStatistics(this);
        }

        #endregion
    }
}