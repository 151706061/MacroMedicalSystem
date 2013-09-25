#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using Macro.Common;
using Macro.ImageViewer.Web.Common;
using Macro.ImageViewer.Web.Common.Entities;
using Macro.ImageViewer.Web.Common.Events;
using Macro.ImageViewer.Web.Common.Messages;
using Macro.Web.Common;
using Macro.Web.Common.Events;
using Macro.Web.Common.Messages;
using Macro.Web.Services;

namespace Macro.ImageViewer.Web
{
	//TODO (CR May 2010): use attributes, scan types.

	[ExtensionOf(typeof (ServiceKnownTypeExtensionPoint))]
	public class KnownEventTypeProvider : ExtensionPoint<IServiceKnownTypeProvider>, IServiceKnownTypeProvider
	{
		#region IServiceKnownTypeProvider Members

		public IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider ignore)
		{
			return new[]
			       	{
						typeof(ApplicationNotFoundEvent),
						typeof(ApplicationStartedEvent),
						typeof(ApplicationStoppedEvent),
						typeof(EntityUpdatedEvent),
						typeof(PropertyChangedEvent),

			       		typeof (ContextMenuEvent),
			       		typeof (TileUpdatedEvent),
			       		typeof (SessionUpdatedEvent),
			       		typeof (MessageBoxShownEvent),
                        typeof(MouseMoveProcessedEvent)
			       	};
		}

		#endregion
	}

	[ExtensionOf(typeof(ServiceKnownTypeExtensionPoint))]
	public class KnownMessageTypeProvider : ExtensionPoint<IServiceKnownTypeProvider>, IServiceKnownTypeProvider
	{
		#region IServiceKnownTypeProvider Members

		public IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider ignore)
		{
			return new[]
			       	{
						typeof (ActionClickedMessage),
						typeof (SetLayoutActionMessage),
			       		typeof (MouseMessage),
			       		typeof (MouseMoveMessage),
			       		typeof (MouseWheelMessage),
                        typeof (DismissMessageBoxMessage),

                        typeof(UpdatePropertyMessage),
					};
		}

		#endregion
	}

	[ExtensionOf(typeof(ServiceKnownTypeExtensionPoint))]
	public class KnownEntityTypeProvider : ExtensionPoint<IServiceKnownTypeProvider>, IServiceKnownTypeProvider
	{
		#region IServiceKnownTypeProvider Members

		public IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider ignore)
		{
			return new[]
			       	{
			       		typeof (Viewer),
			       		typeof (Common.Entities.ImageBox),
			       		typeof (Common.Entities.Tile),
			       		
						typeof (Common.Entities.ImageBox[]),
			       		typeof (Common.Entities.Tile[]),
						typeof(Size),
						typeof(Rectangle),
						typeof(Position),
						typeof(Cursor),
						typeof(Common.Entities.InformationBox),
						typeof(MessageBox),
                        typeof(WebMessageBoxActions),

						//TODO: if we ever include the desktop stuff, move this out of the viewer namespace
						typeof(WebActionNode[]),
						typeof(WebIconSet),
                        typeof(WebIconSize),
						typeof (WebActionNode),
			       		typeof (WebAction),
			       		typeof (WebClickAction),
			       		typeof (WebDropDownButtonAction),
			       		typeof (WebDropDownAction),
			       		typeof (WebLayoutChangerAction)
			       	};
		}

		#endregion
	}

	[ExtensionOf(typeof(ServiceKnownTypeExtensionPoint))]
	public class KnownStartApplicationRequestTypeProvider : ExtensionPoint<IServiceKnownTypeProvider>, IServiceKnownTypeProvider
	{
		#region IServiceKnownTypeProvider Members

		public IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider ignore)
		{
			yield return typeof (StartViewerApplicationRequest);
		}

		#endregion
	}

	[ExtensionOf(typeof(ServiceKnownTypeExtensionPoint))]
	public class KnownApplicationTypeProvider : ExtensionPoint<IServiceKnownTypeProvider>, IServiceKnownTypeProvider
	{
		#region IServiceKnownTypeProvider Members

		public IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider ignore)
		{
			yield return typeof(Common.ViewerApplication);
		}

		#endregion
	}
}