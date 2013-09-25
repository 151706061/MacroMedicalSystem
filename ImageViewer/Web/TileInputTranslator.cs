#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using Macro.Desktop;
using Macro.ImageViewer.InputManagement;
using Message=Macro.Web.Common.Message;

namespace Macro.ImageViewer.Web
{
	//TODO: remove dependency on System.Windows.Forms

    internal class WebTileInputTranslator
    {
        private Keys[] _consumeKeyStrokes = 
						{	Keys.ControlKey,
							Keys.LControlKey,
							Keys.RControlKey,
							Keys.ShiftKey,
							Keys.LShiftKey,
							Keys.RShiftKey,
							Keys.Menu
						};

        public WebTileInputTranslator()
        {
        }

        private Keys Modifiers
        {
            get { return System.Windows.Forms.Control.ModifierKeys; }
        }

        //private Point MousePositionScreen
        //{
        //    get { return System.Windows.Forms.Control.MousePosition; }
        //}

        //private Point MousePositionClient
        //{
        //    get { return _tileControl.PointToClient(this.MousePositionScreen); }
        //}

        //private MouseButtons MouseButtons
        //{
        //    get { return System.Windows.Forms.Control.MouseButtons; }
        //}

        private bool Control
        {
            get { return (this.Modifiers & Keys.Control) == Keys.Control; }
        }

        private bool Alt
        {
            get { return (this.Modifiers & Keys.Alt) == Keys.Alt; }
        }

        private bool Shift
        {
            get { return (this.Modifiers & Keys.Shift) == Keys.Shift; }
        }

        private bool ConsumeKeyStroke(Keys keyCode)
        {
            foreach (Keys keyStroke in _consumeKeyStrokes)
            {
                if (keyCode == keyStroke)
                    return true;
            }

            return false;
        }

        public object OnLostFocus()
        {
            return new LostFocusMessage();
        }

        public object OnMouseLeave()
        {
            return new MouseLeaveMessage();
        }

        public object OnMouseMove(MouseEventArgs e)
        {
            return new TrackMousePositionMessage(e.Location);
        }

        public object OnMouseDown(MouseEventArgs e)
        {
            return new MouseButtonMessage(e.Location, (XMouseButtons)e.Button, MouseButtonMessage.ButtonActions.Down, (uint)e.Clicks, this.Control, this.Alt, this.Shift);
        }

        public object OnMouseUp(MouseEventArgs e)
        {
            return new MouseButtonMessage(e.Location, (XMouseButtons)e.Button, MouseButtonMessage.ButtonActions.Up, 0, this.Control, this.Alt, this.Shift);
        }

        public object OnMouseWheel(MouseEventArgs e)
        {
            return new InputManagement.MouseWheelMessage(e.Delta, this.Control, this.Alt, this.Shift);
        }

        public object OnKeyDown(KeyEventArgs e)
        {
            if (ConsumeKeyStroke(e.KeyCode))
                return null;

            return new KeyboardButtonMessage((XKeys)e.KeyData, KeyboardButtonMessage.ButtonActions.Down);
        }

        public object OnKeyUp(KeyEventArgs e)
        {
            if (ConsumeKeyStroke(e.KeyCode))
                return null;

            return new KeyboardButtonMessage((XKeys)e.KeyData, KeyboardButtonMessage.ButtonActions.Up);
        }

        public object PostProcessMessage(Message msg, bool alreadyHandled)
        {
            //if (msg.Msg == 0x100 && alreadyHandled)
            //{
            //    Keys keyData = (Keys)msg.WParam;
            //    if (!ConsumeKeyStroke(keyData))
            //    {
            //        //when a keystroke gets handled by a control other than the tile, we release the capture.
            //        return new ReleaseCaptureMessage();
            //    }
            //}

            return null;
        }

        public object PreProcessMessage(Message msg)
        {
            //if (msg.Msg == 0x100)
            //{
            //    Keys keyData = (Keys)msg.WParam;
            //    if (!ConsumeKeyStroke(keyData))
            //        return new KeyboardButtonDownPreview((XKeys)msg.WParam);
            //}

            return null;
        }
    }
}