using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace RKT_E2ETracing.Contexts
{
    public class SupportabilityContext
    {
        public IReadOnlyList<SAP.Supportability.Logging.IClientLogEntry> ClientLogs { get; set; }

        public SAP.Net.Http.HttpClient HttpClient { get; set; }

        private SAP.Supportability.Tracing.E2ETraceManager traceManager;

        public SAP.Supportability.Tracing.E2ETraceManager TraceManager
        {
            get
            {
                if (traceManager == null)
                {
                    traceManager = (SAP.Supportability.Tracing.E2ETraceManager)SAP.Supportability.SupportabilityFacade.Instance.E2ETraceManager;
                }

                return traceManager;
            }

            set { }
        }

        private SAP.Supportability.Logging.IClientLogManager logManager;

        public SAP.Supportability.Logging.IClientLogManager LogManager
        {
            get
            {
                if (logManager == null)
                {
                    logManager = SAP.Supportability.SupportabilityFacade.Instance.ClientLogManager;
                }

                return logManager;
            }

            set { }
        }

        private SAP.Supportability.Logging.IClientLogger logger;

        public SAP.Supportability.Logging.IClientLogger Logger
        {
            get
            {
                if (logger == null)
                {
                    LogManager.SetLogLevel(SAP.Supportability.Logging.ClientLogLevel.Info);
                    LogManager.SetLogDestination(SAP.Supportability.Logging.ClientLogDestinations.FileSystem | SAP.Supportability.Logging.ClientLogDestinations.Console);

                    logger = LogManager.GetLogger("testLogger");
                }

                return logger;
            }

            set { }
        }
    }

    public class LoggingContext
    {
        public static SupportabilityContext Context { get; set; }
    }

    public class SupportabilityUploader : SAP.Supportability.IUploader
    {
        SAP.Net.Http.HttpClient clientRef = null; // will be disposed by the owner/caller
        string urlPostfix = null;

        public SupportabilityUploader(SAP.Net.Http.HttpClient client, bool uploadBtx = true)
        {
            if (client == null) throw new ArgumentNullException("client");

            this.urlPostfix = uploadBtx ? "btx" : "clientlogs";
            this.clientRef = client;
        }

        public IAsyncOperation<SAP.Supportability.IUploadResult> SendAsync(IReadOnlyDictionary<string, string> headers, Windows.Storage.Streams.IInputStream payload)
        {
            return Task.Run<SAP.Supportability.IUploadResult>(async () =>
            {
                var result = await this.clientRef.SendAsync(() =>
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "http://smpqa-win12-03.sybase.com:8080/" + this.urlPostfix)
                    {
                        Content = new StreamContent(payload.AsStreamForRead())
                    };

                    foreach (var header in headers)
                    {
                        request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }

                    return request;
                });

                return new UploadResult()
                {
                    ResponseStatusCode = (int)result.StatusCode,
                    Hint = await result.Content.ReadAsStringAsync()
                };
            }).AsAsyncOperation();
        }
    }

    public class UploadResult : SAP.Supportability.IUploadResult
    {
        public int ResponseStatusCode { get; set; }
        public string Hint { get; set; }
    }
}
