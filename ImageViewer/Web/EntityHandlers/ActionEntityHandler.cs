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
using System.IO;
using System.Text;
using Macro.Common;
using Macro.Common.Utilities;
using Macro.ImageViewer.Layout.Basic;
using Macro.Web.Common;
using Macro.ImageViewer.Web.Common.Entities;
using Macro.Desktop.Actions;
using Macro.Web.Services;
using Macro.ImageViewer.Web.Common.Messages;
using Macro.Desktop;
using System.Drawing;
using System.Drawing.Imaging;
using MouseButtonIconSet = Macro.ImageViewer.BaseTools.MouseImageViewerTool.MouseButtonIconSet;

namespace Macro.ImageViewer.Web.EntityHandlers
{
	public abstract class ActionNodeEntityHandler : EntityHandler
	{
		public abstract bool IsEquivalentTo(ActionNodeEntityHandler other);

		public virtual void Update()
		{ }

		////TODO (CR May 2010): We should add in the capability for handler extensions,
		/// much like the applicationcomponent views.
		public static ActionNodeEntityHandler Create(ActionModelNode modelNode)
		{
			if (modelNode is ActionNode)
			{
				IAction action = ((ActionNode)modelNode).Action;
				if (action is DropDownButtonAction)
				{
					IEntityHandler handler = Create<DropDownButtonActionEntityHandler>();
					handler.SetModelObject(action);
					return (ActionNodeEntityHandler)handler;
				}
				if (action is DropDownAction)
				{
					IEntityHandler handler = Create<DropDownActionEntityHandler>();
					handler.SetModelObject(action);
					return (ActionNodeEntityHandler)handler;
				}
				if (action is LayoutChangerAction)
				{
					IEntityHandler handler = Create<LayoutChangerActionEntityHandler>();
					handler.SetModelObject(action);
					return (ActionNodeEntityHandler)handler;
				}
				if (action is IClickAction)
				{
					IEntityHandler handler = Create<ClickActionEntityHandler>();
					handler.SetModelObject(action);
					return (ActionNodeEntityHandler)handler;
				}
			}
			else if (modelNode.ChildNodes.Count > 0)
			{
				IEntityHandler handler = Create<BranchActionEntityHandler>();
				handler.SetModelObject(modelNode);
				return (ActionNodeEntityHandler)handler;
			}

			//TODO (CR May 2010): although we won't get here, if we did, we should throw
			return null;
		}

		public static List<ActionNodeEntityHandler> Create(ActionModelNodeList nodes)
		{
			var handlers = new List<ActionNodeEntityHandler>();
			foreach (ActionModelNode node in nodes)
			{
				//TODO (CR May 2010): remove the try/catch and let it crash?
				try
				{
					ActionNodeEntityHandler handler = Create(node);
					if (handler != null)
						handlers.Add(handler);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, 
					             "Unexpected exception processing action node: {0}", node);
				}
			}

			return handlers;
		}

