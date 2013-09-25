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

namespace Macro.Enterprise.Core
{
	public interface IPersistenceBrokerFactory
	{
		/// <summary>
		/// Returns a broker that implements the specified interface to retrieve data into this persistence context.
		/// </summary>
		/// <typeparam name="TBrokerInterface">The interface of the broker to obtain</typeparam>
		/// <returns></returns>
		TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker;

		/// <summary>
		/// Returns a broker that implements the specified interface to retrieve data into this persistence context.
		/// </summary>
		/// <param name="brokerInterface"></param>
		/// <returns></returns>
		object GetBroker(Type brokerInterface);
	}
}
