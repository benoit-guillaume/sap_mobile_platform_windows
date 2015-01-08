namespace RKT_WPF_GETFlights
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    using RKT_WPF_GETFlights.Contexts;

    /// <summary>
    /// Interaction logic for LogonWindow.xaml
    /// </summary>
    public partial class LogonWindow : Window
    {
        readonly LogonPageContext pageContext = new LogonPageContext();

        public LogonWindow()
        {
            InitializeComponent();

            this.DataContext = this.pageContext;
        }

        public void GoToState(string state)
        {
            if (state == "RegistrationComplete")
            {
                this.Hide();
                (new MainWindow()).Show();
            }
            else
            {
                if (state == "ShouldRegister") this.PwdBox_BackendPassword.Password = this.pageContext.LogonContext.RegistrationContext.BackendPassword;
                VisualStateManager.GoToElementState(this, state, false);
            }
        }

        private async void Button_RegisterClick(object sender, RoutedEventArgs e)
        {
            this.pageContext.LogonContext.RegistrationContext.BackendPassword = this.PwdBox_BackendPassword.Password;
            this.GoToState("Registering");
            string message = null;
            try
            {
                await Globals.LogonCore.RegisterWithContextAsync(this.pageContext.LogonContext);
                this.pageContext.PasswordToggleEnabled = this.pageContext.LogonContext.PasswordPolicy.IsDefaultPasswordAllowed;
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
            //await new Windows.UI.Popups.MessageDialog(message, "error").ShowAsync();
            await Task.Run(() =>
            {
                this.Dispatcher.BeginInvoke((Action)delegate()
                {
                    MessageBox.Show(message);
                    if (state != null) this.GoToState(state);
                });
            });
        }

        private async void Button_NewPasscodeOKClick(object sender, RoutedEventArgs e)
        {
            this.pageContext.Passcode = this.PwdBox_Passcode.Password;
            this.pageContext.ConfirmedPasscode = this.PwdBox_ConfirmedPasscode.Password;

            string message = null;
            bool passcodeEnabled = (this.pageContext.LogonContext.PasswordPolicy.IsDefaultPasswordAllowed) ? (bool)this.PasscodeSwitch.IsChecked : true;

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
                await this.ShowErrorMessage(message, null);
        }

        private void Button_CancelRegistrationClick(object sender, RoutedEventArgs e)
        {
            Globals.LogonCore.CancelRegistration();
        }

        private async void Button_DeleteRegistrationClick(object sender, RoutedEventArgs e)
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
            this.PasscodeSwitch.IsChecked = true;
            this.GoToState("ShouldRegister");
        }

        private async void Button_UnlockClick(object sender, RoutedEventArgs e)
        {
            string message = null;
            bool destroyed = false;
            this.pageContext.UnlockCode = this.PwdBox_UnlockCode.Password;
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
                if (destroyed) this.GoToState("ShouldRegister");
            }
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Globals.LogonCore == null)
                Globals.LogonCore = await SAP.Logon.Core.LogonCore.InitWithApplicationIdAsync("com.sap.flight");

            if (Globals.LogonCore.State.IsRegistered)
            {
                bool shouldEnterPasscode = false;
                try
                {
                    await Globals.LogonCore.UnlockSecureStoreAsync(null);
                    this.pageContext.LogonContext = Globals.LogonCore.LogonContext;
                }
                catch (Exception) { shouldEnterPasscode = true; }
                if ((Globals.LogonCore.LogonContext == null) || (shouldEnterPasscode)) this.GoToState("EnterPasscode");
                else this.GoToState("RegistrationComplete");
            }
            else this.GoToState("ShouldRegister");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown(0);
        }
    }
}
