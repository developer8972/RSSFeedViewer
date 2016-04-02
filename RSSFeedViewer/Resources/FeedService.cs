using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace RSSFeedViewer
{
	public  static class FeedService
	{
		internal static List<Data> GetFeedItems(string url)
		{
			List<Data>  mItems = new List<Data>();
		try
			{
			WebRequest webRequest = WebRequest.Create(url);
			WebResponse webResponse = webRequest.GetResponse();

			Stream stream = webResponse.GetResponseStream();
			XmlDocument xmlDocument = new XmlDocument();

			xmlDocument.Load(stream);

			XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
			nsmgr.AddNamespace("dc", xmlDocument.DocumentElement.GetNamespaceOfPrefix("dc"));
			nsmgr.AddNamespace("content", xmlDocument.DocumentElement.GetNamespaceOfPrefix("content"));
		
			
				XmlNodeList itemNodes = xmlDocument.SelectNodes("rss/channel/item");
			
				for(int i = 0; i < itemNodes.Count; i++)
			{
				Data data = new  Data();

				if(itemNodes[i].SelectSingleNode("title") != null)
				{
					data.Title = itemNodes[i].SelectSingleNode("title").InnerText;
				}

					if(itemNodes[i].SelectSingleNode("link") != null)
					{
						data.Link = itemNodes[i].SelectSingleNode("link").InnerText;
					}

				if(itemNodes[i].SelectSingleNode("pubDate") != null)
				{
						data.PubDate = Convert.ToDateTime(itemNodes[i].SelectSingleNode("pubDate").InnerText);
				}			

					if(itemNodes[i].SelectSingleNode("category") !=null)
					{
						data.Category = itemNodes[i].SelectSingleNode("category").InnerText;
					}

					if(itemNodes[i].SelectSingleNode("description") != null)
					{
						data.Description = itemNodes[i].SelectSingleNode("description").InnerText;
					}
						
					if(itemNodes[i].SelectSingleNode("content:encoded", nsmgr) != null)
					{
							data.Content = itemNodes[i].SelectSingleNode("content:encoded", nsmgr).InnerText;
					}
						else 
					{
						data.Content = data.Description;
						}

	}
			}
				catch(Exception)
				{
					throw;
				}

			return mItems;
}
	}

}



