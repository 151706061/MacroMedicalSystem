#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Macro.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using Macro.ImageViewer.Web.Client.Silverlight.ViewModel;
using Macro.ImageViewer.Web.Client.Silverlight.Helpers;
using System.Windows.Input;
using System.Collections.Generic;
using Macro.ImageViewer.Web.Client.Silverlight.Controls;
using Macro.ImageViewer.Web.Client.Silverlight.Resources;
using Macro.Web.Client.Silverlight.Utilities;

namespace Macro.ImageViewer.Web.Client.Silverlight.Views
{
	public partial class ImageViewer : IDisposable
	{
	    public ImageViewerViewModel ViewModel;
		private volatile ViewerApplication _serverApplication;

		private StudyView _studyView;

        private bool _shuttingDown;
	    private bool _disposed = false;

	    public ServerEventMediator EventMediator { get; set; }

        public ImageViewer(StartViewerApplicationRequest startRequest)
        {
            InitializeComponent();
            ViewModel = new ImageViewerViewModel
                            {
                                IsLoading = true
                            };
            
            DataContext = ViewModel;

            if (ApplicationContext.Current != null)
            {
                ApplicationContext.Initialize();
                if (ApplicationContext.Current == null) throw new Exception();
            }

            EventMediator = new ServerEventMediator();
            EventMediator.Initialize(ApplicationContext.Current.Parameters);

            EventMediator.CriticalError += ErrorHandler_OnCriticalError;

			EventMediator.RegisterEventHandler(typeof(ApplicationStartedEvent), ApplicationStarted);
            EventMediator.RegisterEventHandler(typeof(SessionUpdatedEvent), OnSessionUpdated);
            EventMediator.RegisterEventHandler(typeof(MessageBoxShownEvent), OnMessageBox);
            EventMediator.ServerApplicationStopped += OnServerApplicationStopped;
            
            _studyView = new StudyView(EventMediator);
			StudyViewContainer.Children.Add(_studyView);
            MouseHelper.SetBackgroundElement(LayoutRoot);

            if (startRequest == null)
            {
                //TODO: replace this with the custom dialog. For some reason, it doesn't work here.
                System.Windows.MessageBox.Show(ErrorMessages.MissingParameters);
            }
            else
            {

                ToolstripViewComponent.EventDispatcher = EventMediator;

                LayoutRoot.MouseLeftButtonDown += ToolstripViewComponent.OnLoseFocus;
                LayoutRoot.MouseRightButtonDown += ToolstripViewComponent.OnLoseFocus;

                EventMediator.StartApplication(startRequest);

                TileView.ApplicationRootVisual = _studyView.StudyViewCanvas;
                
                LayoutRoot.KeyUp += OnKeyUp;
            }
		}

        void ErrorHandler_OnCriticalError(object sender, EventArgs e)
        {
            ViewModel.IsLoading = false;

            Shutdown();
        }        

