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
using Macro.Common;
using Macro.ImageServer.Model;

namespace Macro.ImageServer.Services.WorkQueue.ReconcileStudyPostProcessing
{
	[Obsolete("This class is obsolete as of the 1.6 release.")]
    [ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
    class ReconcilePostProcessingProcessorFactoryExtension : IWorkQueueProcessorFactory
    {
        public WorkQueueTypeEnum GetWorkQueueType()
        {
            return WorkQueueTypeEnum.ReconcilePostProcess;
        }

        public IWorkQueueItemProcessor GetItemProcessor()
        {
            ReconcilePostProcessingProcessor processor = new ReconcilePostProcessingProcessor();
            processor.Name = WorkQueueTypeEnum.ReconcilePostProcess.ToString();
            return processor;
        }
    }
}
