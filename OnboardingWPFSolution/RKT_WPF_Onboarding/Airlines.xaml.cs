using System;
using System.Windows;

using RKT_WPF_Onboarding.Helpers;

namespace RKT_WPF_Onboarding
{
    /// <summary>
    /// Interaction logic for Airlines.xaml
    /// </summary>
    public partial class Airlines : Window
    {
        public Airlines()
        {
            InitializeComponent();

            this.ShouldExitAppOnClose = true;
        }

        private void MenuItemAboutClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Flight Application", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuItemDeleteRegistrationClick(object sender, RoutedEventArgs e)
        {
            this.ShouldExitAppOnClose = false;
            this.Close();

            // TODO: Change 6 - Delete registration
            //RegistrationHelper.DeleteRegistration();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            if (ShouldExitAppOnClose)
                ExitApp();
        }

        private void MenuItemExitClick(object sender, RoutedEventArgs e)
        {
            ExitApp();
        }

        private void ExitApp()
        {
            Application.Current.Shutdown();
        }

        private bool ShouldExitAppOnClose { get; set; }
    }
}
