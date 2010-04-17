///*
// * Created by SharpDevelop.
// * User: Chad
// * Date: 2/12/2010
// * Time: 11:50 AM
// * 
// * To change this template use Tools | Options | Coding | Edit Standard Headers.
// */
//using System;
//
//using System.Text;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Collections.Generic;
//using System.IO;
//using SevenZip;
//using System.Xml;
//namespace Org.Kuhn.Yapss.imagesource
//{
//	/// <summary>
//	/// Description of RSSFeedImageSource.
//	/// </summary>
//	public class RSSFeedImageSource
//	{
//		public RSSFeedImageSource(string feedsource){
//			FeedSource = feedsource;
//		}
//		public Image nextimage(){
//			try {
//			if (images.Count == 0)
//			{
//				LoadImages();
//			}
//			Random rnd = new Random();
//			int nextimage = rnd.Next(0,images.Count);
//			img = ImageFromURL(images[nextimage]);
//			images.RemoveAt(nextimage);
//			return img;
//				
//			} catch (Exception) {
//				return null;
//			}
//			
//		}
//		private void LoadImages()
//		{
//			XmlDocument rss = new XmlDocument;
//			rss.Load(FeedSource);
//		}
//		
//		private Image ImageFromURL(string url){
//			try {
//				WebRequest req = WebRequest.Create(url);
//				WebResponse response = req.GetResponse();
//				Stream stream = response.GetResponseStream();
//				return Image.FromStream(stream);
//			} catch (Exception) {
//				
//			}
//		
//		}
//		
//		//Image img;
//		string FeedSource = "";
//		List<string> images = new List<string>();
//		
//	}
//}
