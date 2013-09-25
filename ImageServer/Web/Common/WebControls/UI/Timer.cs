#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using System.Web.UI;
using Macro.Common.Utilities;

namespace Macro.ImageServer.Web.Common.WebControls.UI
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:Timer runat=server></{0}:Timer>")]
    public class Timer : System.Web.UI.Timer
    {
        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        [Localizable(true)]
        public int DisableAfter
        {
            get
            {
                if (ViewState["DisableAfter"] != null)
                {
                    return (int)ViewState["DisableAfter"];
                }
                else
                    return -1;
            }

            set
            {
                ViewState["DisableAfter"] = value;
            }
        }

        public event EventHandler<TimerEventArgs> AutoDisabled;


        private int TickCounter
        {
            get
            {
                if (ViewState["TickCounter"] != null)
                {
                    return (int)ViewState["TickCounter"];
                }
                else
                    return 0;
            }

            set
            {
                ViewState["TickCounter"] = value;
            }
        }

        public void Reset(bool enabled)
        {
            TickCounter = 0;
            Enabled = enabled;
        }

        protected override void OnTick(EventArgs e)
        {
            TickCounter++;
            if (DisableAfter > 0 && TickCounter >= DisableAfter)
            {
                Enabled = false; 
                EventsHelper.Fire(AutoDisabled, this, new TimerEventArgs());
            }
        
            base.OnTick(e);
        }

    }

    public class TimerEventArgs:EventArgs
    {
    }
}