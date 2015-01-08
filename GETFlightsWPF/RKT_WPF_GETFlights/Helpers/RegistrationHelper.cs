namespace RKT_WPF_GETFlights.Helpers
{
    using RKT_WPF_GETFlights.Contexts;

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
