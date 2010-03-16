/*
 * Created by SharpDevelop.
 * User: Chad
 * Date: 3/8/2010
 * Time: 2:12 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Media;

using System.IO;
using System.Windows.Forms;

namespace Org.Kuhn.Yapss.transitions
{
	/// <summary>
	/// Description of transition.
	/// </summary>
	public class transition
	{
		public transition(BufferedGraphics bufferedGraphics)
		{
			bufferedgraphics = bufferedGraphics;		
		}
		public void set(Image setimage, Rectangle setdestRect, Rectangle setsourceRect, GraphicsUnit setgraphicsunit){
			image = setimage;
			destRect = setdestRect;
			sourceRect = setsourceRect;
			graphicsunit = setgraphicsunit;
		}
        public void transitionin(Form frm, Rectangle targetArea, TransitionStyle transitionstyle, BackGroundStyle backgroundstyle)
        {
            
            {
                //Image image = setimage;
                
                //targetarea = targetArea;
                Brush backbrush = backgroundstyle == BackGroundStyle.Black ? Brushes.Black : Brushes.White;

                switch (transitionstyle)
                {
                    case TransitionStyle.Fade:
                        Bitmap bmpImage = new Bitmap(image);

                        //Brush b = new TextureBrush(image,WrapMode.Clamp,destRect);

                        //using Graphics g = frm.CreateGraphics();


                        Bitmap backImage = new Bitmap(image.Width, image.Height);

                        //int t = 50;
                        //frm.DrawToBitmap(backImage, targetArea);
                        for (int t = 0; t <= 100; t += 10)
                        {
                            //	ImageAttributes attr = new ImageAttributes();
                            //attr.SetColorKey(Color.White, Color.White);

                            //int opacity = 100-t;
                            //MessageBox.Show("test");
                            Bitmap newImage = Fade(bmpImage, backImage, t);
                            //lock(bufferedgraphics)
                            //{

                            lock (bufferedgraphics)
                            {
                                if (t == 0)
                                {
                                    bufferedgraphics.Graphics.FillRectangle(backbrush, targetArea);
                                }
                                bufferedgraphics.Graphics.DrawImage(
                                    newImage,
                                    destRect,
                                    sourceRect,
                                    graphicsunit
                                     );
                                //SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(90, 255, 255, 50));
                                //Brush title = Brushes.White;
                                //bufferedgraphics.Graphics.DrawString("Test asdfasdfsadfsadf", new Font("Arial", 14), semiTransBrush, targetArea);
                                newImage.Dispose();
                            }
                            frm.Invalidate(destRect);

                        }
                        
                        //bufferedgraphics.Graphics.Dispose();
                        //frm.Invalidate(targetarea);
                        //Application.DoEvents();
                        //System.Threading.Thread.Sleep(10);
                        //	}
                        break;
                    default:
                        lock (bufferedgraphics)
                        {
                            bufferedgraphics.Graphics.FillRectangle(backbrush, targetArea);
                            bufferedgraphics.Graphics.DrawImage(
                                        image,
                                        destRect,
                                        sourceRect,
                                        graphicsunit
                                         );
                            frm.Invalidate(targetArea);
                        }
                        break;

                }
            }
		}
		
		public void transitionout(Form frm, Rectangle targetArea, TransitionStyle transitionstyle){
			switch (transitionstyle) {
				case TransitionStyle.Fade:
					//bufferedGraphics.Graphics.FillRectangle(theme == Theme.Dark ? Brushes.Black : Brushes.White, Bounds);
					break;
				case TransitionStyle.None:
					break;
				default:
					break;
			}
			//frm.Invalidate(targetArea);
		
		}
		
        public static Bitmap Fade(Bitmap image1, Bitmap image2, int opacity)
        {
            Bitmap newBmp = new Bitmap(Math.Min(image1.Width,image2.Width),Math.Min(image1.Height,image2.Height));
            for (int i = 0; i < image1.Width && i < image2.Width; i++)
            {
                for (int j = 0; j < image1.Height && j < image2.Height; j++)
                {
                    Color image1Pixel = image1.GetPixel(i, j), image2Pixel = image2.GetPixel(i, j);
                    Color newColor = Color.FromArgb((image1Pixel.R*opacity + image2Pixel.R*(100 - opacity))/100,
                                                    (image1Pixel.G*opacity + image2Pixel.G*(100 - opacity))/100,
                                                    (image1Pixel.B*opacity + image2Pixel.B*(100 - opacity))/100);
                    newBmp.SetPixel(i, j, newColor);
                }
            }
            return newBmp;
        }
		
		public static Bitmap Copy(Bitmap srcBitmap, Rectangle section)
{
    // Create the new bitmap and associated graphics object
    Bitmap bmp = new Bitmap(section.Width, section.Height);
    Graphics g = Graphics.FromImage(bmp);

    // Draw the specified section of the source bitmap to the new one
    g.DrawImage(srcBitmap, 0, 0, section, GraphicsUnit.Pixel);

    // Clean up
    g.Dispose();

    // Return the bitmap
    return bmp;
}
        
		
        
        private BufferedGraphics bufferedgraphics;
		private Graphics graphics;
		private Image image;
		private Rectangle destRect;
		private Rectangle sourceRect;
		private Rectangle targetarea;
		private GraphicsUnit graphicsunit;
	}

}
	public enum TransitionStyle{
		None,
		Fade
	}