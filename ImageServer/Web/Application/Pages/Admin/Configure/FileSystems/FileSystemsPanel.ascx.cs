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
using AjaxControlToolkit;
using Macro.ImageServer.Core.Query;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Web.Application.Helpers;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.WebControls.UI;
using Resources;

[assembly: WebResource("Macro.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems.FileSystemsPanel.js", "application/x-javascript")]

namespace Macro.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems
{
    [ClientScriptResource(ComponentType = "Macro.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems.FileSystemsPanel", ResourcePath = "Macro.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems.FileSystemsPanel.js")]
    /// <summary>
    /// Panel to display list of FileSystems for a particular server partition.
    /// </summary>
    public partial class FileSystemsPanel : AJAXScriptControl
    {
        #region Private members

        // the controller used for interaction with the database.
        private FileSystemsConfigurationController _theController;
        // the filesystems whose information will be displayed in this panel
        private IList<Filesystem> _filesystems;
        // list of filesystem tiers users can filter on
        private IList<FilesystemTierEnum> _tiers;

        #endregion Private members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("EditButtonClientID")]
        public string EditButtonClientID
        {
            get { return EditFileSystemButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("FileSystemListClientID")]
        public string FileSystemListClientID
        {
            get { return FileSystemsGridView1.TheGrid.ClientID; }
        }

        /// <summary>
        /// Sets/Gets the filesystems whose information are displayed in this panel.
        /// </summary>
        public IList<Filesystem> FileSystems
        {
            get { return _filesystems; }
            set { _filesystems = value; }
        }

        /// <summary>
        /// Sets or gets the list of filesystems users can filter.
        /// </summary>
        public IList<FilesystemTierEnum> Tiers
        {
            get { return _tiers; }
            set { _tiers = value; }
        }

        private Default _enclosingPage;

        public Default EnclosingPage
        {
            get { return _enclosingPage; }
            set { _enclosingPage = value; }
        }

        #endregion

        #region protected methods

        protected override void OnPreRender(EventArgs e)
        {
            UpdateUI();
            base.OnPreRender(e);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // initialize the controller
            _theController = new FileSystemsConfigurationController();

            // setup child controls
            GridPagerTop.InitializeGridPager(SR.GridPagerFileSystemSingleItem, SR.GridPagerFileSystemMultipleItems, FileSystemsGridView1.TheGrid, delegate { return FileSystemsGridView1.FileSystems.Count; }, ImageServerConstants.GridViewPagerPosition.Top);
            FileSystemsGridView1.Pager = GridPagerTop;
            GridPagerTop.Reset();
                
            Tiers = _theController.GetFileSystemTiers();

            int prevSelectIndex = TiersDropDownList.SelectedIndex;
            if (TiersDropDownList.Items.Count == 0)
            {
                TiersDropDownList.Items.Add(new ListItem(SR.All));
                foreach (FilesystemTierEnum tier in Tiers)
                {
                    TiersDropDownList.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(tier), tier.Lookup));
                }
            }
            TiersDropDownList.SelectedIndex = prevSelectIndex;
        }

        #endregion Protected methods

        /// <summary>
        /// Load the FileSystems for the partition based on the filters specified in the filter panel.
        /// </summary>
        /// <remarks>
        /// This method only reloads and binds the list bind to the internal grid. <seealso cref="UpdateUI()"/> should be called
        /// to explicit update the list in the grid. 
        /// <para>
        /// This is intentionally so that the list can be reloaded so that it is available to other controls during postback.  In
        /// some cases we may not want to refresh the list if there's no change. Calling <seealso cref="UpdateUI()"/> will
        /// give performance hit as the data will be transfered back to the browser.
        ///  
        /// </para>
        /// </remarks>
        public void LoadFileSystems()
        {
            FilesystemSelectCriteria criteria = new FilesystemSelectCriteria();


            if (String.IsNullOrEmpty(DescriptionFilter.Text) == false)
            {
                QueryHelper.SetGuiStringCondition(criteria.Description,
                                      SearchHelper.TrailingWildCard(DescriptionFilter.Text));
            }

            if (TiersDropDownList.SelectedIndex >= 1) /* 0 = "All" */
                criteria.FilesystemTierEnum.EqualTo(Tiers[TiersDropDownList.SelectedIndex - 1]);

            FileSystemsGridView1.FileSystems = _theController.GetFileSystems(criteria);
            FileSystemsGridView1.RefreshCurrentPage();
        }

        /// <summary>
        /// Updates the FileSystem list window in the panel.
        /// </summary>
        /// <remarks>
        /// This method should only be called when necessary as the information in the list window needs to be transmitted back to the client.
        /// If the list is not changed, call <seealso cref="LoadFileSystems()"/> instead.
        /// </remarks>
        public void UpdateUI()
        {
            LoadFileSystems();

            Filesystem dev = FileSystemsGridView1.SelectedFileSystem;
            if (dev == null)
            {
                // no FileSystem being selected
                EditFileSystemButton.Enabled = false;
            }
            else
            {
                EditFileSystemButton.Enabled = true;
            }

            SearchUpdatePanel.Update();
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            //UpdateUI();
        }


        protected void AddFileSystemButton_Click(object sender, ImageClickEventArgs e)
        {
            EnclosingPage.OnAddFileSystem();
        }

        protected void EditFileSystemButton_Click(object sender, ImageClickEventArgs e)
        {
            // Call the edit filesystem delegate 
            LoadFileSystems();
            Filesystem fs = FileSystemsGridView1.SelectedFileSystem;
            if (fs != null)
            {
                EnclosingPage.OnEditFileSystem(_theController, fs);
            }
        }

        protected void RefreshButton_Click(object sender, ImageClickEventArgs e)
        {
            //UpdateUI();
        }
    }
}