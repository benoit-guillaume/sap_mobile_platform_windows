namespace RKT_JSONService.Contexts
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Runtime.CompilerServices;

    using SAP.Data.OData;
    using SAP.Data.OData.Store;

    using Windows.Data.Json;
    using Windows.UI.Xaml;

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
                var execution = this.Store.ScheduleReadEntitySet(collectionName);
                var response = await execution.Response;

                this.EntitySet = (SAP.Data.OData.Online.ODataEntitySet)((IODataResponseSingle)response).Payload;

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
                
                // Create an HttpClient instance
                var client = new SAP.Net.Http.HttpClient(
                                new System.Net.Http.HttpClientHandler()
                                {
                                    Credentials =
                                        new System.Net.NetworkCredential(
                                        Globals.LogonCore.LogonContext.RegistrationContext.BackendUserName,
                                        Globals.LogonCore.LogonContext.RegistrationContext.BackendPassword), 
                                }, 
                                true); // will be disposed by the store!

                client.DefaultRequestHeaders.TryAddWithoutValidation("X-SMP-APPCID", connectionId);
                client.DefaultRequestHeaders.TryAddWithoutValidation("X-SUP-APPCID", connectionId);
                client.ShouldHandleXcsrfToken = true;
 
                // Send a request asynchronously continue when complete
                HttpResponseMessage response = await client.GetAsync(url);
 
                // Check that response was successful or throw exception
                response.EnsureSuccessStatusCode();

                // Read response asynchronously as JsonValue 
                var content = await response.Content.ReadAsStringAsync();

                // Parse the content into a JsonObject
                var jsonContent = JsonObject.Parse(content);

                var airlineId = (string)RelatedEntity.Properties["carrid"].Value;
                var flightNumber = (string)RelatedEntity.Properties["connid"].Value;
                var flightDate = (DateTime)RelatedEntity.Properties["fldate"].Value;
                var price = (decimal)RelatedEntity.Properties["PRICE"].Value;
                var currency = (string)RelatedEntity.Properties["CURRENCY"].Value;

                var airport = jsonContent.GetNamedValue("name").GetString();
                jsonContent = jsonContent.GetNamedObject("weather");
                var weather = jsonContent.GetNamedValue("weather").GetString();
                var temp = jsonContent.GetNamedValue("temp").GetString();
                var wind = jsonContent.GetNamedValue("wind").GetString();

                SharedContext.JsonContext = new JsonContext(airlineId, flightNumber, flightDate, price, currency, airport, weather, temp, wind);

                this.RingVisible = Visibility.Collapsed;
            }
            catch (Exception ex)
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

        public event EventHandler OperationFinished;

        public SAP.Data.OData.Online.Store.ODataStore Store { get; set; }

        private bool isStoreCreated;
        public bool IsStoreCreated
        {
            get
            {
                return this.isStoreCreated;
            }

            set
            {
                this.isStoreCreated = value;
                this.NotifyPropertyChanged();
            }
        }

        private SAP.Data.OData.Online.ODataEntitySet entitySet;
        public SAP.Data.OData.Online.ODataEntitySet EntitySet
        {
            get
            {
                return this.entitySet;
            }

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
        }

        private SAP.Data.OData.Online.ODataEntity entity;
        public SAP.Data.OData.Online.ODataEntity Entity
        {
            get
            {
                return this.entity;
            }

            set
            {
                this.entity = value;
                this.NotifyPropertyChanged();
            }
        }

        public string RelatedEntitySetCount
        {
            get
            {
                return this.relatedEntitySet == null ? "(0)" : "(" + this.relatedEntitySet.Count + ")";
            }
        }

        private SAP.Data.OData.Online.ODataEntitySet relatedEntitySet;
        public SAP.Data.OData.Online.ODataEntitySet RelatedEntitySet
        {
            get
            {
                return this.relatedEntitySet;
            }

            set
            {
                this.relatedEntitySet = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("RelatedEntitySetCount");
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

        private Visibility ringVisible = Visibility.Collapsed;
        public Visibility RingVisible
        {
            get
            {
                return this.ringVisible;
            }

            set
            {
                this.ringVisible = value;
                this.CanEditItems = value != Visibility.Visible;
                if (this.CanEditItems && (this.OperationFinished != null))
                {
                    this.OperationFinished(this, null);
                }

                this.NotifyPropertyChanged();
            }
        }

        private Visibility flightDetailsVisible = Visibility.Collapsed;
        public Visibility FlightDetailsVisible
        {
            get
            {
                return this.flightDetailsVisible;
            }

            set
            {
                this.flightDetailsVisible = value;
                this.NotifyPropertyChanged();
            }
        }

        private bool canEditItems;
        public bool CanEditItems
        {
            get
            {
                return this.canEditItems;
            }

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
