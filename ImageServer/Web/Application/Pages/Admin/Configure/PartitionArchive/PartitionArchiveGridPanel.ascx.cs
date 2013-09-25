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
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Web.Application.Controls;
using Macro.ImageServer.Web.Common.Data;
using GridView=Macro.ImageServer.Web.Common.WebControls.UI.GridView;
using Macro.ImageServer.Model;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive
{
    /// <summary>
    /// Partition list view panel.
    /// </summary>
    public partial class PartitionArchiveGridPanel : GridViewPanel
    {
        #region Private Members

        /// <summary>
        /// list of partitions rendered on the screen.
        /// </summary>
        private IList<Model.PartitionArchive> _partitions;
        private Unit _height;
		private readonly PartitionArchiveConfigController _theController = new PartitionArchiveConfigController();

        #endregion private Members

        #region Public Properties

        /// <summary>
        /// Sets/Gets the list of partitions rendered on the screen.
        /// </summary>
        public IList<Model.PartitionArchive> Partitions
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
        public Model.PartitionArchive SelectedPartition
        {
            get
            {
                if (Partitions.Count == 0 || PartitionGridView.SelectedIndex < 0)
                    return null;
                
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
                else
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

            TheGrid = PartitionGridView;

            if (Height != Unit.Empty)
                ContainerTable.Height = _height;
        }

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			foreach (GridViewRow row in TheGrid.Rows)
			{
				if (row.RowType == DataControlRowType.DataRow)
				{
					Model.PartitionArchive partition = Partitions[row.RowIndex];

					if (partition != null)
					{
						if (_theController.CanDelete(partition))
							row.Attributes.Add("candelete", "true");
					}
				}
			}
		}

        #endregion Protected methods

        #region Public methods

        public void UpdateUI()
        {
            DataBind();
        }

        protected void PartitionGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (PartitionGridView.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Model.PartitionArchive pa = e.Row.DataItem as Model.PartitionArchive;
                    Label archiveTypeLabel = e.Row.FindControl("ArchiveType") as Label;
                    archiveTypeLabel.Text = ServerEnumDescription.GetLocalizedDescription(pa.ArchiveTypeEnum);

                    Label configXml = e.Row.FindControl("ConfigurationXML") as Label;
                    configXml.Text = XmlUtils.GetXmlDocumentAsString(pa.ConfigurationXml, true);

                    Image img = ((Image) e.Row.FindControl("EnabledImage"));
                    if (img != null)
                    {
                        img.ImageUrl = pa.Enabled
                                           ? ImageServerConstants.ImageURLs.Checked
                                           : ImageServerConstants.ImageURLs.Unchecked;
                    }

                    img = ((Image) e.Row.FindControl("ReadOnlyImage"));
                    if (img != null)
                    {
                        img.ImageUrl = pa.ReadOnly
                                           ? ImageServerConstants.ImageURLs.Checked
                                           : ImageServerConstants.ImageURLs.Unchecked;
                    }
                }
            }
        }

        #endregion Public methods
    }
}