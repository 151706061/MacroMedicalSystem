#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using Macro.Common;
using Macro.ImageViewer.Web.Common.Messages;
using Macro.ImageViewer.Web.View;
using Macro.Web.Common;
using MessageBoxEntity=Macro.ImageViewer.Web.Common.Entities.MessageBox;
using Macro.Web.Services;

namespace Macro.ImageViewer.Web.EntityHandlers
{
    public class MessageBoxEntityHandler : EntityHandler<MessageBoxEntity>
    {
        private DesktopWindowView _desktopWindow;

        public override void SetModelObject(object modelObject)
		{
            _desktopWindow = (DesktopWindowView)modelObject;
		}

		protected override void UpdateEntity(Common.Entities.MessageBox entity)
		{
		}

        public override void ProcessMessage(Message message)
        {
            DismissMessageBoxMessage msg = message as DismissMessageBoxMessage;
            if (msg != null)
            {
                switch (msg.Result)
                {
                    case WebDialogBoxAction.Yes:
                        _desktopWindow.Dismiss(DialogBoxAction.Yes);
                        break;
                    case WebDialogBoxAction.No:
                        _desktopWindow.Dismiss(DialogBoxAction.No);
                        break;
                    case WebDialogBoxAction.Ok:
                        _desktopWindow.Dismiss(DialogBoxAction.Ok);
                        break;
                    case WebDialogBoxAction.Cancel:
                        _desktopWindow.Dismiss(DialogBoxAction.Cancel);
                        break;
                }

                ApplicationContext.EntityHandlers.Remove(this);
            }
        }

        protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && _desktopWindow != null)
			{
				_desktopWindow = null;
			}
		}
    }
}
