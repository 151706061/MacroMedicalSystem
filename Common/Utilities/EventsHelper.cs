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

namespace Macro.Common.Utilities
{
	/// <summary>
	/// Helper class for raising events.
	/// </summary>
	public class EventsHelper
	{
		/// <summary>
		/// Helper method for raising events safely.
		/// </summary>
		/// <param name="del">Delegate to invoke.</param>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/>.</param>
		/// <remarks>
		/// Use this method to invoke user code via delegates.
		/// This method will log any exceptions thrown in user code and immediately rethrow it.
		/// The typical usage is shown below.
		/// </remarks>
		/// <example>
		/// <code>
		/// [C#]
		/// public class PresentationImage
		/// {
		///    private event EventHandler _imageDrawingEvent;
		///    
		///    public void Draw()
		///    {
		///       EventsHelper.Fire(_imageDrawingEvent, this, new DrawImageEventArgs());
		///    }
		/// }
		/// </code>
		/// </example>
		public static void Fire(Delegate del, object sender, EventArgs e)
		{
			if (del == null)
				return;

			Delegate[] delegates = del.GetInvocationList();

			foreach(Delegate sink in delegates)
			{
				try
				{
					sink.DynamicInvoke(sender, e);
				}
				catch (Exception ex)
				{
                    Platform.Log(LogLevel.Error, ex);
					throw;
				}
			}
		}
	}
}
