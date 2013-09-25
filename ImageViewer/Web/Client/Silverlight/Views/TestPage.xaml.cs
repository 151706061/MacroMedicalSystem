#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Macro.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using Macro.ImageViewer.Web.Client.Silverlight.Controls;

namespace Macro.ImageViewer.Web.Client.Silverlight.Views
{
	//TODO (CR May 2010): delete this class?
    public partial class TestPage : UserControl, System.IDisposable
	{
        public TestPage()
		{
			InitializeComponent();
			StudyBrowser.StudyOpenedEvent += OnStudyOpen;
		}


	    private void ShowDiagnosticPanel_Click(object sender, RoutedEventArgs e)
	    {
            switch(DiagnosticPanel.Visibility)
            {
                case Visibility.Collapsed:
                    DiagnosticPanel.Visibility = Visibility.Visible;
                    ShowDiagnosticPanel.Content = "Hide Log";
                    break;

                case Visibility.Visible:
                    DiagnosticPanel.Visibility = Visibility.Collapsed;
                    ShowDiagnosticPanel.Content = "Show Log";
                    break;

            }
	    }

		private void OnStudyOpen(StudyInfo studyInfo, string username, string sessionId)
		{
			DisposeViewer();

			var viewer = new ImageViewer(new StartViewerApplicationRequest 
                { 
                    StudyInstanceUid = new Collection<string> { studyInfo.StudyInstanceUid },
                    Username = username
                })
            ;

			ImageViewerContainer.Children.Add(viewer);
		}

		private void DisposeViewer()
		{
			if (ImageViewerContainer.Children.Count > 0)
			{
				var viewer = (ImageViewer)ImageViewerContainer.Children[0];
				viewer.Dispose();
				ImageViewerContainer.Children.Clear();
			}
		}
		
		#region IDisposable Members

		public void Dispose()
		{
			DisposeViewer();
		}

		#endregion
	}
}