using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using RKT_WPF_POSTFlightBooking.Contexts;

namespace RKT_WPF_POSTFlightBooking
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

        private async void BtnShowAirportStatusClick(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement != null)
            {
                var entity = (SAP.Data.OData.IODataEntity)frameworkElement.Tag;

                if (entity == null)
                {
                    return;
                }

                string message = null;
                SharedContext.Context.RelatedEntity = (SAP.Data.OData.Online.ODataEntity)entity;

                var url = "http://" + Globals.LogonCore.LogonContext.RegistrationContext.ServerHost + ":" + Globals.LogonCore.LogonContext.RegistrationContext.ServerPort + "/AirportStatus/" + (string)entity.Properties["flightDetails/airportTo"].Value + "?format=json";

                try
                {
                    SharedContext.Context.RingVisible = Visibility.Visible;
                    await SharedContext.Context.DownloadAirportStatus(url);
                }
                catch (Exception ex)
                {
                    SharedContext.Context.RingVisible = Visibility.Collapsed;
                    message = ex.Message;
                }

                if (message != null)
                {
                    await this.ShowErrorMessageAsync("We could not retrieve airport status: " + message);
                }
                else if (!NavigateToFlightDetailsPage())
                {
                    throw new Exception("Navigation Failed");
                }
            } 
        }

        public bool NavigateToFlightDetailsPage()
        {
            try
            {
                // View flights
                FlightDetails flightDetailsPage = new FlightDetails();
                this.NavigationService.Navigate(flightDetailsPage);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task ShowErrorMessageAsync(string message)
        {
            await this.Dispatcher.BeginInvoke((Action)delegate()
            {
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
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
