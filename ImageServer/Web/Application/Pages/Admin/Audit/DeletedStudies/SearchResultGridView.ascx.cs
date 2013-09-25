#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Web.UI.WebControls;
using Macro.ImageServer.Web.Application.Controls;
using Macro.ImageServer.Web.Common.Data.DataSource;
using Macro.ImageServer.Web.Common.Data.Model;
using GridView=Macro.ImageServer.Web.Common.WebControls.UI.GridView;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    public partial class SearchResultGridView : GridViewPanel
    {
        #region Private Fields

        private DeletedStudyDataSource _dataSource;

        #endregion

        #region Public Properties

        public GridView GridViewControl
        {
            get { return ListControl; }
        }

        public int ResultCount
        {
            get
            {
                if (_dataSource == null)
                {
                    _dataSource = new DeletedStudyDataSource();
                }
                return _dataSource.SelectCount();
            }
        }

        public DeletedStudyInfo SelectedItem
        {
            get { return _dataSource.Find(ListControl.SelectedValue); }
        }

        public ObjectDataSource DataSourceContainer
        {
            get { return DataSource; }
        }

        #endregion

        #region Overriden Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            DataSourceContainer.ObjectCreated += DataSourceContainer_ObjectCreated;
            ListControl.DataSource = DataSource;
            TheGrid = ListControl;
        }

        #endregion

        #region Private Methods

        private void DataSourceContainer_ObjectCreated(object sender, ObjectDataSourceEventArgs e)
        {
            // keep a reference to the data source created, used for other purposes
            _dataSource = e.ObjectInstance as DeletedStudyDataSource;
        }

        #endregion
    }
}