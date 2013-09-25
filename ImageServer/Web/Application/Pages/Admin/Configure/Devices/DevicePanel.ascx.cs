#region License

// Copyright (c) 2012, ClearCanvas Inc.
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
using AjaxControlToolkit;
using Macro.ImageServer.Core.Query;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Resources;
using Macro.ImageServer.Web.Application.Helpers;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.WebControls.UI;

[assembly:
    WebResource("Macro.ImageServer.Web.Application.Pages.Admin.Configure.Devices.DevicePanel.js",
        "application/x-javascript")]

namespace Macro.ImageServer.Web.Application.Pages.Admin.Configure.Devices
{
    /// <summary>
    /// Panel to display list of devices for a particular server partition.
    /// </summary>
    [ClientScriptResource(
        ComponentType = "Macro.ImageServer.Web.Application.Pages.Admin.Configure.Devices.DevicePanel",
        ResourcePath = "Macro.ImageServer.Web.Application.Pages.Admin.Configure.Devices.DevicePanel.js")]    
    public partial class DevicePanel : AJAXScriptControl
    {
        #region Private members

        // the controller used for interaction with the database.
        private DeviceConfigurationController _theController;
        // the partition whose information will be displayed in this panel

        #endregion Private members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteDeviceButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("EditButtonClientID")]
        public string EditButtonClientID
        {
            get { return EditDeviceButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("DeviceListClientID")]
        public string DeviceListClientID
        {
            get { return DeviceGridViewControl1.TheGrid.ClientID; }
        }


        /// <summary>
        /// Sets/Gets the partition whose information is displayed in this panel.
        /// </summary>
        public ServerPartition ServerPartition { get; set; }

        public Default EnclosingPage { get; set; }

        #endregion

        #region Protected Methods

        protected void Clear()
        {
            AETitleFilter.Text = string.Empty;
            IPAddressFilter.Text = string.Empty;
            StatusFilter.SelectedIndex = 0;
            DHCPFilter.SelectedIndex = 0;
        }

        internal void Reset()
        {
            Clear();
            DeviceGridViewControl1.Reset();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // initialize the controller
            _theController = new DeviceConfigurationController();

            // setup child controls
            GridPagerTop.InitializeGridPager(SR.GridPagerDeviceSingleItem, SR.GridPagerDeviceMultipleItems,
                                             DeviceGridViewControl1.TheGrid,
                                             () => DeviceGridViewControl1.Devices.Count,
                                             ImageServerConstants.GridViewPagerPosition.Top);
            DeviceGridViewControl1.Pager = GridPagerTop;
            GridPagerTop.Reset();

            StatusFilter.Items.Add(new ListItem(SR.All));
            StatusFilter.Items.Add(new ListItem(SR.Enabled));
            StatusFilter.Items.Add(new ListItem(SR.Disabled));

            DHCPFilter.Items.Add(new ListItem(SR.All));
            DHCPFilter.Items.Add(new ListItem(SR.DHCP));
            DHCPFilter.Items.Add(new ListItem(SR.NoDHCP));
        }

        /// <summary>
        /// Determines if filters are being specified.
        /// </summary>
        /// <returns></returns>
        protected bool HasFilters()
        {
            return AETitleFilter.Text.Length > 0 || IPAddressFilter.Text.Length > 0 || StatusFilter.SelectedIndex > 0 ||
                   DHCPFilter.SelectedIndex > 0;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Refresh();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // This make sure we have the list to work with. 
            // the list may be out-dated if the add/update event is fired later
            // In those cases, the list must be refreshed again.
            LoadDevices();

            IList<DeviceTypeEnum> deviceTypes = DeviceTypeEnum.GetAll();

            if (DeviceTypeFilter.Items.Count == 0)
            {
                foreach (DeviceTypeEnum t in deviceTypes)
                {
                    DeviceTypeFilter.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(t), t.Lookup));
                }
            }
            else
            {
                var typeItems = new ListItem[DeviceTypeFilter.Items.Count];
                DeviceTypeFilter.Items.CopyTo(typeItems, 0);
                DeviceTypeFilter.Items.Clear();
                int count = 0;
                foreach (DeviceTypeEnum t in deviceTypes)
                {
                    DeviceTypeFilter.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(t), t.Lookup));
                    DeviceTypeFilter.Items[count].Selected = typeItems[count].Selected;
                    count++;
                }
            }
        }

        #endregion Protected methods

        /// <summary>
        /// Load the devices for the partition based on the filters specified in the filter panel.
        /// </summary>
        /// <remarks>
        /// This method only reloads and binds the list bind to the internal grid. <seealso cref="Refresh"/> should be called
        /// to explicit update the list in the grid. 
        /// <para>
        /// This is intentionally so that the list can be reloaded so that it is available to other controls during postback.  In
        /// some cases we may not want to refresh the list if there's no change. Calling <seealso cref="Refresh"/> will
        /// give performance hit as the data will be transfered back to the browser.
        ///  
        /// </para>
        /// </remarks>
        public void LoadDevices()
        {
            var criteria = new DeviceSelectCriteria();

            // only query for device in this partition
            criteria.ServerPartitionKey.EqualTo(ServerPartition.GetKey());

            if (!String.IsNullOrEmpty(AETitleFilter.Text))
            {
                QueryHelper.SetGuiStringCondition(criteria.AeTitle,
                                                  SearchHelper.LeadingAndTrailingWildCard(AETitleFilter.Text));
            }

            if (!String.IsNullOrEmpty(DescriptionFilter.Text))
            {
                QueryHelper.SetGuiStringCondition(criteria.Description,
                                                  SearchHelper.LeadingAndTrailingWildCard(DescriptionFilter.Text));
            }

            if (!String.IsNullOrEmpty(IPAddressFilter.Text))
            {
                QueryHelper.SetGuiStringCondition(criteria.IpAddress,
                                                  SearchHelper.TrailingWildCard(IPAddressFilter.Text));
            }

            if (StatusFilter.SelectedIndex != 0)
            {
                criteria.Enabled.EqualTo(StatusFilter.SelectedIndex == 1);
            }

            if (DHCPFilter.SelectedIndex != 0)
            {
                criteria.Dhcp.EqualTo(DHCPFilter.SelectedIndex == 1);
            }

            if (DeviceTypeFilter.SelectedIndex > -1)
            {
                var types = new List<DeviceTypeEnum>();
                foreach (ListItem item in DeviceTypeFilter.Items)
                {
                    if (item.Selected)
                    {
                        types.Add(DeviceTypeEnum.GetEnum(item.Value));
                    }
                }
                criteria.DeviceTypeEnum.In(types);
            }

            DeviceGridViewControl1.Devices = _theController.GetDevices(criteria);
            DeviceGridViewControl1.RefreshCurrentPage();
        }

        /// <summary>
        /// Updates the device list window in the panel.
        /// </summary>
        /// <remarks>
        /// This method should only be called when necessary as the information in the list window needs to be transmitted back to the client.
        /// If the list is not changed, call <seealso cref="LoadDevices()"/> instead.
        /// </remarks>
        public void Refresh()
        {
            LoadDevices();
            Device dev = DeviceGridViewControl1.SelectedDevice;

            if (dev == null)
            {
                // no device being selected
                EditDeviceButton.Enabled = false;
                DeleteDeviceButton.Enabled = false;
            }
            else
            {
                EditDeviceButton.Enabled = true;
                DeleteDeviceButton.Enabled = true;
            }

            SearchUpdatePanel.Update();
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            LoadDevices();
        }

        protected void AddDeviceButton_Click(object sender, ImageClickEventArgs e)
        {
            EnclosingPage.OnAddDevice(_theController, ServerPartition);
        }

        protected void EditDeviceButton_Click(object sender, ImageClickEventArgs e)
        {
            Device dev = DeviceGridViewControl1.SelectedDevice;
            if (dev != null)
            {
                EnclosingPage.OnEditDevice(_theController, ServerPartition, dev);
            }
        }

        protected void DeleteDeviceButton_Click(object sender, ImageClickEventArgs e)
        {
            Device dev = DeviceGridViewControl1.SelectedDevice;
            if (dev != null)
            {
                EnclosingPage.OnDeleteDevice(_theController, ServerPartition, dev);
            }
        }
    }
}