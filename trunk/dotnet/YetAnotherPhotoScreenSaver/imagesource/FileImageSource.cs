using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Org.Kuhn.Yapss {
    class FileImageSource : IImageSource {
        public FileImageSource(string rootPath, Comicstyle comicstyle) {
            this.rootPath = rootPath;
            ComicImager = new Org.Kuhn.Yapss.imagesource.ComicImageSource(comicstyle);
        }
        public Image GetImage(int minX, int minY) {
            if (files.Count == 0)
                try {
                    
                    colFiles.AddRange(Directory.GetFiles(rootPath, "*.jpg", SearchOption.AllDirectories));
                        foreach (string sExt in ComicImager.ComicExtensions()) 
                        {
                            colFiles.AddRange(Directory.GetFiles(rootPath, "*"+ sExt, SearchOption.AllDirectories));    
                        }
                    	files.AddRange(colFiles.ToArray());
                    Log.Instance.Write("Files Loaded = " + Convert.ToString( colFiles.Count));
                }
                catch (Exception ex) {
                    throw new ImageSourceFailedException("Failed loading file list", ex);
                }
            if (files.Count == 0)
                throw new ImageSourceFailedException("No image files found");
            Image image = null;
            while (image == null)
                try {
                    int nextfileid = random.Next(files.Count);
            	nextfile = files[nextfileid];
            	if (ComicImager.isQueued() == false){files.RemoveAt(nextfileid);}
                //files.RemoveAt(nextfileid);
            	if (ComicImager.isComic(nextfile)|ComicImager.isQueued())
            	    {
            			image = ComicImager.GetImage(nextfile);
            			
            			
            	    }
            	else
            		{
            			image = Image.FromFile(nextfile);
            		}
                    
                    Rotate(image);
                    if (image.Width < minX || image.Height < minY)
                        image = null;
                }
                catch (Exception ex) {
                    Log.Instance.Write("Failed loading disk image", ex);
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

        private List<String> files =new List<string>();
        private string nextfile;
        private Random random = new Random();
        private System.Collections.Generic.List<string> colFiles = new System.Collections.Generic.List<string>();
        private Yapss.imagesource.ComicImageSource ComicImager;
    }
}
