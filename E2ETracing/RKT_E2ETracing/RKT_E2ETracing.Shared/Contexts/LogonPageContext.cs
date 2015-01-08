namespace RKT_E2ETracing.Contexts 
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class LogonPageContext : INotifyPropertyChanged 
    {
        public static SAP.Logon.Core.LogonContext DefaultLogonContext 
        {
            get 
            {
                return new SAP.Logon.Core.LogonContext 
                {
                    RegistrationContext = new SAP.Logon.Core.RegistrationContext 
                    {
                        ApplicationId = "com.sap.windows.flight",
                        ServerHost = "smpqa-win12-03.sybase.com",
                        // ServerHost = "i894769629.sapvcm.com",
                        IsHttps = false,
                        ServerPort = 8080,
                        CommunicatorId = "REST",
                        BackendUserName = "gwdemo",
                        BackendPassword = "welcome"
                    }
                };
            }
        }

        public void Reset()
        {
            this.LogonContext = DefaultLogonContext;
            this.Passcode = this.ConfirmedPasscode = this.UnlockCode = string.Empty;
            this.PasswordToggleEnabled = true;

            this.AppData = null;
            this.AppActionInProgress = false;
        }

        private SAP.Logon.Core.LogonContext logonContext = DefaultLogonContext;
        public SAP.Logon.Core.LogonContext LogonContext
        {
            get
            {
                return this.logonContext;
            }

            set
            {
                this.logonContext = value;
                this.NotifyPropertyChanged();
            }
        }

        private string passcode = string.Empty;
        public string Passcode
        {
            get
            {
                return this.passcode;
            }

            set
            {
                this.passcode = value;
                this.NotifyPropertyChanged();
            }
        }

        private string confirmedPasscode = string.Empty;
        public string ConfirmedPasscode
        {
            get
            {
                return this.confirmedPasscode;
            }

            set
            {
                this.confirmedPasscode = value;
                this.NotifyPropertyChanged();
            }
        }

        private string unlockCode = string.Empty;
        public string UnlockCode
        {
            get
            {
                return this.unlockCode;
            }

            set
            {
                this.unlockCode = value;
                this.NotifyPropertyChanged();
            }
        }

        private bool passwordToggleEnabled = true;
        public bool PasswordToggleEnabled
        {
            get
            {
                return this.passwordToggleEnabled;
            }

            set
            {
                this.passwordToggleEnabled = value;
                this.NotifyPropertyChanged();
            }
        }

        private string appData = string.Empty;
        public string AppData
        {
            get
            {
                return this.appData;
            }

            set
            {
                this.appData = value;
                this.NotifyPropertyChanged();
            }
        }

        private bool appActionInProgress;
        public bool AppActionInProgress
        {
            get
            {
                return this.appActionInProgress;
            }

            set
            {
                this.appActionInProgress = value;
                this.NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}