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

using System.Drawing;
using Macro.Common.Utilities;
using Macro.ImageViewer.Graphics;
using Macro.ImageViewer.InputManagement;

namespace Macro.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// An interactive graphic that adds a standard set of states to a subject graphic, making it an <see cref="IStandardStatefulGraphic"/>.
	/// </summary>
	[Cloneable]
	public class StandardStatefulGraphic : StatefulCompositeGraphic, IStandardStatefulGraphic, ISelectableGraphic, IFocussableGraphic
	{
		/// <summary>
		/// Gets the default value of <see cref="FocusColor"/>.
		/// </summary>
		protected static readonly Color DefaultFocusColor = Color.Orange;

		/// <summary>
		/// Gets the default value of <see cref="FocusSelectedColor"/>.
		/// </summary>
		protected static readonly Color DefaultFocusSelectedColor = Color.Tomato;

		/// <summary>
		/// Gets the default value of <see cref="SelectedColor"/>.
		/// </summary>
		protected static readonly Color DefaultSelectedColor = Color.Tomato;

		/// <summary>
		/// Gets the default value of <see cref="InactiveColor"/>.
		/// </summary>
		protected static readonly Color DefaultInactiveColor = Color.Yellow;

		[CloneIgnore]
		private bool _selected = false;

		[CloneIgnore]
		private bool _focused = false;

		[CloneCopyReference]
		private Color _focusColor = DefaultFocusColor;

		[CloneCopyReference]
		private Color _focusSelectedColor = DefaultFocusSelectedColor;

		[CloneCopyReference]
		private Color _selectedColor = DefaultSelectedColor;

		[CloneCopyReference]
		private Color _inactiveColor = DefaultInactiveColor;

		/// <summary>
		/// Constructs a new instance of <see cref="StandardStatefulGraphic"/>.
		/// </summary>
		public StandardStatefulGraphic(IGraphic subject) : base(subject)
		{
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected StandardStatefulGraphic(StandardStatefulGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			if (this.State == null)
				this.State = this.CreateInactiveState();
		}

		/// <summary>
		/// Gets or sets the color in which to display the graphic when the <see cref="StatefulCompositeGraphic.State"/> is <see cref="FocussedGraphicState"/>.
		/// </summary>
		public Color FocusColor
		{
			get { return _focusColor; }
			set { _focusColor = value; }
		}

		/// <summary>
		/// Gets or sets the color in which to display the graphic when the <see cref="StatefulCompositeGraphic.State"/> is <see cref="SelectedGraphicState"/>.
		/// </summary>
		public Color SelectedColor
		{
			get { return _selectedColor; }
			set { _selectedColor = value; }
		}

		/// <summary>
		/// Gets or sets the color in which to display the graphic when the <see cref="StatefulCompositeGraphic.State"/> is <see cref="FocussedSelectedGraphicState"/>.
		/// </summary>
		public Color FocusSelectedColor
		{
			get { return _focusSelectedColor; }
			set { _focusSelectedColor = value; }
		}

		/// <summary>
		/// Gets or sets the color in which to display the graphic when the <see cref="StatefulCompositeGraphic.State"/> is <see cref="InactiveGraphicState"/>.
		/// </summary>
		public Color InactiveColor
		{
			get { return _inactiveColor; }
			set { _inactiveColor = value; }
		}

		private static void UpdateGraphicStyle(IGraphic graphic, Color color, bool controlGraphics)
		{
			if (graphic is IControlGraphic)
			{
				IControlGraphic controlGraphic = (IControlGraphic) graphic;
				controlGraphic.Show = controlGraphics;
				controlGraphic.Color = color;
			}
			else if (graphic is IVectorGraphic)
			{
				((IVectorGraphic) graphic).Color = color;
			}
			
			if (graphic is CompositeGraphic)
			{
				foreach (IGraphic childGraphic in ((CompositeGraphic) graphic).Graphics)
					UpdateGraphicStyle(childGraphic, color, controlGraphics);
			}
		}

		/// <summary>
		/// Called when the value of <see cref="StatefulCompositeGraphic.State"/> is initialized for the first time.
		/// </summary>
		protected override void OnStateInitialized()
		{
			base.OnStateInitialized();

			if (this.State is InactiveGraphicState)
				UpdateGraphicStyle(this, this.InactiveColor, false);
			else if ( this.State is FocussedGraphicState)
				UpdateGraphicStyle(this, this.FocusColor, true);
			else if (this.State is SelectedGraphicState)
				UpdateGraphicStyle(this, this.SelectedColor, false);
			else if (this.State is FocussedSelectedGraphicState)
				UpdateGraphicStyle(this, this.FocusSelectedColor, true);
		}

		/// <summary>
		/// Called when the value of <see cref="StatefulCompositeGraphic.State"/> changes.
		/// </summary>
		/// <param name="e">An object containing data describing the specific state change.</param>
		protected override void OnStateChanged(GraphicStateChangedEventArgs e)
		{
			base.OnStateChanged(e);

			if (typeof (InactiveGraphicState).IsAssignableFrom(e.NewState.GetType()))
				OnEnterInactiveState(e.MouseInformation);
			else if (typeof (FocussedGraphicState).IsAssignableFrom(e.NewState.GetType()))
				OnEnterFocusState(e.MouseInformation);
			else if (typeof (SelectedGraphicState).IsAssignableFrom(e.NewState.GetType()))
				OnEnterSelectedState(e.MouseInformation);
			else if (typeof (FocussedSelectedGraphicState).IsAssignableFrom(e.NewState.GetType()))
				OnEnterFocusSelectedState(e.MouseInformation);
		}

		/// <summary>
		/// Called then the graphic <see cref="StatefulCompositeGraphic.State"/> enters the inactive state.
		/// </summary>
		/// <param name="mouseInformation">Information about the current mouse input.</param>
		protected virtual void OnEnterInactiveState(IMouseInformation mouseInformation)
		{
			// If the currently selected graphic is this one,
			// and we're about to go inactive, set the selected graphic
			// to null, indicating that no graphic is currently selected
			if (this.ParentPresentationImage != null)
			{
				if (this.ParentPresentationImage.SelectedGraphic == this)
					this.ParentPresentationImage.SelectedGraphic = null;

				if (this.ParentPresentationImage.FocussedGraphic == this)
					this.ParentPresentationImage.FocussedGraphic = null;
			}

			UpdateGraphicStyle(this, this.InactiveColor, false);
			Draw();
		}

		/// <summary>
		/// Called then the graphic <see cref="StatefulCompositeGraphic.State"/> enters the focused state.
		/// </summary>
		/// <param name="mouseInformation">Information about the current mouse input.</param>
		protected virtual void OnEnterFocusState(IMouseInformation mouseInformation)
		{
			this.Focussed = true;

			UpdateGraphicStyle(this, this.FocusColor, true);
			Draw();
		}

		/// <summary>
		/// Called then the graphic <see cref="StatefulCompositeGraphic.State"/> enters the selected state.
		/// </summary>
		/// <param name="mouseInformation">Information about the current mouse input.</param>
		protected virtual void OnEnterSelectedState(IMouseInformation mouseInformation)
		{
			this.Selected = true;

			if (this.ParentPresentationImage != null && this.ParentPresentationImage.FocussedGraphic == this)
				this.ParentPresentationImage.FocussedGraphic = null;

			UpdateGraphicStyle(this, this.SelectedColor, false);
			Draw();
		}

		/// <summary>
		/// Called then the graphic <see cref="StatefulCompositeGraphic.State"/> enters the selected and focused state.
		/// </summary>
		/// <param name="mouseInformation">Information about the current mouse input.</param>
		protected virtual void OnEnterFocusSelectedState(IMouseInformation mouseInformation)
		{
			this.Selected = true;
			this.Focussed = true;

			UpdateGraphicStyle(this, this.FocusSelectedColor, true);
			Draw();
		}

		#region IStandardStatefulGraphic Members

		/// <summary>
		/// Creates an inactive <see cref="GraphicState"/> for the current graphic.
		/// </summary>
		/// <returns>An inactive <see cref="GraphicState"/> for the current graphic.</returns>
		public virtual GraphicState CreateInactiveState()
		{
			return new InactiveGraphicState(this);
		}

		/// <summary>
		/// Creates a focussed <see cref="GraphicState"/> for the current graphic.
		/// </summary>
		/// <returns>An inactive <see cref="GraphicState"/> for the current graphic.</returns>
		public virtual GraphicState CreateFocussedState()
		{
			return new FocussedGraphicState(this);
		}

		/// <summary>
		/// Creates a selected <see cref="GraphicState"/> for the current graphic.
		/// </summary>
		/// <returns>An inactive <see cref="GraphicState"/> for the current graphic.</returns>
		public virtual GraphicState CreateSelectedState()
		{
			return new SelectedGraphicState(this);
		}

		/// <summary>
		/// Creates a focussed and selected <see cref="GraphicState"/> for the current graphic.
		/// </summary>
		/// <returns>An inactive <see cref="GraphicState"/> for the current graphic.</returns>
		public virtual GraphicState CreateFocussedSelectedState()
		{
			return new FocussedSelectedGraphicState(this);
		}

		#endregion

		#region ISelectable Members

		/// <summary>
		/// Gets or set a value indicating whether the <see cref="StandardStatefulGraphic"/> is selected.
		/// </summary>
		public bool Selected
		{
			get { return _selected; }
			set
			{
				if (_selected != value)
				{
					_selected = value;

					if (_selected && this.ParentPresentationImage != null)
						this.ParentPresentationImage.SelectedGraphic = this;

					if (_focused)
					{
						if (_selected)
							this.State = CreateFocussedSelectedState();
						else
							this.State = CreateFocussedState();
					}
					else
					{
						if (_selected)
							this.State = CreateSelectedState();
						else
							this.State = CreateInactiveState();
					}
				}
			}
		}

		#endregion

		#region IFocussable Members

		/// <summary>
		/// Gets or set a value indicating whether the <see cref="StandardStatefulGraphic"/> is in focus.
		/// </summary>
		public bool Focussed
		{
			get { return _focused; }
			set
			{
				if (_focused != value)
				{
					_focused = value;

					if (_focused)
					{
						if (this.ParentPresentationImage != null)
							this.ParentPresentationImage.FocussedGraphic = this;

						if (this.Selected)
							this.State = CreateFocussedSelectedState();
						else
							this.State = CreateFocussedState();
					}
					else
					{
						if (this.Selected)
							this.State = CreateSelectedState();
						else
							this.State = CreateInactiveState();
					}
				}
			}
		}

		#endregion
	}
}