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
using Macro.Common.Utilities;
using Macro.Enterprise.Common.Admin.AuthorityGroupAdmin;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.Data.DataSource;

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class AddAuthorityGroupDialog : UserControl
    {
        private EventHandler<EventArgs> _authorityGroupEditedHandler;

        public event EventHandler<EventArgs> AuthorityGroupsEdited
        {
            add { _authorityGroupEditedHandler += value; }
            remove { _authorityGroupEditedHandler -= value; }
        }

        /// <summary>
        /// Sets or gets the displayed study
        /// </summary>
        public StudySummary Study
        {
            get; set;
        }

 

        /// <summary>
        /// Displays the add/edit device dialog box.
        /// </summary>
        public void Show()
        {
            //If the validation failed, keep everything as is, and 
            //make sure the dialog stays visible.
            if (!Page.IsValid)
            {
                ModalDialog.Show();
                return;
            }

            if (Thread.CurrentPrincipal.IsInRole(Macro.ImageServer.Enterprise.Authentication.AuthorityTokens.Study.EditDataAccess) && Study != null)
            {
                AuthorityGroupCheckBoxList.Items.Clear();

                var controller = new StudyDataAccessController();
                var list = controller.ListDataAccessGroupsForStudy(Study.TheStudyStorage.Key);

                var adapter = new ServerPartitionDataAdapter();
                IList<AuthorityGroupDetail> accessAllStudiesList;
                var groups = adapter.GetAuthorityGroupsForPartition(Study.ThePartition.Key, true, out accessAllStudiesList);


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
                                              @group.AuthorityGroupRef.ToString(false, false));
                    item.Attributes["title"] = @group.Description;

                    foreach (AuthorityGroupStudyAccessInfo s in list)
                    {
                        if (s.AuthorityOID.Equals(group.AuthorityGroupRef.ToString(false, false)))
                            item.Selected = true;
                    }

                    items.Add(item);
                }

                AuthorityGroupCheckBoxList.Items.AddRange(CollectionUtils.ToArray(items));
            }

            CancelButton.Visible = true;
            UpdateButton.Visible = true;

            ModalDialog.Show();
        }

        public void Close()
        {
            ModalDialog.Hide();
        }

        protected void UpdateButton_Click(object sender, ImageClickEventArgs e)
        {
            if (Page.IsValid)
            {
                var assignedGroups = new List<string>();
                foreach (ListItem item in AuthorityGroupCheckBoxList.Items)
                {
                    if (item.Selected && item.Enabled)
                        assignedGroups.Add(item.Value);
                }

                var controller = new StudyDataAccessController();
                controller.UpdateStudyAuthorityGroups(Study.StudyInstanceUid, Study.AccessionNumber, Study.TheStudyStorage.Key, assignedGroups);
                
                OnAuthorityGroupsUpdated();
                
                Close();
            }
            else
            {
                Show();
            }
        }

        protected void CancelButton_Click(object sender, ImageClickEventArgs e)
        {
            Close();
        }

        private void OnAuthorityGroupsUpdated()
        {
            var args = new EventArgs();
            EventsHelper.Fire(_authorityGroupEditedHandler, this, args);
        }
    }
}