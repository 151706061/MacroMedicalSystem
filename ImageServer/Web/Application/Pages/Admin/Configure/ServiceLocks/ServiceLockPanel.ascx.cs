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
using Macro.Common.Utilities;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Web.Application.Controls;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.WebControls.UI;
using Resources;

[assembly: WebResource("Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockPanel.js", "application/x-javascript")]
namespace Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks
{
    [ClientScriptResource(ComponentType = "Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockPanel", ResourcePath = "Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockPanel.js")]
    /// <summary>
    /// Panel to display list of devices for a particular server partition.
    /// </summary>
    public partial class ServiceLockPanel : AJAXScriptControl
    {
        #region Private members

        #endregion Private members

        #region Events

        public delegate void ServiceLockUpdatedListener(ServiceLock serviceLock);

        public event ServiceLockUpdatedListener ServiceLockUpdated;
        
        #endregion Events

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("EditButtonClientID")]
        public string EditButtonClientID
        {
            get { return EditServiceScheduleButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ServiceLockListClientID")]
        public string ServiceLockListClientID
        {
            get { return ServiceLockGridViewControl.TheGrid.ClientID; }
        }

        #endregion


        #region protected methods


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            IList<ServiceLockTypeEnum> types = ServiceLockTypeEnum.GetAll();
            TypeDropDownList.Items.Add(new ListItem(SR.All)); 
            foreach (ServiceLockTypeEnum t in types)
            {
                TypeDropDownList.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(t), t.Lookup));
            }

            EditServiceLockDialog.ServiceLockUpdated += AddEditServiceLockDialog_ServiceLockUpdated; 
            
            // setup child controls
            GridPagerTop.InitializeGridPager(SR.GridPagerServiceSingleItem, SR.GridPagerServiceMultipleItems, ServiceLockGridViewControl.TheGrid, delegate { return ServiceLockGridViewControl.ServiceLocks != null ? ServiceLockGridViewControl.ServiceLocks.Count : 0; }, ImageServerConstants.GridViewPagerPosition.Top);
            ServiceLockGridViewControl.Pager = GridPagerTop;
           
            StatusFilter.Items.Add(new ListItem(SR.All));
            StatusFilter.Items.Add(new ListItem(SR.Enabled));
            StatusFilter.Items.Add(new ListItem(SR.Disabled));

            FileSystemsConfigurationController fileSystemController = new FileSystemsConfigurationController();
            IList<Model.Filesystem> fileSystems = fileSystemController.GetAllFileSystems();

            foreach(Model.Filesystem fs in fileSystems)
            {
                FileSystemFilter.Items.Add(new ListItem(fs.Description, fs.Key.ToString()));
            }
            
