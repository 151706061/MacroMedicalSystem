#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Resources;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive
{
    //
    // Dialog for adding new Partition.
    //
    public partial class AddEditPartitionDialog : UserControl
    {
        #region Private Members

        private bool _editMode;
        private Model.PartitionArchive _partitionArchive;
    	private ServerPartition _partition;

    	#endregion

        #region Public Properties

        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                _editMode = value;
                ViewState[ "EditMode"] = value;
            }
        }

        /// <summary>
        /// Sets/Gets the current editing partition.
        /// </summary>
        public Model.PartitionArchive PartitionArchive
        {
            set
            {
                _partitionArchive = value;
                if (_partitionArchive != null && _partitionArchive.Key != null)
                    ViewState[ "EdittedPartitionArchive"] = _partitionArchive.GetKey();
            }
            get { return _partitionArchive; }
        }

		/// <summary>
		/// Sets the list of partitions users allowed to pick.
		/// </summary>
		public ServerPartition Partition
		{
			set
			{
				_partition = value;
				ViewState[ "ServerPartition"] = value;
			}

			get { return _partition; }
		}

        #endregion // public members

        #region Events

        /// <summary>
        /// Defines the event handler for <seealso cref="OKClicked"/>.
        /// </summary>
        /// <param name="partition">The partition being added.</param>
        public delegate void OnOKClickedEventHandler(Model.PartitionArchive partition);

        /// <summary>
        /// Occurs when users click on "OK".
        /// </summary>
        public event OnOKClickedEventHandler OKClicked;

        #endregion Events

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                if (ViewState[ "EditMode"] != null)
                    _editMode = (bool) ViewState[ "EditMode"];

                if (ViewState[ "EdittedPartitionArchive"] != null)
                {
                    ServerEntityKey partitionKey = ViewState[ "EdittedPartitionArchive"] as ServerEntityKey;
                    _partitionArchive = Model.PartitionArchive.Load(partitionKey);
                }

				if (ViewState[ "ServerPartition"] != null)
					_partition = (ServerPartition)ViewState[ "ServerPartition"];

            }

            ArchiveTypeDropDownList.Items.Clear();

            foreach (ArchiveTypeEnum archiveTypeEnum in ArchiveTypeEnum.GetAll())
            {
                ArchiveTypeDropDownList.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(archiveTypeEnum), archiveTypeEnum.Lookup));
            }
            
        }


        /// <summary>
        /// Handles event when user clicks on "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OKButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveData();

                if (OKClicked != null)
                    OKClicked(PartitionArchive);

                Close();
            }
            else
            {
                Show(false);
            }
        }

        /// <summary>
        /// Handles event when user clicks on "Cancel" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        

        protected void UpdateUI()
        {
            // Update the title and OK button text. Changing the image is the only way to do this, since the 
            // SkinID cannot be set dynamically after Page_PreInit.
            if (EditMode)
            {
                ModalDialog.Title = Titles.EditPartitionArchiveTitle;
                OKButton.Visible = false;
                UpdateButton.Visible = true;
            }
            else
            {
                ModalDialog.Title = Titles.AddPartitionArchiveTitle;
                OKButton.Visible = true;
                UpdateButton.Visible = false;
            }

            if (PartitionArchive == null)
            {
                Description.Text = SR.AdminPartitionArchive_AddEditDialog_ArchiveNameDefaultText;
                ArchiveDelay.Text = "12";
                EnabledCheckBox.Checked = true;
                ReadOnlyCheckBox.Checked = false;
                ArchiveTypeDropDownList.SelectedIndex = 0;
                ConfigurationXML.Text = "<HsmArchive>\n\t<RootDir>C:\\ImageServer\\Archive</RootDir>\n</HsmArchive>";
            }
            else if (Page.IsValid)
            {
                Description.Text = PartitionArchive.Description;
                ArchiveDelay.Text = PartitionArchive.ArchiveDelayHours.ToString();
                EnabledCheckBox.Checked = PartitionArchive.Enabled;
                ReadOnlyCheckBox.Checked = PartitionArchive.ReadOnly;
                ArchiveTypeDropDownList.SelectedValue = PartitionArchive.ArchiveTypeEnum.Lookup;
                ConfigurationXML.Text = XmlUtils.GetXmlDocumentAsString(PartitionArchive.ConfigurationXml, false);
            }
        }


        #region Private Methods


        private void SaveData()
        {
            if (PartitionArchive == null)
            {
                PartitionArchive = new Model.PartitionArchive();
            }

            PartitionArchive.ServerPartitionKey = Partition.Key;
            PartitionArchive.Description = Description.Text;
            PartitionArchive.ArchiveDelayHours = int.Parse(ArchiveDelay.Text);

            PartitionArchive.ConfigurationXml = new XmlDocument();
            if (ConfigurationXML.Text != string.Empty)
            {
                PartitionArchive.ConfigurationXml.Load(new StringReader(ConfigurationXML.Text));
            }
            else
            {
                PartitionArchive.ConfigurationXml.Load(new StringReader(ImageServerConstants.DefaultConfigurationXml));                
            }
            
            PartitionArchive.Enabled = EnabledCheckBox.Checked;
            PartitionArchive.ReadOnly = ReadOnlyCheckBox.Checked;
            PartitionArchive.ArchiveTypeEnum = ArchiveTypeEnum.GetEnum(ArchiveTypeDropDownList.SelectedValue);
        }

        #endregion Private Methods

        #endregion Protected methods

        #region Public Methods

        /// <summary>
        /// Displays the add device dialog box.
        /// </summary>
        public void Show(bool updateUI)
        {
            if (updateUI)
                UpdateUI();
            else
            {
                if (EditMode)
                {
                    ModalDialog.Title = Titles.EditPartitionArchiveTitle;
                }
                else
                {
                    ModalDialog.Title = Titles.AddPartitionArchiveTitle;
                }
            }


            if (Page.IsValid)
            {
//                ServerPartitionTabContainer.ActiveTabIndex = 0;
            }


            ModalDialog.Show();
        }

        /// <summary>
        /// Dismisses the dialog box.
        /// </summary>
        public void Close()
        {
            ModalDialog.Hide();   
        }

        #endregion Public methods
    }
}
