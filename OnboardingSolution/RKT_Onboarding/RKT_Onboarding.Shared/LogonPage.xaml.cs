using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RKT_Onboarding 
{
    using RKT_Onboarding.Common;
    using RKT_Onboarding.Contexts;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LogonPage : Page 
    {
        private readonly NavigationHelper navigationHelper;
        private readonly LogonPageContext pageContext = new LogonPageContext();

        public LogonPage() 
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            this.DataContext = this.pageContext;
        }

        public void GoToState(string state)
        {
            if (state == "RegistrationComplete")
            {
                this.Frame.Navigate(typeof(Airlines));
            }
            else
            {
                VisualStateManager.GoToState(this, state, false);
            }
        }

        private async void ButtonRegisterClick(object sender, RoutedEventArgs e)
        {
            this.GoToState("Registering");
            string message = null;
            try
            {
                await Globals.LogonCore.RegisterWithContextAsync(this.pageContext.LogonContext);
                this.pageContext.PasswordToggleEnabled =
                    this.pageContext.LogonContext.PasswordPolicy.IsDefaultPasswordAllowed;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            if (message != null)
            {
                await this.ShowErrorMessage(message);
            }
            else
            {
                if (this.pageContext.LogonContext.PasswordPolicy.IsEnabled)
                {
                    this.GoToState("NewPasscode");
                }
                else
                {
                    await Globals.LogonCore.PersistRegistrationAsync(null, this.pageContext.LogonContext);
                    this.GoToState("RegistrationComplete");
                }
            }
        }

        private async System.Threading.Tasks.Task ShowErrorMessage(string message, string state = "ShouldRegister") 
        {
            await new Windows.UI.Popups.MessageDialog(message, "error").ShowAsync();
            if (state != null)
            {
                this.GoToState(state);
            }
        }

        private async void ButtonNewPasscodeOkClick(object sender, RoutedEventArgs e)
        {
            string message = null;
            bool passcodeEnabled = (!this.pageContext.LogonContext.PasswordPolicy.IsDefaultPasswordAllowed) || this.PasscodeSwitch.IsOn;

            if (passcodeEnabled)
            {
                if (this.pageContext.ConfirmedPasscode != this.pageContext.Passcode)
                {
                    await this.ShowErrorMessage("Passcodes do not match", null);
                    return;
                }
            }

            try
            {
                await Globals.LogonCore.PersistRegistrationAsync(passcodeEnabled ? this.pageContext.Passcode : null, this.pageContext.LogonContext);
                this.GoToState("RegistrationComplete");
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            if (message != null)
            {
                await this.ShowErrorMessage(message, null);
            }
        }

        private void ButtonCancelRegistrationClick(object sender, RoutedEventArgs e) 
        {
            Globals.LogonCore.CancelRegistration();
        }

        private async void ButtonDeleteRegistrationClick(object sender, RoutedEventArgs e)
        {
            try
            {
                await Globals.LogonCore.UnregisterAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            this.pageContext.Reset();
            this.PasscodeSwitch.IsOn = true;
            this.GoToState("ShouldRegister");
        }

        private async void ButtonUnlockClick(object sender, RoutedEventArgs e)
        {
            string message = null;
            bool destroyed = false;
            try
            {
                await Globals.LogonCore.UnlockSecureStoreAsync(string.IsNullOrEmpty(this.pageContext.UnlockCode) ? null : this.pageContext.UnlockCode);
                this.pageContext.LogonContext = Globals.LogonCore.LogonContext;
                this.GoToState("RegistrationComplete");
            }
            catch (Exception ex)
            {
                message = ex.Message;

                var dvex = ex as SAP.SecureStore.IDataVaultException;
                if ((dvex != null) && (dvex.Type == SAP.SecureStore.DataVaultExceptionType.VaultDoesNotExist))
                {
                    destroyed = true;
                    message = "Too many retries, registration deleted";
                    this.pageContext.Reset();
                }
            }

            if (message != null)
            {
                await this.ShowErrorMessage(message, null);
                if (destroyed)
                {
                    this.GoToState("ShouldRegister");
                }
            }
        }

        #region navigation related methods
		/// <summary>
		/// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
		/// </summary>
		public NavigationHelper NavigationHelper {
			get { return this.navigationHelper; }
		}

		/// <summary>
		/// Gets the view model for this <see cref="Page"/>.
		/// This can be changed to a strongly typed view model.
		/// </summary>

		/// <summary>
		/// Populates the page with content passed during navigation.  Any saved state is also
		/// provided when recreating a page from a prior session.
		/// </summary>
		/// <param name="sender">
		/// The source of the event; typically <see cref="NavigationHelper"/>
		/// </param>
		/// <param name="e">Event data that provides both the navigation parameter passed to
		/// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
		/// a dictionary of state preserved by this page during an earlier
		/// session.  The state will be null the first time a page is visited.</param>
		private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e) {
		}

		/// <summary>
		/// Preserves state associated with this page in case the application is suspended or the
		/// page is discarded from the navigation cache.  Values must conform to the serialization
		/// requirements of <see cref="SuspensionManager.SessionState"/>.
		/// </summary>
		/// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
		/// <param name="e">Event data that provides an empty dictionary to be populated with
		/// serializable state.</param>
		private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e) {
		}
		#endregion

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);

            if (Globals.LogonCore == null)
            {
                Globals.LogonCore = await SAP.Logon.Core.LogonCore.InitWithApplicationIdAsync("com.sap.windows.flight");
            }

            if (Globals.LogonCore.State.IsRegistered)
            {
                bool shouldEnterPasscode = false;
                try
                {
                    await Globals.LogonCore.UnlockSecureStoreAsync(null);
                    this.pageContext.LogonContext = Globals.LogonCore.LogonContext;
                }
                catch (Exception)
                {
                    shouldEnterPasscode = true;
                }

                if ((Globals.LogonCore.LogonContext == null) || (shouldEnterPasscode))
                {
                    this.GoToState("EnterPasscode");
                }
                else
                {
                    this.GoToState("RegistrationComplete");
                }
            }
            else
            {
                this.GoToState("ShouldRegister");
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
