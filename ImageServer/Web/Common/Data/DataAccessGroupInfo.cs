#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using Macro.Enterprise.Common.Admin.AuthorityGroupAdmin;

namespace Macro.ImageServer.Web.Common.Data
{
    public class DataAccessGroupInfo
    {
        public string AuthorityGroupRef { get; private set; }
        public string Name { get; private set; }
        public string Description { get; set; }
        public bool HasAccessToCurrentPartition { get; set; }
        public bool CanAccessAllPartitions { get; set; }
        public bool CanAccessAllStudies { get; set; }

        public DataAccessGroupInfo(string authorityGroupRef, string name)
        {
            AuthorityGroupRef = authorityGroupRef;
            Name = name;
        }

        public DataAccessGroupInfo(AuthorityGroupDetail detail)
        {
            Name = detail.Name;
            Description = detail.Description;
            AuthorityGroupRef = detail.AuthorityGroupRef.ToString(false, false);
            
        }
    }


    public class DataAccessGroupInfoCollection : List<DataAccessGroupInfo>
    {
        public DataAccessGroupInfoCollection(IEnumerable<DataAccessGroupInfo> list)
            : base(list)
        { }

        public bool ContainsGroupWithAllPartitionAccess
        {
            get { return Exists(item => item.CanAccessAllPartitions); }
        }

        public bool ContainsGroupWithAllStudiesAccess
        {
            get { return Exists(item => item.CanAccessAllStudies); }
        }
    }

    public static class DataAccessGroupListItemConverter
    {
        public static ListItem Convert(DataAccessGroupInfo info)
        {
            string displayContent = GetRenderedHtml(info);

            var item = new ListItem(displayContent, info.AuthorityGroupRef);
            item.Attributes["title"] = info.Description;

            item.Selected = info.HasAccessToCurrentPartition;
            item.Enabled = !info.CanAccessAllPartitions;

            return item;
        }

        private static string GetRenderedHtml(DataAccessGroupInfo info)
        {
            StringBuilder html = new StringBuilder();
            html.Append(info.Name);

            if (info.CanAccessAllStudies)
                html.AppendFormat("<span class='GlocalSeeNotesMarker'/> * </span>");

            return html.ToString();

        }
    }

    public class DatagroupComparer : IComparer<DataAccessGroupInfo>
    {
        public int Compare(DataAccessGroupInfo x, DataAccessGroupInfo y)
        {
            if (x.CanAccessAllPartitions)
            {
                if (!y.CanAccessAllPartitions)
                    return -1; //x first

                return x.Name.CompareTo(y.Name); // alphabetically
            }
            else
            {
                if (y.CanAccessAllPartitions)
                    return 1; // y first

                return x.Name.CompareTo(y.Name); // alphabetically
            }
        }
    }

    public class AuthorityGroupCategories
    {
        public IList<AuthorityGroupSummary> RegularAuthorityGroups { get; set; }
        public IList<AuthorityGroupSummary> DataAccessAuthorityGroups { get; set; }

        public IList<AuthorityGroupSummary> All
        {
            get
            {
                var list = new List<AuthorityGroupSummary>();
                list.AddRange(RegularAuthorityGroups);
                list.AddRange(DataAccessAuthorityGroups);
                return list;
            }
        }


    }
}