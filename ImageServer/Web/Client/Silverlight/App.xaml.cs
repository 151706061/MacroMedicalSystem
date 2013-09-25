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
using System.Threading;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Input;
using Macro.ImageViewer.Web.Client.Silverlight;
using Macro.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using System.Windows.Controls;
using Macro.ImageViewer.Web.Client.Silverlight.Resources;
using Macro.Web.Client.Silverlight;
using Macro.ImageViewer.Web.Client.Silverlight.Helpers;
using System.Globalization;
using System.ComponentModel.Composition.Hosting;
using Macro.Web.Client.Silverlight.Utilities;
using Macro.Web.Client.Silverlight.Views;
using Imageviewer = Macro.ImageViewer.Web.Client.Silverlight.Views.ImageViewer;

namespace Macro.ImageServer.Web.Client.Silverlight
{
	public partial class App
	{
		private readonly object _startLock = new object();
		private ApplicationContext _context;
	    private LogPanel _logPanel;
        ChildWindow _stateDialog;

		public App()
		{
            Startup += OnAppStartup;

			Exit += ApplicationExit;
			UnhandledException += ApplicationUnhandledException;

            InitializeComponent();
            
            MenuManager.SuppressBrowserContextMenu = true;
            MenuManager.AutoCloseMenus = false;
		}

        private void OnAppStartup(object sender, StartupEventArgs e)
        {
            if (!String.IsNullOrEmpty(ApplicationStartupParameters.Current.Language) && 
                !ApplicationStartupParameters.Current.Language.StartsWith("EN", StringComparison.InvariantCultureIgnoreCase))
            {
                var culture = new CultureInfo(ApplicationStartupParameters.Current.Language);

                var catalog = new DeploymentCatalog(
                    new Uri(String.Format("{0}.{1}.xap", "Silverlight", culture.TwoLetterISOLanguageName), UriKind.Relative));

                CompositionHost.Initialize(catalog);

                catalog.DownloadCompleted += (s, args) =>
                {
                    if (null == args.Error)
                    {
                        Thread.CurrentThread.CurrentCulture = culture;
                        Thread.CurrentThread.CurrentUICulture = culture;
                        Start();
                    }
                    else
                    {
                        // cannot download resources for specific language, continue with default language
                        Start();
                    }
                };

                catalog.DownloadAsync();
            }
            else
            {
                Start();
            }            
		}

        private void Start()
        {
            // Initialize the communication channel with the host
            ApplicationBridge.Initialize();

            Panel rootPanel = new Grid();
            RootVisual = rootPanel;

            // Test the connection speed and launch the webviewer once it's completed
            ConnectionTester.TestConnection(new Action(StartWebViewer) );
        }



