using System;
using System.Windows;
using System.Windows.Controls;

using RKT_WPF_E2ETracing.Contexts;

namespace RKT_WPF_E2ETracing
{
    /// <summary>
    /// Interaction logic for FlightDetails.xaml
    /// </summary>
    public partial class FlightDetails : Page
    {
        public FlightDetails()
        {
            InitializeComponent();

            this.DataContext = SharedContext.JsonContext;
        }

        private async void BookFlight(object sender, RoutedEventArgs e)
        {
            var entity = new SAP.Data.OData.Online.ODataEntity("RMTSAMPLEFLIGHT.Booking");
            SharedContext.Context.Store.AllocateProperties(entity, SAP.Data.OData.Store.PropertyCreationMode.All);

            entity.Properties["carrid"].Value = SharedContext.JsonContext.AirlineID;
            entity.Properties["connid"].Value = SharedContext.JsonContext.FlightNumber;
            entity.Properties["fldate"].Value = SharedContext.JsonContext.FlightDate;
            entity.Properties["CUSTOMID"].Value = Constants.DefaultCustomerID;
            entity.Properties["AGENCYNUM"].Value = Constants.DefaultAgencyID;
            entity.Properties["ORDER_DATE"].Value = SharedContext.JsonContext.FlightDate;
            entity.Properties["bookid"].Value = "00000001";

            //entity.Properties["CUSTTYPE"].Value = "P";
            //entity.Properties["SMOKER"].Value = string.Empty;
            //entity.Properties["WUNIT"].Value = "KGM";
            //entity.Properties["LUGGWEIGHT"].Value = (decimal)17.4000;
            //entity.Properties["INVOICE"].Value = string.Empty;
            //entity.Properties["CLASS"].Value = "C";
            //entity.Properties["FORCURAM"].Value = (decimal)922.01;
            //entity.Properties["FORCURKEY"].Value = "EUR";
            //entity.Properties["LOCCURAM"].Value = (decimal)845.88;
            //entity.Properties["LOCCURKEY"].Value = "USD";
            //entity.Properties["COUNTER"].Value = "00000000";
            //entity.Properties["CANCELLED"].Value = string.Empty;
            //entity.Properties["RESERVED"].Value = string.Empty;
            //entity.Properties["PASSNAME"].Value = "Milton Chandradas";
            //entity.Properties["PASSFORM"].Value = string.Empty;
            //entity.Properties["PASSBIRTH"].Value = DateTime.Today;

            string message = null;
            try
            {
                await SharedContext.Context.BookFlight(entity, "BookingCollection");
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            if (message != null)
            {
                ShowErrorMessage(message);
            }
            else
            {
                ShowSuccessMessage();
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowSuccessMessage()
        {
            MessageBox.Show("Flight Booked.  Booking Confirmation : " + (string)SharedContext.Context.FlightBookingEntity.Properties["bookid"].Value, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AppBarButtonGoBackClick(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
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
