using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;


namespace OfflineDemo.Contexts
{
    public class ODataContext : INotifyPropertyChanged
    {
        // methods
        public static void Initialize()
        {
            SAP.Data.OData.Offline.Store.ODataOfflineStore.GlobalInit();
        }
        public static void Finalize()
        {
            SAP.Data.OData.Offline.Store.ODataOfflineStore.GlobalFini();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // methods
        public ODataContext()
        {
            this.Store = new SAP.Data.OData.Offline.Store.ODataOfflineStore();
            this.IsStoreCreated = false;
        }

        public async System.Threading.Tasks.Task DownloadCollection(string collectionName)
        {
            try
            {
                this.RingVisible = Visibility.Visible;
                var execution = this.Store.ScheduleReadEntitySet(collectionName);
                var response = await execution.Response;

                this.EntitySet = (SAP.Data.OData.IODataEntitySet)((SAP.Data.OData.Store.IODataResponseSingle)response).Payload;

                this.RingVisible = Visibility.Collapsed;
            }
            catch (Exception)
            {
                this.RingVisible = Visibility.Collapsed;
                throw;
            }
        }

        public SAP.Data.OData.Offline.Store.ODataOfflineStore Store { get; set; }

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

        private SAP.Data.OData.IODataEntitySet entitySet;
        public SAP.Data.OData.IODataEntitySet EntitySet
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

        private SAP.Data.OData.IODataEntity entity;
        public SAP.Data.OData.IODataEntity Entity
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
                this.CanEditItems = (((Visibility)value) == Visibility.Visible) ? false : true;
                if (this.CanEditItems && (this.OperationFinished != null))
                {
                    this.OperationFinished(this, null);
                }

                this.NotifyPropertyChanged();
            }
        }

        public bool IsInEditMode { get; set; }

        private bool canEditItems = false;
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

        public event EventHandler OperationFinished;
    }

    public class SharedContext
    {
        public static ODataContext Context { get; set; }
    }
}
