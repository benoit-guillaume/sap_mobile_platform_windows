// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace RKT_POSTFlightBooking
{
    using System;

    using RKT_JSONService;

    using RKT_POSTFlightBooking.Common;
    using RKT_POSTFlightBooking.Contexts;
    using RKT_POSTFlightBooking.Helpers;

    using Windows.ApplicationModel.Resources;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class Airlines : Page
    {
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private readonly NavigationHelper navigationHelper;

        public Airlines()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.navigationHelper_LoadState;
            this.navigationHelper.SaveState += this.navigationHelper_SaveState;

            if (SharedContext.Context == null)
            {
                this.CreateContext();
            }
            else
            {
                this.DataContext = SharedContext.Context;
                SharedContext.Context.IsStoreCreated = true;
            }

#if WINDOWS_PHONE_APP
			((AppBarButton)this.AppCommandBar.SecondaryCommands[0]).Visibility = Visibility.Visible;
#endif
        }

        private async void CreateContext()
        {
            string message = null;
            try
            {
                var connectionId = (string)Globals.LogonCore.LogonContext.RegistrationContext.ConnectionData["ApplicationConnectionId"];
                var serviceUrl = (string)Globals.LogonCore.LogonContext.RegistrationContext.ConnectionData["ProxyApplicationEndpoint"];
                this.DataContext = SharedContext.Context = new ODataContext(serviceUrl);
                SharedContext.Context.RingVisible = Visibility.Visible;

                var client = new SAP.Net.Http.HttpClient(
                                new System.Net.Http.HttpClientHandler()
                                    {
                                        Credentials =
                                            new System.Net.NetworkCredential(
                                            Globals.LogonCore.LogonContext.RegistrationContext.BackendUserName,
                                            Globals.LogonCore.LogonContext.RegistrationContext.BackendPassword),
                                    },
                                true); // will be disposed by the store!
                client.DefaultRequestHeaders.TryAddWithoutValidation("X-SMP-APPCID", connectionId);
                client.DefaultRequestHeaders.TryAddWithoutValidation("X-SUP-APPCID", connectionId);
                client.ShouldHandleXcsrfToken = true;

                await SharedContext.Context.Store.OpenAsync(client);
                await SharedContext.Context.DownloadCollection("CarrierCollection");
                SharedContext.Context.IsStoreCreated = true;
            }
            catch (Exception ex)
            {
                SharedContext.Context.RingVisible = Visibility.Collapsed;
                message = ex.Message;
            }

            if (message != null)
            {
                await
                    new Windows.UI.Popups.MessageDialog("We could not initialize the store. Details: " + message)
                        .ShowAsync();
            }
        }

        private async void AppBarButtonRefreshClick(object sender, RoutedEventArgs e)
        {
            await this.RefreshCollection();
        }

        private async System.Threading.Tasks.Task RefreshCollection()
        {
            if (SharedContext.Context.RingVisible == Visibility.Visible)
            {
                return;
            }

            string message = null;
            try
            {
                SharedContext.Context.RingVisible = Visibility.Visible;
                await SharedContext.Context.DownloadCollection("CarrierCollection");
            }
            catch (Exception ex)
            {
                SharedContext.Context.RingVisible = Visibility.Collapsed;
                message = ex.Message;
            }

            if (message != null)
            {
                await
                    new Windows.UI.Popups.MessageDialog(
                        "We could not retrieve the list of airlines. Details: " + message).ShowAsync();
            }
        }

        private void AppBarButtonDeleteRegistrationClick(object sender, RoutedEventArgs e)
        {
            RegistrationHelper.DeleteRegistration();
        }

        #region navigation related methods
        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
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
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }
        #endregion

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            if (this.Frame.CanGoBack)
            {
                this.Frame.BackStack.RemoveAt(0); // remove the logon page so that the user cannot navigate back to it (even by pressing ALT+Left)
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void ListRightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private async void BtnShowFlightsClick(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement != null)
            {
                var entity = (SAP.Data.OData.IODataEntity)frameworkElement.Tag;

                SharedContext.Context.IsInEditMode = true;
                var copiedEntity = (SAP.Data.OData.Online.ODataEntity)((SAP.Data.OData.Online.ODataEntity)entity).DeepCopy();
                SharedContext.Context.Entity = copiedEntity;

                System.Diagnostics.Debug.WriteLine(entity.GetNavigationProperty("carrierFlights"));
            }

            string message = null;
            try
            {
                SharedContext.Context.RingVisible = Visibility.Visible;
                await SharedContext.Context.DownloadRelatedItems("carrierFlights");
            }
            catch (Exception ex)
            {
                SharedContext.Context.RingVisible = Visibility.Collapsed;
                message = ex.Message;
            }

            if (message != null)
            {
                await
                    new Windows.UI.Popups.MessageDialog("We could not download related items: " + message).ShowAsync();
            }
            else if (!this.Frame.Navigate(typeof(Flights), null))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }
    }
}
