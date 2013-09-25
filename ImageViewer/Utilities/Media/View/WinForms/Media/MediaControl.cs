using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Macro.Common.Media.IMAPI2;
using Macro.Desktop.View.WinForms;
using Macro.Desktop.Trees;
using System.Runtime.InteropServices;

namespace Macro.ImageViewer.Utilities.Media.View.WinForms
{
    public partial class MediaControl : ApplicationComponentUserControl
    {
        private MediaWriterComponent _component;

        public MediaControl(MediaWriterComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;
            SelectTreeView.DataBindings.Add("Tree", _component, "Tree", true, DataSourceUpdateMode.OnPropertyChanged);
            VolumeName.DataBindings.Add("Text", _component, "VolumeName", true, DataSourceUpdateMode.OnPropertyChanged);
            StagingFolder.DataBindings.Add("Text", _component, "StagingFolderPath", true, DataSourceUpdateMode.OnPropertyChanged);
            MediaWriter.DataBindings.Add("DataSource", _component, "MediaWriters", true, DataSourceUpdateMode.OnPropertyChanged);
            studylabel.DataBindings.Add("Text", _component, "NumberOfStudies", true, DataSourceUpdateMode.OnPropertyChanged);
            DiscInfo.DataBindings.Add("Text", _component, "CurrentMediaDescription", true, DataSourceUpdateMode.OnPropertyChanged);
            Caption.DataBindings.Add("Text", _component, "RequiredMediaSpace", true, DataSourceUpdateMode.OnPropertyChanged);
            BurningInfo.DataBindings.Add("Text", _component, "CurrentWriteStageName", true, DataSourceUpdateMode.OnPropertyChanged);
            BurnProgressBar.DataBindings.Add("Value", _component, "CurrentWriteStagePercent", true, DataSourceUpdateMode.OnPropertyChanged);
            CaptionInfo.DataBindings.Add("Value", _component, "CurrentMediaSpacePercent", true, DataSourceUpdateMode.OnPropertyChanged);

            Write.Click += delegate(object sender, EventArgs args)
            {
                _component.WriteMedia();
            };

            Cancel.Click += delegate(object sender, EventArgs e)
            {
                _component.Cancel();
            };

            Eject.Click += delegate(object sender, EventArgs e)
            {
                _component.EjectMedia();
            };

            ClearAll.Click += delegate(object sender, EventArgs args)
            {
                _component.ClearStudies();
            };

            DetectMedia.Click += delegate(object sender, EventArgs args)
            {
                _component.DetectMedia();
            };

        }

        private void Additionaloptions_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _component.OpenOptions();
        }

        private void MediaWriter_ValueChanged(object sender, EventArgs e)
        {
            if (_component != null)
            {
                _component.SelectedMediaWriter = (IDiscRecorder2)(MediaWriter.SelectedValue);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WndProMsgConst.WM_DEVICECHANGE)
            {
                DEV_BROADCAST_HDR lpdb = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_HDR));
                switch (m.WParam.ToInt32())
                {
                    case WndProMsgConst.DBT_DEVICEARRIVAL:
                        if (lpdb.dbch_devicetype == WndProMsgConst.DBT_DEVTYP_VOLUME)
                        {
                            DEV_BROADCAST_VOLUME lpdbv = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_VOLUME));
                            int a = lpdbv.dbcv_flags & WndProMsgConst.DBTF_MEDIA;
                            if ((lpdbv.dbcv_flags & WndProMsgConst.DBTF_MEDIA) == WndProMsgConst.DBTF_MEDIA)
                            {
                                if (_component != null)
                                {
                                    _component.DetectMedia();
                                }
                            }
                        }
                        break;
                    case WndProMsgConst.DBT_DEVICEREMOVECOMPLETE:
                        if (lpdb.dbch_devicetype == WndProMsgConst.DBT_DEVTYP_VOLUME)
                        {
                            DEV_BROADCAST_VOLUME lpdbv = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_VOLUME));
                            int a = lpdbv.dbcv_flags & WndProMsgConst.DBTF_MEDIA;
                            if ((lpdbv.dbcv_flags & WndProMsgConst.DBTF_MEDIA) == WndProMsgConst.DBTF_MEDIA)
                            {
                                if (_component != null)
                                {
                                    _component.DetectMedia();
                                }
                            }
                        }
                        break;
                }
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// Gets drive letter from a bit mask where bit 0 = A, bit 1 = B etc.
        /// There can actually be more than one drive in the mask but we 
        /// just use the last one in this case.
        /// </summary>       
        private static char DriveMaskToLetter(int mask)
        {
            char letter;
            string drives = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            // 1 = A, 2 = B, 4 = C...
            int cnt = 0;
            int pom = mask / 2;
            while (pom != 0)
            {
                // while there is any bit set in the mask, shift it to the right... 
                pom = pom / 2;
                cnt++;
            }
            if (cnt < drives.Length)
                letter = drives[cnt];
            else
                letter = '?';
            return letter;

        }

        private void ExploreTo_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.StagingFolder.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void EreaseDisc_Click(object sender, EventArgs e)
        {
           _component.EreaseDisc();
        }
    }
}
