using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Org.Kuhn.Yapss {
    class FileImageSource : IImageSource {
        public FileImageSource(string rootPath, Comicstyle comicstyle) {
            this.rootPath = rootPath;
            this.comicstyle = comicstyle;
            ComicImager = new Org.Kuhn.Yapss.ComicImageSource(comicstyle);
        }
        public Image GetImage(int minX, int minY) {
            Log.Instance.Write("Starting comicImager");
            
            //Log.Instance.Write("Files = " + Convert.ToString(files.Count));
            if (files.Count <=1)
                try {
                    foreach (string curroothpath in rootPath.Split(';'))
                    {
                        System.Diagnostics.Debug.Print(curroothpath);
                        colFiles.AddRange(Directory.GetFiles(curroothpath, "*.jpg", SearchOption.AllDirectories));
                        foreach (string sExt in ComicImager.ComicExtensions()) 
                        {
                            colFiles.AddRange(Directory.GetFiles(curroothpath, "*" + sExt, SearchOption.AllDirectories));    
                        }
                    	files.AddRange(colFiles.ToArray());
                    Log.Instance.Write("Files Loaded = " + Convert.ToString( colFiles.Count));
                    System.Diagnostics.Debug.Print("Files Loaded = " + Convert.ToString(colFiles.Count));
                    }
                    Log.Instance.Write("Total Files Loaded = " + Convert.ToString(files.Count));
                    System.Diagnostics.Debug.Print("Files Loaded = " + Convert.ToString(files.Count));
                }
                catch (Exception ex) {
                    throw new ImageSourceFailedException("Failed loading file list", ex);
                }
            if (files.Count == 0)
                throw new ImageSourceFailedException("No image files found");
            Image image = null;
            while (image == null)
            {try
                {
                    if (files.Count == 0) { break; };
                    int nextfileid = random.Next(0 ,files.Count - 1);
                    Log.Instance.Write("nextfileid = " + Convert.ToString(nextfileid));
                    Log.Instance.Write("Total files = " + Convert.ToString(files.Count));
                    nextfile = files[nextfileid];
                    if (ComicImager.isQueued() == false) { files.RemoveAt(nextfileid); }
                    //files.RemoveAt(nextfileid);
                    if (ComicImager.isComic(nextfile) | ComicImager.isQueued())
                    {
                        image = ComicImager.GetImage(nextfile);
                        if (image==null){Log.Instance.Write("Image is null.  Failed to get image from ComicImager.");};
                    }
                    else
                    {
                        image = Image.FromFile(nextfile);
                    }
                    
                    if (image != null)
                    {
                        Rotate(image);
                        //if (image.Width < minX || image.Height < minY)
                        //    image = null;
                    }
                    if (image==null){Log.Instance.Write("Image is null.  Trying again.");};
                }
                catch (ThreadAbortException)
                {
                    //ignore}
                }
                catch (Exception ex)
                {
                    Log.Instance.Write("Failed loading disk image", ex);
                    image = null;
                }
            }
            return image;
        }
        private void Rotate(Image image) {
            foreach (PropertyItem pi in image.PropertyItems) {
                if (pi.Id == 274) { // that's the orientation EXIF ID
                    RotateFlipType t = RotateFlipType.RotateNoneFlipNone;
                    switch (pi.Value[0]) {
                        case 1:
                            t = RotateFlipType.RotateNoneFlipNone;
                            break;
                        case 2:
                            t = RotateFlipType.RotateNoneFlipX;
                            break;
                        case 3:
                            t = RotateFlipType.Rotate180FlipNone;
                            break;
                        case 4:
                            t = RotateFlipType.Rotate180FlipX;
                            break;
                        case 5:
                            t = RotateFlipType.Rotate90FlipX;
                            break;
                        case 6:
                            t = RotateFlipType.Rotate90FlipNone;
                            break;
                        case 7:
                            t = RotateFlipType.Rotate270FlipX;
                            break;
                        case 8:
                            t = RotateFlipType.Rotate270FlipNone;
                            break;
                    }
                    image.RotateFlip(t);
                    break;
                }
            }
        }
        private string rootPath;
        private Comicstyle comicstyle;
        private List<String> files =new List<string>();
        private string nextfile;
        private Random random = new Random();
        private System.Collections.Generic.List<string> colFiles = new System.Collections.Generic.List<string>();
        private Yapss.ComicImageSource ComicImager;
    }
}
