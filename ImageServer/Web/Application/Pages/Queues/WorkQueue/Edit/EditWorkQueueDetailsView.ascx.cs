using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Macro.ImageServer.Core.Edit;

namespace Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    public partial class EditWorkQueueDetailsView : WorkQueueDetailsViewBase
    {
   
        #region Private members

        #endregion Private members

        #region Public Properties

        /// <summary>
        /// Sets or gets the width of work queue details view panel
        /// </summary>
        public override Unit Width
        {
            get { return base.Width; }
            set
            {
                base.Width = value;
                EditInfoDetailsView.Width = value;
            }
        }

        public UpdateItem[] UpdateItems { get; set; }

        #endregion Public Properties

        #region Protected Methods

        #endregion Protected Methods

        #region Public Methods

        public override void DataBind()
        {
            if (WorkQueue != null)
            {
                var details = WorkQueueDetailsAssembler.CreateWorkQueueDetail(WorkQueue);
                UpdateItems = details.EditUpdateItems;
                var detailsList = new List<WorkQueueDetails>
                                      {
                                          details
                                      };
                EditInfoDetailsView.DataSource = detailsList;
            }
            else
                EditInfoDetailsView.DataSource = null;


            base.DataBind();
        }


        protected void GeneralInfoDetailsView_DataBound(object sender, EventArgs e)
        {
            var item = EditInfoDetailsView.DataItem as WorkQueueDetails;
            if (item != null)
            {
            }
        }

        #endregion Public Methods
        
    }
}