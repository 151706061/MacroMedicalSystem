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
using System.Threading;
using System.Web.UI;
using AjaxControlToolkit;
using Macro.Enterprise.Core;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Web.Common;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.Data.DataSource;
using Macro.ImageServer.Web.Common.WebControls.UI;
using Macro.ImageServer.Enterprise.Authentication;

[assembly: WebResource("Macro.ImageServer.Web.Application.Pages.WebViewer.SearchPanel.js", "application/x-javascript")]

namespace Macro.ImageServer.Web.Application.Pages.WebViewer
{
    [ClientScriptResource(ComponentType="Macro.ImageServer.Web.Application.Pages.WebViewer.SearchPanel", ResourcePath="Macro.ImageServer.Web.Application.Pages.WebViewer.SearchPanel.js")]
    public partial class SearchPanel : AJAXScriptControl
    {
        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("StudyListClientID")]
        public string StudyListClientID
        {
            get { return StudyListGridView.TheGrid.ClientID; }
        }
       
        [ExtenderControlProperty]
        [ClientPropertyName("CanViewImages")]
        public bool CanViewImages
        {
            get { return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Study.ViewImages); }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ViewImageButtonClientID")]
        public string ViewImageButtonClientID
        {
            get { return ViewImageButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ViewImagePageUrl")]
        public string ViewImagePageUrl
        {
            get { return Page.ResolveClientUrl(ImageServerConstants.PageURLs.WebViewerDefaultPage); }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("Username")]
        public string Username
        {
            get; set;
        }

        [ExtenderControlProperty]
        [ClientPropertyName("SessionId")]
        public string SessionId
        {
            get; set;
        }

        public WebViewerInitParams InitParams
        { get; set; }
        
        #endregion Public Properties  

        #region Private Methods

        private void SetupChildControls()
        {
           
            GridPagerTop.InitializeGridPager(Resources.SR.GridPagerStudySingleItem, Resources.SR.GridPagerStudyMultipleItems, StudyListGridView.TheGrid, delegate { return StudyListGridView.Studies.Count; }, ImageServerConstants.GridViewPagerPosition.Top);
            StudyListGridView.Pager = GridPagerTop;
        }
        
        /// <summary>
        /// Set a <see cref="ISearchCondition{T}"/> for DICOM string based (wildcard matching) value.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="val"></param>
        private static void SetStringCondition(ISearchCondition<string> cond, string val)
        {
            if (val.Length == 0)
                return;

            if (val.Contains("*") || val.Contains("?"))
            {
                String value = val.Replace("%", "[%]").Replace("_", "[_]");
                value = value.Replace('*', '%');
                value = value.Replace('?', '_');
                cond.Like(value);
            }
            else
                cond.EqualTo(val);
        }

        private static IList<StudySummary> LoadStudies(WebViewerInitParams initParams)
        {

            ValidateParameters(initParams);

            var controller = new StudyController();
            var partitionAdapter = new ServerPartitionDataAdapter();
            StudySelectCriteria studyCriteria;
            var partitionCriteria = new ServerPartitionSelectCriteria();
            ServerPartition partition = null;
            IList<Study> studies;
            List<StudySummary> totalStudies = new List<StudySummary>();

            if (!string.IsNullOrEmpty(initParams.AeTitle))
            {
                partitionCriteria.AeTitle.EqualTo(initParams.AeTitle);
                IList<ServerPartition> partitions = partitionAdapter.GetServerPartitions(partitionCriteria);
                if (partitions.Count == 1)
                {
                    partition = partitions[0];
                }
            }

            foreach (string patientId in initParams.PatientIds)
            {
                studyCriteria = new StudySelectCriteria();                
                if (partition != null) studyCriteria.ServerPartitionKey.EqualTo(partition.Key);
                SetStringCondition(studyCriteria.PatientId, patientId);
                studyCriteria.StudyDate.SortDesc(0);
                studies = controller.GetStudies(studyCriteria);

                foreach (Study study in studies)
                {
                    totalStudies.Add(StudySummaryAssembler.CreateStudySummary(HttpContextData.Current.ReadContext, study));
                }
            }

            foreach (string accession in initParams.AccessionNumbers)
            {
                studyCriteria = new StudySelectCriteria();
                if (partition != null) studyCriteria.ServerPartitionKey.EqualTo(partition.Key);
                SetStringCondition(studyCriteria.AccessionNumber, accession);
                studyCriteria.StudyDate.SortDesc(0);
                studies = controller.GetStudies(studyCriteria);

                foreach (Study study in studies)
                {
                    totalStudies.Add(StudySummaryAssembler.CreateStudySummary(HttpContextData.Current.ReadContext, study));
                }
            }

            if (initParams.StudyInstanceUids.Count > 0)
            {
                studyCriteria = new StudySelectCriteria();
                if (partition != null) studyCriteria.ServerPartitionKey.EqualTo(partition.Key);
                studyCriteria.StudyInstanceUid.In(initParams.StudyInstanceUids);
                studyCriteria.StudyDate.SortDesc(0);
                studies = controller.GetStudies(studyCriteria);

                foreach (Study study in studies)
                {
                    totalStudies.Add(StudySummaryAssembler.CreateStudySummary(HttpContextData.Current.ReadContext, study));
                }
            }

            totalStudies.Sort((a, b) => a.StudyDate.CompareTo(b.StudyDate) * -1);

            return totalStudies;
        }



        private static void ValidateParameters(WebViewerInitParams parameters)
        {
            if (parameters == null)
                throw new Exception("Parameters are missing");


			// According to the techinical document, AE Title is optional
			//
            // if (string.IsNullOrEmpty(parameters.AeTitle.Trim()))
            // {
            //    throw new Exception("'aetitle' cannot be empty");
            // }

            if (parameters.AccessionNumbers!=null)
            {
                foreach(var value in parameters.AccessionNumbers)
                {
                    if (string.IsNullOrEmpty(value.Trim()))
                    {
                        throw new Exception("'accession' is specified but contain an empty value");
                    }
                }
            }

            if (parameters.PatientIds != null)
            {
                foreach (var value in parameters.PatientIds)
                {
                    if (string.IsNullOrEmpty(value.Trim()))
                    {
                        throw new Exception("'patientid' is specified but contain an empty value");
                    }
                }
            }

            if (parameters.StudyInstanceUids != null)
            {
                foreach (var value in parameters.StudyInstanceUids)
                {
                    if (string.IsNullOrEmpty(value.Trim()))
                    {
                        throw new Exception("'study' is specified but contain an empty value");
                    }
                }
            }
        }


        #endregion Private Methods

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            SetupChildControls();
            StudyListGridView.Studies = LoadStudies(InitParams);
            StudyListGridView.Refresh();
        }

        #endregion Protected Methods
    }
}