        void OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D:
                {
                    if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt)) == (ModifierKeys.Control | ModifierKeys.Alt))
                    {
                        //TODO: close this on error/timeout
                        var panel = new StackPanel { Orientation = Orientation.Horizontal };
                        panel.Children.Add(new StatisticsPanel { Margin = new Thickness(10) });
                        panel.Children.Add(new ThrottlePanel { Margin = new Thickness(10)});

                        PopupHelper.PopupContent(DialogTitles.ThrottleSettings, panel);
                    }
                    break;
                }
            }
        }

        private void OnServerApplicationStopped(object sender, ServerApplicationStopEventArgs e)
        {
            Shutdown();        
        }
        
        private void OnSessionUpdated(object sender, ServerEventArgs ev)
        {
            UIThread.Execute(() =>
            {
                var @event = ev.ServerEvent as SessionUpdatedEvent;
                ApplicationBridge.Current.OnViewerSessionUpdated(this, @event);
            });
        }

        private void OnMessageBox(object sender, ServerEventArgs ev)
        {
            var @event = ev.ServerEvent as MessageBoxShownEvent;

			//TODO (CR May 2010): can this be consolidate or split up?
            List<Button> buttonList = new List<Button>();

            if (@event != null && @event.MessageBox.Actions == WebMessageBoxActions.Ok)
            {
                var okButton = new Button { Content = Labels.ButtonOK, Margin = new Thickness(5) };
                okButton.Click += (s, e) => EventMediator.DispatchMessage(new DismissMessageBoxMessage
                                                                                                             {
                                                                                                                 TargetId = @event.MessageBox.Identifier,
                                                                                                                 Result = WebDialogBoxAction.Ok, 
                                                                                                                 Identifier = Guid.NewGuid()
                                                                                                             });
                buttonList.Add(okButton);
            }
            else if (@event != null && @event.MessageBox.Actions == WebMessageBoxActions.OkCancel)
            {
                var okButton = new Button { Content = Labels.ButtonOK, Margin = new Thickness(5) };
                okButton.Click += (s, e) => EventMediator.DispatchMessage(new DismissMessageBoxMessage
                                                                                                             {
                                                                                                                 TargetId = @event.MessageBox.Identifier,
                                                                                                                 Result = WebDialogBoxAction.Ok,
                                                                                                                 Identifier = Guid.NewGuid()
                                                                                                             });
                buttonList.Add(okButton);
                var cancelButton = new Button { Content = Labels.ButtonCancel, Margin = new Thickness(5) };
                cancelButton.Click += (s, e) => EventMediator.DispatchMessage(new DismissMessageBoxMessage
                                                                                               {
                                                                                                   TargetId = @event.MessageBox.Identifier,
                                                                                                   Result = WebDialogBoxAction.Cancel,
                                                                                                   Identifier = Guid.NewGuid()
                                                                                               });
                buttonList.Add(cancelButton);
            }
            else if (@event != null && @event.MessageBox.Actions == WebMessageBoxActions.YesNo)
            {
                var yesButton = new Button { Content = Labels.ButtonYes, Margin = new Thickness(5) };
                yesButton.Click += (s, e) => EventMediator.DispatchMessage(new DismissMessageBoxMessage
                                                                                                              {
                                                                                                                  TargetId = @event.MessageBox.Identifier,
                                                                                                                  Result = WebDialogBoxAction.Yes,
                                                                                                                  Identifier = Guid.NewGuid()
                                                                                                              });
                buttonList.Add(yesButton);
                var noButton = new Button { Content = Labels.ButtonNo, Margin = new Thickness(5) };
                noButton.Click += (s, e) => EventMediator.DispatchMessage(new DismissMessageBoxMessage
                                                                                                             {
                                                                                                                 TargetId = @event.MessageBox.Identifier,
                                                                                                                 Result = WebDialogBoxAction.No,
                                                                                                                 Identifier = Guid.NewGuid()
                                                                                                             });
                buttonList.Add(noButton);
            }
            else if (@event != null && @event.MessageBox.Actions == WebMessageBoxActions.YesNoCancel)
            {
                var yesButton = new Button { Content = Labels.ButtonYes, Margin = new Thickness(5) };
                yesButton.Click += (s, e) => EventMediator.DispatchMessage(new DismissMessageBoxMessage
                                                                                                              {
                                                                                                                  TargetId = @event.MessageBox.Identifier,
                                                                                                                  Result = WebDialogBoxAction.Yes,
                                                                                                                  Identifier = Guid.NewGuid()
                                                                                                              });
                buttonList.Add(yesButton);
                var noButton = new Button { Content = Labels.ButtonNo, Margin = new Thickness(5) };
                noButton.Click += (s, e) => EventMediator.DispatchMessage(new DismissMessageBoxMessage
                                                                                                             {
                                                                                         TargetId =
                                                                                             @event.MessageBox.
                                                                                             Identifier,
                                                                                         Result = WebDialogBoxAction.No,
                                                                                         Identifier = Guid.NewGuid()
                                                                                     });
                buttonList.Add(noButton);
                var cancelButton = new Button { Content = Labels.ButtonCancel, Margin = new Thickness(5) };
                cancelButton.Click += (s, e) => EventMediator.DispatchMessage(
                    new DismissMessageBoxMessage
                        {
                            TargetId = @event.MessageBox.Identifier,
                            Result = WebDialogBoxAction.Cancel,
                            Identifier = Guid.NewGuid()
                        });
                buttonList.Add(cancelButton);
            }
                           
            PopupHelper.PopupContent(@event.MessageBox.Title, @event.MessageBox.Message, buttonList.ToArray());      
        }

		private void ApplicationStarted(object sender, ServerEventArgs e)
		{
            if (EventMediator == null)
            {
                return;
            }

			var ev = (ApplicationStartedEvent)e.ServerEvent;
            if (ev == null)
            {
                EventMediator.HandleCriticalError("Unexpected event type: {0}", e.ServerEvent);
                return;
            }

			Visibility = Visibility.Visible;

            if (_serverApplication != null)
            {
                // Stop the prior application, note that it may have been stopped already, still send the
                // message just in case
                EventMediator.StopApplication();
            }

			//TODO (CR May 2010): we don't unregister these
            EventMediator.RegisterEventHandler(ev.StartRequestId, OnApplicationEvent);
        }

        private void OnApplicationEvent(object sender, ServerEventArgs e)
        {
            if (!(e.ServerEvent is PropertyChangedEvent))
                return;

            var ev = (PropertyChangedEvent)e.ServerEvent;

            if (ev.PropertyName == "Application")
            {
                ViewModel.IsLoading = false;

                _serverApplication = (ViewerApplication)ev.Value;

                ApplicationContext.Current.ViewerVersion = _serverApplication.VersionString;

				//TODO (CR May 2010): we don't unregister these
                EventMediator.RegisterEventHandler(_serverApplication.Viewer.Identifier, OnViewerEvent);
                ToolstripViewComponent.SetIconSize(_serverApplication.Viewer.ToolStripIconSize);
                ToolstripViewComponent.SetActionModel(_serverApplication.Viewer.ToolbarActions);
                _studyView.SetImageBoxes(_serverApplication.Viewer.ImageBoxes);
                return;
            }
        }

		private void OnViewerEvent(object sender, ServerEventArgs e)
		{
			if (!(e.ServerEvent is PropertyChangedEvent))
				return;

			var ev = (PropertyChangedEvent)e.ServerEvent;

			//TODO (CR May 2010): this is in the method above, too.  Which one works?
            if (ev.PropertyName == "Application")
            {
                _serverApplication = (ViewerApplication)ev.Value;
				//TODO (CR May 2010): we don't unregister these
                EventMediator.RegisterEventHandler(_serverApplication.Viewer.Identifier, OnViewerEvent);
                ToolstripViewComponent.SetIconSize(_serverApplication.Viewer.ToolStripIconSize);
                ToolstripViewComponent.SetActionModel(_serverApplication.Viewer.ToolbarActions);
                _studyView.SetImageBoxes(_serverApplication.Viewer.ImageBoxes);
                return;
            }

            if (ev.PropertyName == "ImageBoxes")
            {
                var imageBoxes = (Collection<ImageBox>)ev.Value;
                _serverApplication.Viewer.ImageBoxes = imageBoxes;
                _studyView.SetImageBoxes(imageBoxes);
                return;
            }

            if (ev.PropertyName == "ToolbarActions")
            {
                var actionModel = (Collection<WebActionNode>)ev.Value;
                _serverApplication.Viewer.ToolbarActions = actionModel;
                ToolstripViewComponent.SetActionModel(_serverApplication.Viewer.ToolbarActions);
                return;
            }
		}

        public void Shutdown()
        {
            if (!_shuttingDown)
            {
                _shuttingDown = true;
                EventMediator.StopApplication();

                Visibility = Visibility.Collapsed;

                if (_studyView != null)
                {
                    StudyViewContainer.Children.Clear();
         
                    MouseHelper.SetBackgroundElement(null);
                    _studyView.Dispose();
                    _studyView = null;
                }
            }                      
        }

	    public void Dispose()
	    {
	        Dispose(true);
	        GC.SuppressFinalize(this);
	    }

        public virtual void Dispose(bool disposing)
        {
            if(!_disposed)
            {
                Shutdown();

                // Must do this beofre we work with the EventMediator
                ToolstripViewComponent.Dispose();

                if (_studyView != null)
                {
                    StudyViewContainer.Children.Clear();
                    if (disposing)
                    {
                        _studyView.Dispose();
                    }
                    _studyView = null;
                }

                if (EventMediator != null)
                {
                    if (_serverApplication != null)
                    {
                        EventMediator.UnregisterEventHandler(typeof(ApplicationStartedEvent), ApplicationStarted);
                        EventMediator.UnregisterEventHandler(_serverApplication.Viewer.Identifier);
                        EventMediator.ServerApplicationStopped -= OnServerApplicationStopped;
                        _serverApplication = null;
                    }

                    EventMediator.Dispose();
                    EventMediator = null;
                }


                _disposed = true;
            }
        }
    }
}
