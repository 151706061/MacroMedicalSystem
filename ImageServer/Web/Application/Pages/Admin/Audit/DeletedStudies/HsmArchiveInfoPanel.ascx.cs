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
using System.IO;
using System.Xml.Serialization;
using Macro.Common.Utilities;
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Model;
using Macro.ImageServer.Services.WorkQueue.DeleteStudy.Extensions;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    public partial class HsmArchiveInfoPanel : BaseDeletedStudyArchiveUIPanel
    {
        public override void DataBind()
        {
            IList<DeletedStudyArchiveInfo> infoList = new List<DeletedStudyArchiveInfo>();
            if (ArchiveInfo != null)
                infoList.Add(ArchiveInfo);

            ArchiveInfoView.DataSource = CollectionUtils.Map(
                infoList,
                delegate(DeletedStudyArchiveInfo info)
                    {
                        var dataModel = new HsmArchivePanelInfoDataModel
                                            {
                                                PartitionArchive = info.PartitionArchive,
                                                ArchiveTime = info.ArchiveTime,
                                                TransferSyntaxUid = info.TransferSyntaxUid,
                                                ArchiveXml = XmlUtils.Deserialize<HsmArchiveXml>(info.ArchiveXml)
                                            };
                        return dataModel;
                    });

            base.DataBind();
        }
    }

    /// <summary>
    /// View Model for the <see cref="HsmArchiveInfoPanel"/>
    /// </summary>
    public class HsmArchivePanelInfoDataModel
    {
        #region Private Fields

        private PartitionArchive _archive;
        private string _archivePath;

        #endregion

        #region Public Properties

        public DateTime ArchiveTime { get; set; }

        public string TransferSyntaxUid { get; set; }

        public HsmArchiveXml ArchiveXml { get; set; }


        public string ArchiveFolderPath
        {
            get
            {
                if (String.IsNullOrEmpty(_archivePath) && _archive != null)
                {
                    var config = XmlUtils.Deserialize<HsmArchiveConfigXml>(_archive.ConfigurationXml);
                    _archivePath = StringUtilities.Combine(new[]
                                                               {
                                                                   config.RootDir, ArchiveXml.StudyFolder,
                                                                   ArchiveXml.Uid, ArchiveXml.Filename
                                                               }, String.Format("{0}", Path.DirectorySeparatorChar));
                }
                return _archivePath;
            }
        }

        public PartitionArchive PartitionArchive
        {
            get { return _archive; }
            set { _archive = value; }
        }

        #endregion
    }


    /// <summary>
    /// Represents the data of an Hsm Archive entry.
    /// </summary>
    [Serializable]
    [XmlRoot("HsmArchive")]
    public class HsmArchiveXml
    {
        #region Private Fields

        private string _filename;
        private string _studyFolder;
        private string _uid;

        #endregion

        #region Public Properties

        public string StudyFolder
        {
            get { return _studyFolder; }
            set { _studyFolder = value; }
        }

        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        public string Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }

        #endregion
    }

    /// <summary>
    /// Represents the Hsm Archive configuration
    /// </summary>
    [Serializable]
    [XmlRoot("HsmArchive")]
    public class HsmArchiveConfigXml
    {
        #region Private Fields

        private string _rootDir;

        #endregion

        #region Public Properties

        public string RootDir
        {
            get { return _rootDir; }
            set { _rootDir = value; }
        }

        #endregion
    }
}