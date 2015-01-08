using SAP.Data.OData;
using SAP.Data.OData.Store;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Windows.UI.Xaml;

namespace Contexts {
	public class ODataContext : INotifyPropertyChanged {
		// methods
		public ODataContext() {
			this.ServiceUrl = "http://services.odata.org/V3/OData/OData.svc/";
			//this.Store = new SAP.Data.OData.Online.Store.ODataStore(serviceUrl);
			this.IsStoreCreated = false;
		}
		public void InitializeStore() {
			this.Store = new SAP.Data.OData.Online.Store.ODataStore(this.ServiceUrl);
		}

		private void NotifyPropertyChanged(string propertyName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		public async System.Threading.Tasks.Task DownloadCollection(string CollectionName) {
			try {
				this.RingVisible = Visibility.Visible;
				var execution = Store.ScheduleReadEntitySet(CollectionName);
				var response = await execution.Response;
				this.EntitySet = (SAP.Data.OData.Online.ODataEntitySet)((IODataResponseSingle)response).Payload;
				this.RingVisible = Visibility.Collapsed;
			} catch (Exception) {
				this.RingVisible = Visibility.Collapsed;
				throw;
			}
		}

		public async System.Threading.Tasks.Task DownloadRelatedItems(string navigationPropertyName) {
			if (this.Entity == null) {
				this.RingVisible = Visibility.Collapsed;
				throw new Exception("There is no entity in the context.");
			}

			IODataNavigationProperty navigationProperty = this.entity.GetNavigationProperty(navigationPropertyName);

			try {
				this.RingVisible = Visibility.Visible;
				var execution = Store.ScheduleRequest(new SAP.Data.OData.Store.ODataRequestParametersSingle(navigationProperty.AssociationResourcePath));
				var response = await execution.Response;
				if (response is IODataResponseSingle) {
					if (((IODataResponseSingle)response).PayloadType == SAP.Data.OData.ODataType.EntitySet) {
						this.RelatedEntitySet = (SAP.Data.OData.Online.ODataEntitySet)((IODataResponseSingle)response).Payload;
					}
				}
				//if (response.PayloadType == SAP.Data.OData.ODataPayloadType.EntitySet) {
				//	this.EntitySet = (SAP.Data.OData.Online.ODataEntitySet)response;
				//}
				this.RingVisible = Visibility.Collapsed;
			} catch (Exception) {
				this.RingVisible = Visibility.Collapsed;
				throw;
			}
		}

		// events
		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler OperationFinished;

		// properties
		public SAP.Data.OData.Online.Store.ODataStore Store { get; set; }

		private string serviceUrl;

		public string ServiceUrl {
			get { return serviceUrl; }
			set {
				serviceUrl = value;
				NotifyPropertyChanged("ServiceUrl");
			}
		}
		

		private bool isStoreCreated = false;
		public bool IsStoreCreated {
			get { return isStoreCreated; }
			set {
				isStoreCreated = value;
				NotifyPropertyChanged("IsStoreCreated");
			}
		}

		protected SAP.Data.OData.Online.ODataEntitySet entitySet;
		public SAP.Data.OData.Online.ODataEntitySet EntitySet {
			get { return entitySet; }
			set {
				entitySet = value;
				NotifyPropertyChanged("EntitySet");
				NotifyPropertyChanged("EntitySetCount");
			}
		}


		IEnumerable<string> entitySetNames = null;
		public IEnumerable<string> EntitySetNames {
			get {
				return entitySetNames;
			}

			set {
				entitySetNames = value;
				NotifyPropertyChanged("EntitySetNames");
			}
		}

		public string EntitySetCount {
			get {
				return this.entitySet == null ? "(0)" : "(" + this.entitySet.Count + ")";
			}
		}

		protected SAP.Data.OData.IODataEntity entity;
		public SAP.Data.OData.IODataEntity Entity {
			get { return this.entity; }
			set {
				this.entity = value;
				NotifyPropertyChanged("Entity");
			}
		}

		protected SAP.Data.OData.Online.ODataEntitySet relatedEntitySet;
		public SAP.Data.OData.Online.ODataEntitySet RelatedEntitySet {
			get { return relatedEntitySet; }
			set {
				relatedEntitySet = value;
				NotifyPropertyChanged("RelatedEntitySet");
			}
		}

		private Visibility ringVisible = Visibility.Collapsed;
		public Visibility RingVisible {
			get { return this.ringVisible; }
			set {
				this.ringVisible = value;
				this.CanEditItems = (((Visibility)value) == Visibility.Visible) ? false : true;
				if ((this.CanEditItems) && (OperationFinished != null)) OperationFinished(this, null);
				NotifyPropertyChanged("RingVisible");
			}
		}

		private bool canEditItems = false;
		public bool CanEditItems {
			get { return canEditItems; }
			set {
				canEditItems = value;
				NotifyPropertyChanged("CanEditItems");
			}
		}


		public bool IsInEditMode { get; set; }
	}

	public class SharedContext {
		public static ODataContext Context { get; set; }
	}
}
