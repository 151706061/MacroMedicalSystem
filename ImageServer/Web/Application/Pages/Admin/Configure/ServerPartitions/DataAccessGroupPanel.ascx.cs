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
using System.Text;
using System.Threading;
using System.Web.UI.WebControls;
using Macro.Common.Utilities;
using Macro.Enterprise.Common.Admin.AuthorityGroupAdmin;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Common.Data;
using Macro.Web.Enterprise.Admin;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions
{
    public partial class DataAccessGroupPanel : System.Web.UI.UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                InitDataAccessGroupList();
            
        }

        public ServerPartition Partition
        {    
            set
            {
                var current = ViewState["CurrentPartition"] as ServerPartition; ;
                if (ReferenceEquals(current, value))
                    return;


                ViewState["CurrentPartition"] = value;
                if (value==null)
                {
                    // adding a new partition, clear the list
                    DataAccessGroupCheckBoxList.Items.Clear();
                    InitDataAccessGroupList();
                }
                else
                {
                    // key == null when new partition is added (OK is pressed)
                    if (value.Key == null)
                    {
                        // if the current partition was not the same one then refresh the list
                        if (current!=null && current.Key!=null)
                        {
                            DataAccessGroupCheckBoxList.Items.Clear();
                            InitDataAccessGroupList();
                        }
                    }
                    else
                    {
                        // Refresh if no partition is assigned or new one was assigned or
                        // a different one was assigned
                        if (current == null || current.Key==null || !current.Key.Equals(value.Key))
                        {
                            DataAccessGroupCheckBoxList.Items.Clear();
                            InitDataAccessGroupList();
                        }
                    }
                    
                        
                }
                
                
                
            }
            get { return ViewState["CurrentPartition"] as ServerPartition; }
        }

        private void InitDataAccessGroupList()
        {
            if (DataAccessGroupCheckBoxList.Items.Count != 0)
                return;

            if (Thread.CurrentPrincipal.IsInRole(Macro.Enterprise.Common.AuthorityTokens.Admin.Security.AuthorityGroup))
            {
                var list = LoadDataAccessGroupInfo();

                DataAccessGroupCheckBoxList.Items.Clear();
                var listItems = CollectionUtils.Map<DataAccessGroupInfo, ListItem>(list, DataAccessGroupListItemConverter.Convert);
                DataAccessGroupCheckBoxList.Items.AddRange(listItems.ToArray());

                Legends.Visible = list.ContainsGroupWithAllPartitionAccess || list.ContainsGroupWithAllStudiesAccess;
            }
        }

        private static bool HasToken(List<AuthorityTokenSummary> tokens, string token)
        {
            return tokens.Exists(t => t.Name.Equals(token, StringComparison.InvariantCultureIgnoreCase));
        }


        private DataAccessGroupInfoCollection LoadDataAccessGroupInfo()
        {
            using (AuthorityManagement service = new AuthorityManagement())
            {
                var dataGroups = service.ListDataAccessAuthorityGroupDetails();

                // Include those that are not data access groups but have access to all partitions
                var accessToAllPartitionGroups = CollectionUtils.Select(service.ListAllAuthorityGroupDetails(),
                            g => HasToken(g.AuthorityTokens, Macro.Enterprise.Common.AuthorityTokens.DataAccess.AllPartitions));

                var combinedGroups = new List<AuthorityGroupDetail>();
                combinedGroups.AddRange(dataGroups);
                foreach(var g in accessToAllPartitionGroups){
                    if (combinedGroups.Find(item=>item.AuthorityGroupRef.Equals(g.AuthorityGroupRef, true))==null)
                    {
                        combinedGroups.Add(g);
                    }
                }

                //convert to DataAccessGroupInfo for sorting
                var list = new DataAccessGroupInfoCollection(CollectionUtils.Map<AuthorityGroupDetail, DataAccessGroupInfo>(combinedGroups,
                    (group) =>
                    {

                        var authorityRecordRef = group.AuthorityGroupRef.ToString(false, false);
                        var fullServerPartitionAccess = HasToken(group.AuthorityTokens, Macro.Enterprise.Common.AuthorityTokens.DataAccess.AllPartitions);
                        var allStudiesAccess = HasToken(group.AuthorityTokens, Macro.Enterprise.Common.AuthorityTokens.DataAccess.AllStudies);
                        return new DataAccessGroupInfo(authorityRecordRef, group.Name)
                        {
                            Description = group.Description,
                            HasAccessToCurrentPartition = fullServerPartitionAccess || (Partition != null && Partition.Key != null && Partition.IsAuthorityGroupAllowed(authorityRecordRef)),
                            CanAccessAllPartitions = fullServerPartitionAccess,
                            CanAccessAllStudies = allStudiesAccess
                        };
                    }));

                list.Sort(new DatagroupComparer());

                return list;
            }
        }

        public IEnumerable<string> SelectedGroupRefs
        {
            get
            {
                foreach (ListItem item in DataAccessGroupCheckBoxList.Items)
                {
                    if (item.Selected)
                        yield return item.Value;
                }

            }
        }
    }
}