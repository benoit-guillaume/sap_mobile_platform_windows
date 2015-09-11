using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using SAP.Usage;
using System.IO;
using Windows.Security.Cryptography.Certificates;
using Windows.Security.Cryptography;

namespace UsageWindows.Contexts
{
    public class Globals
    {
        public static SAP.Logon.Core.LogonCore LogonCore { get; set; }

        private static string uploadUrl = string.Empty;
        public static string UploadUrl
        {
            get
            {
                var registrationContext = LogonCore.LogonContext.RegistrationContext;

                if (string.IsNullOrEmpty(uploadUrl))
                {
                    //uploadUrl = string.Format("{0}://{1}:{2}/clientusage", registrationContext.IsHttps ? "https" : "http", registrationContext.ServerHost, registrationContext.ServerPort);
                    uploadUrl = "http://127.0.0.1:9735";
                }

                return uploadUrl;
            }

            private set
            {
                uploadUrl = value;
            }
        }

        private static SAP.Net.Http.HttpClient httpClient = new SAP.Net.Http.HttpClient();
        public static SAP.Net.Http.HttpClient HttpClient
        {
            get
            {
                var applicationSettings = LogonCore.ApplicationSettings;
                var uploadTime = applicationSettings.UsageSettings.Usage3GUploadTime;
                Usage.SetTimeFor3GUpload(uploadTime ?? 1);
         
                var registrationContext = LogonCore.LogonContext.RegistrationContext;

                SAP.Logon.Core.Settings.IReadOnlyProperty baseUrlObject = null;
                applicationSettings.TryGetValue("BaseUrl", out baseUrlObject);
                // object baseUrlObject = null;
                // registrationContext.ConnectionData.TryGetValue("BaseUrl", out baseUrlObject);
                Uri baseUrl = (Uri)baseUrlObject;

                SAP.Logon.Core.Settings.IReadOnlyProperty cookieObject = null;
                applicationSettings.TryGetValue("Cookies", out cookieObject);
                // object cookieObject = null;
                // registrationContext.ConnectionData.TryGetValue("Cookies", out cookieObject);
                string cookieString = cookieObject == null ? null : cookieObject.ToString();

                if ((cookieString != null) && (baseUrl != null))
                {
                    httpClient.SetInitialCookies(cookieString, baseUrl);
                }
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-SMP-APPCID", applicationSettings.ApplicationConnectionId);

                return httpClient;
            }
        }

        public static bool IsCertificateInstalled
        {
            get
            {
                bool result = false;
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("CertificateInstalled"))
                {
                    result = (bool)ApplicationData.Current.LocalSettings.Values["CertificateInstalled"];
                }
                    
                return result;
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["CertificateInstalled"] = value;
            }
        }

        public static async Task InstallCertificateAsync()
        {
            var pfx = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///milton.pfx"));

            byte[] rawCertificate = null;
            using (var stream = await pfx.OpenReadAsync())
            {
                using (var reader = new BinaryReader(stream.AsStreamForRead()))
                {
                    rawCertificate = reader.ReadBytes((int)stream.Size);
                }
            }

            //var base64Certificate = Windows.Security.Cryptography.CryptographicBuffer.EncodeToBase64String(rawCertificate.AsBuffer());
            var base64Certificate = Windows.Security.Cryptography.CryptographicBuffer.EncodeToBase64String(CryptographicBuffer.CreateFromByteArray(rawCertificate));
            await Windows.Security.Cryptography.Certificates.CertificateEnrollmentManager.ImportPfxDataAsync(
                              base64Certificate, "mobile123",
                              ExportOption.NotExportable,
                              KeyProtectionLevel.NoConsent,
                              InstallOptions.DeleteExpired, "userCert"
                    );
        }
    }
}
