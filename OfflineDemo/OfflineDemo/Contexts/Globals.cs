using Windows.Storage;

namespace OfflineDemo.Contexts
{
    public static class Globals
    {
        public static SAP.Logon.Core.LogonCore LogonCore { get; set; }

        public static bool IsInitialConnection { get; set; }

        public static ApplicationDataContainer Settings = ApplicationData.Current.LocalSettings;
    }
}