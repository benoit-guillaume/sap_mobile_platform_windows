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

using SAP.Logon.FlowEngine;
using LogonFlowWindows.Contexts;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LogonFlowWindows
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
                ApplicationId = "com.sap.windows.flight",
                ServerHost = "54.80.15.207",
                IsHttps = false,
                ServerPort = 8080,
                CommunicatorId = "REST",
                BackendUserName = "gwdemo",
                BackendPassword = "welcome"
            };

            logonFlow.CanEditApplicationId = true;

            logonFlow.LogonCompleted += (sender, e) =>
                {
                    Globals.LogonCore = e.LogonCore;
                    this.Frame.Navigate(typeof(MainPage));
                };
        }
    }
}
