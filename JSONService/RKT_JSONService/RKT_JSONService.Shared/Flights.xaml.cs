using RKT_JSONService.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace RKT_JSONService
{
    using Windows.ApplicationModel.Resources;

    using RKT_JSONService.Contexts;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Flights : Page
    {
        private NavigationHelper navigationHelper;
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        public Flights()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            // TODO: Change 5 - Setting the page's datacontext
            //this.DataContext = SharedContext.Context;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        private void ListRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private async void BtnAirportStatusClick(object sender, RoutedEventArgs e)
        {
            var entity = (SAP.Data.OData.IODataEntity)ItemGridView.SelectedItem;

            if (entity == null)
            {
                return;
            }

            string message = null;
            SharedContext.Context.RelatedEntity = (SAP.Data.OData.Online.ODataEntity)entity;

            // TODO: Change 4 - Prepare the URL and call DownloadAirportStatus
            //var url = "http://" + Globals.LogonCore.LogonContext.RegistrationContext.ServerHost + ":" + Globals.LogonCore.LogonContext.RegistrationContext.ServerPort + "/AirportStatus/" + (string)entity.Properties["flightDetails/airportTo"].Value + "?format=json";

            //try
            //{
            //    SharedContext.Context.RingVisible = Visibility.Visible;
            //    await SharedContext.Context.DownloadAirportStatus(url);
            //}
            catch (Exception ex)
            {
                SharedContext.Context.RingVisible = Visibility.Collapsed;
                message = ex.Message;
            }

            if (message != null)
            {
                await
                    new Windows.UI.Popups.MessageDialog("We could not retrieve airport status: " + message).ShowAsync();
            }
            else if (!Frame.Navigate(typeof(FlightDetails), null))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        private void ItemGridViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Btn_Status.Visibility = (((ListViewBase)sender).SelectedItem != null) ? Visibility.Visible : Visibility.Collapsed;
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

        private void AppBarButtonGoBackClick(object sender, RoutedEventArgs e)
        {
            if (navigationHelper.CanGoBack())
            {
                navigationHelper.GoBack();
            }
        }
    }
}
