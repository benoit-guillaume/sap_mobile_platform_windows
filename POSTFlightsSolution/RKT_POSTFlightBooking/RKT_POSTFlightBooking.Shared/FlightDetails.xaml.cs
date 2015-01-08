namespace RKT_POSTFlightBooking
{
    using System;
    using System.Globalization;

    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    using RKT_POSTFlightBooking.Common;
    using RKT_POSTFlightBooking.Contexts;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FlightDetails : Page
    {
        private NavigationHelper navigationHelper;

        public FlightDetails()
        {
            this.InitializeComponent();
            this.DataContext = SharedContext.JsonContext;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

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
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void AppBarButtonGoBackClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (this.navigationHelper.CanGoBack())
            {
                this.navigationHelper.GoBack();
            }
        }

        private async void BookFlight(object sender, Windows.UI.Xaml.RoutedEventArgs e)
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
                await this.ShowErrorMessage(message);
            }
            else
            {
                await this.ShowSuccessMessage();
            }
        }

        private async System.Threading.Tasks.Task ShowErrorMessage(string message)
        {
            await new Windows.UI.Popups.MessageDialog(message, "error").ShowAsync();
        }

        private async System.Threading.Tasks.Task ShowSuccessMessage()
        {
            await new Windows.UI.Popups.MessageDialog("Flight Booked.  Booking Confirmation : " + (string)SharedContext.Context.FlightBookingEntity.Properties["bookid"].Value, "success").ShowAsync();
        }
    }
}
