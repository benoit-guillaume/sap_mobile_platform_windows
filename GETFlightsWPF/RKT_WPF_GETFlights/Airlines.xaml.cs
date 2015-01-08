using System.Windows.Navigation;

namespace RKT_WPF_GETFlights
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows;

    using RKT_WPF_GETFlights.Contexts;
    using RKT_WPF_GETFlights.Helpers;

    /// <summary>
    /// Interaction logic for Airlines.xaml
    /// </summary>
    public partial class Airlines : Page
    {
        public Airlines()
        {
            InitializeComponent();

            this.ShouldExitAppOnClose = true;

            // TODO: Change 1 - Setting the page's datacontext
            //if (SharedContext.Context == null)
            //{
            //    this.CreateContext();
            //}
            //else
            //{
            //    this.DataContext = SharedContext.Context;
            //    SharedContext.Context.IsStoreCreated = true;
            //}
        }

        private async void CreateContext()
        {
            string message = null;
            try
            {
                var registrationContext = Globals.LogonCore.LogonContext.RegistrationContext;
                string connectionId = (string)registrationContext.ConnectionData["ApplicationConnectionId"];
                string serviceUrl = (string)registrationContext.ConnectionData["ProxyApplicationEndpoint"];

                // TODO: Change 3 - Creating the datacontext
                //this.DataContext = SharedContext.Context = new ODataContext(serviceUrl);
                //SharedContext.Context.RingVisible = Visibility.Visible;

                //var client = new SAP.Net.Http.HttpClient(
                //                new System.Net.Http.HttpClientHandler()
                //                {
                //                    Credentials = new System.Net.NetworkCredential((string)registrationContext.BackendUserName,
                //                                                                   (string)registrationContext.BackendPassword),
                //                    UseProxy = false
                //                }, true
                //             ); // will be disposed by the store!
                //client.DefaultRequestHeaders.TryAddWithoutValidation("X-SMP-APPCID", connectionId);
                //client.DefaultRequestHeaders.TryAddWithoutValidation("X-SUP-APPCID", connectionId);
                //client.ShouldHandleXcsrfToken = true;

                //await SharedContext.Context.Store.OpenAsync(client);
                //await SharedContext.Context.DownloadCollection("CarrierCollection");
                SharedContext.Context.IsStoreCreated = true;
            }
            catch (Exception ex)
            {
                SharedContext.Context.RingVisible = Visibility.Collapsed;
                message = ex.Message;
            }

            if (message != null)
                await this.ShowErrorMessageAsync("We could not initialize the store. Details: " + message);
        }

        private async Task ShowErrorMessageAsync(string message)
        {
            await this.Dispatcher.BeginInvoke((Action)delegate()
            {
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        private async void MenuItemRefreshClick(object sender, RoutedEventArgs e)
        {
            await this.RefreshCollection();
        }

        private async Task RefreshCollection()
        {
            if (SharedContext.Context.RingVisible == Visibility.Visible) return;
            string message = null;
            try
            {
                SharedContext.Context.EntitySet = null;
                SharedContext.Context.RingVisible = Visibility.Visible;
                await SharedContext.Context.DownloadCollection("CarrierCollection");
            }
            catch (Exception ex)
            {
                SharedContext.Context.RingVisible = Visibility.Collapsed;
                message = ex.Message;
            }

            if (message != null)
                await this.ShowErrorMessageAsync("We could not retrieve the list of carriers. Details: " + message);
        }

        private void MenuItemAboutClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Flight Application", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuItemDeleteRegistrationClick(object sender, RoutedEventArgs e)
        {
            this.ShouldExitAppOnClose = false;
            // close the window
            ((NavigationWindow)(this.Parent)).Close();

            RegistrationHelper.DeleteRegistration();
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
                await this.ShowErrorMessageAsync("We could not download related items: " + message);
            }
            else if (!NavigateToFlightsPage())
            {
                throw new Exception("Navigation Failed");
            }
        }

        public bool NavigateToFlightsPage()
        {
            try
            {
                // View flights
                Flights flightPage = new Flights();
                this.NavigationService.Navigate(flightPage);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
