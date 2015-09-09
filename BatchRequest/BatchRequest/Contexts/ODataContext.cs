using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using SAP.Data.OData.Online;
using SAP.Data.OData.Online.Store;
using SAP.Data.OData.Store;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace BatchRequest.Contexts
{
    public class ODataContext : INotifyPropertyChanged
    {
        public ODataContext()
        {
            var url = "http://services.odata.org/V3/(S(n3ko05udokwu3er2ctjitk4h))/OData/OData.svc/";
            this.Store = new ODataStore(url);
            Store.OpenAsync();

            this.ChangeSet = new ODataRequestChangeSet();
            this.BatchRequest = new ODataRequestParametersBatch();
            this.EntityList = new List<ODataEntity>();
            this.ResponseStatusCodes = new List<string>();
            this.BatchRequestItems = new ObservableCollection<string>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public async Task DownloadCollection(string collectionName)
        {
            var execution = Store.ScheduleReadEntitySet(collectionName);
            IODataResponse response = await execution.Response;
            this.EntitySet = (SAP.Data.OData.Online.ODataEntitySet)((IODataResponseSingle)response).Payload;
        }

        public async Task ReadEntity(string resourcePath)
        {
            try
            {
                var execution = Store.ScheduleReadEntity(resourcePath);
                IODataResponse response = await execution.Response;

                this.Entity = (SAP.Data.OData.Online.ODataEntity)((IODataResponseSingle)response).Payload;
            }
            catch
            {
                this.Entity = null;
            }
        }

        public async Task ReadEntity(SAP.Data.OData.IODataEntity entity)
        {
            var execution = Store.ScheduleReadEntity(entity);
            IODataResponse response = await execution.Response;
            if (response is IODataResponseSingle)
            {
                if (((IODataResponseSingle)response).PayloadType == SAP.Data.OData.ODataType.Entity)
                {
                    this.Entity = (SAP.Data.OData.Online.ODataEntity)((IODataResponseSingle)response).Payload;
                }
            }
        }

        public async Task CreateEntity(string collectionName)
        {
            var execution = Store.ScheduleCreateEntity(Entity, collectionName);
            await execution.Response;
        }

        public async Task UpdateEntity(SAP.Data.OData.IODataEntity entity)
        {
            var execution = Store.ScheduleUpdateEntity(entity);
            await execution.Response;
        }

        public async Task PatchEntity(SAP.Data.OData.IODataEntity entity)
        {
            var execution = Store.SchedulePatchEntity(entity);
            await execution.Response;
        }

        public async Task DeleteEntity(SAP.Data.OData.IODataEntity entity)
        {
            var execution = Store.ScheduleDeleteEntity(entity);
            await execution.Response;
        }

        public async Task ReadPropertyRaw(string resourcePath)
        {
            var execution = Store.ScheduleReadPropertyRaw(resourcePath);
            IODataResponse response = await execution.Response;
            if (response is IODataResponseSingle)
            {
                if (((IODataResponseSingle)response).PayloadType == SAP.Data.OData.ODataType.Raw)
                {
                    this.RawValue = (SAP.Data.OData.ODataRawValue)((IODataResponseSingle)response).Payload;
                }
            }
        }

        public void AddReadToBatchRequest(string collectionName, int readId)
        {
            // TODO: Change 1 - Add READ operation to batch request
           // var item = new ODataRequestParametersSingle(collectionName + "(" + readId + ")", RequestMode.Read);
            var item = new ODataRequestParametersSingle(collectionName, RequestMode.Read);
            this.BatchRequest.Add(item);

            BatchRequestItems.Add("Single Request Mode: " + ((ODataRequestParametersSingle)item).Mode.ToString() + ", Path: " + ((ODataRequestParametersSingle)item).ResourcePath);
        }

        public void AddFunctionToBatchRequest(string resourcePath)
        {
            var item = new ODataRequestParametersSingle(resourcePath, RequestMode.Read);
            this.BatchRequest.Add(item);

            BatchRequestItems.Add("Single Request Mode: " + ((ODataRequestParametersSingle)item).Mode.ToString() + ", Path: " + ((ODataRequestParametersSingle)item).ResourcePath);
        }

        public void AddCreateToChangeSet(SAP.Data.OData.Online.ODataEntity entity)
        {
            // TODO: Change 2 - Add CREATE operation to ChangeSet
            var item = new ODataRequestParametersSingle("Suppliers", RequestMode.Create, entity);
            this.ChangeSet.Add(item);

            BatchRequestItems.Add("SingleRequest Mode: " + item.Mode.ToString() + ", Path: " + item.ResourcePath);
        }

        public void AddUpdateToChangeSet(int id, SAP.Data.OData.Online.ODataEntity entity)
        {
            // TODO: Change 3 - Add UPDATE operation to ChangeSet
            var item = new ODataRequestParametersSingle("Suppliers(" + id + ")", RequestMode.Update, entity);
            this.ChangeSet.Add(item);

            BatchRequestItems.Add("SingleRequest Mode: " + item.Mode.ToString() + ", Path: " + item.ResourcePath);
        }

        public void AddDeleteToChangeSet(int id)
        {
            // TODO: Change 4 - Add DELETE operation to ChangeSet
            var item = new ODataRequestParametersSingle("Suppliers(" + id + ")", RequestMode.Delete);
            this.ChangeSet.Add(item);

            BatchRequestItems.Add("SingleRequest Mode: " + item.Mode.ToString() + ", Path: " + item.ResourcePath);
        }

        public void AddDeepInsertToChangeSet(string contentId, SAP.Data.OData.Online.ODataEntity parentEntity, SAP.Data.OData.Online.ODataEntity childEntity)
        {
            var parentItem = new ODataRequestParametersSingle("Suppliers", RequestMode.Create, parentEntity);
            parentItem.ContentId = contentId;
            this.ChangeSet.Add(parentItem);
            BatchRequestItems.Add("Deep Insert Parent: " + parentItem.Mode.ToString() + ", Path: " + parentItem.ResourcePath);

            var childItem = new ODataRequestParametersSingle("$" + contentId, RequestMode.Create, childEntity);
            this.ChangeSet.Add(childItem);
            BatchRequestItems.Add("Deep Insert Child: " + childItem.Mode.ToString() + ", Path: " + childItem.ResourcePath);
        }

        public async Task ExecuteBatch()
        {
            // TODO: Change 6 - Executing batch request
            this.ResponseList = ((IReadOnlyCollection<IODataResponseBatchItem>)((IODataResponseBatch)((await this.Store.ScheduleRequest(this.BatchRequest).Response))).Responses);
            ParseResponse();
            this.BatchRequest = new ODataRequestParametersBatch();
            this.BatchRequestItemsCount = 0;
        }

        public void ClearBatchRequest()
        {
            this.BatchRequest.Clear();
            this.BatchRequestItemsCount = 0;
        }

        public void CloseChangeSet()
        {
            // TODO: Change 5 - Closing ChangeSet
            if (ChangeSet.Count > 0)
            {
                this.BatchRequest.Add(ChangeSet);
            }

            this.ChangeSet = new ODataRequestChangeSet();
        }

        public void ParseResponse()
        {
            foreach (var item in BatchRequest.ToList())
            {
                if (item is IODataRequestChangeSet)
                {
                    Debug.WriteLine("ODataRequestChangeSet");

                    foreach (var element in ((ODataRequestChangeSet)item).ToList())
                    {
                        Debug.WriteLine("SingleRequest Mode: " + element.Mode.ToString() + ", Path: " + element.ResourcePath);
                    }
                }
                else if (item is IODataRequestParametersSingle)
                {
                    Debug.WriteLine("Single Request Mode: " + ((ODataRequestParametersSingle)item).Mode.ToString() + ", Path: " + ((ODataRequestParametersSingle)item).ResourcePath);
                }
            }

            foreach (var item in ResponseList)
            {
                // TODO: Change 7 - Parsing ChangeSets response
                if (item is IODataResponseChangeSet)
                {
                    Debug.WriteLine("ODataRequestChangeSet");
                    foreach (var element in ((ODataResponseChangeSet)item).Responses)
                    {
                        Debug.WriteLine("Status Code: " + element.Headers["http.code"].ElementAt(0));
                        BatchRequestItems.Add("ODataResponseChangeSet Status Code: " + element.Headers["http.code"].ElementAt(0));
                        this.ResponseStatusCodes.Add(element.Headers["http.code"].ElementAt(0));
                    }
                }
                // TODO: Change 8 - Parsing Query response
                else if (item is IODataResponseSingle)
                {
                    var response = (ODataResponseSingle)item;
                    // this.EntityList.Add((SAP.Data.OData.Online.ODataEntity)(response).Payload);
                    Debug.WriteLine("ODataResponseSingle Status Code: " + response.Headers["http.code"].ElementAt(0));
                    BatchRequestItems.Add("ODataResponseSingle Status Code: " + response.Headers["http.code"].ElementAt(0));
                    this.ResponseStatusCodes.Add(response.Headers["http.code"].ElementAt(0));
                }
            }
        }

        private bool isCreateEnabled;
        public bool IsCreateEnabled
        {
            get
            {
                return this.isCreateEnabled;
            }

            set
            {
                this.isCreateEnabled = value;
                this.NotifyPropertyChanged();
            }
        }

        private bool isUpdateEnabled;
        public bool IsUpdateEnabled
        {
            get
            {
                return this.isUpdateEnabled;
            }

            set
            {
                this.isUpdateEnabled = value;
                this.NotifyPropertyChanged();
            }
        }

        private bool isDeleteEnabled;
        public bool IsDeleteEnabled
        {
            get
            {
                return this.isDeleteEnabled;
            }

            set
            {
                this.isDeleteEnabled = value;
                this.NotifyPropertyChanged();
            }
        }

        private bool isDeepInsertEnabled;
        public bool IsDeepInsertEnabled
        {
            get
            {
                return this.isDeepInsertEnabled;
            }

            set
            {
                this.isDeepInsertEnabled = value;
                this.NotifyPropertyChanged();
            }
        }

        private bool isAddChangeSetEnabled;
        public bool IsAddChangeSetEnabled
        {
            get
            {
                return this.isAddChangeSetEnabled;
            }

            set
            {
                this.isAddChangeSetEnabled = value;
                this.NotifyPropertyChanged();
            }
        }

        private bool isExecuteBatchRequestEnabled;
        public bool IsExecuteBatchRequestEnabled
        {
            get
            {
                return this.isExecuteBatchRequestEnabled;
            }

            set
            {
                this.isExecuteBatchRequestEnabled = value;
                this.NotifyPropertyChanged();
            }
        }

        public ODataStore Store { get; set; }

        public ODataEntity Entity { get; set; }

        public ODataEntity ParentEntity { get; set; }

        public ODataEntity ChildEntity { get; set; }

        public List<ODataEntity> ChildEntities { get; set; }

        public List<ODataEntity> EntityList { get; set; }

        public ODataEntitySet EntitySet { get; set; }

        public SAP.Data.OData.ODataRawValue RawValue { get; set; }

        public ODataRequestChangeSet ChangeSet { get; set; }

        public IReadOnlyCollection<IODataResponseBatchItem> ResponseList { get; set; }

        public ODataRequestParametersBatch BatchRequest { get; set; }

        public List<string> ResponseStatusCodes { get; set; }

        public ObservableCollection<string> BatchRequestItems { get; set; }

        public int BatchRequestItemsCount;

        public int CreateCount { get; set; }

        public int UpdateCount { get; set; }

        public int DeleteCount { get; set; }
    }

    public class SharedContext
    {
        public static ODataContext Context { get; set; }
    }
}