		public static bool AreEquivalent(IList<ActionNodeEntityHandler> handlers1, IList<ActionNodeEntityHandler> handlers2)
		{
			if (handlers1 == null && handlers2 == null)
				return true;
			if (handlers1 == null || handlers2 == null)
				return false;

			if (handlers1.Count != handlers2.Count)
				return false;

			for (int i = 0; i < handlers1.Count; ++i)
			{
				if (!handlers1[i].IsEquivalentTo(handlers2[i]))
					return false;
			}

			return true;
		}
	}

	public class BranchActionEntityHandler : ActionNodeEntityHandler
	{
		protected ActionModelNode ActionModelNode { get; private set; }
		private List<ActionNodeEntityHandler> ChildHandlers { get; set; }

		public override bool IsEquivalentTo(ActionNodeEntityHandler other)
		{
			if (other.GetType() != GetType())
				return false;

			return AreEquivalent(ChildHandlers, ((BranchActionEntityHandler)other).ChildHandlers);
		}

		protected override Entity CreateEntity()
		{
			return new WebActionNode();
		}

		public override void Update()
		{
			if (ChildHandlers == null)
				return;

			foreach (var childHandler in ChildHandlers)
				childHandler.Update();
		}

		public override void SetModelObject(object modelObject)
		{
			ActionModelNode = (ActionModelNode)modelObject;
			ChildHandlers = Create(ActionModelNode.ChildNodes);
		}

		protected override void UpdateEntity(Entity entity)
		{
			WebActionNode webAction = (WebActionNode)entity;

			webAction.LocalizedText = ActionModelNode.PathSegment.LocalizedText;
			webAction.Children = CollectionUtils.Map(ChildHandlers,
			                                         (ActionNodeEntityHandler handler) => (WebActionNode)handler.GetEntity()).ToArray();
		}

		public override void ProcessMessage(Message message)
		{
		}

		private void DisposeChildHandlers()
		{
			if (ChildHandlers == null)
				return;

			foreach (ActionNodeEntityHandler child in ChildHandlers)
				child.Dispose();

			ChildHandlers.Clear();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing) return;

			DisposeChildHandlers();
		}
	}

	public abstract class ActionEntityHandler : ActionNodeEntityHandler
	{
		protected ActionEntityHandler()
		{
		}

		protected IAction Action { get; private set; }

		public override bool IsEquivalentTo(ActionNodeEntityHandler other)
		{
			if (other.GetType() != GetType())
				return false;

			return Action.ActionID == ((ActionEntityHandler)other).Action.ActionID;
		}

		public override void SetModelObject(object modelObject)
		{
			Action = (IAction)modelObject;
			InitializeAction();
		}

		protected virtual void InitializeAction()
		{
			Action.VisibleChanged += OnVisibleChanged;
			Action.EnabledChanged += OnEnabledChanged;
			Action.TooltipChanged += OnTooltipChanged;
			Action.LabelChanged += OnLabelChanged;
			Action.IconSetChanged += OnIconSetChanged;
		}

		protected override void  UpdateEntity(Entity entity)
		{
			WebAction webAction = (WebAction)entity;
			webAction.ToolTip = Action.Tooltip;
			webAction.Label = Action.Label;
			webAction.Enabled = Action.Enabled;
			webAction.Visible = Action.Visible;
		    webAction.Available = Action.Available;

			if (Action.IconSet == null)
				return;

			webAction.IconSet = CreateWebIconSet(Action);
		}

		protected static WebIconSet CreateWebIconSet(IAction action)
		{
			return new WebIconSet
			       	{
			       		LargeIcon = LoadIcon(action, IconSize.Large),
			       		SmallIcon = LoadIcon(action, IconSize.Small),
			       		MediumIcon = LoadIcon(action, IconSize.Medium),
						HasOverlay = IconsHaveOverlay(action.IconSet)
			       	};
		}

		protected static bool IconsHaveOverlay(IconSet iconSet)
		{
			return iconSet is MouseButtonIconSet && ((MouseButtonIconSet)iconSet).ShowMouseButtonIconOverlay;
		}

		protected static byte[] LoadIcon(IAction action, IconSize size)
		{
			Image image = action.IconSet.CreateIcon(size, action.ResourceResolver);
			using (MemoryStream theStream = new MemoryStream())
			{
				image.Save(theStream, ImageFormat.Png);
				theStream.Position = 0;
				return theStream.GetBuffer();
			}
		}

		private void OnVisibleChanged(object sender, EventArgs e)
		{
			NotifyEntityPropertyChanged("Visible", Action.Visible);
		}

		private void OnEnabledChanged(object sender, EventArgs e)
		{
			NotifyEntityPropertyChanged("Enabled", Action.Enabled);
		}

		private void OnTooltipChanged(object sender, EventArgs e)
		{
			NotifyEntityPropertyChanged("ToolTip", Action.Tooltip);
		}

		private void OnLabelChanged(object sender, EventArgs e)
		{
			NotifyEntityPropertyChanged("Label", Action.Label);
		}

		private void OnIconSetChanged(object sender, EventArgs e)
		{
			NotifyEntityPropertyChanged("IconSet", CreateWebIconSet(Action));
		}


        protected override string[] GetDebugInfo()
        {
            StringBuilder info = new StringBuilder();
            info.AppendLine(string.Format("Action ID : {0}", Action.ActionID));
            info.AppendLine(string.Format("Action Label : {0}", Action.Label));
            info.AppendLine(string.Format("Action Path : {0}", Action.Path));
            info.AppendLine(string.Format("Action Availablity : {0}", Action.Available)); 
            
            return new[] {info.ToString()};
        }

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing) return;
			Action.VisibleChanged -= OnVisibleChanged;
			Action.EnabledChanged -= OnEnabledChanged;
			Action.TooltipChanged -= OnTooltipChanged;
			Action.LabelChanged -= OnLabelChanged;
			Action.IconSetChanged -= OnIconSetChanged;
		}
	}

	public class ClickActionEntityHandler : ActionEntityHandler
	{
		public new IClickAction Action
		{
			get { return (IClickAction)base.Action; }	
		}

		protected override Entity CreateEntity()
		{
			return new WebClickAction();
		}

		protected override void InitializeAction()
		{
			base.InitializeAction();
			Action.CheckedChanged += OnCheckChanged;
		}

		protected override void UpdateEntity(Entity entity)
		{
			base.UpdateEntity(entity);
			WebClickAction webClickAction = (WebClickAction)entity;

			webClickAction.IsCheckAction = Action.IsCheckAction;
			webClickAction.Checked = Action.Checked;
		}

	    public override void ProcessMessage(Message message)
		{
			if (message is ActionClickedMessage)
				Action.Click();
		}

		private void OnCheckChanged(object sender, EventArgs e)
		{
			NotifyEntityPropertyChanged("Checked", Action.Checked);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
				return;

			Action.CheckedChanged -= OnCheckChanged;
		}
	}

	//TODO: the 2 drop-down handlers share pretty much exactly the same code.  Try to share it.
	public class DropDownButtonActionEntityHandler : ClickActionEntityHandler
	{
		private List<ActionNodeEntityHandler> ChildHandlers { get; set; }

		public new DropDownButtonAction Action
		{
			get { return (DropDownButtonAction)base.Action; }
		}

		public override bool IsEquivalentTo(ActionNodeEntityHandler other)
		{
			return base.IsEquivalentTo(other) && 
			       AreEquivalent(ChildHandlers, ((DropDownButtonActionEntityHandler)other).ChildHandlers);
		}

		protected override Entity CreateEntity()
		{
			return new WebDropDownButtonAction();
		}

		protected override void InitializeAction()
		{
			base.InitializeAction();
			ChildHandlers = Create(Action.DropDownMenuModel.ChildNodes);
		}

		protected override void UpdateEntity(Entity entity)
		{
			base.UpdateEntity(entity);
			((WebDropDownButtonAction)entity).DropDownActions = GetDropDownWebActions();
		}

		public override void Update()
		{
			var newChildren = Create(Action.DropDownMenuModel.ChildNodes);
			if (!AreEquivalent(ChildHandlers, newChildren))
			{
				DisposeChildren();
				ChildHandlers = newChildren;
				NotifyEntityPropertyChanged("DropDownActions", GetDropDownWebActions());
			}
			else
			{
				foreach (var handler in newChildren)
					handler.Dispose();
			}
		}

		private WebActionNode[] GetDropDownWebActions()
		{
			return CollectionUtils.Map(ChildHandlers,
			                           (ActionEntityHandler handler) => (WebActionNode)handler.GetEntity()).ToArray();
		}

		private void DisposeChildren()
		{
			if (ChildHandlers == null)
				return;

			foreach (ActionEntityHandler childHandler in ChildHandlers)
				childHandler.Dispose();

			ChildHandlers.Clear();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing) return;

			DisposeChildren();
		}
	}

	public class DropDownActionEntityHandler : ActionEntityHandler
	{
		private List<ActionNodeEntityHandler> ChildHandlers { get; set; }

		public new DropDownAction Action
		{
			get { return (DropDownAction)base.Action; }
		}

		public override bool IsEquivalentTo(ActionNodeEntityHandler other)
		{
			return base.IsEquivalentTo(other) && 
			       AreEquivalent(ChildHandlers, ((DropDownActionEntityHandler)other).ChildHandlers);
		}

		public override void ProcessMessage(Message message)
		{
			// There should be no messages received.
			throw new NotImplementedException();
		}

		protected override Entity CreateEntity()
		{
			return new WebDropDownAction();
		}

		protected override void InitializeAction()
		{
			base.InitializeAction();
			ChildHandlers = Create(Action.DropDownMenuModel.ChildNodes);
		}

		protected override void UpdateEntity(Entity entity)
		{
			base.UpdateEntity(entity);
			((WebDropDownAction)entity).DropDownActions = GetDropDownWebActions();
		}

		public override void Update()
		{
			var newChildren = Create(Action.DropDownMenuModel.ChildNodes);
			if (!AreEquivalent(ChildHandlers, newChildren))
			{
				DisposeChildren();
				ChildHandlers = newChildren;
				NotifyEntityPropertyChanged("DropDownActions", GetDropDownWebActions());
			}
			else
			{
				foreach (var handler in newChildren)
					handler.Dispose();
			}
		}

		private WebActionNode[] GetDropDownWebActions()
		{
			return CollectionUtils.Map(ChildHandlers,
			                           (ActionEntityHandler handler) => (WebActionNode)handler.GetEntity()).ToArray();
		}

		private void DisposeChildren()
		{
			if (ChildHandlers == null)
				return;

			foreach (ActionEntityHandler childHandler in ChildHandlers)
				childHandler.Dispose();

			ChildHandlers.Clear();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing) return;

			DisposeChildren();
		}
	}

	public class LayoutChangerActionEntityHandler : ActionEntityHandler
	{
		public new LayoutChangerAction Action
		{
			get { return (LayoutChangerAction)base.Action; }
		}

		public override void ProcessMessage(Message message)
		{
			SetLayoutActionMessage msg = message as SetLayoutActionMessage;
			if (msg != null)
				Action.SetLayout(msg.Rows, msg.Columns);
		}

		protected override Entity CreateEntity()
		{
			return new WebLayoutChangerAction();
		}

		protected override void  UpdateEntity(Entity entity)
		{
			base.UpdateEntity(entity);

			var layoutChangerEntity = (WebLayoutChangerAction)entity;
			layoutChangerEntity.MaxColumns = Action.MaxColumns;
			layoutChangerEntity.MaxRows = Action.MaxRows;
			layoutChangerEntity.ActionID = Action.ActionID;
		}
	}
}
