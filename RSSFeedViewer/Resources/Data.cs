using System;

namespace RSSFeedViewer
{
	public class Data
	{
		public String Title { get; set; }
		public String Link  { get; set; }
		public DateTime PubDate {get;set;}
		public String Category { get; set; }
		public String Description { get; set; }
		public String Content { get; set; }
	

		public Data ()
		{
		}
	}
}

