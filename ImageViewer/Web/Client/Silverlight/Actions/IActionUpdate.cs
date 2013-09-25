#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using Macro.ImageViewer.Web.Client.Silverlight.AppServiceReference;

namespace Macro.ImageViewer.Web.Client.Silverlight.Actions
{
	public interface IActionUpdate
	{
		void Update(PropertyChangedEvent e);
	}
}
