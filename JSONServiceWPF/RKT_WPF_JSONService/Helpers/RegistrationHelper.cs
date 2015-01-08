using RKT_WPF_JSONService.Contexts;

namespace RKT_WPF_JSONService.Helpers
{
    static class RegistrationHelper
    {
        public static async void DeleteRegistration()
        { 
            // this method should not be awaitable therefore using async void
            await Globals.LogonCore.UnregisterAsync();
            SharedContext.Context.Store.Dispose();
            SharedContext.Context = null;
            NavigateToLogonPage();
        }

        public static void NavigateToLogonPage()
        {
            new LogonWindow().Show();
        }
    }
}