            ConfirmEditDialog.Confirmed += ConfirmEditDialog_Confirmed;

        }

        
        protected override void OnPreRender(EventArgs e)
        {
            UpdateToolbarButtons();
            UpdateListPanel();
            base.OnPreRender(e);
            
        }



        protected void UpdateListPanel()
        {
            ServiceLockGridViewControl.RefreshGridPanel();
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            LoadServiceLocks();
        }


        protected void EditServiceScheduleButton_Click(object sender, ImageClickEventArgs e)
        {
            LoadServiceLocks();
            ServiceLock service = ServiceLockGridViewControl.SelectedServiceLock;
            if (service != null)
            {
                EditServiceLock(service);
            }
        }



        protected void RefreshButton_Click(object sender, ImageClickEventArgs e)
        {
            LoadServiceLocks();
        }

        #endregion Protected methods


        #region Private Methods
        void AddEditServiceLockDialog_ServiceLockUpdated(ServiceLock serviceLock)
        {
            DataBind();
            if (ServiceLockUpdated != null)
                ServiceLockUpdated(serviceLock);
        }

        void ConfirmEditDialog_Confirmed(object data)
        {
            ShowEditServiceLockDialog();
        }



        private void EditServiceLock(ServiceLock service)
        {
            EditServiceLockDialog.ServiceLock = service;

            if (service != null)
            {
                if (service.Lock)
                {
                    ConfirmEditDialog.Message = SR.ServiceLockUpdate_Confirm_ServiceIsLocked;
                    ConfirmEditDialog.MessageType =
                        MessageBox.MessageTypeEnum.YESNO;
                    ConfirmEditDialog.Show();
                }
                else
                {
                    ShowEditServiceLockDialog();
                }

            }


        }


        private void ShowEditServiceLockDialog()
        {
            EditServiceLockDialog.Show();
        }


        #endregion Private Methods


        #region Public methods

        public override void DataBind()
        {
            LoadServiceLocks();
            base.DataBind();
        }

        /// <summary>
        /// Load the devices for the partition based on the filters specified in the filter panel.
        /// </summary>
        /// <remarks>
        /// This method only reloads and binds the list bind to the internal grid.
        /// </remarks>
        public void LoadServiceLocks()
        {
            ServiceLockSelectCriteria criteria = new ServiceLockSelectCriteria();

            ServiceLockConfigurationController controller = new ServiceLockConfigurationController();

            if (TypeDropDownList.SelectedValue != SR.All)
            {
                criteria.ServiceLockTypeEnum.EqualTo(ServiceLockTypeEnum.GetEnum(TypeDropDownList.SelectedValue));
            }
            
            if (StatusFilter.SelectedIndex != 0)
            {
                if (StatusFilter.SelectedIndex == 1)
                    criteria.Enabled.EqualTo(true);
                else
                    criteria.Enabled.EqualTo(false);
            }

            if (FileSystemFilter.SelectedIndex != -1)
            {
                FileSystemsConfigurationController fsController = new FileSystemsConfigurationController();
                List<ServerEntityKey> fileSystemKeys = new List<ServerEntityKey>();
                foreach (ListItem fileSystem in FileSystemFilter.Items)
                {
                    if (fileSystem.Selected)
                    {
                        fileSystemKeys.Add(new ServerEntityKey("FileSystem", fileSystem.Value));
                    }
                }

                IList<Model.Filesystem> fs = fsController.GetFileSystems(fileSystemKeys);

                if(fs != null)
                {
                    List<ServerEntityKey> entityKeys = new List<ServerEntityKey>();
                    foreach(Filesystem f in fs)
                    {
                        entityKeys.Add(f.Key);
                    }
                    criteria.FilesystemKey.In(entityKeys);    
                }                
            }

            IList<ServiceLock> services = controller.GetServiceLocks(criteria);

        	List<ServiceLock> sortedServices =
        		CollectionUtils.Sort(services, delegate(ServiceLock a, ServiceLock b)
        		                               	{
        		                               		if (a == null)
        		                               		{
        		                               			if (b == null)
        		                               			{
        		                               				// If both null, they're equal. 
        		                               				return 0;
        		                               			}
        		                               			else
        		                               			{
        		                               				// If x is null and y is not null, y
        		                               				// is greater. 
        		                               				return -1;
        		                               			}
        		                               		}
        		                               		else
        		                               		{
        		                               			// If a is not null...
        		                               			if (b == null)
        		                               			{
															// ...and b is null, x is greater.
															return 1;
        		                               			}
        		                               			else
        		                               			{
        		                               				// just compare
															if (a.Filesystem == null || b.Filesystem == null)
																return a.ServiceLockTypeEnum.Description.CompareTo(b.ServiceLockTypeEnum.Description);

															int retVal =
        		                               					a.Filesystem.Description.CompareTo(
        		                               						b.Filesystem.Description);
															if (retVal == 0)
																return a.ServiceLockTypeEnum.Description.CompareTo(b.ServiceLockTypeEnum.Description);
        		                               				return retVal;
        		                               			}
        		                               		}
        		                               	});

			
            ServiceLockCollection items = new ServiceLockCollection();
            items.Add(sortedServices);

            ServiceLockGridViewControl.ServiceLocks = items;

            ServiceLockGridViewControl.RefreshCurrentPage();
        }

        /// <summary>
        /// Updates the device list window in the panel.
        /// </summary>
        /// <remarks>
        /// This method should only be called when necessary as the information in the list window needs to be transmitted back to the client.
        /// If the list is not changed, call <seealso cref="LoadServiceLocks()"/> instead.
        /// </remarks>
        public void UpdateToolbarButtons()
        {

            ServiceLock service = ServiceLockGridViewControl.SelectedServiceLock;
            EditServiceScheduleButton.Enabled = service != null;

            ToolbarUpdatePanel.Update();
        }


        #endregion Public methods
    }
}