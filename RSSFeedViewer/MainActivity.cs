using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RSSFeedViewer
{
	[Activity (Label = "RSSFeedViewer", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		private List<Data> mItems;
		private ListView listView;
		private ProgressDialog progressDialog;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			listView = FindViewById<ListView> (Resource.Id.listView);
			progressDialog = new ProgressDialog (this);
			this.progressDialog.SetMessage (" In Progress .....");

				GetFeedItemsList ();
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
		protected void OnListItemClick(object sender, AdapterView.ItemClickEventArgs  e)
		{
			var t = mItems[e.Position];
			Android.Widget.Toast.MakeText(this,"I am working",Android.Widget.ToastLength.Short).Show();
				}
				}
	}
