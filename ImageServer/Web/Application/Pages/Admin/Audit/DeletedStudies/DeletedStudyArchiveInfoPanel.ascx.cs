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
using System.Xml;
using AjaxControlToolkit;
using Macro.Common;
using Macro.Dicom;
using Macro.ImageServer.Model;
using Macro.ImageServer.Services.WorkQueue.DeleteStudy.Extensions;
using Macro.ImageServer.Web.Common.Utilities;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    public interface IDeletedStudyArchiveUIPanel
    {
        DeletedStudyArchiveInfo ArchiveInfo { get; set; }
    }

    public abstract class BaseDeletedStudyArchiveUIPanel : UserControl, IDeletedStudyArchiveUIPanel
    {
        #region IDeletedStudyArchiveUIPanel Members

        public DeletedStudyArchiveInfo ArchiveInfo { get; set; }

        #endregion
    }

    public partial class DeletedStudyArchiveInfoPanel : UserControl
    {
        #region Private Fields

        private DeletedStudyDetailsDialogViewModel _viewModel;

        #endregion

        internal DeletedStudyDetailsDialogViewModel ViewModel
        {
            get { return _viewModel; }
            set { _viewModel = value; }
        }


        private void CreateArchivePanel()
        {
            DeletedStudyArchiveInfoCollection archiveList = _viewModel.DeletedStudyRecord.Archives;
            Platform.CheckTrue(archiveList.Count > 0, "archiveList is empty");

            // make sure the list is sorted by timestamp
            archiveList.Sort(
                (archive1, archive2) => archive2.ArchiveTime.CompareTo(archive1.ArchiveTime));

            Control panel = LoadArchiveInformationPanel(GetArchiveType(archiveList[0]), archiveList[0]);
            ArchiveViewPlaceHolder.Controls.Add(panel);

            if (archiveList.Count > 1)
            {
                var container = new TabContainer {CssClass = "DialogTabControl"};

                for (int i = 1; i < archiveList.Count; i++)
                {
                    DeletedStudyArchiveInfo theArchive = archiveList[i];
                    Control detailPanel = LoadArchiveInformationPanel(GetArchiveType(theArchive), theArchive);

                    var tabPanel = new TabPanel
                                       {
                                           HeaderText = String.Format("{0} {1}",
                                                                      DateTimeFormatter.Format(theArchive.ArchiveTime,
                                                                                               DateTimeFormatter.Style.
                                                                                                   Date),
                                                                      TransferSyntax.GetTransferSyntax(
                                                                          theArchive.TransferSyntaxUid).
                                                                          LossyCompressed
                                                                          ? "(Lossy)"
                                                                          : String.Empty)
                                       };

                    tabPanel.Controls.Add(detailPanel);

                    container.Tabs.Add(tabPanel);
                }

                AdditionalArchivePlaceHolder.Controls.Add(container);
            }

            AdditionalArchivePlaceHolder.Visible = archiveList.Count > 1;

            ArchiveViewPlaceHolder.DataBind();
            AdditionalArchivePlaceHolder.DataBind();
        }

        private Control LoadArchiveInformationPanel(ArchiveTypeEnum type, DeletedStudyArchiveInfo info)
        {
            BaseDeletedStudyArchiveUIPanel panel = null;
            if (type == null)
            {
                panel = LoadControl("GeneralArchiveInfoPanel.ascx") as BaseDeletedStudyArchiveUIPanel;
                if (panel != null) panel.ArchiveInfo = info;
            }
            else if (type == ArchiveTypeEnum.HsmArchive)
            {
                panel = LoadControl("HsmArchiveInfoPanel.ascx") as BaseDeletedStudyArchiveUIPanel;
                if (panel != null) panel.ArchiveInfo = info;
            }

            if (panel != null)
            {
                panel.DataBind();
            }

            return panel;
        }

        private static ArchiveTypeEnum GetArchiveType(DeletedStudyArchiveInfo archiveInfo)
        {
            Platform.CheckForNullReference(archiveInfo, "archiveInfo");
            Platform.CheckForNullReference(archiveInfo.ArchiveXml, "archiveInfo.ArchiveXml");

            XmlNode node = archiveInfo.ArchiveXml.DocumentElement;
            if (node != null)
                if (node.Name.Equals("HsmArchive", StringComparison.InvariantCultureIgnoreCase))
                {
                    return ArchiveTypeEnum.HsmArchive;
                }
                else
                {
                    return null; //unknown
                }
            return null;
        }


        public override void DataBind()
        {
            if (_viewModel != null && _viewModel.DeletedStudyRecord.Archives != null &&
                _viewModel.DeletedStudyRecord.Archives.Count > 0)
            {
                NoArchiveMessagePanel.Visible = false;
                ArchiveViewPlaceHolder.Visible = true;
                CreateArchivePanel();
            }
            else
            {
                NoArchiveMessagePanel.Visible = true;
                ArchiveViewPlaceHolder.Visible = false;
                AdditionalArchivePlaceHolder.Visible = false;
            }

            base.DataBind();
        }
    }
}