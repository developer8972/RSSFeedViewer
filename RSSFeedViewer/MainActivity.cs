using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using RestSharp;
using Android.Webkit;
using Android.Net;
using Android.Net.Wifi;
using KinveyXamarin;
using SQLite.Net.Platform.XamarinAndroid;
using System.Xml.Serialization;

using System.Threading.Tasks;
using System.Collections.Generic;

namespace RSSFeedViewer
{
	[Activity (Label = "RSSFeedViewer", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		WifiManager wifi;
		WebView web_view;
		private List<Data> mItems;

		private  ListView listView;
		private ProgressDialog progressDialog;

		private string appKey = "kid_WkwnyQlse-";
		private string appSecret = "e9e2436ba7ca44d386cc5b69aaccfb34";

		private static string COLLECTION = "collect";
		private static string STABLE_ID = "rssfeedviewer";



		Client kinveyClient;
		InMemoryCache<Data> myCache = new InMemoryCache<Data>();

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			listView = FindViewById<ListView> (Resource.Id.listView);

			ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService (ConnectivityService);
			NetworkInfo wifiInfo = connectivityManager.GetNetworkInfo (ConnectivityType.Wifi);
			wifi = (Android.Net.Wifi.WifiManager)GetSystemService(Context.WifiService);

			//ProgressDialog before the Data Loads
			progressDialog = new ProgressDialog (this);
			this.progressDialog.SetMessage (" In Progress .....");

			//Kinvey Library inorder to cache the data 
			kinveyClient = new Client.Builder(appKey, appSecret)
				.setFilePath(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal))
				.setOfflinePlatform(new SQLitePlatformAndroid())
				.setLogger(delegate(string msg) { Console.WriteLine(msg);})
				.build();

				
			// Check the wifi connectivity
			if (wifiInfo.IsConnected) 
			{
				Toast.MakeText (this, "saved: " ,ToastLength.Short).Show ();
				
				GetFeedItemsList ();
				saveAndToast ();
			} else {
				loadFromCacheAndToast ();
				
			}
		
		}



		private async void saveAndToast(){

			AsyncAppData<Data> entityCollection = kinveyClient.AppData<Data>(COLLECTION, typeof(Data));

			Data ent = new Data();

			entityCollection.setOffline(new SQLiteOfflineStore(), OfflinePolicy.LOCAL_FIRST);
			try{
				Data entity = await entityCollection.SaveAsync (ent);
				Toast.MakeText (this, "saved: " + entity.PubDate +entity.Title, ToastLength.Short).Show ();
			}catch(Exception e){
				Toast.MakeText (this, "something went wrong; " + e.Message, ToastLength.Short).Show ();
			}


		}

		private async void loadAndToast(){
			AsyncAppData<Data> entityCollection = kinveyClient.AppData<Data>(COLLECTION, typeof(Data));
			//entityCollection.setOffline(new SQLiteOfflineStore(), OfflinePolicy.LOCAL_FIRST);



			Data res = await entityCollection.GetEntityAsync (STABLE_ID);
			Toast.MakeText(this, "got: " + res.PubDate +res.Title, ToastLength.Short).Show();
		}

		private async void loadFromCacheAndToast(){
			AsyncAppData<Data> entityCollection = kinveyClient.AppData<Data>(COLLECTION, typeof(Data));
			entityCollection.setCache (myCache, CachePolicy.NETWORK_FIRST);

			Data entity = await entityCollection.GetEntityAsync (STABLE_ID);
			Toast.MakeText (this, "got: " + entity.PubDate +entity.Title, ToastLength.Short).Show ();
					}

			private void GetFeedItemsList(){
			this.progressDialog.Show ();
			Task <List<Data>> task1 =  Task.Factory.StartNew(() =>{return FeedService.GetFeedItems("http://feeds.feedburner.com/androidcentral?format=xml");}
			);

			Task task2 = task1.ContinueWith((antecedent) =>
			{
				try
				{
					this.progressDialog.Dismiss();
					this.mItems = antecedent.Result;
					this.PopulateListView(this.mItems);
				}
				catch(AggregateException aex)
				{
					Toast.MakeText (this,aex.InnerException.Message,ToastLength.Short).Show ();
				}
			}, TaskScheduler.FromCurrentSynchronizationContext()
			);
		}

		private void PopulateListView(List<Data> feedItemsList)
		{
			var adapter = new MyCustomAdapter(this,feedItemsList);
			this.listView.Adapter = adapter;
			this.listView.ItemClick += OnListItemClick; 

		}

		//Onclick of the item takes to the webpage 
		protected void OnListItemClick(object sender, AdapterView.ItemClickEventArgs  e)
		{
			var t = mItems[e.Position];

//     		var uri = Android.Net.Uri.Parse (t.Link);
//			var intent = new Intent (Intent.ActionView, uri);
//			StartActivity (intent);
			web_view = FindViewById<WebView> (Resource.Id.webview);
			web_view.Settings.JavaScriptEnabled = true;
			web_view.LoadUrl (t.Link);
							}
				}
	}
