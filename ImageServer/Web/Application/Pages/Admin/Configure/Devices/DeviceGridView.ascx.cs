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
using System.Web.UI;
using System.Web.UI.WebControls;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Application.Controls;
using Resources;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Configure.Devices
{
    //
    //  Used to display the list of devices.
    //
    public partial class DeviceGridView : GridViewPanel
    {
        #region private members

        // server partitions lookup table based on server key
        // list of devices to display
        private IList<Device> _devices;
        private Dictionary<string, ServerPartition> _dictionaryPartitions = new Dictionary<string, ServerPartition>();
        private Unit _height;

        #endregion Private members

        #region protected properties

        protected Dictionary<string, ServerPartition> DictionaryPartitions
        {
            get { return _dictionaryPartitions; }
            set { _dictionaryPartitions = value; }
        }

        #endregion protected properties

        #region public properties

        /// <summary>
        /// Gets/Sets the current selected device.
        /// </summary>
        public Device SelectedDevice
        {
            get
            {
                if (Devices.Count == 0 || GridView1.SelectedIndex < 0)
                    return null;

                // SelectedIndex is for the current page. Must convert to the index of the entire list
                int index = GridView1.PageIndex*GridView1.PageSize + GridView1.SelectedIndex;

                if (index < 0 || index > Devices.Count - 1)
                    return null;

                return Devices[index];
            }
        }

        /// <summary>
        /// Gets/Sets the list of devices rendered on the screen.
        /// </summary>
        public IList<Device> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                GridView1.DataSource = _devices; // must manually call DataBind() later
            }
        }


        /// <summary>
        /// Gets/Sets the height of device list panel.
        /// </summary>
        public Unit Height
        {
            get { return ContainerTable != null ? ContainerTable.Height : _height; }
            set
            {
                _height = value;
                if (ContainerTable != null)
                    ContainerTable.Height = value;
            }
        }

        #endregion

        #region protected methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TheGrid = GridView1;

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (GridView1.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    CustomizeActiveColumn(e);
                    CustomizeIpAddressColumn(e);
                    CustomizeDHCPColumn(e);
                    CustomizeServerPartitionColumn(e);
                    CustomizeFeaturesColumn(e);
                }
            }
        }

        protected void CustomizeFeaturesColumn(GridViewRowEventArgs e)
        {
            var placeHolder = e.Row.FindControl("FeaturePlaceHolder") as PlaceHolder;

            if (placeHolder != null)
            {
                // add an image for each enabled feature
                AddAllowStorageImage(e, placeHolder);
                AddAllowAutoRouteImage(e, placeHolder);
                AddAllowRetrieveImage(e, placeHolder);
                AddAllowQueryImage(e, placeHolder);
            }
        }

        private void AddAllowRetrieveImage(GridViewRowEventArgs e, PlaceHolder placeHolder)
        {
            Image img;
            img = new Image();
            if (Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "AllowRetrieve")))
            {
                img = new Image();
                img.ImageUrl = ImageServerConstants.ImageURLs.RetrieveFeature;
                img.AlternateText = Tooltips.DeviceFeatures_Retrieve;
            }
            else
            {
                img.ImageUrl = ImageServerConstants.ImageURLs.Blank;
                img.AlternateText = string.Empty;
            }
            placeHolder.Controls.Add(img);
        }

        private void AddAllowQueryImage(GridViewRowEventArgs e, PlaceHolder placeHolder)
        {
            Image img;
            img = new Image();
            if (Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "AllowQuery")))
            {
                img = new Image();
                img.ImageUrl = ImageServerConstants.ImageURLs.QueryFeature;
                img.AlternateText = Tooltips.DeviceFeatures_Query;
            }
            else
            {
                img.ImageUrl = ImageServerConstants.ImageURLs.Blank;
                img.AlternateText = string.Empty;
            }
            placeHolder.Controls.Add(img);
        }

        private void AddAllowStorageImage(GridViewRowEventArgs e, PlaceHolder placeHolder)
        {
            var img = new Image();
            if (Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "AcceptKOPR")))
            {
                img.ImageUrl = ImageServerConstants.ImageURLs.AcceptKOPRFeature;
                img.AlternateText = Tooltips.DeviceFeatures_AcceptKOPRFeature;
            }
            else if (Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "AllowStorage")))
            {
                img.ImageUrl = ImageServerConstants.ImageURLs.StoreFeature;
                img.AlternateText = Tooltips.DeviceFeatures_Store;
            }
            else
            {
                img.ImageUrl = ImageServerConstants.ImageURLs.Blank;
                img.AlternateText = string.Empty;
            }
            placeHolder.Controls.Add(img);
        }

        private void AddAllowAutoRouteImage(GridViewRowEventArgs e, PlaceHolder placeHolder)
        {
            var img = new Image();
            if (Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "AllowAutoRoute")))
            {
                img.ImageUrl = ImageServerConstants.ImageURLs.AutoRouteFeature;
                img.AlternateText = Tooltips.DeviceFeatures_AutoRoute;
            }
            else
            {
                //img.Visible = false;
                img.ImageUrl = ImageServerConstants.ImageURLs.Blank;
                img.AlternateText = string.Empty;
            }
            placeHolder.Controls.Add(img);
        }

        protected void CustomizeDHCPColumn(GridViewRowEventArgs e)
        {
            var img = ((Image) e.Row.FindControl("DHCPImage"));
            if (img != null)
            {
                bool active = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "DHCP"));
                if (active)
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Checked;
                }
                else
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Unchecked;
                }
            }
        }

        protected void CustomizeActiveColumn(GridViewRowEventArgs e)
        {
            var img = ((Image) e.Row.FindControl("ActiveImage"));

            if (img != null)
            {
                bool active = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "Enabled"));
                if (active)
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Checked;
                }
                else
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Unchecked;
                }
            }
        }

        // Display the Partition Description in Server Partition column
        protected void CustomizeServerPartitionColumn(GridViewRowEventArgs e)
        {
            var dev = e.Row.DataItem as Device;
            var lbl = e.Row.FindControl("ServerParitionLabel") as Label; // The label is added in the template
            if(lbl != null && dev != null) lbl.Text = dev.ServerPartition.AeTitle;
        }

        // Display the Partition Description in Server Partition column
        protected void CustomizeIpAddressColumn(GridViewRowEventArgs e)
        {
            var dev = e.Row.DataItem as Device;
            var lbl = e.Row.FindControl("IpAddressLabel") as Label; // The label is added in the template
            if(lbl != null && dev != null) lbl.Text = dev.IpAddress;
        }

        protected void GridView1_PageIndexChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            Refresh();
        }

        #endregion
    }
}