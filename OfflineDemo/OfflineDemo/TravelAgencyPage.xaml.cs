using OfflineDemo.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using OfflineDemo.Contexts;
using OfflineDemo.Helpers;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace OfflineDemo
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class TravelAgencyPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public TravelAgencyPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            if (SharedContext.Context == null)
            {
                this.CreateContext();
            }
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
                var applicationSettings = Globals.LogonCore.ApplicationSettings;
                var connectionId = applicationSettings.ApplicationConnectionId;

                this.DataContext = SharedContext.Context = new ODataContext();

                // TODO: Change 1 - Open the offline store
                var client = new SAP.Net.Http.HttpClient(
                                new System.Net.Http.HttpClientHandler()
                                {
                                    Credentials =
                                        new System.Net.NetworkCredential(
                                        Globals.LogonCore.LogonContext.RegistrationContext.BackendUserName,
                                        Globals.LogonCore.LogonContext.RegistrationContext.BackendPassword)
                                },
                                true); // will be disposed by the store!
                client.DefaultRequestHeaders.TryAddWithoutValidation("X-SMP-APPCID", connectionId);
                client.DefaultRequestHeaders.TryAddWithoutValidation("X-SUP-APPCID", connectionId);
                client.ShouldHandleXcsrfToken = true;

                var options = new SAP.Data.OData.Offline.Store.ODataOfflineStoreOptions();
                options.ConversationManager = client;

                options.Host = "10.4.64.212";
                options.Port = 8080;
                options.ServiceRoot = "com.sap.flight";
                options.EnableHttps = false;
                options.StoreName = "OfflineStore";
                options.StoreEncryptionKey = "SuperSecretEncryptionKey";
                options.URLSuffix = "";
                options.AddDefiningRequest("AgencyDefiningRequest", "TravelagencyCollection?$top=5", false);
                options.EnableRepeatableRequests = false;

                await SharedContext.Context.Store.OpenAsync(options);

                System.Diagnostics.Debug.WriteLine("Application Folder: " + Windows.Storage.ApplicationData.Current.LocalFolder.Path);

                // TODO: Change 2 - Read from the offline store
                await SharedContext.Context.DownloadCollection("TravelagencyCollection");

                //if (Globals.IsInitialConnection)
                //{
                //    await SharedContext.Context.DownloadCollection("TravelagencyCollection");
                //    Globals.Settings.Values["IsInitialConnection"] = "false";
                //}
                //else
                //{
                //    Globals.Settings.Values["IsInitialConnection"] = "false";
                //    await SharedContext.Context.DownloadCollection("TravelagencyCollection");
                //}

                SharedContext.Context.IsStoreCreated = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            if (message != null)
            {
                await
                    new Windows.UI.Popups.MessageDialog("We could not initialize the store. Details: " + message)
                        .ShowAsync();
            }
        }

        private void BtnShowFlightsClick(object sender, RoutedEventArgs e)
        {

        }

        private void ListRightTapped(object sender, RightTappedRoutedEventArgs e)
        {

        }

        private async void AppBarButtonRefreshClick(object sender, RoutedEventArgs e)
        {
            await this.RefreshCollection();
        }

        private void AppBarButtonDeleteRegistrationClick(object sender, RoutedEventArgs e)
        {
            RegistrationHelper.DeleteRegistration();
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
                // TODO: Change 5 - Calling refresh to download server changes
                await SharedContext.Context.Store.ScheduleRefreshAsync();
                SharedContext.Context.RingVisible = Visibility.Collapsed;
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
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void BtnAddClick(object sender, RoutedEventArgs e)
        {
            OpenItem(null);
        }

        private void OpenItem(SAP.Data.OData.IODataEntity entity)
        {
            if ((SharedContext.Context.IsInEditMode = (entity != null)))
            {
                SharedContext.Context.IsInEditMode = true;
                var copiedEntity = entity.DeepCopy();

                SharedContext.Context.Entity = copiedEntity;
            }
            else
            {
                SharedContext.Context.Entity = null;
            }

            if (!Frame.Navigate(typeof(EditorPage), null))
            {
                throw new Exception("Navigation failed !!");
            }
        }

        private async void AppBarButtonFlushClick(object sender, RoutedEventArgs e)
        {
            SharedContext.Context.RingVisible = Visibility.Visible;
            // TODO: Change 4 - Calling flush to upload the local changes
            await SharedContext.Context.Store.ScheduleFlushQueuedRequestsAsync();
            SharedContext.Context.RingVisible = Visibility.Collapsed;
        }

        private void BtnEditClick(object sender, RoutedEventArgs e)
        {
            OpenItem((SAP.Data.OData.IODataEntity)ItemGridView.SelectedItem);
        }

        private async void BtnDeleteClick(object sender, RoutedEventArgs e)
        {
            var entity = (SAP.Data.OData.IODataEntity)ItemGridView.SelectedItem;
            if (entity == null)
            {
                entity = (SAP.Data.OData.IODataEntity)(sender as FrameworkElement).Tag;
            }

            if (entity == null)
            {
                return;
            }

            var dialog = new Windows.UI.Popups.MessageDialog("Are you sure you want to delete '" + entity.Properties["NAME"].Value + "'?", "Delete Travel Agency?");
            dialog.Commands.Add(new Windows.UI.Popups.UICommand
            {
                Label = "yes",
                Invoked = async (c) =>
                {
                    string message = null;
                    try
                    {
                        SharedContext.Context.RingVisible = Visibility.Visible;
                        await SharedContext.Context.Store.ScheduleDeleteEntity(entity).Response;
                        await SharedContext.Context.DownloadCollection("TravelagencyCollection");
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                    }
                    SharedContext.Context.RingVisible = Visibility.Collapsed;
                }
            });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand
            {
                Label = "no"
            });

            var command = await dialog.ShowAsync();
            if (command.Invoked == null) return; // "no" is selected
            command.Invoked(command);
        }

        private void ItemGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == ItemGridView) ItemGridView.SelectedIndex = ItemGridView.SelectedIndex;
            else ItemGridView.SelectedIndex = ItemGridView.SelectedIndex;

            BtnEdit.Visibility = BtnDelete.Visibility = (((ListViewBase)sender).SelectedItem != null) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
