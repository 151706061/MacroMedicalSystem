#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion
using Macro.Desktop.Trees;
using Macro.ImageViewer.StudyManagement.Core;

namespace Macro.ImageViewer.Utilities.Media
{
    public interface ISeriesTreeItem
    {
        IStudyTreeItem ParentStudy { get; }

        bool Ischecked { get; }
        string Modality { get; }
        string Description { get; }
        int Seriesnumber { get; }
        void SetChecked(bool check);
    }

    public class SeriesTreeItem : ISeriesTreeItem
    {
        private readonly ISeries _series;

        public ISeries Series
        {
            get { return _series; }
        } 

        private readonly IStudyTreeItem _parentStudy;
        private bool isChecked = true;

        internal SeriesTreeItem(ISeries series, IStudyTreeItem parent)
        {
            _series = series;
            _parentStudy = parent;
        }

        public IStudyTreeItem ParentStudy
        {
            get { return _parentStudy; }
        }

        public string Modality
        {
            get { return _series.Modality; }
        }

        public string Description
        {
            get { return _series.SeriesDescription; }
        }

        public int Seriesnumber
        {
            get { return _series.SeriesNumber; }
        }

        public bool Ischecked
        {
            get { return isChecked; }
        }

        public void SetChecked(bool check)
        {
            isChecked = check;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", this.Seriesnumber, this.Modality, this.Description);
        }
    }

    public class SeriesTreeItemBinding : TreeItemBindingBase
    {

        public override string GetNodeText(object item)
        {
            return ((ISeriesTreeItem)item).ToString();
        }

        public override bool CanHaveSubTree(object item)
        {
            return false;
        }

        public override ITree GetSubTree(object item)
        {
            return null;
        }

        public override bool GetIsChecked(object item)
        {
            return ((ISeriesTreeItem)item).Ischecked;
        }

        public override void SetIsChecked(object item, bool value)
        {
            ((ISeriesTreeItem)item).SetChecked(value);
        }
    }
}
