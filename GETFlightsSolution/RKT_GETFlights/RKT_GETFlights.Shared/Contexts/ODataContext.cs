using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using SAP.Data.OData;
using SAP.Data.OData.Store;

using Windows.UI.Xaml;

namespace RKT_GETFlights.Contexts
{
    using SAP.Data.OData.Online.Store;

    public class ODataContext : INotifyPropertyChanged
    {
        // methods
        public ODataContext(string serviceUrl)
        {
            this.Store = new ODataStore(serviceUrl, ODataStore.EntityFormat.JSON);
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

        public async System.Threading.Tasks.Task DownloadRelatedItems(string navigationPropertyName)
        {
            if (this.Entity == null)
            {
                this.RingVisible = Visibility.Collapsed;
                throw new Exception("There is no entity in the context.");
            }

            try
            {
                this.RingVisible = Visibility.Visible;
                
                IODataNavigationProperty navigationProperty = this.entity.GetNavigationProperty(navigationPropertyName);
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
    }
}
