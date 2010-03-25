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
                Brush backbrush = backgroundstyle == BackGroundStyle.Black ? Brushes.Black : Brushes.White;
                Color color = backgroundstyle == BackGroundStyle.Black ? Color.Black : Color.White;
                switch (transitionstyle)
                {
                    case TransitionStyle.Zoom:
                        for (int t = 0; t <= 100; t += 5)
                        {
                            lock (bufferedgraphics)
                            {
                                decimal perc = (decimal)t / 100;
                                Rectangle fill = new Rectangle();//Convert.ToInt16((destRect.X + (destRect.Width/2)) + Convert.ToInt16((destRect.Width/2) * perc))), Convert.ToInt16((destRect.Y + (destRect.Height/2)) + Convert.ToInt16(((destRect.Height/2) * perc)) , Convert.ToInt16(destRect.Width/2) + Convert.ToInt16((destRect.Width/2) * perc), Convert.ToInt16(destRect.Height/2)+ Convert.ToInt16((destRect.Height/2) * perc));
                                fill.X = Convert.ToInt16((destRect.X + (destRect.Width / 2)) - Convert.ToInt16((destRect.Width / 2) * perc));
                                fill.Y = Convert.ToInt16((destRect.Y + (destRect.Height / 2)) - Convert.ToInt16((destRect.Height / 2) * perc));
                                fill.Width = Convert.ToInt16( destRect.Width * perc);
                                fill.Height = Convert.ToInt16(destRect.Height * perc);
                                bufferedgraphics.Graphics.DrawImage(image, fill, sourceRect, graphicsunit);
                            }
                            frm.Invalidate(targetArea);
                            Application.DoEvents();
                        }
                        lock (bufferedgraphics)
                        {
                            bufferedgraphics.Graphics.FillRectangle(backbrush, targetArea);
                            bufferedgraphics.Graphics.DrawImage(image, destRect, sourceRect, graphicsunit);
                        }
                        frm.Invalidate(targetArea);
                    break;
                    case TransitionStyle.Fade:
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
                    case TransitionStyle.PageTurn:
                        for (int t = 0; t <= 100; t += 5)
                        {
                            lock (bufferedgraphics)
                            {
                                decimal perc = (decimal) t / 100;
                                Rectangle fill = new Rectangle(destRect.X, destRect.Y,Convert.ToInt16(destRect.Width *perc), destRect.Height);
                                
                                bufferedgraphics.Graphics.DrawImage(image, fill, sourceRect, graphicsunit);
                            }

                            frm.Invalidate(targetArea);
                            Application.DoEvents();
                        }
                        lock (bufferedgraphics)
                        {
                            bufferedgraphics.Graphics.FillRectangle(backbrush, targetArea);
                            bufferedgraphics.Graphics.DrawImage(image, destRect, sourceRect, graphicsunit);
                        }
                        frm.Invalidate(targetArea);
                        
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
            Color color = backgroundstyle == BackGroundStyle.Black ? Color.Black : Color.White;
            switch (transitionstyle) {
				case TransitionStyle.Fade:
                    
                    for (int t = 100; t >= 0; t -= 1)
                    {
                        lock (bufferedgraphics)
                        {
                            decimal perc = ((100-(decimal)t) / 100) ;
                            int AlphaColor = Convert.ToInt16(perc * 255);
                            Color veryTransparentColor = Color.FromArgb(AlphaColor, color.R, color.G, color.B);
                            bufferedgraphics.Graphics.FillRectangle(new SolidBrush(veryTransparentColor), targetArea);
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
		Fade,
        PageTurn,
        Zoom
	}