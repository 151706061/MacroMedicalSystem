﻿#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Application.Controls;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Configure.DataRules
{
    public partial class DataRuleGridView : GridViewPanel
    {
        #region private members

        // list of devices to display
        private Unit _height;
        private IList<ServerRule> _serverRules;

        #endregion Private members

        #region public properties

        /// <summary>
        /// Gets/Sets the server rule panel
        /// </summary>
        public DataRulePanel DataRulePanel { get; set; }

        /// <summary>
        /// Gets/Sets the height of the server rule list panel
        /// </summary>
        public Unit Height
        {
            get
            {
                return ContainerTable != null ? ContainerTable.Height : _height;
            }
            set
            {
                _height = value;
                if (ContainerTable != null)
                    ContainerTable.Height = value;
            }
        }

        /// <summary>
        /// Gets/Sets the current selected device.
        /// </summary>
        public ServerRule SelectedRule
        {
            get
            {
                if (DataRules == null || DataRules.Count == 0 || GridView.SelectedIndex < 0)
                    return null;

                // SelectedIndex is for the current page. Must convert to the index of the entire list
                int index = GridView.PageIndex * GridView.PageSize + GridView.SelectedIndex;

                if (index < 0 || index > DataRules.Count - 1)
                    return null;

                return DataRules[index];
            }
            set
            {
                GridView.SelectedIndex = DataRules.IndexOf(value);
                //  if (OnStudySelectionChanged != null)
                //     OnDeviceSelectionChanged(this, value);
            }
        }

        /// <summary>
        /// Gets/Sets the list of devices rendered on the screen.
        /// </summary>
        public IList<ServerRule> DataRules
        {
            get { return _serverRules; }
            set
            {
                _serverRules = value;
                GridView.DataSource = _serverRules; // must manually call DataBind() later
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

            TheGrid = GridView;
        }

        protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (GridView.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    CustomizeColumns(e);
                }
            }
        }

        protected void CustomizeColumns(GridViewRowEventArgs e)
        {
            var fs = e.Row.DataItem as ServerRule;
            var lbl = e.Row.FindControl("ServerRuleApplyTimeEnum") as Label; // The label is added in the template
            if (lbl != null && fs != null) 
                lbl.Text = ServerEnumDescription.GetLocalizedDescription(fs.ServerRuleApplyTimeEnum);

            lbl = e.Row.FindControl("ServerRuleTypeEnum") as Label; // The label is added in the template
            if (lbl != null && fs != null) 
                lbl.Text = ServerEnumDescription.GetLocalizedDescription(fs.ServerRuleTypeEnum);


            var img = ((Image)e.Row.FindControl("EnabledImage"));
            if (img != null)
            {
                img.ImageUrl = fs.Enabled ? ImageServerConstants.ImageURLs.Checked : ImageServerConstants.ImageURLs.Unchecked;
            }

            img = ((Image)e.Row.FindControl("DefaultImage"));
            if (img != null)
            {
                img.ImageUrl = fs.DefaultRule ? ImageServerConstants.ImageURLs.Checked : ImageServerConstants.ImageURLs.Unchecked;
            }
            img = ((Image)e.Row.FindControl("ExemptImage"));
            if (img != null)
            {
                img.ImageUrl = fs.ExemptRule ? ImageServerConstants.ImageURLs.Checked : ImageServerConstants.ImageURLs.Unchecked;
            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView.PageIndex = e.NewPageIndex;
            GridView.DataBind();
        }

        #endregion Protected Methods

        #region public methods

        /// <summary>
        /// Binds the list to the control.
        /// </summary>
        /// <remarks>
        /// This method must be called after setting <seeaslo cref="Devices"/> to update the grid with the list.
        /// </remarks>
        public override void DataBind()
        {
            GridView.DataBind();
        }

        #endregion // public methods
    }
}