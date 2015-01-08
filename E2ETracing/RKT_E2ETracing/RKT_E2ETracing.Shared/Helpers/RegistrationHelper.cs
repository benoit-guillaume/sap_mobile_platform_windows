namespace RKT_E2ETracing.Helpers
{
    using System;

    using RKT_E2ETracing;

    using RKT_E2ETracing.Contexts;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public static class RegistrationHelper
    {
        public static async void DeleteRegistration()
        {
            // this method should not be awaitable therefore using async void
            var rootFrame = Window.Current.Content as Frame;
            await Globals.LogonCore.UnregisterAsync();
            SharedContext.Context.Store.Dispose();
            SharedContext.Context = null;
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

        public static async Task UploadLogs()
        {
            // TODO: Change 6 - Calling UploadClientLogsAsync
            //await SAP.Supportability.SupportabilityFacade.Instance.ClientLogManager.UploadClientLogsAsync(new SupportabilityUploader(LoggingContext.Context.HttpClient, false));
        }

        public static async Task UploadTransactions()
        {
            LoggingContext.Context.ClientLogs = null;
            var traceManager = (SAP.Supportability.Tracing.E2ETraceManager)SAP.Supportability.SupportabilityFacade.Instance.E2ETraceManager;
            traceManager.ClientHost = "WinDemo-Client";
            traceManager.TraceLevel = SAP.Supportability.Tracing.E2ETraceLevel.Low;

            var transaction = await traceManager.StartTransactionAsync("WpfNewTransactionWin");

            var step = transaction.StartStep();
            var request = step.StartRequest();
            request.SetRequestLine("GET http//www.test.com HTTP/1.1");

            request.SetRequestHeaders(new Dictionary<string, string> { 
					{"SAP-PASSPORT",request.PassportHttpHeader} ,
					{"X-CorrelationID","correlationID0101"} 
				});
            request.SetByteCountSent(0);
            request.EndRequest();
            step.EndStep();

            transaction.EndTransaction();

            // TODO: Change 8 - Calling UploadBtxAsync
            //await SAP.Supportability.SupportabilityFacade.Instance.E2ETraceManager.UploadBtxAsync(new SupportabilityUploader(LoggingContext.Context.HttpClient));
        }
    }
}
