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
using GridView = Macro.ImageServer.Web.Common.WebControls.UI.GridView;

namespace Macro.ImageServer.Web.Application.Pages.WebViewer
{
    /// <summary>
    /// Control to display the summary information of a grid
    /// </summary>
    public partial class GridPager : UserControl
    {
        #region Private Members

        private GridView _target;
        private ImageServerConstants.GridViewPagerPosition _position;
        private string _targetUpdatePanelID;

        #endregion Private Members

        #region Public Properties

        public ImageServerConstants.GridViewPagerPosition PagerPosition
        {
            get { return _position; }
            set { _position = value; }
        }

        public string CurrentPageSaver
        {
            get; set;
        }

        public string AssociatedUpdatePanelID
        {
            set { _targetUpdatePanelID = value; }
        }

        /// <summary>
        /// Sets/Gets the grid associated with this control
        /// </summary>
        public GridView Target
        {
            get { return _target; }
            set { _target = value; }
        }

        /// <summary>
        /// Sets/Retrieve the name of the item in the list.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Sets/Retrieves the name for the more than one items in the list.
        /// </summary>
        public string PluralItemName { get; set; }

        public int ItemCount
        {
            get 
            {
                int count = 0;
                if(GetRecordCountMethod != null)
                {
                    count = GetRecordCountMethod();
                    ViewState[ImageServerConstants.PagerItemCount] = count;
                }

                return count;
            }
        }

        #endregion Public Properties

        #region Public Delegates

        /// <summary>
        /// Methods to retrieve the number of records.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The number of records may be different than the value reported by <seealso cref="GridPager.Target.Rows.Count"/>
        /// </remarks>
        public delegate int GetRecordCountMethodDelegate();

        /// <summary>
        /// Sets the method to be used by this control to retrieve the total number of records.
        /// </summary>
        public GetRecordCountMethodDelegate GetRecordCountMethod;

        #endregion Public Delegates

