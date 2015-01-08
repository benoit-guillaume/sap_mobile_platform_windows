using System.Windows;
using System.Windows.Controls;

using RKT_WPF_GETFlights.Contexts;

namespace RKT_WPF_GETFlights
{
    /// <summary>
    /// Interaction logic for Flights.xaml
    /// </summary>
    public partial class Flights : Page
    {
        public Flights()
        {
            InitializeComponent();

            this.DataContext = SharedContext.Context;
        }

        private void MenuItemExitClick(object sender, System.Windows.RoutedEventArgs e)
        {
            ExitApp();
        }

        private void MenuItemAboutClick(object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBox.Show("Flight Application", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExitApp()
        {
            Application.Current.Shutdown();
        }
    }
}
