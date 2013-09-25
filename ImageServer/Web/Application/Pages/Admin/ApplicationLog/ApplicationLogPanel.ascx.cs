#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Macro.Dicom.Utilities;
using Macro.ImageServer.Web.Application.Helpers;
using Macro.ImageServer.Web.Common.Data.DataSource;
using Macro.ImageServer.Web.Common.Utilities;
using Resources;

namespace Macro.ImageServer.Web.Application.Pages.Admin.ApplicationLog
{
	public partial class ApplicationLogSearchPanel : System.Web.UI.UserControl
	{
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !Page.IsAsync)
            {
                string startTime = Request["From"];
                string endTime = Request["To"];
                string hostname = Request["HostName"];

                if (startTime != null && endTime != null)
                {
                    string[] start = startTime.Split(' ');
                    string[] end = endTime.Split(' ');

                    FromDateFilter.Text = start[0];
                    ToDateFilter.Text = end[0];
                    FromDateCalendarExtender.SelectedDate = DateTime.Parse(start[0]);
                    ToDateCalendarExtender.SelectedDate = DateTime.Parse(end[0]);
                    FromTimeFilter.Text = start[1] + ".000";
                    ToTimeFilter.Text = end[1] + ".000";
                    HostFilter.Text = hostname;
                    ApplicationLogGridView.SetDataSource();
                    ApplicationLogGridView.Refresh();
                }
            }
        }

	    protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            ClearToDateFilterButton.Attributes["onclick"] = ScriptHelper.ClearDate(ToDateFilter.ClientID, ToDateCalendarExtender.ClientID);
            ClearFromDateFilterButton.Attributes["onclick"] = ScriptHelper.ClearDate(FromDateFilter.ClientID, FromDateCalendarExtender.ClientID);
            ToDateFilter.Attributes["OnChange"] = ScriptHelper.CheckDateRange(FromDateFilter.ClientID, ToDateFilter.ClientID, ToDateFilter.ClientID, ToDateCalendarExtender.ClientID, "To Date must be greater than From Date") + " " + ScriptHelper.PopulateDefaultToTime(ToTimeFilter.ClientID) + " return false;";
            FromDateFilter.Attributes["OnChange"] = ScriptHelper.CheckDateRange(FromDateFilter.ClientID, ToDateFilter.ClientID, FromDateFilter.ClientID, FromDateCalendarExtender.ClientID, "From Date must be less than To Date") + " " + ScriptHelper.PopulateDefaultFromTime(FromTimeFilter.ClientID) + " return false;";

			GridPagerTop.InitializeGridPager(SR.GridPagerApplicationLogSingleItem, SR.GridPagerApplicationLogMultipleItems, ApplicationLogGridView.ApplicationLogListGrid, delegate { return ApplicationLogGridView.ResultCount; }, ImageServerConstants.GridViewPagerPosition.Top);
		    ApplicationLogGridView.Pager = GridPagerTop;

			ApplicationLogGridView.DataSourceCreated += delegate(ApplicationLogDataSource source)
			                                       	{
														if (!String.IsNullOrEmpty(HostFilter.Text))
															source.Host = SearchHelper.LeadingAndTrailingWildCard(HostFilter.Text);
														if (!String.IsNullOrEmpty(ThreadFilter.Text))
															source.Thread = SearchHelper.LeadingAndTrailingWildCard(ThreadFilter.Text);
														if (!String.IsNullOrEmpty(MessageFilter.Text))
															source.Message = SearchHelper.LeadingAndTrailingWildCard(MessageFilter.Text);
														if (!String.IsNullOrEmpty(LogLevelListBox.SelectedValue))
															if (!LogLevelListBox.SelectedValue.Equals("ANY"))
																source.LogLevel = LogLevelListBox.SelectedValue;
														if (!String.IsNullOrEmpty(FromDateFilter.Text) || !String.IsNullOrEmpty(FromTimeFilter.Text))
														{
															DateTime val;

															if (DateTime.TryParseExact(FromDateFilter.Text + " " + FromTimeFilter.Text,DateTimeFormatter.DefaultTimestampFormat,CultureInfo.CurrentCulture,DateTimeStyles.None, out val))
																source.StartDate = val;
															else if (DateTime.TryParse(FromDateFilter.Text + " " + FromTimeFilter.Text, out val))
																source.StartDate = val;
														}

			                                       		if (!String.IsNullOrEmpty(ToDateFilter.Text) || !String.IsNullOrEmpty(ToTimeFilter.Text))
														{
															DateTime val;
															if (DateTime.TryParseExact(ToDateFilter.Text + " " + ToTimeFilter.Text, DateTimeFormatter.DefaultTimestampFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out val))
																source.EndDate = val;
															else if (DateTime.TryParse(ToDateFilter.Text + " " + ToTimeFilter.Text, out val))
																source.EndDate = val;
														}
														
													};

		}

		protected void SearchButton_Click(object sender, ImageClickEventArgs e)
		{
            if(Page.IsValid) ApplicationLogGridView.Refresh();
		}
	}
}