using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Org.Kuhn.Yapss {
    class FileImageSource : IImageSource {
        public FileImageSource(string rootPath) {
            this.rootPath = rootPath;
        }
        public Image GetImage(int minX, int minY) {
            if (files == null)
                try {
            			colFiles.AddRange(Directory.GetFiles(rootPath, "*.jpg", SearchOption.AllDirectories));
            			colFiles.AddRange(Directory.GetFiles(rootPath, "*.cbr", SearchOption.AllDirectories));
            			colFiles.AddRange(Directory.GetFiles(rootPath, "*.cbz", SearchOption.AllDirectories));
                    	files = colFiles.ToArray();
                }
                catch (Exception ex) {
                    throw new ImageSourceFailedException("Failed loading file list", ex);
                }
            if (files.Length == 0)
                throw new ImageSourceFailedException("No image files found");
            Image image = null;
            while (image == null)
                try {
            	nextfile = files[random.Next(files.Length)];
            	if (ComicImager.isComic(nextfile))
            	    {
            			image = ComicImager.GetCover(nextfile);
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
        private string[] files;
        private string nextfile;
        private Random random = new Random();
        private System.Collections.Generic.List<string> colFiles = new System.Collections.Generic.List<string>();
        private Yapss.imagesource.ComicImageSource ComicImager = new Org.Kuhn.Yapss.imagesource.ComicImageSource();
    }
}
