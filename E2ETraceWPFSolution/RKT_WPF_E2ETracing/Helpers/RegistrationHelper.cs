using RKT_WPF_E2ETracing.Contexts;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace RKT_WPF_E2ETracing.Helpers
{
    static class RegistrationHelper
    {
        public static async void DeleteRegistration()
        { 
            // this method should not be awaitable therefore using async void
            await Globals.LogonCore.UnregisterAsync();
            SharedContext.Context.Store.Dispose();
            SharedContext.Context = null;
        }

        public static async Task UploadLogs()
        {
            await SAP.Supportability.SupportabilityFacade.Instance.ClientLogManager.UploadClientLogsAsync(new SupportabilityUploader(LoggingContext.Context.HttpClient, false));
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
            await SAP.Supportability.SupportabilityFacade.Instance.E2ETraceManager.UploadBtxAsync(new SupportabilityUploader(LoggingContext.Context.HttpClient));
        }
    }
}
