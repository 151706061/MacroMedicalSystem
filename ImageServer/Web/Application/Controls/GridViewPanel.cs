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
using System.Web.UI.WebControls;
using GridView=Macro.ImageServer.Web.Common.WebControls.UI.GridView;

namespace Macro.ImageServer.Web.Application.Controls
{
    public class GridViewPanel : UserControl
    {
        private GridPager _gridPager;
        private GridView _theGrid;
        private bool _dataBindOnPreRender = false;
        
        public GridPager Pager
        {
            set { _gridPager = value; }
            get { return _gridPager;  }
        }

        public bool DataBindOnPreRender
        {
            set { _dataBindOnPreRender = value;  }
            get { return _dataBindOnPreRender;  }

        }

        public GridView TheGrid
        {
            set { _theGrid = value; }
            get { return _theGrid; }
        }

        public void Reset()
        {
            if (_gridPager != null) _gridPager.Reset();
            _theGrid.ClearSelections();
            _theGrid.PageIndex = 0;
            _theGrid.DataSource = null;
            _theGrid.DataBind();
        }

        public void Refresh()
        {
            if(_gridPager != null) _gridPager.Reset();
            _theGrid.ClearSelections();
            _theGrid.PageIndex = 0;
            _theGrid.DataBind();
        }

        public void RefreshWithoutPagerUpdate()
        {
            _theGrid.ClearSelections();
            _theGrid.PageIndex = 0;
            _theGrid.DataBind();
        }

        public void RefreshAndKeepSelections()
        {
            if (_gridPager != null) _gridPager.Reset();
            _theGrid.PageIndex = 0;
            _theGrid.DataBind();
        }

        public void RefreshCurrentPage()
        {
            if(_gridPager != null) _gridPager.Reset();
            _theGrid.DataBind();
        }

        protected void DisposeDataSource(object sender, ObjectDataSourceDisposingEventArgs e)
        {
            e.Cancel = true;
        }

        protected override void OnPreRender(EventArgs e)
        {
        if(!_theGrid.IsDataBound && _dataBindOnPreRender) _theGrid.DataBind();
        }
    }
}
