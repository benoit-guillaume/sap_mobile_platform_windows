using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using RKT_WPF_E2ETracing.Contexts;
using RKT_WPF_E2ETracing.Helpers;

using SAP.Supportability.Logging;

namespace RKT_WPF_E2ETracing
{
    /// <summary>
    /// Interaction logic for Airlines.xaml
    /// </summary>
    public partial class Airlines : Page
    {
        public Airlines()
        {
            InitializeComponent();

            this.ShouldExitAppOnClose = true;
            if (SharedContext.Context == null) this.CreateContext();
            else
            {
                this.DataContext = SharedContext.Context;
                SharedContext.Context.IsStoreCreated = true;
            }
        }

        private async void CreateContext()
        {
            string message = null;
            try
            {
                var registrationContext = Globals.LogonCore.LogonContext.RegistrationContext;
                string connectionId = (string)registrationContext.ConnectionData["ApplicationConnectionId"];
                string serviceUrl = (string)registrationContext.ConnectionData["ProxyApplicationEndpoint"];
                this.DataContext = SharedContext.Context = new ODataContext(serviceUrl);
                SharedContext.Context.RingVisible = Visibility.Visible;

                var client = new SAP.Net.Http.HttpClient(
                                new System.Net.Http.HttpClientHandler()
                                {
                                    Credentials = new System.Net.NetworkCredential((string)registrationContext.BackendUserName,
                                                                                   (string)registrationContext.BackendPassword),
                                    UseProxy = false
                                }, true
                             ); // will be disposed by the store!
                client.DefaultRequestHeaders.TryAddWithoutValidation("X-SMP-APPCID", connectionId);
                client.DefaultRequestHeaders.TryAddWithoutValidation("X-SUP-APPCID", connectionId);
                client.ShouldHandleXcsrfToken = true;
                //client.ShouldHandleSamlRequests = true;
                //client.SamlFinishEndpoint = new UriBuilder(registrationContext.IsHttps ? "https" : "http",
                //                                           registrationContext.ServerHost,
                //                                           registrationContext.ServerPort, "/SAMLAuthLauncher").Uri;
                //client.SamlFinishEndpointParameter = "finishEndpointParam";

                LoggingContext.Context.HttpClient = client;

                await SharedContext.Context.Store.OpenAsync(client);

                LoggingContext.Context.Logger.LogInfo("E2ETracing - Retrieving carriers");
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
                await this.ShowErrorMessageAsync("We could not initialize the store. Details: " + message);
            }
            else
            {
                LoggingContext.Context.Logger.LogInfo("E2ETracing - Successfully initialized the store");
            }
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

        private async void ExecuteAsyncActionSafe(Func<Task> action)
        {
            string message = null;
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            if (message != null)
                await Dispatcher.BeginInvoke((Action)(() =>
                {
                    MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }));
        }

        private void MenuItemUploadLogsClick(object sender, RoutedEventArgs e)
        {
            this.ExecuteAsyncActionSafe(async () =>
            {
                string message = null;

                // TODO: Change 5 - Calling UploadLogs
                //try
                //{
                //    await RegistrationHelper.UploadLogs();
                //}
                //catch (Exception ex)
                //{
                //    var supportabilityException = ex as SAP.Supportability.ISupportabilityException;
                //    message = ex.Message + ((supportabilityException != null) ? ("(" + supportabilityException.UploadResult.ResponseStatusCode + ")") : "");
                //}

                await Dispatcher.BeginInvoke((Action)(() =>
                {
                    MessageBox.Show(message ?? "Successfully uploaded client logs !", (message == null) ? "Information" : "Error", MessageBoxButton.OK, ((message == null) ? MessageBoxImage.Information : MessageBoxImage.Error));
                }));
            });
        }

        private async void MenuItemUploadTransactionsClick(object sender, RoutedEventArgs e)
        {
            this.ExecuteAsyncActionSafe(async () =>
            {
                string message = null;

                // TODO: Change 7 - Calling UploadTransactions
                //try
                //{
                //    await RegistrationHelper.UploadTransactions();
                //}
                //catch (Exception ex)
                //{
                //    var supportabilityException = ex as SAP.Supportability.ISupportabilityException;
                //    message = ex.Message + ((supportabilityException != null) ? ("(" + supportabilityException.UploadResult.ResponseStatusCode + ")") : "");
                //}

                await Dispatcher.BeginInvoke((Action)(() =>
                {
                    MessageBox.Show(message ?? "Successfully uploaded trace logs !", (message == null) ? "Information" : "Error", MessageBoxButton.OK, ((message == null) ? MessageBoxImage.Information : MessageBoxImage.Error));
                }));
            });      
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
            {
                LoggingContext.Context.Logger.LogError("E2ETracing - Error retrieving list of carriers: " + message);
                await this.ShowErrorMessageAsync("We could not retrieve the list of carriers. Details: " + message);
            }
            else
            {
                LoggingContext.Context.Logger.LogInfo("E2ETracing - Successfully retrieved list of carriers");
            }
        }

        private void MenuItemAboutClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Flight Application", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuItemDeleteRegistrationClick(object sender, RoutedEventArgs e)
        {
            LoggingContext.Context.Logger.LogInfo("E2ETracing - Deleting registration");
            this.ShouldExitAppOnClose = false;
            DeleteRegistrationWindowClose = true;
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

        public bool ShouldExitAppOnClose { get; set; }

        public static bool DeleteRegistrationWindowClose { get; set; }

        private async void BtnShowFlightsClick(object sender, RoutedEventArgs e)
        {
            LoggingContext.Context.Logger.LogInfo("E2ETracing - Retrieving flights");

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
