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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Common;

namespace Macro.ImageServer.Web.Application.Controls
{
    public partial class ServerPartitionSelector : System.Web.UI.UserControl
    {
        #region Private Members
        private IList<ServerPartition> _partitionList;
        private ServerPartition _selectedPartition;
        private UpdatePanel _updatePanel;
        private readonly Dictionary<ServerEntityKey, LinkButton> _buttons = new Dictionary<ServerEntityKey, LinkButton>(); 
        #endregion

        #region Events/Delegates

        /// <summary>
        /// Defines the event handler for <seealso cref="PartitionChanged"/> event.
        /// </summary>
        /// <param name="thePartition">The selected </param>
        public delegate void PartitionChangedEvent(ServerPartition thePartition);

        /// <summary>
        /// Occurs when users click on a partition link
        /// </summary>
        public event PartitionChangedEvent PartitionChanged;

        #endregion

        public ServerPartition SelectedPartition
        {
            get
            {
                if (_selectedPartition == null)
                {
                    if (SelectedPartitionKey != null)
                    {
                        foreach (ServerPartition partition in ServerPartitionList)
                            if (partition.Key.Equals(SelectedPartitionKey))
                            {
                                _selectedPartition = partition;
                                break;
                            }
                    }
                    else
                    {
                        string aeTitle = Request["AETitle"];

                        if (aeTitle != null)
                        {
                            foreach (ServerPartition partition in ServerPartitionList)
                                if (partition.AeTitle == aeTitle)
                                {
                                    _selectedPartition = partition;
                                    SelectedPartitionKey = _selectedPartition.Key;
                                    break;
                                }
                        }
                    }

                    if (_selectedPartition == null)
                    {
                        _selectedPartition = ServerPartitionList.First();
                        SelectedPartitionKey = _selectedPartition.Key;
                    }
                }

                return _selectedPartition;
            }
            set
            {
                _selectedPartition = value;
                SelectedPartitionKey = _selectedPartition.Key;
            }
        }

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

        private ServerEntityKey SelectedPartitionKey
        {
            set
            {
                ViewState["SelectedPartitionKey"] = value;
            }
            get
            {
                return ViewState["SelectedPartitionKey"] as ServerEntityKey;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            foreach (ServerPartition partition in ServerPartitionList)
            {
                var button = new LinkButton
                                 {
                                     Text = partition.AeTitle,
                                     CommandArgument = partition.AeTitle,
                                     ID = partition.AeTitle + "_ID",
                                     CssClass = "PartitionLink"
                                 };
                button.Command += LinkClick;
                
                if (partition.Key.Equals(SelectedPartition.Key))
                {
                    button.Enabled = false;
                    button.CssClass = "PartitionLinkDisabled";
                }

                _buttons.Add(partition.Key, button);

                PartitionLinkPanel.Controls.Add(button);
                PartitionLinkPanel.Controls.Add(new LiteralControl(" "));
                if (_updatePanel != null)
                {
                    var trigger = new AsyncPostBackTrigger
                                      {
                                          ControlID = button.ID, 
                                          EventName = "Click"
                                      };
                    _updatePanel.Triggers.Add(trigger);
                }
            }
        }

        #region Public Methods
        internal void SetUpdatePanel(UpdatePanel updatePanel)
        {
            _updatePanel = updatePanel;
        }
        #endregion

        #region Private Methods

        protected void LinkClick(object sender, CommandEventArgs e)
        {
            foreach (ServerPartition partition in ServerPartitionList)
            {
                var button = _buttons[partition.Key];

                if (partition.AeTitle.Equals(e.CommandArgument.ToString()))
                {
                    SelectedPartition = partition;

                    if (PartitionChanged != null)
                        PartitionChanged(SelectedPartition);

                    button.Enabled = false;
                    button.CssClass = "PartitionLinkDisabled";
                }
                else
                {
                    button.Enabled = true;
                    button.CssClass = "PartitionLink";
                }
            }

            //if (_updatePanel != null)
            //    _updatePanel.Update();
        }

        private IList<ServerPartition> LoadPartitions()
        {
            try
            {
                var xp = new ServerPartitionTabsExtensionPoint();
                var extension = xp.CreateExtension() as IServerPartitionTabsExtension;

                if (extension != null)
                {
                    return new List<ServerPartition>(extension.LoadServerPartitions());
                }
            }
            catch (Exception)
            {

            }

            var defaultImpl = new DefaultServerPartitionTabsExtension();
            return new List<ServerPartition>(defaultImpl.LoadServerPartitions());
        }

        #endregion
    }
}