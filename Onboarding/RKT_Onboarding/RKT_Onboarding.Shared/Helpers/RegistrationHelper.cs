using System;

using RKT_Onboarding.Contexts;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RKT_Onboarding.Helpers
{
    public static class RegistrationHelper
    {
        public static async void DeleteRegistration()
        {
            // this method should not be awaitable therefore using async void
            // TODO: Change 7 - Delete registration
            //var rootFrame = Window.Current.Content as Frame;
            //await Globals.LogonCore.UnregisterAsync();
            //NavigateToLogonPage(rootFrame);
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
