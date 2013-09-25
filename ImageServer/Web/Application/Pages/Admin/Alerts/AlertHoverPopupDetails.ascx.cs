#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Web.UI;
using Macro.ImageServer.Model;
using Macro.ImageServer.Services.WorkQueue;
using Macro.ImageServer.Web.Common.Data.DataSource;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Alerts
{
    public partial class AlertHoverPopupDetails : System.Web.UI.UserControl
    {
        #region Private Members

        #endregion
        
        #region Public Properties

        public AlertSummary Alert { get; set; }

        #endregion

        public override void DataBind()
        {
            if (Alert!=null && Alert.ContextData!=null)
            {
                IAlertPopupView popupView = null;
                if (Alert.ContextData is WorkQueueAlertContextData)
                {
                    popupView = Page.LoadControl("WorkQueueAlertContextDataView.ascx") as IAlertPopupView;
                    
                }

                if (Alert.ContextData is StudyAlertContextInfo)
                {
                    popupView = Page.LoadControl("StudyAlertContextInfoView.ascx") as IAlertPopupView;
                }

                if (popupView!=null)
                {
                    popupView.SetAlert(Alert);
                    DetailsPlaceHolder.Controls.Add(popupView as UserControl);
                }                
            }
            base.DataBind();
        }
    }
}