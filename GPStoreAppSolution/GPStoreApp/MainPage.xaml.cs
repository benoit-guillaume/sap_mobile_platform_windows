using Contexts;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GPStoreApp {
	public class TemplateSelector : DataTemplateSelector {
		public DataTemplate SimplePropertyTemplate { get; set; }
		public DataTemplate ComplexPropertyTemplate { get; set; }

		protected override DataTemplate SelectTemplateCore(object item, DependencyObject container) {
			var property = item as SAP.Data.OData.IODataProperty;
			if (property != null) {
				return (property.IsComplex) ? ComplexPropertyTemplate : SimplePropertyTemplate;
			}

			return base.SelectTemplateCore(item, container);
		}
	}

	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page {
		public MainPage() {
			this.InitializeComponent();

			this.DataContext = SharedContext.Context = new ODataContext();
		}

		private async void InitializeContext() {
			string message = null;
			try {
				SharedContext.Context.InitializeStore();
				SharedContext.Context.RingVisible = Visibility.Visible;
				var client = new SAP.Net.Http.HttpClient(
					new System.Net.Http.HttpClientHandler() {
						Credentials = new System.Net.NetworkCredential(string.IsNullOrEmpty(TB_User.Text) ? null : TB_User.Text, 
																	   string.IsNullOrEmpty(TB_Password.Password) ? null : TB_Password.Password)
					}, true);
				await SharedContext.Context.Store.OpenAsync(client);
				SharedContext.Context.IsStoreCreated = true;
				SharedContext.Context.EntitySetNames = SharedContext.Context.Store.Metadata.GetMetaEntityContainer(SharedContext.Context.Store.Metadata.MetaEntityContainerNames.First()).EntitySetNames;
			} catch (Exception ex) {
				message = ex.Message;
			}
			SharedContext.Context.RingVisible = Visibility.Collapsed;

			if (message != null)
				await (new Windows.UI.Popups.MessageDialog("We could not initialize the store. Details: " + message).ShowAsync());
		}

		private async void LB_Collections_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			string message = null;
			try {
				string collectionName = (string)((ListBox)sender).SelectedItem;
				if (collectionName == null) SharedContext.Context.EntitySet = null;
				else await SharedContext.Context.DownloadCollection(collectionName);
			} catch (Exception ex) {
				message = ex.Message;
			}

			SharedContext.Context.RingVisible = Visibility.Collapsed;

			if (message != null)
				await(new Windows.UI.Popups.MessageDialog("We could not initialize the store. Details: " + message).ShowAsync());
		}

		private void Button_ConnectClick(object sender, RoutedEventArgs e) {
			Popup_Settings.IsOpen = false;
			InitializeContext();
		}

		private void Page_SizeChanged(object sender, SizeChangedEventArgs e) {
			Grid_Settings.Width = this.ActualWidth;
			Grid_Settings.Height = this.ActualHeight;
		}


	}
}
