using RKT_WPF_Onboarding.Contexts;

namespace RKT_WPF_Onboarding.Helpers
{
    static class RegistrationHelper
    {
        public static async void DeleteRegistration()
        { 
            // this method should not be awaitable therefore using async void

            // TODO: Change 7 - Delete registration
            //await Globals.LogonCore.UnregisterAsync();
            //NavigateToLogonPage();
        }

        public static void NavigateToLogonPage()
        {
            new LogonWindow().Show();
        }
    }
}
