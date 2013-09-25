#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Web.UI;
using Macro.ImageServer.Web.Common.Data.Model;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    public partial class DeletedStudyDetailsDialogGeneralPanel : UserControl
    {
        #region Private Fields

        private DeletedStudyDetailsDialogViewModel _viewModel;

        #endregion

        #region Internal Properties

        internal DeletedStudyDetailsDialogViewModel ViewModel
        {
            get { return _viewModel; }
            set { _viewModel = value; }
        }

        #endregion

        #region Public Methods

        public override void DataBind()
        {
            IList<DeletedStudyInfo> dataSource = new List<DeletedStudyInfo>();
            if (_viewModel != null)
                dataSource.Add(_viewModel.DeletedStudyRecord);
            StudyDetailView.DataSource = dataSource;
            base.DataBind();
        }

        #endregion
    }
}