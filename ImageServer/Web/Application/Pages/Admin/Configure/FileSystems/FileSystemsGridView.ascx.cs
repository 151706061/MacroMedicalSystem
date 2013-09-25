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
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Macro.Common;
using Macro.ImageServer.Common.ServiceModel;
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Application.Controls;
using GridView = Macro.ImageServer.Web.Common.WebControls.UI.GridView;
using Resources;
using SR = Resources.SR;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems
{
    //
    //  Used to display the list of devices.
    //
    public partial class FileSystemsGridView : GridViewPanel
    {
        #region private members

        private IList<Filesystem> _fileSystems;
        private Unit _height;
        #endregion Private members

        #region protected properties

        #endregion protected properties

        #region public properties

        /// <summary>
        /// Gets/Sets the height of the filesystem list panel.
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

        /// <summary>
        /// Gets/Sets the current selected FileSystem.
        /// </summary>
        public Filesystem SelectedFileSystem
        {
            get
            {
                if (FileSystems.Count == 0 || GridView1.SelectedIndex < 0)
                    return null;

                // SelectedIndex is for the current page. Must convert to the index of the entire list
                int index = GridView1.PageIndex*GridView1.PageSize + GridView1.SelectedIndex;

                if (index < 0 || index > FileSystems.Count - 1)
                    return null;

                return FileSystems[index];
            }
            set
            {
                GridView1.SelectedIndex = FileSystems.IndexOf(value);
                if (FileSystemSelectionChanged != null)
                    FileSystemSelectionChanged(this, value);
            }
        }

        /// <summary>
        /// Gets/Sets the list of file systems rendered on the screen.
        /// </summary>
        public IList<Filesystem> FileSystems
        {
            get { return _fileSystems; }
            set
            {
                _fileSystems = value;
                GridView1.DataSource = _fileSystems; // must manually call DataBind() later
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Defines the handler for <seealso cref="FileSystemSelectionChanged"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedFileSystem"></param>
        public delegate void FileSystemSelectedEventHandler(object sender, Filesystem selectedFileSystem);

        /// <summary>
        /// Occurs when the selected filesystem in the list is changed.
        /// </summary>
        /// <remarks>
        /// The selected filesystem can change programmatically or by users selecting the filesystem in the list.
        /// </remarks>
        public event FileSystemSelectedEventHandler FileSystemSelectionChanged;

        #endregion // Events


        #region private methods

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (GridView1.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    CustomizeUsageColumn(e.Row);
                    CustomizePathColumn(e.Row);
                    CustomizeEnabledColumn(e);
                    CustomizeReadColumn(e);
                    CustomizeWriteColumn(e);
                    CustomizeFilesystemTierColumn(e.Row);
                }
            }
        }


        private float GetFilesystemUsedPercentage(Filesystem fs)
        {
            if (!isServiceAvailable())
                return float.NaN;

            try
            {
                FilesystemInfo fsInfo = null;
                Platform.GetService(delegate(IFilesystemService service)
                {
                    fsInfo = service.GetFilesystemInfo(fs.FilesystemPath);
                }); 
                
                _serviceIsOffline = false;
                _lastServiceAvailableTime = Platform.Time;
                return 100.0f - ((float)fsInfo.FreeSizeInKB) / fsInfo.SizeInKB * 100.0F;
            }
            catch (Exception)
            {
                _serviceIsOffline = true;
                _lastServiceAvailableTime = Platform.Time;
            }

            return float.NaN;
        }

        private void CustomizeUsageColumn(GridViewRow row)
        {
            Filesystem fs = row.DataItem as Filesystem;
            Image img = row.FindControl("UsageImage") as Image;

            float usage = GetFilesystemUsedPercentage(fs);
            if (img != null)
            {
                img.ImageUrl = string.Format(ImageServerConstants.PageURLs.BarChartPage,
                                             usage,
                                             fs.HighWatermark,
                                             fs.LowWatermark);
                img.AlternateText = string.Format(Server.HtmlEncode(Tooltips.AdminFilesystem_DiskUsage).Replace(Environment.NewLine,"<br/>"),
                                  float.IsNaN(usage) ? SR.Unknown : usage.ToString(),
                                  fs.HighWatermark, fs.LowWatermark);
            }
        }

        private void CustomizePathColumn(GridViewRow row)
        {
            Filesystem fs = row.DataItem as Filesystem;
            Label lbl = row.FindControl("PathLabel") as Label; // The label is added in the template

            if (fs.FilesystemPath != null)
            {
                // truncate it
                if (fs.FilesystemPath.Length > 50)
                {
                    lbl.Text = fs.FilesystemPath.Substring(0, 45) + "...";
                    lbl.ToolTip = string.Format("{0}: {1}", fs.Description, fs.FilesystemPath);
                }
                else
                {
                    lbl.Text = fs.FilesystemPath;
                }
            }
        }

        private void CustomizeFilesystemTierColumn(GridViewRow row)
        {
            Filesystem fs = row.DataItem as Filesystem;
            Label lbl = row.FindControl("FilesystemTierDescription") as Label; // The label is added in the template
            lbl.Text = ServerEnumDescription.GetLocalizedDescription(fs.FilesystemTierEnum);
        }


        private void CustomizeBooleanColumn(GridViewRow row, string controlName, string fieldName)
        {
            Image img = ((Image)row.FindControl(controlName));
            if (img != null)
            {
                bool active = Convert.ToBoolean(DataBinder.Eval(row.DataItem, fieldName));
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

        #endregion

        #region protected methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TheGrid = GridView1;

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

        }

        #endregion   

        protected void CustomizeReadColumn(GridViewRowEventArgs e)
        {
            Image img = ((Image) e.Row.FindControl("ReadImage"));
            if (img != null)
            {
                bool enabled = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "Enabled"));
                bool readOnly = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "ReadOnly"));
                bool writeOnly = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "WriteOnly"));

                bool canRead = enabled && (readOnly || ( /*not readonly and */ !writeOnly));

                if (canRead)
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Checked;
                }
                else
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Unchecked;
                }
            }
        }

        protected void CustomizeEnabledColumn(GridViewRowEventArgs e)
        {
            CustomizeBooleanColumn(e.Row, "EnabledImage", "Enabled");
        }

        protected void CustomizeWriteColumn(GridViewRowEventArgs e)
        {
            Image img = ((Image) e.Row.FindControl("WriteImage"));
            if (img != null)
            {
                bool enabled = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "Enabled"));
                bool readOnly = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "ReadOnly"));
                bool writeOnly = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "WriteOnly"));

                bool canWrite = enabled && (writeOnly || ( /*not write only and also */ !readOnly));

                if (canWrite)
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Checked;
                }
                else
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Unchecked;
                }
            }
        }

        #region Private Static members
        static private bool _serviceIsOffline = false;
        static private DateTime _lastServiceAvailableTime = Platform.Time;

        /// <summary>
        /// Return a value indicating whether the last web service call was successful.
        /// </summary>
        /// <returns></returns>
        static private bool isServiceAvailable()
        {
            TimeSpan elapsed = Platform.Time - _lastServiceAvailableTime;
            return (!_serviceIsOffline || /*service was offline but */ elapsed.Seconds > 15);
        }

        #endregion Private Static members

    }
}