        private void StartWebViewer()
        {
            var rootPanel = RootVisual as Panel;
            
            //TODO (CR May 2010): need the lock?
            lock (_startLock)
            {
                if (_context != null)
                    return;

                _context = ApplicationContext.Initialize();
            }

            string query = HtmlPage.Document.DocumentUri.Query;

            Imageviewer viewer;

            if (!string.IsNullOrEmpty(query))
            {
                var request = new StartViewerApplicationRequest
                {
                    AccessionNumber = new ObservableCollection<string>(),
                    StudyInstanceUid = new ObservableCollection<string>(),
                    PatientId = new ObservableCollection<string>()
                };

                string[] vals = HttpUtility.UrlDecode(query).Split(new[] { '?', ';', '=', ',', '&' });
                for (int i = 0; i < vals.Length - 1; i++)
                {
                    if (String.IsNullOrEmpty(vals[i]))
                        continue;

                    if (vals[i].Equals("study"))
                    {
                        i++;
                        request.StudyInstanceUid.Add(vals[i]);
                    }
                    else if (vals[i].Equals("patientid"))
                    {
                        i++;
                        request.PatientId.Add(vals[i]);
                    }
                    else if (vals[i].Equals("aetitle"))
                    {
                        i++;
                        request.AeTitle = vals[i];
                    }
                    else if (vals[i].Equals("accession"))
                    {
                        i++;
                        request.AccessionNumber.Add(vals[i]);
                    }
                    else if (vals[i].Equals("application"))
                    {
                        i++;
                        request.ApplicationName = vals[i];
                    }
                }

                request.Username = ApplicationContext.Current.Parameters.Username;
                request.SessionId = ApplicationContext.Current.Parameters.SessionToken;
                request.IsSessionShared = ApplicationContext.Current.Parameters.IsSessionShared;

                viewer = new Imageviewer(request);
            }
            else
            {
                viewer = new Imageviewer(null);
            }

            viewer.EventMediator.CriticalError += CriticalError;
            viewer.EventMediator.ServerApplicationStopped += OnServerApplicationStopped;
            viewer.EventMediator.ChannelOpened += OnChannelOpened;
            viewer.EventMediator.ChannelOpening += OnChannelOpening;
            viewer.EventMediator.WarningEvent += OnWarning;

            if (rootPanel != null)
            {
                // Add a Log Panel at the bottom of the screen.  This can be opened by CTRL-ALT-L.
                _logPanel = new LogPanel
                {
                    Visibility = Visibility.Collapsed,
                    Height = 200
                };
                var theGrid = new Grid();
                theGrid.RowDefinitions.Add(new RowDefinition());
                theGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });
                _logPanel.SetValue(Grid.RowProperty, 1);
                viewer.SetValue(Grid.RowProperty, 0);
                theGrid.Children.Add(viewer);
                theGrid.Children.Add(_logPanel);
                rootPanel.Children.Add(theGrid);
                rootPanel.KeyUp += OnKeyUp;
                rootPanel.UpdateLayout();
            }
        }

	    private void OnWarning(object sender, EventArgs e)
	    {
            var message = sender as string;
            if (message != null)
            {
                PopupHelper.PopupMessage(DialogTitles.Error, message);
            }
	    }

	    void OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.L:
                    {
                        if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt)) == (ModifierKeys.Control | ModifierKeys.Alt))
                        {
                            _logPanel.Visibility = _logPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                            _logPanel.UpdateLayout();
                        }
                        break;
                    }
            }
        }
        
        private void OnChannelOpening(object sender, EventArgs e)
        {
            _stateDialog = PopupHelper.PopupMessage(DialogTitles.Initializing, SR.OpeningConnection);
        }

	    private void OnChannelOpened(object sender, EventArgs e)
        {
            if (_stateDialog != null)
            {
                _stateDialog.Close();
                _stateDialog = null;
            }
        }

	    private void CriticalError(object sender, EventArgs e)
        {
            var message = sender as string;
            if (message != null)
            {
                Platform.Log(LogLevel.Error, "Critical error: {0}", message);
                PopupHelper.PopupMessage(DialogTitles.Error, message);
            }
        }

        private void OnServerApplicationStopped(object sender, ServerApplicationStopEventArgs e)
        {
            UIThread.Execute(() =>
            {
                ApplicationStoppedEvent @event = e.ServerEvent;

                var title = @event.IsTimedOut ? DialogTitles.Timeout : DialogTitles.Error;
                var message = @event.Message;

                PopupHelper.PopupMessage(title, message);
            });
        }

		private void ApplicationExit(object sender, EventArgs e)
		{
			if (RootVisual is IDisposable)
			{
				((IDisposable)RootVisual).Dispose();
			}

			if (_context != null)
			{
				// TODO: SL does not support calling web service in Application Exit event
				_context.Dispose();
				_context = null;
			}
		}

		private void ApplicationUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			// If the app is running outside of the debugger then report the exception using
			// the browser's exception mechanism. On IE this will display it a yellow alert 
			// icon in the status bar and Firefox will display a script error.
			if (!System.Diagnostics.Debugger.IsAttached)
			{

				// NOTE: This will allow the application to continue running after an exception has been thrown
				// but not handled. 
				// For production applications this error handling should be replaced with something that will 
				// report the error to the website and stop the application.
				e.Handled = true;
				Deployment.Current.Dispatcher.BeginInvoke(() => ReportErrorToDOM(e));
			}
		}

		private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
		{
			try
			{
				string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
				errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

				HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
			}
			catch (Exception x)
			{
                Platform.Log(LogLevel.Error,x,"Exception reporting Error to the DOM");
			}
		}
    }
}
