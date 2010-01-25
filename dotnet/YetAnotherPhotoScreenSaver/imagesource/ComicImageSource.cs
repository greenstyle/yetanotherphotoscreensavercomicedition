using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SevenZip;
/*
 * Created by SharpDevelop.
 * User: Chad
 * Date: 1/14/2010
 * Time: 11:15 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace Org.Kuhn.Yapss.imagesource
{
	/// <summary>
	/// Description of ComicImageSource.
	/// </summary>
	public class ComicImageSource
	{
		public ComicImageSource()
		{
			imageExtensions = new System.Collections.ArrayList();
			imageExtensions.Add(".JPG");
			imageExtensions.Add(".BMP");
			imageExtensions.Add(".GIF");
			imageExtensions.Add(".PNG");
            comicExtensions = new System.Collections.ArrayList();
            comicExtensions.Add(".CBR");
            comicExtensions.Add(".CBZ");
		 }
		public Image GetCover(string filename)
		{
			Image oPic;
			try
			{
                //System.IO.File.AppendAllText("ComicInfo.log", DateTime.Now + " " + filename + Environment.NewLine);
                SevenZipExtractor oExt = new SevenZipExtractor(filename);
	    		string sname = "";
		    	int xfile = 0;
                System.Collections.SortedList filelist = new System.Collections.SortedList();
                foreach (string newname in oExt.ArchiveFileNames)
                {
                    if (IsImage(newname))
                    {
                        filelist.Add(newname, newname);
                    }
                }
                if (oExt.ArchiveFileNames.Contains("ComicInfo.xml"))
                    { 
                    MemoryStream xmlStream = new MemoryStream();
                    oExt.ExtractFile("ComicInfo.xml",xmlStream);
                    xmlStream.Position = 0;
                    xfile = firstimagefromxml(xmlStream);
                    if (xfile == -1)

                        {
                            sname =Convert.ToString( filelist.GetByIndex(0));
                        }

                }
                else
                    {
                       sname = Convert.ToString(filelist.GetByIndex(0));
                    }
            //sname = oExt.ArchiveFileNames[(xfile)];
			
			Stream iStream = new System.IO.MemoryStream();
			oExt.ExtractFile(sname, iStream);
			 oPic = System.Drawing.Bitmap.FromStream(iStream);
			}
			catch(Exception e)
			{
				oPic = null;
				System.IO.File.AppendAllText("ComicError.log", filename + Environment.NewLine + e.Message + Environment.NewLine);
			}
		return oPic;
			
		}
		private Boolean IsImage(string filename)
		{
			Boolean answer = false;
			string extension = Path.GetExtension(filename).ToUpper();
			answer = imageExtensions.Contains(extension);
			return answer;
		}
        public Boolean isComic(string filename)
        { 
           return comicExtensions.Contains(Path.GetExtension(filename));
        }
        public int firstimagefromxml(MemoryStream xmlStream)
        {
            int answer = -1;
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(xmlStream);
            System.Xml.XmlNode nderoot = doc.SelectSingleNode("Pages");
            answer = Convert.ToInt16(nderoot.FirstChild.Attributes.GetNamedItem("Image").Value);
            return answer;
        
        }
        public System.Collections.ArrayList ComicExtensions()
            {
                return comicExtensions;
            }
		System.Collections.ArrayList imageExtensions;
        System.Collections.ArrayList comicExtensions;
	}
    
}
