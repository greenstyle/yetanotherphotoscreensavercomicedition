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
		 }
		public Image GetCover(string filename)
		{
			Image oPic;
			try
			{

			SevenZipExtractor oExt = new SevenZipExtractor(filename);
			string sname = "";
			int xfile = 0;

			do
			{
				sname = oExt.ArchiveFileNames[(xfile)];
				if (xfile == oExt.ArchiveFileData.Count) break;
				xfile ++;
			}  while (IsImage(sname) == false);
			
			Stream iStream = new System.IO.MemoryStream();
			oExt.ExtractFile(sname, iStream);
			 oPic = System.Drawing.Bitmap.FromStream(iStream);
			}
			catch(Exception e)
			{
				//oPic = System.Drawing.Bitmap.FromFile("C:\\Users\\Chad\\Pictures\\IMG_0001.jpg");
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

		System.Collections.ArrayList imageExtensions;
	}

}