        #region Protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if(!Target.IsDataBound)
                {
                    Target.DataBind();    
                }
            }
            Target.DataBound += DataBoundHandler;
            UpdateUI();
         }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchUpdateProgress.AssociatedUpdatePanelID = _targetUpdatePanelID;
            SetPageContainerCssClass();
        }

        protected void PageButtonClick(object sender, CommandEventArgs e)
        {
            // get the current page selected
            int intCurIndex = Target.PageIndex;

            switch (e.CommandArgument.ToString().ToLower())
            {
                case "":
                    Target.PageIndex = intCurIndex;
                    break;
                case ImageServerConstants.Prev:
                    Target.PageIndex = intCurIndex - 1;
                    break;
                case ImageServerConstants.Next:
                    Target.PageIndex = intCurIndex + 1;
                    break;
                case ImageServerConstants.First:
                    Target.PageIndex = 0;
                    break;
                case ImageServerConstants.Last:
                    Target.PageIndex = Target.PageCount - 1;
                    break;
                default:

                    if (CurrentPage.Text.Equals(string.Empty))
                        Target.PageIndex = intCurIndex;
                    else
                    {
                        int newPage = Convert.ToInt32(Request.Form[CurrentPage.UniqueID]);

                        //Adjust page to match 0..n, and handle boundary conditions.
                        if (newPage > Target.PageCount)
                        {
                            newPage = _target.PageCount - 1;
                            if (newPage < 0) newPage = 0;
                        }
                        else if (newPage != 0) newPage -= 1;

                        Target.PageIndex = newPage;
                    }

                    break;
            }

            Target.Refresh();
        }

        private int AdjustCurrentPageForDisplay(int page)
        {
            if (_target.PageCount == 0)
            {
                page = 0;
            } else if (page == 0 )
            {
                page = 1;
            } else if (page >= _target.PageCount)
            {
                page = _target.PageCount;
            } else
            {
                page += 1;
            }

            return page;
        }

        private void EnableCurrentPage(bool enable)
        {
            if(enable)
            {
                string script =
                    "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" +
                    ChangePageButton.ClientID + "').click();return false;}} else {return true}; ";

                CurrentPage.Attributes.Add("onkeydown", script);
                CurrentPage.Attributes.Add("onclick", "javascript: document.getElementById('" + CurrentPage.ClientID + "').select();");
                CurrentPage.Enabled = true;
            } else
            {
                CurrentPage.Attributes.Add("onkeydown", "return false;");
                CurrentPage.Attributes.Add("onclick", "return false;");
                CurrentPage.Enabled = false;
            }
        }

        #endregion Protected methods

        #region Public methods

        /// <summary>
        /// Update the UI contents
        /// </summary>
        public void UpdateUI()
        {
            if (_target != null && _target.DataSource != null)
            {                
                CurrentPage.Text = AdjustCurrentPageForDisplay(_target.PageIndex).ToString();
                EnableCurrentPage(_target.PageCount > 1);

                PageCountLabel.Text =
                    string.Format(" {0} {1}", Resources.GridPager.PageOf, AdjustCurrentPageForDisplay(_target.PageCount));

                ItemCountLabel.Text = string.Format("{0} {1}", ItemCount, ItemCount == 1 ? ItemName : PluralItemName);

                SetPageContainerWidth(_target.PageCount);
               
                if (_target.PageIndex > 0)
                {
                    PrevPageButton.Enabled = true;
                    PrevPageButton.ImageUrl = ImageServerConstants.ImageURLs.WebViewerPagerPreviousEnabled;

                    FirstPageButton.Enabled = true;
                    FirstPageButton.ImageUrl = ImageServerConstants.ImageURLs.WebViewerPagerFirstEnabled;
                }
                else
                {
                    PrevPageButton.Enabled = false;
                    PrevPageButton.ImageUrl = ImageServerConstants.ImageURLs.WebViewerPagerPreviousDisabled;

                    FirstPageButton.Enabled = false;
                    FirstPageButton.ImageUrl = ImageServerConstants.ImageURLs.WebViewerPagerFirstDisabled;
                }


                if (_target.PageIndex < _target.PageCount - 1)
                {
                    NextPageButton.Enabled = true;
                    NextPageButton.ImageUrl = ImageServerConstants.ImageURLs.WebViewerPagerNextEnabled;

                    LastPageButton.Enabled = true;
                    LastPageButton.ImageUrl = ImageServerConstants.ImageURLs.WebViewerPagerLastEnabled;
                }
                else
                {
                    NextPageButton.Enabled = false;
                    NextPageButton.ImageUrl = ImageServerConstants.ImageURLs.WebViewerPagerNextDisabled;

                    LastPageButton.Enabled = false;
                    LastPageButton.ImageUrl = ImageServerConstants.ImageURLs.WebViewerPagerLastDisabled;

                }
            } else
            {
                ItemCountLabel.Text = string.Format("0 {0}", PluralItemName);
                CurrentPage.Text = "0";
                EnableCurrentPage(false);
                PageCountLabel.Text = string.Format(" {0} 0", Resources.GridPager.PageOf);
                PrevPageButton.Enabled = false;
                PrevPageButton.ImageUrl = ImageServerConstants.ImageURLs.WebViewerPagerPreviousDisabled;
                NextPageButton.Enabled = false;
                NextPageButton.ImageUrl = ImageServerConstants.ImageURLs.WebViewerPagerNextDisabled;

                FirstPageButton.Enabled = false;
                FirstPageButton.ImageUrl = ImageServerConstants.ImageURLs.WebViewerPagerFirstDisabled;
                LastPageButton.Enabled = false;
                LastPageButton.ImageUrl = ImageServerConstants.ImageURLs.WebViewerPagerLastDisabled;

            }
        }

        public void InitializeGridPager(string singleItemLabel, string multipleItemLabel, GridView grid, GetRecordCountMethodDelegate recordCount, ImageServerConstants.GridViewPagerPosition position)
        {
            _position = position;
            ItemName = singleItemLabel;
            PluralItemName = multipleItemLabel;
            Target = grid;
            GetRecordCountMethod = recordCount;

            // TODO: add this code so that the pager is updated automatically whenever the grid is updated
            //      Target.DataBound += delegate { GridPagerTop.Refresh(); };
            //
            // Becareful though, because the pager is calling Databind() in Page_Load(),
            // some pages may be end up in an infinite loop with this change. 
        }

        

        public void Reset()
        {
            ViewState[ImageServerConstants.PagerItemCount] = null;
        }

        /// <summary>
        /// Refresh the pager based on the latest data of the associated gridview control.
        /// This should be called whenever the gridview is databound.
        /// Note: this method will move the page
        /// </summary>
        /// 
        public void Refresh()
        {
            if (Target.Rows.Count == 0 && ItemCount > 0)
            {
                // This happens when the last item on the current page is removed
                Target.Refresh();
                // Note: if this method is called on DataBound event, it will be called again when the target is updated
                // However, we shoud have not have infinite loop because the the if condition
            }
            else
            {
                UpdateUI();
            }   
        }

        #endregion Public methods

        #region Private Methods

        private void SetPageContainerWidth(int pageCount)
        {
            if (pageCount > 9999) CurrentPageContainer.Style.Add("width", "187px");
            else if (pageCount > 99999) CurrentPageContainer.Style.Add("width", "197px");
            else if (pageCount > 999999) CurrentPageContainer.Style.Add("width", "207px");
            else if (pageCount > 9999999) CurrentPageContainer.Style.Add("width", "217px");
        }

        private void SetPageContainerCssClass()
        {
            if (Request.UserAgent.Contains("Chrome")) CurrentPageContainer.CssClass = "WebViewerCurrentPageContainer_Chrome";
            else if (Request.UserAgent.Contains("MSIE")) CurrentPageContainer.CssClass = "WebViewerCurrentPageContainer";
            else CurrentPageContainer.CssClass = "WebViewerCurrentPageContainer_FF";
        }

        private void DataBoundHandler(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }
}