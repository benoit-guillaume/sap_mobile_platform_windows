using System;
using System.Windows;

using RKT_WPF_E2ETracing.Contexts;

namespace RKT_WPF_E2ETracing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public LogonWindow Logon;

        public MainWindow(LogonWindow logon)
        {
            InitializeComponent();
            Logon = logon;
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            if (!Airlines.DeleteRegistrationWindowClose)
            {
                ExitApp();
            }
            else
            {
                Airlines.DeleteRegistrationWindowClose = false;
                Logon.pageContext.Reset();
                Logon.PasscodeSwitch.IsChecked = true;
                Logon.GoToState("ShouldRegister");
                Logon.Show();
            }
        }

        private void ExitApp()
        {
            Application.Current.Shutdown();
        }
    }
}
