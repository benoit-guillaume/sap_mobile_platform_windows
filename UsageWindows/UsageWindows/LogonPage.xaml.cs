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

using UsageWindows.Contexts;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UsageWindows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LogonPage : Page
    {
        public LogonPage()
        {
            this.InitializeComponent();

            

            logonFlow.DefaultValues = new SAP.Logon.Core.RegistrationContext()
            {
                ApplicationId = "com.sap.milton.appmessaging",
                ServerHost = "mobilepiathanamobile-xeb4962cf.neo.ondemand.com",
                IsHttps = true,
                ServerPort = 443,
                //ApplicationId = "com.sap.flight",
                //ServerHost = "127.0.0.1",
                //IsHttps = false,
                //ServerPort = 8080,
                CommunicatorId = "REST",
                BackendUserName = "gwdemo",
                BackendPassword = "welcome"        
            };

            logonFlow.LogonCompleted += async (sender, eventArgs) =>
            {
                Globals.LogonCore = eventArgs.LogonCore;
                this.Frame.Navigate(typeof(MainPage));   
       
                await SAP.Usage.Usage.InitUsageAsync(Globals.UploadUrl, Globals.HttpClient);
                // notify the Usage that the app goes to foreground
                SAP.Usage.Usage.ApplicationWillEnterForeground();
            };
        }
    }
}
