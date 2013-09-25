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
using System.Security.Permissions;
using Macro.ImageServer.Enterprise.Authentication;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Web.Application.Pages.Common;
using Macro.ImageServer.Web.Common.Data;
using Resources;

namespace Macro.ImageServer.Web.Application.Pages.Studies.Move
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Study.Move)]
    public partial class Default : BasePage
    {
        #region constants
        private const string QUERY_KEY_STUDY_INSTANCE_UID = "studyuid";
        private const string QUERY_KEY_SERVER_AE = "serverae";
        #endregion constants

        #region Private Members
        private readonly IDictionary<string, string> _uids = new Dictionary<string, string>();
        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            StudyController studyController = new StudyController();
            ServerPartitionConfigController partitionConfigController = new ServerPartitionConfigController();

            string serverae = Request.QueryString[QUERY_KEY_SERVER_AE];
            if (!String.IsNullOrEmpty(serverae))
            {
                // Load the Partition
                ServerPartitionSelectCriteria partitionCriteria = new ServerPartitionSelectCriteria();
                partitionCriteria.AeTitle.EqualTo(serverae);
                IList<ServerPartition> list = partitionConfigController.GetPartitions(partitionCriteria);
                this.Move.Partition = list[0];

                for (int i = 1;; i++)
                {
                    string studyuid = Request.QueryString[String.Format("{0}{1}", QUERY_KEY_STUDY_INSTANCE_UID, i)];

                    if (!String.IsNullOrEmpty(studyuid))
                    {
                        _uids.Add(studyuid, serverae);

                        StudySelectCriteria studyCriteria = new StudySelectCriteria();
                        studyCriteria.StudyInstanceUid.EqualTo(studyuid);
                        studyCriteria.ServerPartitionKey.EqualTo(list[0].GetKey());

                        IList<Study> studyList = studyController.GetStudies(studyCriteria);

                        this.Move.StudyGridView.StudyList.Add(studyList[0]);
                        this.Move.StudyGridView.Partition = this.Move.Partition;
                    }
                    else
                        break;
                }
            }

            SetPageTitle(Titles.MoveStudiesPageTitle);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Hide the UserPanel information
            IMasterProperties master = Master as IMasterProperties;
            master.DisplayUserInformationPanel = false;
        }
    }
}
