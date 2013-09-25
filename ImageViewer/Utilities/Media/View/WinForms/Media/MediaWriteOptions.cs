using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Macro.Desktop.View.WinForms;
using Macro.Desktop;

namespace Macro.ImageViewer.Utilities.Media.View.WinForms
{
    public partial class MediaWriteOptions : ApplicationComponentUserControl
    {
        private MediaWriterOptionsComponent _component;

        public MediaWriteOptions(IApplicationComponent component)
        {
            InitializeComponent();

            _component = (MediaWriterOptionsComponent)component;

            this.UserStagingFolder.DataBindings.Add("Text", _component, "UserStagingFolder",
                true, DataSourceUpdateMode.OnPropertyChanged);

            this.IncludePortableWorkstation.DataBindings.Add("Checked", _component, "IncludePortableWorkstation",
                true, DataSourceUpdateMode.OnPropertyChanged);

            this.IncludeIdeographicNames.DataBindings.Add("Checked", _component, "IncludeIdeographicNames",
                true, DataSourceUpdateMode.OnPropertyChanged);

            this.IncludePhoneticNames.DataBindings.Add("Checked", _component, "IncludePhoneticNames",
                true, DataSourceUpdateMode.OnPropertyChanged);
            this.DeleteStagedFilesOnCompleted.DataBindings.Add("Checked", _component, "DeleteStagedFilesOnCompleted",
                true, DataSourceUpdateMode.OnPropertyChanged);
            this.VerifyMediaOnCompleted.DataBindings.Add("Checked", _component, "VerifyMediaOnCompleted",
                true, DataSourceUpdateMode.OnPropertyChanged);
            this.EjectMediaOnCompleted.DataBindings.Add("Checked", _component, "EjectMediaOnCompleted",
                true, DataSourceUpdateMode.OnPropertyChanged);

            this.Save.Click += delegate(object sender, EventArgs args)
            {
                _component.Save();
            };

            this.Cancel.Click += delegate(object sender, EventArgs e)
            {
                _component.Cancel();
            };

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonSpecifyDirectory.Checked)
            {
                this.UserStagingFolder.Enabled = true;
                this.BrowseFolder.Enabled = true;
            }
            else
            {
                this.UserStagingFolder.Enabled = false;
                this.BrowseFolder.Enabled = false;
            }
        }

        private void BrowseFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.UserStagingFolder.Text = folderBrowserDialog.SelectedPath;
            }
        }


    }
}
