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
using AjaxControlToolkit;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Common;

namespace Macro.ImageServer.Web.Application.Controls
{
    public partial class ServerPartitionTabs : UserControl
    {
        #region Delegates

        public delegate UserControl GetTabPanel(ServerPartition partition);

        #endregion

        #region Private members

        // Map between the server partition and the panel
        private readonly IDictionary<ServerEntityKey, Control> _mapPanel = new Dictionary<ServerEntityKey, Control>();

        private IList<ServerPartition> _partitionList;

        #endregion

        #region Public Properties

        public IList<ServerPartition> ServerPartitionList
        {
            get
            {
                if (_partitionList == null)
                {
                    _partitionList = LoadPartitions();
                }

                return _partitionList;
            }
            set { _partitionList = value; }
        }

        #endregion

        #region Private Methods

        private IList<ServerPartition> LoadPartitions()
        {
            try
            {
                var xp = new ServerPartitionTabsExtensionPoint();
                var extension = xp.CreateExtension() as IServerPartitionTabsExtension;

                if( extension!=null)
                {
                    return new List<ServerPartition>(extension.LoadServerPartitions());
                }
            }
            catch(Exception ex)
            {
                
            }

            var defaultImpl = new DefaultServerPartitionTabsExtension();
            return new List<ServerPartition>(defaultImpl.LoadServerPartitions());
        }

        #endregion

        #region Public Methods

        public void UpdateCurrentPartition()
        {
            Update(true);
        }

        public ServerEntityKey GetCurrentPartitionKey()
        {
            ServerPartition partition = GetCurrentPartition();

            return partition != null ? partition.Key : null;
        }

        public ServerPartition GetCurrentPartition()
        {
            if (_partitionList == null || _partitionList.Count == 0)
                return null;

            ServerPartition partition = _partitionList[PartitionTabContainer.ActiveTabIndex];

            return partition;
        }

        public Control GetUserControlForCurrentPartition()
        {
            return GetUserControlForPartition(GetCurrentPartitionKey());
        }

        public Control GetUserControlForPartition(ServerEntityKey key)
        {
            if (_mapPanel.ContainsKey(key))
                return _mapPanel[key];

            return null;
        }

        public void SetupLoadPartitionTabs(GetTabPanel tabDelegate)
        {
            if(ServerPartitionList == null || ServerPartitionList.Count == 0)
            {
                NoPartitionPanel.Visible = true;
                PartitionPanel.Visible = false;
                return;
            }

            NoPartitionPanel.Visible = false;
            PartitionPanel.Visible = true;

            int n = 0;

            this.PartitionTabContainer.Tabs.Clear();
            
            foreach (ServerPartition part in ServerPartitionList)
            {
                n++;

                // create a tab
                TabPanel tabPanel = new TabPanel();
                tabPanel.HeaderText = part.AeTitle;
                tabPanel.ID = "Tab_" + n;

                if (tabDelegate != null)
                {
                    // create a panel for the control 
                    UserControl panel = tabDelegate(part);

                    // wrap an updatepanel around the tab
                    UpdatePanel updatePanel = new UpdatePanel();
                    updatePanel.ContentTemplateContainer.Controls.Add(panel);

                    // put the panel into a lookup table to be used later
                    _mapPanel[part.GetKey()] = updatePanel;


                    // Add the device panel into the tab
                    tabPanel.Controls.Add(updatePanel);
                }

                // Add the tab into the tabstrip
                PartitionTabContainer.Tabs.Add(tabPanel);
            }

            if (ServerPartitionList != null && ServerPartitionList.Count > 0)
            {
                PartitionTabContainer.ActiveTabIndex = 0;
            }
            else
            {
                PartitionTabContainer.ActiveTabIndex = -1;
            }
        }

        /// <summary>
        /// Update the specified partition tab
        /// </summary>
        /// <param name="key">The server partition key</param>
        /// <remarks>
        /// 
        /// </remarks>
        public void Update(ServerEntityKey key)
        {
            Control ctrl = GetUserControlForPartition(key);
            if (ctrl != null)
            {
                ctrl.DataBind();

                if (ctrl is UpdatePanel)
                {
                    UpdatePanel panel = ctrl as UpdatePanel;
                    if (panel.UpdateMode == UpdatePanelUpdateMode.Conditional)
                    {
                        panel.Update();
                    }
                }

            }
            

        }

        /// <summary>
        /// Update the specified partition tab
        /// </summary>
        /// <param name="tabIndex">The server partition tab index</param>
        /// <remarks>
        /// 
        /// </remarks>
        public void Update(int tabIndex)
        {
            if (_partitionList == null || _partitionList.Count == 0)
                return;

            ServerPartition partition = _partitionList[tabIndex];

            Update(partition.GetKey());
        }

        public void Update(bool current)
        {
            if (_partitionList == null || _partitionList.Count == 0)
                return;

            if(current)
            {
                Update(PartitionTabContainer.ActiveTabIndex);
            } else
            {
                for (int i = 0; i < PartitionTabContainer.Tabs.Count; i++)
                {
                    Update(i);
                }
            }
        }

        public void SetActivePartition(string aeTitle)
        {
            TabPanelCollection tabs = PartitionTabContainer.Tabs;

            foreach(TabPanel tab in tabs)
            {
                if(tab.HeaderText == aeTitle)
                {
                    PartitionTabContainer.ActiveTab = tab;
                }
            }
        }

        #endregion Public Methods
    }
}