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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace OfflineDemo
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class EditorPage : Page
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


        public EditorPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            AppBarBackButton.Visibility = Visibility.Collapsed;
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
            if (SharedContext.Context.Entity == null)
            {
                var entitySet = SharedContext.Context.EntitySet;
                var entity = SharedContext.Context.Entity = new SAP.Data.OData.Online.ODataEntity("RMTSAMPLEFLIGHT.Travelagency");
                SharedContext.Context.Store.AllocateProperties(entity, SAP.Data.OData.Store.PropertyCreationMode.All);
            }

            this.DataContext = SharedContext.Context;
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

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            VisualStateManager.GoToState(this, (e.NewSize.Width < 600) ? "MinimalLayout" : "Filled", false);
        }

        private async void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            SharedContext.Context.RingVisible = Windows.UI.Xaml.Visibility.Visible;
            
            try
            {
                // TODO: Change 3 - Creating new entity or updating existing entity
                var entityToCreateOrModify = SharedContext.Context.Entity;
                var execution = (SharedContext.Context.IsInEditMode) ? SharedContext.Context.Store.ScheduleUpdateEntity(entityToCreateOrModify) :
                                                                       SharedContext.Context.Store.ScheduleCreateEntity(entityToCreateOrModify, "TravelagencyCollection");

                Frame.GoBack();
                await execution.Response;
                await SharedContext.Context.DownloadCollection("TravelagencyCollection");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            SharedContext.Context.RingVisible = Visibility.Collapsed;
        }

        private void AppBarButtonGoBackClick(object sender, RoutedEventArgs e)
        {
            if (navigationHelper.CanGoBack())
                navigationHelper.GoBack();
        }
    }
}
