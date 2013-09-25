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

using Macro.Common;

namespace Macro.ImageServer.Core.Reconcile.ProcessAsIs
{
	/// <summary>
	/// A processor implementing <see cref="IReconcileProcessor"/> to handle "MergeStudy" operation
	/// </summary>
    internal class ReconcileProcessAsIsProcessor : ReconcileProcessorBase, IReconcileProcessor
	{
		public ReconcileProcessAsIsProcessor()
			: base("Process As Is Processor")
		{

		}

		#region IReconcileProcessor Members

		public void Initialize(ReconcileStudyProcessorContext context, bool complete)
		{
			Platform.CheckForNullReference(context, "context");
			Context = context;

			ProcessAsIsCommand command = new ProcessAsIsCommand(Context, complete);

			AddCommand(command);

            if (complete)
            {
                AddCleanupCommands();
            }
		}

		#endregion
	}
}