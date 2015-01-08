using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;

using Newtonsoft.Json.Linq;

using SAP.Data.OData;
using SAP.Data.OData.Store;


namespace RKT_WPF_JSONService.Contexts
{
    public class ODataContext : INotifyPropertyChanged
    {
        // methods
        public ODataContext(string serviceUrl)
        {
            this.Store = new SAP.Data.OData.Online.Store.ODataStore(serviceUrl);
            this.IsStoreCreated = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public async System.Threading.Tasks.Task DownloadCollection(string collectionName)
        {
            try
            {
                this.RingVisible = Visibility.Visible;
                var execution = this.Store.ScheduleRequest(new ODataRequestParametersSingle(collectionName));
                var response = await execution.Response;
                if (response is IODataResponseSingle)
                {
                    if (((IODataResponseSingle)response).PayloadType == ODataType.EntitySet)
                    {
                        this.EntitySet = (SAP.Data.OData.Online.ODataEntitySet)((IODataResponseSingle)response).Payload;
                    }
                }
                this.RingVisible = Visibility.Collapsed;
            }
            catch (Exception)
            {
                this.RingVisible = Visibility.Collapsed;
                throw;
            }
        }

        public async System.Threading.Tasks.Task DownloadRelatedItems(string navigationPropertyName)
        {
            if (this.Entity == null)
            {
                this.RingVisible = Visibility.Collapsed;
                throw new Exception("There is no entity in the context.");
            }

            IODataNavigationProperty navigationProperty = this.entity.GetNavigationProperty(navigationPropertyName);

            try
            {
                this.RingVisible = Visibility.Visible;
                var execution = this.Store.ScheduleRequest(new ODataRequestParametersSingle(navigationProperty.AssociationResourcePath));
                var response = await execution.Response;
                if (response is IODataResponseSingle)
                {
                    if (((IODataResponseSingle)response).PayloadType == ODataType.EntitySet)
                    {
                        this.RelatedEntitySet = (SAP.Data.OData.Online.ODataEntitySet)((IODataResponseSingle)response).Payload;
                    }
                }

                this.RingVisible = Visibility.Collapsed;
            }
            catch (Exception)
            {
                this.RingVisible = Visibility.Collapsed;
                throw;
            }
        }

        public async System.Threading.Tasks.Task DownloadAirportStatus(string url)
        {
            try
            {
                this.RingVisible = Visibility.Visible;

                var connectionId = (string)Globals.LogonCore.LogonContext.RegistrationContext.ConnectionData["ApplicationConnectionId"];

                // TODO: Change 1 - Create a new instance of SAP.Net.Http.HttpClient
                // Create an HttpClient instance
                //var client = new SAP.Net.Http.HttpClient(
                //                new System.Net.Http.HttpClientHandler()
                //                {
                //                    Credentials =
                //                        new System.Net.NetworkCredential(
                //                        Globals.LogonCore.LogonContext.RegistrationContext.BackendUserName,
                //                        Globals.LogonCore.LogonContext.RegistrationContext.BackendPassword), UseProxy = false
                //                },
                //                true); // will be disposed by the store!

                //client.DefaultRequestHeaders.TryAddWithoutValidation("X-SMP-APPCID", connectionId);
                //client.DefaultRequestHeaders.TryAddWithoutValidation("X-SUP-APPCID", connectionId);
                //client.ShouldHandleXcsrfToken = true;

                // TODO: Change 2 - Process the response from server
                //// Send a request asynchronously continue when complete
                //HttpResponseMessage response = await client.GetAsync(url);

                //// Check that response was successful or throw exception
                //response.EnsureSuccessStatusCode();

                //// Read response asynchronously as JsonValue 
                //var content = await response.Content.ReadAsStringAsync();

                //// Parse the content into a JsonObject
                //var jsonContent = JObject.Parse(content);

                var airlineId = (string)RelatedEntity.Properties["carrid"].Value;
                var flightNumber = (string)RelatedEntity.Properties["connid"].Value;
                var flightDate = (DateTime)RelatedEntity.Properties["fldate"].Value;
                var price = (decimal)RelatedEntity.Properties["PRICE"].Value;
                var currency = (string)RelatedEntity.Properties["CURRENCY"].Value;

                // TODO: Change 3 - Retrieve property values
                //var airport = jsonContent["name"].ToString();
                //jsonContent = jsonContent["weather"].ToObject<JObject>();
                //var weather = jsonContent["weather"].ToString();
                //var temp = jsonContent["temp"].ToString();
                //var wind = jsonContent["wind"].ToString();

                //SharedContext.JsonContext = new JsonContext(airlineId, flightNumber, flightDate, price, currency, airport, weather, temp, wind);

                this.RingVisible = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                this.RingVisible = Visibility.Collapsed;
                throw;
            }
        }

        // events
        public event EventHandler OperationFinished;

        // properties
        public SAP.Data.OData.Online.Store.ODataStore Store { get; set; }

        private bool isStoreCreated;
        public bool IsStoreCreated
        {
            get { return this.isStoreCreated; }
            set
            {
                this.isStoreCreated = value;
                this.NotifyPropertyChanged();
            }
        }

        private SAP.Data.OData.Online.ODataEntitySet entitySet;
        public SAP.Data.OData.Online.ODataEntitySet EntitySet
        {
            get { return this.entitySet; }
            set
            {
                this.entitySet = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("EntitySetCount");
            }
        }

        public string EntitySetCount
        {
            get
            {
                return this.entitySet == null ? "(0)" : "(" + this.entitySet.Count + ")";
            }
            set { }
        }

        private SAP.Data.OData.Online.ODataEntity entity;
        public SAP.Data.OData.Online.ODataEntity Entity
        {
            get { return this.entity; }
            set
            {
                this.entity = value;
                this.NotifyPropertyChanged();
            }
        }

        private SAP.Data.OData.Online.ODataEntitySet relatedEntitySet;
        public SAP.Data.OData.Online.ODataEntitySet RelatedEntitySet
        {
            get { return this.relatedEntitySet; }
            set
            {
                this.relatedEntitySet = value;
                this.NotifyPropertyChanged();
            }
        }

        private SAP.Data.OData.Online.ODataEntity relatedEntity;
        public SAP.Data.OData.Online.ODataEntity RelatedEntity
        {
            get
            {
                return this.relatedEntity;
            }

            set
            {
                this.relatedEntity = value;
                this.NotifyPropertyChanged();
            }
        }

        public string RelatedEntitySetCount
        {
            get
            {
                return this.relatedEntitySet == null ? "(0)" : "(" + this.relatedEntitySet.Count + ")";
            }
            set { }
        }

        private Visibility ringVisible = Visibility.Collapsed;
        public Visibility RingVisible
        {
            get { return this.ringVisible; }
            set
            {
                this.ringVisible = value;
                this.CanEditItems = (value != Visibility.Visible);

                if ((this.CanEditItems) && (this.OperationFinished != null))
                {
                    this.OperationFinished(this, null);
                }

                this.NotifyPropertyChanged();
            }
        }

        private bool canEditItems;
        public bool CanEditItems
        {
            get { return this.canEditItems; }
            set
            {
                this.canEditItems = value;
                this.NotifyPropertyChanged();
            }
        }

        public bool IsInEditMode { get; set; }
    }

    public class SharedContext
    {
        public static ODataContext Context { get; set; }

        public static JsonContext JsonContext { get; set; }
    }
}
