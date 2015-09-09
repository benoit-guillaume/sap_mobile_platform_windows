using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using BatchRequest.Contexts;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BatchRequest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BatchRequestPage : Page
    {
        private static string entity = "Suppliers";

        public BatchRequestPage()
        {
            this.InitializeComponent();

            this.DataContext = SharedContext.Context = new ODataContext();
        }

        private void create_Checked(object sender, RoutedEventArgs e)
        {
            SharedContext.Context.IsAddChangeSetEnabled = true;
        }

        private void update_Checked(object sender, RoutedEventArgs e)
        {
            SharedContext.Context.IsAddChangeSetEnabled = true;
        }

        private void delete_Checked(object sender, RoutedEventArgs e)
        {
            SharedContext.Context.IsAddChangeSetEnabled = true;
        }

        private void deepInsert_Checked(object sender, RoutedEventArgs e)
        {
            SharedContext.Context.IsAddChangeSetEnabled = true;
        }

        private async void addChangeSet_Click(object sender, RoutedEventArgs e)
        {
            int id = 0;
            int productId = 0;

            await SharedContext.Context.DownloadCollection("Suppliers");
            var entitySet = SharedContext.Context.EntitySet;
            id = (entitySet == null) ? 10 : (entitySet.Count == 0) ? 10 : entitySet.Max(entry => (int)entry.Properties["ID"].Value) + 1;
            await SharedContext.Context.ReadPropertyRaw("Products/$count");
            productId = int.Parse(SharedContext.Context.RawValue.Value.ToString());
            
            if (SharedContext.Context.IsDeleteEnabled)
            {   
                for (int i = 0; i < Int32.Parse((string)this.deleteCount.SelectedValue); i++)
                {
                    var entity = SharedContext.Context.Entity = new SAP.Data.OData.Online.ODataEntity("ODataDemo.Supplier");
                    SharedContext.Context.Store.AllocateProperties(entity, SAP.Data.OData.Store.PropertyCreationMode.All);

                    entity.Properties["ID"].Value = id;
                    entity.Properties["Name"].Value = "Unit Test Supplier";
                    entity.Properties["Address/Street"].Value = "Unit Test Street";
                    entity.Properties["Address/City"].Value = "Unit Test City";
                    entity.Properties["Address/State"].Value = "GA";
                    entity.Properties["Address/ZipCode"].Value = "55555";
                    entity.Properties["Address/Country"].Value = "USA";
                    entity.Properties["Concurrency"].Value = 1;

                    await SharedContext.Context.CreateEntity("Suppliers");

                    SharedContext.Context.AddDeleteToChangeSet(id);
                    id++;
                }          
            }

            if (SharedContext.Context.IsUpdateEnabled)
            {
                for (int i = 0; i < Int32.Parse((string)this.updateCount.SelectedValue); i++)
                {
                    var entity = SharedContext.Context.Entity = new SAP.Data.OData.Online.ODataEntity("ODataDemo.Supplier");
                    SharedContext.Context.Store.AllocateProperties(entity, SAP.Data.OData.Store.PropertyCreationMode.All);

                    entity.Properties["ID"].Value = id;
                    entity.Properties["Name"].Value = "Unit Test Supplier";
                    entity.Properties["Address/Street"].Value = "Unit Test Street";
                    entity.Properties["Address/City"].Value = "Unit Test City";
                    entity.Properties["Address/State"].Value = "GA";
                    entity.Properties["Address/ZipCode"].Value = "55555";
                    entity.Properties["Address/Country"].Value = "USA";
                    entity.Properties["Concurrency"].Value = 1;

                    await SharedContext.Context.CreateEntity("Suppliers");

                    await SharedContext.Context.ReadEntity("Suppliers(" + id + ")");
                    entity = SharedContext.Context.Entity;
                    var dynamicName = entity.Properties["Name"].Value.ToString() + new Random().Next(0, 10);
                    entity.Properties["Name"].Value = dynamicName;
                    SharedContext.Context.AddUpdateToChangeSet(id, entity);
                    id++;
                }
            }

            if (SharedContext.Context.IsCreateEnabled)
            {
                for (int i = 0; i < Int32.Parse((string)this.createCount.SelectedValue); i++)
                {
                    var entity = SharedContext.Context.Entity = new SAP.Data.OData.Online.ODataEntity("ODataDemo.Supplier");
                    SharedContext.Context.Store.AllocateProperties(entity, SAP.Data.OData.Store.PropertyCreationMode.All);

                    entity.Properties["ID"].Value = id;
                    entity.Properties["Name"].Value = "Unit Test Supplier";
                    entity.Properties["Address/Street"].Value = "Unit Test Street";
                    entity.Properties["Address/City"].Value = "Unit Test City";
                    entity.Properties["Address/State"].Value = "GA";
                    entity.Properties["Address/ZipCode"].Value = "55555";
                    entity.Properties["Address/Country"].Value = "USA";
                    entity.Properties["Concurrency"].Value = 1;

                    SharedContext.Context.AddCreateToChangeSet(entity);
                }
            }

            //if (SharedContext.Context.IsDeepInsertEnabled)
            //{
            //    for (int i = 0; i < Int32.Parse((string)this.deepInsertCount.SelectedValue); i++)
            //    {
            //        var parentEntity = SharedContext.Context.ParentEntity = new SAP.Data.OData.Online.ODataEntity("ODataDemo.Supplier");
            //        SharedContext.Context.Store.AllocateProperties(parentEntity, SAP.Data.OData.Store.PropertyCreationMode.All);

            //        parentEntity.Properties["ID"].Value = id;
            //        parentEntity.Properties["Name"].Value = "Deep Insert Supplier";
            //        parentEntity.Properties["Address/Street"].Value = "Deep Insert Street";
            //        parentEntity.Properties["Address/City"].Value = "Deep Insert City";
            //        parentEntity.Properties["Address/State"].Value = "GA";
            //        parentEntity.Properties["Address/ZipCode"].Value = "55555";
            //        parentEntity.Properties["Address/Country"].Value = "USA";
            //        parentEntity.Properties["Concurrency"].Value = 1;

            //        var childEntity = SharedContext.Context.ChildEntity = new SAP.Data.OData.Online.ODataEntity("ODataDemo.Product");
            //        SharedContext.Context.Store.AllocateProperties(childEntity, SAP.Data.OData.Store.PropertyCreationMode.All);

            //        childEntity.Properties["ID"].Value = productId;
            //        childEntity.Properties["Name"].Value = "Deep Insert Product";
            //        childEntity.Properties["Description"].Value = "Deep Insert Description";
            //        childEntity.Properties["ReleaseDate"].Value = DateTime.Now.AddYears(-10);
            //        childEntity.Properties["DiscontinuedDate"].Value = DateTime.Now.AddYears(-7);
            //        childEntity.Properties["Rating"].Value = 3;
            //        childEntity.Properties["Price"].Value = 20.35m;

            //        SharedContext.Context.AddDeepInsertToChangeSet(id.ToString(), parentEntity, childEntity);
            //    }
            //}


            this.create.IsChecked = false;
            this.update.IsChecked = false;
            this.delete.IsChecked = false;
            //this.deepInsert.IsChecked = false;
            this.createCount.SelectedIndex = 0;
            this.updateCount.SelectedIndex = 0;
            this.deleteCount.SelectedIndex = 0;
            //this.deepInsertCount.SelectedIndex = 0;

            SharedContext.Context.IsAddChangeSetEnabled = false;
            SharedContext.Context.IsExecuteBatchRequestEnabled = true;
        }

        private void create_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!SharedContext.Context.IsUpdateEnabled && !SharedContext.Context.IsDeleteEnabled && !SharedContext.Context.IsDeepInsertEnabled)
            {
                SharedContext.Context.IsAddChangeSetEnabled = false;
            }
        }

        private void update_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!SharedContext.Context.IsCreateEnabled && !SharedContext.Context.IsDeleteEnabled && !SharedContext.Context.IsDeepInsertEnabled)
            {
                SharedContext.Context.IsAddChangeSetEnabled = false;
            }
        }

        private void delete_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!SharedContext.Context.IsCreateEnabled && !SharedContext.Context.IsUpdateEnabled && !SharedContext.Context.IsDeepInsertEnabled)
            {
                SharedContext.Context.IsAddChangeSetEnabled = false;
            }
        }

        private void deepinsert_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!SharedContext.Context.IsCreateEnabled && !SharedContext.Context.IsUpdateEnabled && !SharedContext.Context.IsDeleteEnabled)
            {
                SharedContext.Context.IsAddChangeSetEnabled = false;
            }
        }

        private async void executeBatchRequest_Click(object sender, RoutedEventArgs e)
        {
            SharedContext.Context.CloseChangeSet();
            await SharedContext.Context.ExecuteBatch();
            SharedContext.Context.IsExecuteBatchRequestEnabled = false;
        }

        private void addQuery_Click(object sender, RoutedEventArgs e)
        {
           //  SharedContext.Context.AddReadToBatchRequest(entity, new Random().Next(0, 1));
            SharedContext.Context.AddFunctionToBatchRequest("GetProductsByRating?rating=5");
            SharedContext.Context.IsExecuteBatchRequestEnabled = true;
        }

        private void clearResults_Click(object sender, RoutedEventArgs e)
        {
            SharedContext.Context.BatchRequestItems.Clear();
        }
    }
}
