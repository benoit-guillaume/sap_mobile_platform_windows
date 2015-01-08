using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RKT_WPF_POSTFlightBooking.Contexts
{
    class LogonPageContext : INotifyPropertyChanged
    {
        public static SAP.Logon.Core.LogonContext DefaultLogonContext
        {
            get
            {
                return new SAP.Logon.Core.LogonContext
                {
                    RegistrationContext = new SAP.Logon.Core.RegistrationContext
                    {
                        ApplicationId = "com.sap.flight",
                        ServerHost = "127.0.0.1",
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
            this.Passcode = this.ConfirmedPasscode = this.UnlockCode = "";
            this.PasswordToggleEnabled = true;

            this.AppData = null;
            this.AppActionInProgress = false;
        }

        SAP.Logon.Core.LogonContext logonContext = DefaultLogonContext;
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

        private string passcode = "";
        public string Passcode
        {
            get { return this.passcode; }
            set
            {
                this.passcode = value;
                this.NotifyPropertyChanged();
            }
        }

        private string confirmedPasscode = "";
        public string ConfirmedPasscode
        {
            get { return this.confirmedPasscode; }
            set
            {
                this.confirmedPasscode = value;
                this.NotifyPropertyChanged();
            }
        }

        private string unlockCode = "";
        public string UnlockCode
        {
            get { return this.unlockCode; }
            set
            {
                this.unlockCode = value;
                this.NotifyPropertyChanged();
            }
        }

        private bool passwordToggleEnabled = true;
        public bool PasswordToggleEnabled
        {
            get { return this.passwordToggleEnabled; }
            set
            {
                this.passwordToggleEnabled = value;
                this.NotifyPropertyChanged();
            }
        }

        private string appData = "";
        public string AppData
        {
            get { return this.appData; }
            set
            {
                this.appData = value;
                this.NotifyPropertyChanged();
            }
        }

        private bool appActionInProgress;

        public bool AppActionInProgress
        {
            get { return this.appActionInProgress; }
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