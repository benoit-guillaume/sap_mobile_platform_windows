using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using RKT_WPF_JSONService.Contexts;

namespace RKT_WPF_JSONService
{
    /// <summary>
    /// Interaction logic for FlightDetails.xaml
    /// </summary>
    public partial class FlightDetails : Page
    {
        public FlightDetails()
        {
            InitializeComponent();

            // TODO: Change 5 - Setting the page's datacontext
            //this.DataContext = SharedContext.JsonContext;
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
