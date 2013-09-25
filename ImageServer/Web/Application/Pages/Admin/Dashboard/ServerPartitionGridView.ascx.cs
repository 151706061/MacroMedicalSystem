#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Web.Application.Controls;
using Macro.ImageServer.Web.Common.Data;
using Resources;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Dashboard
{
    /// <summary>
    /// Partition list view panel.
    /// </summary>
    public partial class ServerPartitionGridView : GridViewPanel
    {
        #region Private Members

        /// <summary>
        /// list of partitions rendered on the screen.
        /// </summary>
        private IList<ServerPartition> _partitions;
        private Unit _height;
		private readonly ServerPartitionConfigController _theController = new ServerPartitionConfigController();
        #endregion private Members

        #region Public Properties

        /// <summary>
        /// Sets/Gets the list of partitions rendered on the screen.
        /// </summary>
        public IList<ServerPartition> Partitions
        {
            get { return _partitions; }
            set
            {
                _partitions = value;
                PartitionGridView.DataSource = _partitions;
            }
        }

        /// <summary>
        /// Retrieve the current selected partition.
        /// </summary>
        public ServerPartition SelectedPartition
        {
            get
            {
                TheGrid.DataBind();
                int index = TheGrid.PageIndex*TheGrid.PageSize + TheGrid.SelectedIndex;

                if (index < 0 || index >= Partitions.Count)
                    return null;

                return Partitions[index];
            }
        }

        /// <summary>
        /// Gets/Sets the height of server partition list panel.
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

        #endregion Public Properties

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var criteria = new ServerPartitionSelectCriteria();
            criteria.AeTitle.SortAsc(0);
            Partitions = _theController.GetPartitions(criteria);

            TheGrid = PartitionGridView;

            GridPagerTop.InitializeGridPager(SR.GridPagerPartitionSingleItem, SR.GridPagerPartitionMultipleItems, TheGrid,
                                             () => Partitions.Count, ImageServerConstants.GridViewPagerPosition.Top);
            Pager = GridPagerTop;
            GridPagerTop.Reset();

            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

            DataBind();
        }

		protected override void OnPreRender(EventArgs e)
		{
			foreach (GridViewRow row in TheGrid.Rows)
			{
				if (row.RowType == DataControlRowType.DataRow)
				{
					ServerPartition partition = Partitions[row.RowIndex];

					if (partition != null)
					{
						if (_theController.CanDelete(partition))
							row.Attributes.Add("candelete", "true");
					}
				}
			}
			base.OnPreRender(e);
		}

        protected void PartitionGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (PartitionGridView.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    CustomizeActiveColumn(e);
                    CustomizeAcceptAnyDeviceColumn(e);
                    CustomizeDuplicateSopPolicyColumn(e.Row);
                    CustomizePartitionStorageConfiguration(e.Row);
                }
            }
        }

        private void CustomizePartitionStorageConfiguration(GridViewRow row)
        {
            ServerPartition partition = row.DataItem as ServerPartition;
            Label lbl = row.FindControl("PartitionStorageConfigurationLabel") as Label; // The label is added in the template
            if (lbl != null)
            {
                if (partition.HasEnabledDeleteRules)
                    lbl.Text = Resources.SR.PartitionStorageConfiguration_DeleteRuleExists;
                else
                {
                    lbl.Text = partition.ArchiveExists
                            ? Resources.SR.PartitionStorageConfiguration_ArchiveConfiguredNoDeleteRule
                            : Resources.SR.PartitionStorageConfiguration_NoArchiveConfiguredNoDeleteRule;
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

        protected void CustomizeAcceptAnyDeviceColumn(GridViewRowEventArgs e)
        {
            var img = ((Image) e.Row.FindControl("AcceptAnyDeviceImage"));

            if (img != null)
            {
                bool acceptAnyDevice = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "AcceptAnyDevice"));
                if (acceptAnyDevice)
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Checked;
                }
                else
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Unchecked;
                }
            }
        }

        private void CustomizeDuplicateSopPolicyColumn(GridViewRow row)
        {
            var partition = row.DataItem as ServerPartition;
            var lbl = row.FindControl("DuplicateSopDescription") as Label; // The label is added in the template
            lbl.Text = partition.DuplicateSopPolicyEnum.Description;
        }

        #endregion Protected methods

        #region Public methods

        public void UpdateUI()
        {
            Refresh();
        }

        #endregion Public methods
    }
}