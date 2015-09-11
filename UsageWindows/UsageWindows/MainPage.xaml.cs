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

using SAP.Usage;
using UsageWindows.Contexts;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UsageWindows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            if (SharedContext.Context == null)
            {
                this.DataContext = SharedContext.Context = new UsageContext();
            }
            else
            {
                this.DataContext = SharedContext.Context;
            }
        }

        private void DeviceInfoClick(object sender, RoutedEventArgs e)
        {
            SharedContext.Context.DeviceInformation = "Application: " + Usage.DeviceInformation.Application + "\nAppversion: " + Usage.DeviceInformation.AppVersion + "\nModel: " + Usage.DeviceInformation.Model + "\nPlatform: " + Usage.DeviceInformation.Platform + "\nSystem version: " + Usage.DeviceInformation.SystemVersion + "\nNetwork: " + UsageReachability.CurrentReachabilityStatus;
        }

        private void StartTimerClick(object sender, RoutedEventArgs e)
        {
            Usage.TimeStart("myTimer");
        }

        private async void StopTimerClick(object sender, RoutedEventArgs e)
        {
            // stops the timer. The last 2 parameters are optional
            await Usage.TimeEndAsync("myTimer", new Dictionary<string, string>() { { "customKey", "customVal" } }, "customType");
        }

        private async void LogClick(object sender, RoutedEventArgs e)
        {
            // adds a log entry into the report. The last 2 parameters are optional
            await Usage.LogAsync("logKey", new Dictionary<string, string>() { { "testKey", "testValue" } }, "testType");
        }
    }
}
