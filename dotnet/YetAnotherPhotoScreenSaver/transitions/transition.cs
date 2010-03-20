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
                        Color color = backgroundstyle == BackGroundStyle.Black ? Color.Black : Color.White;
                        for (int t = 0; t <= 100; t += 5)
                        {
                            lock (bufferedgraphics)
                            {
                                bufferedgraphics.Graphics.FillRectangle(backbrush, targetArea);
                                decimal perc = ((decimal)(100 - t) / 100) * 255;
                                int AlphaColor = Convert.ToInt32(perc);
                                Color veryTransparentColor = Color.FromArgb(AlphaColor, color.R, color.G, color.B);
                                bufferedgraphics.Graphics.DrawImage( image,destRect,sourceRect,graphicsunit);
                                bufferedgraphics.Graphics.FillRectangle(new SolidBrush(veryTransparentColor), destRect);
                            }

                            frm.Invalidate(targetArea);
                            Application.DoEvents();
                        }
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

        public void transitionout(Form frm, Rectangle targetArea, TransitionStyle transitionstyle, BackGroundStyle backgroundstyle)
        {
            Brush backbrush = backgroundstyle == BackGroundStyle.Black ? Brushes.Black : Brushes.White;
            switch (transitionstyle) {
				case TransitionStyle.Fade:
                    Color color = backgroundstyle == BackGroundStyle.Black ? Color.Black : Color.White;
                    for (int t = 100; t >= 0; t -= 5)
                    {
                        lock (bufferedgraphics)
                        {
                            decimal perc = ((decimal)(100 - 95) / 100) * 255;
                            int AlphaColor = Convert.ToInt32(perc);
                            Color veryTransparentColor = Color.FromArgb(AlphaColor, color.R, color.G, color.B);
                            
                            bufferedgraphics.Graphics.FillRectangle(new SolidBrush(veryTransparentColor), destRect);
                        }

                        frm.Invalidate(targetArea);
                        Application.DoEvents();
                    }
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

        private void fadeimages(Image startimage, Rectangle rect, Color color, int trans)
        {
            Image result = startimage;
            decimal perc = ((decimal)trans / 100) * 255;
            int AlphaColor = Convert.ToInt32(perc);
            Color veryTransparentColor = Color.FromArgb(AlphaColor, color.R, color.G, color.B);
            bufferedgraphics.Graphics.DrawImage(result, rect);
            bufferedgraphics.Graphics.FillRectangle(new SolidBrush(veryTransparentColor), rect);
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
		private Image image;
		private Rectangle destRect;
		private Rectangle sourceRect;
		private GraphicsUnit graphicsunit;
	}

}
	public enum TransitionStyle{
		None,
		Fade
	}