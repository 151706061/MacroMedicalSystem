#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System.Windows;
using System.Windows.Controls;

namespace Macro.ImageViewer.Web.Client.Silverlight.Controls
{
	//TODO (CR May 2010): still use this class?
	public partial class StudyBrowser : UserControl
	{
		public delegate void OnStudyOpened(StudyInfo studyInfo, string username, string password);

		public OnStudyOpened StudyOpenedEvent;

		public StudyBrowser()
		{
			InitializeComponent();

			Populate_StudyList();

		}

		private void Populate_StudyList()
		{
			StudyList.Items.Add(new StudyInfo
			{
				StudyInstanceUid = "1.3.6.1.4.1.25403.2200303521616.2332.20100209084408.1",
				PatientsName = "Test Patient For Stewart",
				Modality = "CT"
			});

			StudyList.Items.Add(new StudyInfo
			{
				StudyInstanceUid = "1.2.804.114118.13.1501947339",
				PatientsName = "Test Patient For Thanh",
				Modality = "MR"
			});

			StudyList.Items.Add(new StudyInfo
			{
				StudyInstanceUid = "1.2.392.200036.9116.2.6.1.48.1215614729.1213145196.350350",
				PatientsName = "Test Patient 2 For Thanh",
				Modality = "CT"
			});
			StudyList.Items.Add(new StudyInfo
			{
				StudyInstanceUid = "1.3.6.1.4.1.25403.120207566079.3112.20091214052441.1",
				PatientsName = "Test Patient 3 For Thanh",
				Modality = "CR"
			});

			StudyList.Items.Add(new StudyInfo
			{
				StudyInstanceUid = "1.2.840.113619.2.176.3596.3364260.7739.1152011061.753",
				PatientsName = "Test Patient 4 For Thanh",
				Modality = "MR"
			});



			StudyList.Items.Add(new StudyInfo
			{
				StudyInstanceUid = "1.2.804.114118.13.1502791316",
				PatientsName = "Test Patient For Steve",
				Modality = "CT"
			});

			StudyList.Items.Add(new StudyInfo
			{
				StudyInstanceUid = "1.2.840.113781.284.96629106580.653",
				PatientsName = "Test Patient For Steve 2",
				Modality = "MR"
			});

			StudyList.SelectedItem = StudyList.Items[0];
		}

	    private void Open_Click(object sender, RoutedEventArgs e)
	    {
			if (StudyList.SelectedItem != null)
			{
				if (StudyOpenedEvent != null)
					StudyOpenedEvent(new StudyInfo { StudyInstanceUid = StudyInstanceUid.Text }, "admin", "123");
			}
	    }

	    private void StudyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
	    {
			StudyInstanceUid.Text = (e.AddedItems[0] as StudyInfo).StudyInstanceUid;
	    }
	}
}
