using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SevenZip;
using System.Net;
/*
 * Created by SharpDevelop.
 * User: Chad
 * Date: 1/14/2010
 * Time: 11:15 AM
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
            imageExtensions = new System.Collections.Generic.List<string>();
			imageExtensions.Add(".JPG");
			imageExtensions.Add(".BMP");
			imageExtensions.Add(".GIF");
			imageExtensions.Add(".PNG");
            comicExtensions = new System.Collections.Generic.List<string>();
            comicExtensions.Add(".CBR");
            comicExtensions.Add(".CBZ");
		 }
		public Image GetCover(string filename)
		{
            ComicInfo("Opening " + filename);
            currentFile = filename;
            Image oPic = null;
			try
			{
                try {
                    oExt = new SevenZipExtractor(filename);
                }
                catch(Exception ex)
                {
                    ComicError("Connot open " + filename);
                    ComicError(ex.Message);
                    return null;
                }
                
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
                sname = Convert.ToString(filelist.GetByIndex(0));
                if (oExt.ArchiveFileNames.Contains("ComicInfo.xml"))
                    {
                    MemoryStream xmlStream = new MemoryStream();
                    oExt.ExtractFile("ComicInfo.xml",xmlStream);
                    xmlStream.Position = 0;
                    xfile = coverfromxml(xmlStream);
                    if (xfile != -1)
                        {sname = Convert.ToString(filelist.GetByIndex(xfile));}
                    }
			Stream iStream = new System.IO.MemoryStream();
            try
            {
                oExt.ExtractFile(sname, iStream);
            }
            catch (Exception ex)
            {
                ComicError("Connot open " + filename);
                ComicError(ex.Message);
                return null;
            }
			oPic = System.Drawing.Bitmap.FromStream(iStream);
			}
			catch(Exception e)
			{
				oPic = null;
                Log.Instance.Write(filename + Environment.NewLine + e.Message + Environment.NewLine);
			}
		return oPic;
			
		}
		private Boolean IsImage(string filename)
		{
            try
            {
                return imageExtensions.Contains(Path.GetExtension(filename).ToUpper());
            }
            catch (Exception ex)
            {
                return false;
            }
            
		}
        public Boolean isComic(string filename)
        {
            try
            {
                return comicExtensions.Contains(Path.GetExtension(filename.ToUpper()));
            }
            catch
            { return false; }
        }
        public int coverfromxml(MemoryStream xmlStream)
        {

            int answer = 0;
            try
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                xmlStream.Position = 0;
                doc.Load(xmlStream);
                System.Xml.XmlNode nderoot = doc.SelectSingleNode("/ComicInfo/Pages");
                if (nderoot != null)
                {
                    if (nderoot.ChildNodes.Count > 1)
                    { answer = Convert.ToInt16(nderoot.FirstChild.Attributes.GetNamedItem("Image").Value); }
                    answer = MultipleCovers(answer, nderoot);
                }
                else
                {
                    answer = -1;
                }

            }
            catch (Exception ex)
                {
                    Log.Instance.Write("XML Error! ", ex);
                    answer = -1;
                }
            return answer;
        
        }
        private int MultipleCovers(int orig, System.Xml.XmlNode ndePages)
        {
            int answer = orig;

            System.Collections.Generic.List<System.Xml.XmlNode> alCovers = new System.Collections.Generic.List<System.Xml.XmlNode>();
            foreach (System.Xml.XmlNode ndePage in ndePages.ChildNodes)
            {
                try
                {
                    try
                    {
                        if (ndePage.Attributes.GetNamedItem("Type").Value == "FrontCover")
                        {
                            alCovers.Add(ndePage);
                        }
                    }
                    catch (Exception exe)
                    {
                    }
                    if (alCovers.Count > 1)
                    {
                        Random rndPage = new Random();
                        System.Xml.XmlNode ansCover = alCovers[rndPage.Next(alCovers.Count)];
                        answer = Convert.ToUInt16(ansCover.Attributes.GetNamedItem("Image").Value);
                    }
                }
                catch (Exception ex)
                    {
                        ComicError(ex.Message);
                        answer = orig;    
                    }   
            }
            return answer;

        }
        private void ComicInfo(string message)
        {
            Log.Instance.Write( message);
        }
        private void ComicError(string message)
        {
            Log.Instance.Write(currentFile);
            Log.Instance.Write( message);
        }
        public System.Collections.Generic.List<string> ComicExtensions()
            {return comicExtensions;}
        SevenZipExtractor oExt;
		System.Collections.Generic.List<string> imageExtensions;
        System.Collections.Generic.List<string> comicExtensions;
        string currentFile;
	}
    
}
