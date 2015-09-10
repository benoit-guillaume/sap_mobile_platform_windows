using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using PushDemo.Contexts;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PushDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LogonPage : Page
    {
        private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

        public LogonPage()
        {
            this.InitializeComponent();

            logonFlow.DefaultValues = new SAP.Logon.Core.RegistrationContext()
            {
                ApplicationId = "com.sap.push",
                ServerHost = "10.9.38.77",
                IsHttps = false,
                ServerPort = 8080,
                CommunicatorId = "REST",
                BackendUserName = "gwdemo",
                BackendPassword = "welcome"
            };

            logonFlow.LogonCompleted += logonFlow_LogonCompleted;
        }

        public async void logonFlow_LogonCompleted(object sender, SAP.Logon.FlowEngine.LogonCompletedEventArgs e)
        {
            // TODO: Change 1 - Request a push notification channel and define event handler method for PushNotificationReceived event
            PushNotificationChannel currentChannel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

            currentChannel.PushNotificationReceived += currentChannel_PushNotificationReceived;

            // TODO: Change 2 - Submit push notification channel uri to SAP Mobile Platform if it has changed
            if (currentChannel.Uri != RetrieveStoredChannelUri())
            {
                await PostChannelUriToSMPServer(e.LogonCore, currentChannel.Uri);
                settings.Values["ChannelUri"] = currentChannel.Uri;
            }

            Globals.LogonCore = e.LogonCore;
            this.Frame.Navigate(typeof(MainPage));
        }

        private async System.Threading.Tasks.Task PostChannelUriToSMPServer(SAP.Logon.Core.LogonCore logonCore, string uri)
        {
            var writableSettings = logonCore.ApplicationSettings.GetWritableSettings();
            writableSettings["WnsChannelURI"].Value = uri;
            await logonCore.ApplicationSettings.UpdateSettingsAsync(writableSettings);
        }

        private void currentChannel_PushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            // TODO: Change 4 - Event handler method for PushNotificationReceived event 
            var notificationContent = String.Empty;

            switch (args.NotificationType)
            {
                case PushNotificationType.Badge:
                    notificationContent = args.BadgeNotification.Content.GetXml();
                    break;

                case PushNotificationType.Tile:
                    notificationContent = args.TileNotification.Content.GetXml();
                    break;

                case PushNotificationType.Toast:
                    notificationContent = args.ToastNotification.Content.GetXml();
                    break;

                case PushNotificationType.Raw:
                    notificationContent = args.RawNotification.Content;
                    break;
            }

            args.Cancel = true;
        }

        private string RetrieveStoredChannelUri()
        {
            object uri = settings.Values["ChannelUri"];
            if (uri != null)
            {
                return uri.ToString();
            }

            return string.Empty;
        }
    }
}
