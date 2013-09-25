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
using System.Collections.Generic;

namespace Macro.Desktop.Actions
{
    /// <summary>
    /// Default implementation of <see cref="IActionSet"/>.
    /// </summary>
    public class ActionSet : IActionSet
    {
        private readonly List<IAction> _actions;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActionSet()
            : this(null)
        {
        }

        /// <summary>
        /// Constructs an action set containing the specified actions.
        /// </summary>
        public ActionSet(IEnumerable<IAction> actions)
        {
            _actions = new List<IAction>();

            if(actions != null)
                _actions.AddRange(actions);
        }

        #region IActionSet members

        /// <summary>
        /// Returns a subset of this set containing only the elements for which the predicate is true.
        /// </summary>
        public IActionSet Select(Predicate<IAction> predicate)
        {
            List<IAction> subset = new List<IAction>();
            foreach (IAction action in _actions)
            {
                if (predicate(action))
                    subset.Add(action);
            }
            return new ActionSet(subset);
        }

        /// <summary>
        /// Gets the number of actions in the set.
        /// </summary>
        public int Count
        {
            get { return _actions.Count; }
        }

        /// <summary>
        /// Returns a set that corresponds to the union of this set with another set.
        /// </summary>
        public IActionSet Union(IActionSet other)
        {
            List<IAction> union = new List<IAction>();
            union.AddRange(this);
            union.AddRange(other);
            return new ActionSet(union);
        }

        #endregion

        #region IEnumerable<IAction> Members

		/// <summary>
		/// Gets an enumerator for the <see cref="IAction"/>s in the set.
		/// </summary>
        public IEnumerator<IAction> GetEnumerator()
        {
            return _actions.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

		/// <summary>
		/// Gets an enumerator for the <see cref="IAction"/>s in the set.
		/// </summary>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _actions.GetEnumerator();
        }

        #endregion

    }
}
