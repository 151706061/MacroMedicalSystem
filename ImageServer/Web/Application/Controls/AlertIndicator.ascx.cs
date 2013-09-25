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
using System.Data;
using System.Configuration;
using System.Security.Permissions;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Web.Common.Data;

namespace Macro.ImageServer.Web.Application.Controls
{
    [PrincipalPermission(SecurityAction.Demand, Role = Macro.ImageServer.Enterprise.Authentication.AuthorityTokens.Admin.Alert.View)]
    public partial class AlertIndicator : System.Web.UI.UserControl
    {
        protected IList<Alert> alerts;
       
        protected void Page_Load(object sender, EventArgs e)
        {           
            AlertController controller = new AlertController();
            AlertSelectCriteria criteria = new AlertSelectCriteria();

            criteria.AlertLevelEnum.EqualTo(AlertLevelEnum.Critical);
            criteria.InsertTime.SortDesc(1);

            AlertsCount.Text = controller.GetAlertsCount(criteria).ToString();

            alerts = controller.GetAlerts(criteria);

            if (alerts.Count > 0) {

                int rows = 0;
                foreach (Alert alert in alerts)
                {
                    TableRow alertRow = new TableRow();

                    alertRow.Attributes.Add("class", "AlertTableCell");

                    TableCell component = new TableCell();
                    TableCell source = new TableCell();
                    TableCell description = new TableCell();

                    description.Wrap = false;

                    component.Text = alert.Component;
                    component.Wrap = false;
                    source.Text = alert.Source;
                    source.Wrap = false;

                    string content = alert.Content.GetElementsByTagName("Message").Item(0).InnerText;
                    description.Text = content.Length < 50 ? content : content.Substring(0, 50);
                    description.Text += " ...";
                    description.Wrap = false;

                    alertRow.Cells.Add(component);
                    alertRow.Cells.Add(source);
                    alertRow.Cells.Add(description);

                    AlertTable.Rows.Add(alertRow);

                    rows++;
                    if (rows == 5) break;
                }
            }
        }

        protected void SwitchToEnglishClicked(object sender, EventArgs e)
        {
            HttpContext.Current.Items["Language"] = "en";
        }
    }
}