/*
 * Created by SharpDevelop.
 * User: Chad
 * Date: 3/8/2010
 * Time: 2:12 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Media;
using System.Diagnostics;
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
            try
            {
                image = setimage;
                destRect = setdestRect;
                sourceRect = setsourceRect;
                graphicsunit = setgraphicsunit;
            }
            catch(Exception ex) {
                Log.Instance.Write("Error setting up transition");
                Log.Instance.Write(ex.Message);
               
            }
		}


        public void transitionin(Form frm, Rectangle targetArea, TransitionStyle transitionstyle, BackGroundStyle backgroundstyle)
        {
            {
                try
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
                                    fill.Width = Convert.ToInt16(destRect.Width * perc);
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
                            Application.DoEvents();
                            break;
                        case TransitionStyle.Fade:
                            lock (bufferedgraphics) {
                                bufferedgraphics.Graphics.FillRectangle(backbrush, targetArea);
                            }
                            frm.Invalidate(targetArea);
                            Application.DoEvents();
                            for (int t = 0; t <= 100; t += 5)
                            {
                                lock (bufferedgraphics)
                                {
                                    
                                    decimal perc = ((decimal)(100 - t) / 100) * 255;
                                    int AlphaColor = Convert.ToInt32(perc);
                                    Color veryTransparentColor = Color.FromArgb(AlphaColor, color.R, color.G, color.B);
                                    bufferedgraphics.Graphics.DrawImage(image, destRect, sourceRect, graphicsunit);
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
                                    decimal perc = (decimal)t / 100;
                                    Rectangle fill = new Rectangle(destRect.X, destRect.Y, Convert.ToInt16(destRect.Width * perc), destRect.Height);

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
                                bufferedgraphics.Graphics.DrawImage(image,destRect,sourceRect,graphicsunit);
                                frm.Invalidate(targetArea);
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Write("Transtion in Error");
                    Log.Instance.Write(ex.Message);
                }
                
            }
		}

        public void transitionout(Form frm, Rectangle targetArea, TransitionStyle transitionstyle, BackGroundStyle backgroundstyle)
        {
            try
            {

                Brush backbrush = backgroundstyle == BackGroundStyle.Black ? Brushes.Black : Brushes.White;
                Color color = backgroundstyle == BackGroundStyle.Black ? Color.Black : Color.White;
                //Form frmimg = new Form();
                //
                //
                //frmimg.Show();
                //frmimg.BringToFront();
                //frmimg.Left = Screen.AllScreens[1].Bounds.X;
                Image origimg;
                lock (bufferedgraphics)
                {
                    origimg = new Bitmap(targetArea.Width, targetArea.Height, bufferedgraphics.Graphics);
                }
                //Graphics g = Graphics.FromImage(origimg);
                
                //g.CopyFromScreen(targetArea.X, targetArea.Y, 0, 0, targetArea.Size);
                //frmimg.BackgroundImage = origimg;
                
                switch (transitionstyle)
                {
                    case TransitionStyle.Fade:

                        for (int t = 100; t >= 0; t--)
                        {
                            lock (bufferedgraphics)
                            {
                                decimal perc = ((100 - (decimal)t) / 100);
                                //decimal perc = 90;
                                int AlphaColor = Convert.ToInt16(perc * 255);
                                Color veryTransparentColor = Color.FromArgb(AlphaColor, color.R, color.G, color.B);
                                bufferedgraphics.Graphics.DrawImage(origimg, targetArea, targetArea, graphicsunit);
                                bufferedgraphics.Graphics.FillRectangle(new SolidBrush(veryTransparentColor), targetArea);
                            }

                            frm.Invalidate(targetArea);
                            Application.DoEvents();
                        }
                        break;
                    case TransitionStyle.Zoom:
                        for (int t = 100; t >= 0; t -= 5)
                        {
                            lock (bufferedgraphics)
                            {
                                decimal perc = (decimal)t / 100;
                                Debug.Print("Perc =" + Convert.ToString(perc));
                                Rectangle fill = new Rectangle();//Convert.ToInt16((destRect.X + (destRect.Width/2)) + Convert.ToInt16((destRect.Width/2) * perc))), Convert.ToInt16((destRect.Y + (destRect.Height/2)) + Convert.ToInt16(((destRect.Height/2) * perc)) , Convert.ToInt16(destRect.Width/2) + Convert.ToInt16((destRect.Width/2) * perc), Convert.ToInt16(destRect.Height/2)+ Convert.ToInt16((destRect.Height/2) * perc));
                                fill.X = Convert.ToInt16((destRect.X + (destRect.Width / 2)) - Convert.ToInt16((destRect.Width / 2) * perc));
                                fill.Y = Convert.ToInt16((destRect.Y + (destRect.Height / 2)) - Convert.ToInt16((destRect.Height / 2) * perc));
                                fill.Width = Convert.ToInt16(destRect.Width * perc);
                                fill.Height = Convert.ToInt16(destRect.Height * perc);
                                bufferedgraphics.Graphics.FillRectangle(backbrush, targetArea);
                                bufferedgraphics.Graphics.DrawImage(origimg, fill, sourceRect, graphicsunit);
                            }
                            frm.Invalidate(targetArea);
                            Application.DoEvents();
                        }
                        frm.Invalidate(targetArea);
                        Application.DoEvents();
                        break;
                    case TransitionStyle.PageTurn:
                        for (int t = 100; t >= 0; t -= 5)
                        {
                            lock (bufferedgraphics)
                            {
                                decimal perc = (decimal)t / 100;
                                Rectangle fill = new Rectangle(targetArea.X, targetArea.Y, Convert.ToInt16(targetArea.Width * perc), targetArea.Height);

                                bufferedgraphics.Graphics.DrawImage(origimg, fill, targetArea, graphicsunit);
                            }

                            frm.Invalidate(targetArea);
                            Application.DoEvents();
                        }
                        lock (bufferedgraphics)
                        {
                            bufferedgraphics.Graphics.FillRectangle(backbrush, targetArea);
                        }
                        frm.Invalidate(targetArea);
                        break;
                    case TransitionStyle.None:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex) {
                Log.Instance.Write("Transistion Out Error:");
                Log.Instance.Write(ex.Message);
            }  //frm.Invalidate(targetArea);
		
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