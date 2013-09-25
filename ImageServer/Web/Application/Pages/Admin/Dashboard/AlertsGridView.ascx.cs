#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Configuration;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Application.Controls;
using Macro.ImageServer.Web.Application.Pages.Admin.Alerts;
using Macro.ImageServer.Web.Common.Data.DataSource;
using GridView = Macro.ImageServer.Web.Common.WebControls.UI.GridView;
using SR = Resources.SR;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Dashboard
{
    //
    //  Used to display the list of Archive Queue Items.
    //
    public partial class AlertsGridView : GridViewPanel
    {
        #region Delegates
        public delegate void AlertDataSourceCreated(AlertDataSource theSource);
        public event AlertDataSourceCreated DataSourceCreated;
        #endregion

        #region Private members
        // list of studies to display
        private Unit _height;
        private AlertDataSource _dataSource;
        #endregion Private members

        #region Public properties

        public int ResultCount
        {
            get
            {
                if (_dataSource == null)
                {
                    _dataSource = new AlertDataSource();

                    _dataSource.AlertFoundSet += delegate(IList<AlertSummary> newlist)
                                            {
                                                AlertItems = new AlertItemCollection(newlist);
                                            };
                    if (DataSourceCreated != null)
                        DataSourceCreated(_dataSource);
                    _dataSource.SelectCount();
                }
                if (_dataSource.ResultCount == 0)
                {
                    if (DataSourceCreated != null)
                        DataSourceCreated(_dataSource);

                    _dataSource.SelectCount();
                }
                return _dataSource.ResultCount;
            }
        }

        /// <summary>
        /// Retrieve reference to the grid control being used to display the devices.
        /// </summary>
        public GridView AlertGrid
        {
            get { return AlertGridView; }
        }

        /// <summary>
        /// Gets/Sets the current selected device.
        /// </summary>
        public IList<Alert> SelectedItems
        {
            get
            {
                if(!AlertGrid.IsDataBound) AlertGrid.DataBind();
                
                if (AlertItems == null || AlertItems.Count == 0)
                    return null;

                int[] rows = AlertGrid.SelectedIndices;
                if (rows == null || rows.Length == 0)
                    return null;

                var queueItems = new List<Alert>();
                for (int i = 0; i < rows.Length; i++)
                {
                    if (rows[i] < AlertItems.Count)
                    {
                        queueItems.Add(AlertItems[rows[i]].TheAlertItem);
                    }
                }

                return queueItems;
            }
        }

        /// <summary>
        /// Gets/Sets the list of Alert Items
        /// </summary>
        public AlertItemCollection AlertItems { get; set; }

        /// <summary>
        /// Gets/Sets the height of the study list panel
        /// </summary>
        public Unit Height
        {
            get
            {
                if (ContainerTable != null)
                    return ContainerTable.Height;
                return _height;
            }
            set
            {
                _height = value;
                if (ContainerTable != null)
                    ContainerTable.Height = value;
            }
        }

        /// <summary>
        /// Gets/Sets a key of the selected work queue item.
        /// </summary>
        public AlertSummary SelectedAlert
        {
            get
            {
                if (SelectedAlertKey != null && AlertItems.ContainsKey(SelectedAlertKey))
                {
                    return AlertItems[SelectedAlertKey];
                }
                return null;
            }
            set
            {
                SelectedAlertKey = value.Key;
                AlertGrid.SelectedIndex = AlertItems.RowIndexOf(SelectedAlertKey, AlertGrid);
            }
        }

        #endregion

        #region protected methods

        protected ServerEntityKey SelectedAlertKey
        {
            set
            {
                ViewState["SelectedAlertKey"] = value;
            }
            get
            {
                return ViewState["SelectedAlertKey"] as ServerEntityKey;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TheGrid = AlertGrid;

            GridPagerTop.InitializeGridPager(SR.GridPagerAlertSingleItemFound, SR.GridPagerAlertMultipleItemsFound, AlertGrid,
                                             () => ResultCount, ImageServerConstants.GridViewPagerPosition.Top);
            Pager = GridPagerTop;
            GridPagerTop.Reset();

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

            AlertGrid.DataSource = AlertDataSourceObject;
            AlertGrid.DataBind();
        }

        protected void AlertGridView_DataBound(object sender, EventArgs e)
        {
            // reselect the row based on the new order
            if (SelectedAlertKey != null)
            {
                AlertGrid.SelectedIndex = AlertItems.RowIndexOf(SelectedAlertKey, AlertGrid);
            }
        }

        protected void AlertGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;

            if (AlertGrid.EditIndex != e.Row.RowIndex)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {                   
                    var alert = e.Row.DataItem as AlertSummary;
                    var level = e.Row.FindControl("Level") as Label;
                   
                    if(level != null && alert != null)
                    {
                        if (alert.LevelIsErrorOrCritical)
                        {
                            level.ForeColor = Color.Red;
                        }
                        level.Text = alert.Level;

                        var appLogLink = e.Row.FindControl("AppLogLink") as LinkButton;


                        int timeRange = int.Parse(ConfigurationManager.AppSettings["AlertTimeRange"]);
                        string hostname = GetHostName(alert.Source);

                            DateTime startTime = alert.InsertTime.AddSeconds(-timeRange/2);
                            DateTime endTime = alert.InsertTime.AddSeconds(timeRange / 2);
                            appLogLink.PostBackUrl = ImageServerConstants.PageURLs.ApplicationLog + "?From=" +
                                                     HttpUtility.UrlEncode(startTime.ToString("yyyy-MM-dd") + " " +
                                                                           startTime.ToString("HH:mm:ss")) + "&To=" +
                                                     HttpUtility.UrlEncode(endTime.ToString("yyyy-MM-dd") + " " +
                                                                           endTime.ToString("HH:mm:ss")) +
                                                     "&HostName=" + hostname;

                    }

                    if (alert.ContextData!=null)
                    {
                        var ctrl =
                       Page.LoadControl("AlertHoverPopupDetails.ascx") as AlertHoverPopupDetails;

                        ctrl.Alert = alert;

                        e.Row.FindControl("DetailsHoverPlaceHolder").Controls.Add(ctrl);
                        ctrl.DataBind();
                    }
                   
                }
            }
        }

        protected void DisposeAlertDataSource(object sender, ObjectDataSourceDisposingEventArgs e)
        {
            e.Cancel = true;
        }

        protected void GetAlertDataSource(object sender, ObjectDataSourceEventArgs e)
        {
            if (_dataSource == null)
            {
                _dataSource = new AlertDataSource();

                _dataSource.AlertFoundSet += delegate(IList<AlertSummary> newlist)
                                        {
                                            AlertItems = new AlertItemCollection(newlist);
                                        };
            }

            e.ObjectInstance = _dataSource;

            if (DataSourceCreated != null)
                DataSourceCreated(_dataSource);

        }

        #endregion

        protected bool HasContextData(AlertSummary item)
        {
            return item.ContextData != null;
        }

        private string GetHostName(string source)
        {
            source = source.Substring(source.IndexOf("Host=") + 5);
            source = source.Substring(0, source.IndexOf("/"));
            return source;
        }
    }

}
