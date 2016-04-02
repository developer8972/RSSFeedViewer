using System;
using Android.Widget;
using Android.App;
using Android.Content;
using System.Collections.Generic;
using Android.Views;
using Android.Media;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Android.Graphics;

namespace RSSFeedViewer
{
	public class MyCustomAdapter:BaseAdapter<Data>
	{

		List<Data> mItems;
		Context mContext;

		public  MyCustomAdapter(Context context,List<Data> items){
			mItems = items;
			mContext = context;
					}

		public override int Count {
			get {return mItems.Count;
			}
		}
		#region implemented abstract members of BaseAdapter
		public override long GetItemId (int position)
		{
			return position;
		}

		#endregion
		#region implemented abstract members of BaseAdapter
		public override Data this [int position] {
			get {
				return mItems [position];
			}
		}

		#endregion
		public override View GetView (int position, View convertView, ViewGroup parent)
		{			View row = convertView;
			if (row == null) {
				row = LayoutInflater.From (mContext).Inflate (Resource.Layout.List_item, null, false);
			}

			TextView title = row.FindViewById<TextView> (Resource.Id.title);
			TextView pubDate = row.FindViewById<TextView> (Resource.Id.pubDate);
			title.Text = mItems[position].Title;
			pubDate.Text = mItems [position].PubDate.ToString("dd/MM/yyyy HH:mm");

			return row;
		}
	}
	
}

