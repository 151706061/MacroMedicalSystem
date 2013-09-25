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
using Macro.Common;
using Macro.Desktop;
using Macro.Web.Common;
using Macro.ImageViewer.Web.Common.Entities;
using Macro.Common.Utilities;
using Macro.Web.Services;

namespace Macro.ImageViewer.Web.EntityHandlers
{
	public class ViewerEntityHandler : EntityHandler<Viewer>
	{
		private ImageViewerComponent _viewer;

	    private readonly ToolStripSettings _toolStripSettings = new ToolStripSettings();
		private readonly List<ImageBoxEntityHandler> _imageBoxHandlers = new List<ImageBoxEntityHandler>();
		private readonly List<ActionNodeEntityHandler> _actionHandlers = new List<ActionNodeEntityHandler>();

	    private Common.Entities.ImageBox[] GetImageBoxEntities()
		{
			return CollectionUtils.Map(_imageBoxHandlers, 
				(ImageBoxEntityHandler handler) => handler.GetEntity()).ToArray();
		}

		private WebActionNode[] GetToolbarActionEntities()
		{
			var toolbarActions = CollectionUtils.Map(_actionHandlers,
										 (ActionNodeEntityHandler handler) => (WebActionNode)handler.GetEntity());
			return FlattenWebActions(toolbarActions).ToArray();
		}

		public override void SetModelObject(object modelObject)
		{
			_viewer = (ImageViewerComponent)modelObject;
			_viewer.PhysicalWorkspace.LayoutCompleted += OnLayoutCompleted;
			_viewer.PhysicalWorkspace.Drawing += OnPhysicalWorkspaceDrawing;
			_viewer.EventBroker.PresentationImageSelected += OnPresentationImageSelected;

			UpdateActionModel(false);
			RefreshImageBoxHandlers(false);
		}

		protected override void UpdateEntity(Viewer entity)
		{
		    entity.ToolStripIconSize = LoadToolbarIconSizeFromSettings();
			entity.ImageBoxes = GetImageBoxEntities();
			entity.ToolbarActions = GetToolbarActionEntities();
		}

		private void RefreshImageBoxHandlers(bool notify)
		{
			DisposeImageBoxHandlers();

			foreach (IImageBox imageBox in _viewer.PhysicalWorkspace.ImageBoxes)
			{
				ImageBoxEntityHandler newHandler = Create<ImageBoxEntityHandler>();
				((IEntityHandler)newHandler).SetModelObject(imageBox);
				_imageBoxHandlers.Add(newHandler);
			}

			if (notify)
				base.NotifyEntityPropertyChanged("ImageBoxes", GetImageBoxEntities());
		}

		public override void ProcessMessage(Message message)
		{
		}

		private void OnLayoutCompleted(object sender, EventArgs e)
		{
			RefreshImageBoxHandlers(true);
		}

		private void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			//TODO (CR May 2010): this is not ideal.  We should actually implement DropDownMenuModelChanged on the
			//dropdown action classes, which is really the only reason for this being here.
			foreach (var actionHandler in _actionHandlers)
				actionHandler.Update();
		}

		void OnPhysicalWorkspaceDrawing(object sender, EventArgs e)
		{
			foreach (ImageBoxEntityHandler handler in _imageBoxHandlers)
				handler.Draw();
		}

		private void UpdateActionModel(bool notify)
		{
			DisposeActionHandlers();
			_actionHandlers.AddRange(ActionNodeEntityHandler.Create(_viewer.ToolbarModel.ChildNodes));
			if (!notify)
				return;

            NotifyEntityPropertyChanged("ToolbarActions", GetToolbarActionEntities());
		}

        private WebIconSize LoadToolbarIconSizeFromSettings()
        {
            try
            {
                switch (_toolStripSettings.IconSize)
                {
                    case IconSize.Large:
                        return WebIconSize.Large;
                    case IconSize.Medium:
                        return WebIconSize.Medium;
                    case IconSize.Small:
                        return WebIconSize.Small;

                    default:
                        return WebIconSize.Medium;
                }
            }
            catch (Exception ex)
            {
                // eat it since it's not really important
                Platform.Log(LogLevel.Error, ex, "Unable to read icon size from settings. Default to Medium.");
                return WebIconSize.Medium;
            }
        }


		private static List<WebActionNode> FlattenWebActions(IEnumerable<WebActionNode> actionNodes)
		{
			List<WebActionNode> nodes = new List<WebActionNode>();

			foreach (WebActionNode node in actionNodes)
			{
				//TODO (CR May 2010): use some kind of interface or attribute (e.g. AssociateHandlerAttribute)?
				if (node is WebDropDownButtonAction || node is WebClickAction || node is WebDropDownAction)
					nodes.Add(node);
				else if (node.Children != null && node.Children.Length > 0)
					nodes.AddRange(FlattenWebActions(node.Children));
			}

			return nodes;
		}

		private void DisposeImageBoxHandlers()
		{
			foreach (ImageBoxEntityHandler handler in _imageBoxHandlers)
				handler.Dispose();

			_imageBoxHandlers.Clear();
		}

		private void DisposeActionHandlers()
		{
			foreach (ActionNodeEntityHandler handler in _actionHandlers)
				handler.Dispose();

			_actionHandlers.Clear();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing && _viewer != null)
			{
				_viewer.PhysicalWorkspace.LayoutCompleted -= OnLayoutCompleted;
				_viewer.PhysicalWorkspace.Drawing -= OnPhysicalWorkspaceDrawing;
				_viewer.EventBroker.PresentationImageSelected -= OnPresentationImageSelected;

				DisposeActionHandlers();
				DisposeImageBoxHandlers();

				_viewer = null;
			}
		}
	}
}
