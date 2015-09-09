using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using OfflineDemo.Contexts;

namespace OfflineDemo.Helpers
{
    public static class RegistrationHelper
    {
        public static async void DeleteRegistration()
        {
            // this method should not be awaitable therefore using async void
            var rootFrame = Window.Current.Content as Frame;
            await Globals.LogonCore.UnregisterAsync();
            SharedContext.Context.Store.Dispose();
            SharedContext.Context = null;

            Globals.Settings.Values.Remove("IsInitialConnection");
            NavigateToLogonPage(rootFrame);
        }

        public static void NavigateToLogonPage(Frame rootFrame)
        {
            rootFrame.Navigate(typeof(LogonPage));

            // remove all other items in the backstack!
            if (rootFrame.BackStack.Count >= 1)
            {
                while (rootFrame.BackStack.Count >= 1)
                {
                    rootFrame.BackStack.RemoveAt(0);
                }
            }
        }
    }
}
