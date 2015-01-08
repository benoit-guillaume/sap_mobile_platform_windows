using System;

using RKT_Onboarding.Contexts;
using RKT_Onboarding.Helpers;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
#if !WINDOWS_PHONE_APP
using Windows.UI.ApplicationSettings;
#endif

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace RKT_Onboarding {
    /// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public sealed partial class App : Application {
#if WINDOWS_PHONE_APP
		private TransitionCollection transitions;
#endif

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App() {
			this.InitializeComponent();
			this.Suspending += this.OnSuspending;
			this.Resuming += this.OnResuming;
		}

		void OnResuming(object sender, object e) {
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used when the application is launched to open a specific file, to display
		/// search results, and so forth.
		/// </summary>
		/// <param name="e">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs e) {
#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached) {
				this.DebugSettings.EnableFrameRateCounter = true;
			}
#endif

			Frame rootFrame = Window.Current.Content as Frame;

			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active
			if (rootFrame == null) {
				// Create a Frame to act as the navigation context and navigate to the first page
				rootFrame = new Frame();

				// TODO: change this value to a cache size that is appropriate for your application
				rootFrame.CacheSize = 2;

				if (e.PreviousExecutionState == ApplicationExecutionState.Terminated) {
					// TODO: Load state from previously suspended application
				}

				// Place the frame in the current Window
				Window.Current.Content = rootFrame;
				Window.Current.VisibilityChanged += this.Current_VisibilityChanged;
			}

			if (rootFrame.Content == null) {
#if WINDOWS_PHONE_APP
				// Removes the turnstile navigation for startup.
				if (rootFrame.ContentTransitions != null) {
					this.transitions = new TransitionCollection();
					foreach (var c in rootFrame.ContentTransitions) {
						this.transitions.Add(c);
					}
				}

				rootFrame.ContentTransitions = null;
				rootFrame.Navigated += this.RootFrame_FirstNavigated;

				//Resources["TemplateMargin"] = new Thickness(3);
				Resources["PageTitleMargin"] = new Thickness(19, 19, 0, 20);
				Resources["PageSubtitleMargin"] = new Thickness(19, -6, 0, 12);
				Resources["ListControlPadding"] = new Thickness(13, 0, 13, 46);
				Resources["ItemDefaultBackground"] = new SolidColorBrush(Windows.UI.Colors.Transparent);
				Resources["ImagePlaceholderBackground"] = new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0xEE, 0xEE, 0xEE));
				//Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = Windows.UI.Colors.Black;
				Windows.UI.ViewManagement.StatusBar.GetForCurrentView().BackgroundColor = Windows.UI.Color.FromArgb(255, 0x00, 0x7C, 0xC0);
				Windows.UI.ViewManagement.StatusBar.GetForCurrentView().BackgroundOpacity = 255;
#endif

				// When the navigation stack isn't restored navigate to the first page,
				// configuring the new page by passing required information as a navigation
				// parameter
				if (!rootFrame.Navigate(typeof(LogonPage), e.Arguments)) {
					throw new Exception("Failed to create initial page");
				}
			}

			// Ensure the current window is active
			Window.Current.Activate();
		}

        void Current_VisibilityChanged(object sender, Windows.UI.Core.VisibilityChangedEventArgs e)
        {
            if (e.Visible)
            {
                Frame rootFrame = Window.Current.Content as Frame;
                // if we are not on the logon page then we should check if the secure store locked itself
                if (rootFrame.CurrentSourcePageType != typeof(LogonPage))
                {
                    if (Globals.LogonCore != null)
                    {
                        if (!Globals.LogonCore.State.IsSecureStoreOpen)
                            RegistrationHelper.NavigateToLogonPage(rootFrame);
                    }
                }
            }
        }

#if WINDOWS_PHONE_APP
		/// <summary>
		/// Restores the content transitions after the app has launched.
		/// </summary>
		/// <param name="sender">The object where the handler is attached.</param>
		/// <param name="e">Details about the navigation event.</param>
		private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e) {
			var rootFrame = sender as Frame;
			rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
			rootFrame.Navigated -= this.RootFrame_FirstNavigated;
		}
#endif

		/// <summary>
		/// Invoked when application execution is being suspended.  Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private void OnSuspending(object sender, SuspendingEventArgs e) {
			var deferral = e.SuspendingOperation.GetDeferral();

			// TODO: Save application state and stop any background activity
			deferral.Complete();
		}
#if !WINDOWS_PHONE_APP
		protected override void OnWindowCreated(WindowCreatedEventArgs args) {
			SettingsPane.GetForCurrentView().CommandsRequested += this.OnCommandsRequested;
		}

        private void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            if (Globals.LogonCore == null)
            {
                return;
            }

            //Frame rootFrame = Window.Current.Content as Frame;
            //if (Globals.LogonCore.State.IsRegistered && Globals.LogonCore.State.HasSecureStore)
            //{
            //    args.Request.ApplicationCommands.Add(
            //        new SettingsCommand(
            //            "delreg",
            //            "Delete Registration",
            //            (handler) =>
            //                {
            //                    if (Globals.LogonCore != null)
            //        {
            //            if (SharedContext.Context.CanEditItems)
            //            {
            //                RegistrationHelper.DeleteRegistration();
            //            }
            //            else
            //            {
            //                SharedContext.Context.OperationFinished += this.Context_OperationFinished;
            //            }
            //        }
            //    }));
            //}
        }

		void Context_OperationFinished(object sender, EventArgs e) {
			// SharedContext.Context.OperationFinished -= this.Context_OperationFinished;
			RegistrationHelper.DeleteRegistration();
		}
#endif

	}
}