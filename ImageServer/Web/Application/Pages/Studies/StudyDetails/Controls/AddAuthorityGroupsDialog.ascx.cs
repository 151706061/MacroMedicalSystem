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
using System.Web.UI.WebControls;
using Macro.Common;
using Macro.Common.Utilities;
using Macro.Enterprise.Common.Admin.AuthorityGroupAdmin;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.Data.DataSource;

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class AddAuthorityGroupsDialog : UserControl
    {
        public IList<StudySummary> AuthorityGroupStudies
        {
            get
            {
                return ViewState["AuthorityGroupStudies"] as IList<StudySummary>;
            }
            set { ViewState["AuthorityGroupStudies"] = value; }
        }
        
        public override void DataBind()
        {
            StudyListing.DataSource = AuthorityGroupStudies;

            if (Thread.CurrentPrincipal.IsInRole(Macro.ImageServer.Enterprise.Authentication.AuthorityTokens.Study.EditDataAccess))
            {
                if (AuthorityGroupStudies != null)
                {
                    AuthorityGroupCheckBoxList.Items.Clear();

                    var study = CollectionUtils.FirstElement(AuthorityGroupStudies);
                    var adapter = new ServerPartitionDataAdapter();
                    IList<AuthorityGroupDetail> accessAllStudiesList;
                    IList<AuthorityGroupDetail> groups = adapter.GetAuthorityGroupsForPartition(study.ThePartition.Key, true, out accessAllStudiesList);


                    IList<ListItem> items = CollectionUtils.Map(
                        accessAllStudiesList,
                        delegate(AuthorityGroupDetail group)
                            {

                                var item = new ListItem(@group.Name,
                                                        @group.AuthorityGroupRef.ToString(false, false))
                                               {
                                                   Enabled = false,
                                                   Selected = true
                                               };
                                item.Attributes["title"] = @group.Description;
                                return item;
                            });

                    foreach (var group in groups)
                    {
                        var item = new ListItem(@group.Name,
                                                @group.AuthorityGroupRef.ToString(false, false))
                                       {
                                           Selected = false
                                       };
                        item.Attributes["title"] = @group.Description;
                        items.Add(item);
                    }

                    AuthorityGroupCheckBoxList.Items.AddRange(CollectionUtils.ToArray(items));
                }
            }

            base.DataBind();
        }       

        protected void AddButton_Clicked(object sender, ImageClickEventArgs e)
        {

            if (Page.IsValid)
            {
                try
                {
                    var assignedGroups = new List<string>();
                    foreach (ListItem item in AuthorityGroupCheckBoxList.Items)
                    {
                        if (item.Selected && item.Enabled)
                            assignedGroups.Add(item.Value);
                        item.Selected = false;
                    }

                    foreach (StudySummary study in AuthorityGroupStudies)
                    {
                        try
                        {
                            StudyDataAccessController controller = new StudyDataAccessController();                            
                            controller.AddStudyAuthorityGroups(study.StudyInstanceUid, study.AccessionNumber, study.TheStudyStorage.Key, assignedGroups);
                        }
                        catch (Exception ex)
                        {
                            Platform.Log(LogLevel.Error, ex, "AddClicked failed: Unable to add authority groups to studies");
                            throw;
                        }
                    }           
                }
                finally
                {
                    Close();
                }
            }
            else
            {
                EnsureDialogVisible();
            }
        }

        protected void CancelButton_Clicked(object sender, ImageClickEventArgs e)
        {
            Close();
        }

        internal void EnsureDialogVisible()
        {
            ModalDialog.Show();
        }

        public void Close()
        {
            ModalDialog.Hide();
        }

        public void Initialize(IList<StudySummary> list)
        {
            AuthorityGroupStudies = list;
        }

        internal void Show()
        {
            DataBind();
            EnsureDialogVisible();
        }
    }
}