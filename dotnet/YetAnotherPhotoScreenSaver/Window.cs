using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using Org.Kuhn.Yapss.transitions;

namespace Org.Kuhn.Yapss {
    class Window : Form {
        public Window(Rectangle bounds, int xSize, Config config, IImageSource imageSource) {
            Log.Instance.Write("Creating screen saver window at bounds " + bounds);
            this.bounds = bounds;
            this.xSize = xSize;
            this.config = config;
            this.backgroundstyle = config.BackGroundStyle;
            this.imagestyle = config.ImageStyle;
            //this.theme = theme;
            this.imageSource = imageSource;
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnLoad(EventArgs e) {
            // appearance
            Bounds = bounds;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            TopMost = true;
            //Cursor.Hide();
            xSize = (xSize<Height) ? xSize:Height;
               
            // calculate ySize and offsets
            if (imagestyle == ImageStyle.CenterFill) {
                ySize = xSize;
            }
            else {
                ySize = Height / (Height / xSize);
            }
            xOff = (Width % xSize) / 2;
            yOff = (Height % ySize) / 2;

            // build the controller
            int w = Width / xSize;
            int h = Height / ySize;
            controller = new Controller(w, h, xSize, ySize, imageSource);

            // build the graphics buffer
            using (Graphics g = CreateGraphics()) {
                bufferedGraphics = BufferedGraphicsManager.Current.Allocate(g, new Rectangle(Point.Empty, Size));
            }
            Log.Instance.Write("Background = " + Enum.GetName(typeof(BackGroundStyle), backgroundstyle));
            Log.Instance.Write("Filling bounds = x:" + Bounds.X + " Y:" + Bounds.Y);
            bufferedGraphics.Graphics.FillRectangle(config.BackBrush, new Rectangle(0,0,bounds.Width,bounds.Height));
            // draw the start state
            string banner = "Yet Another Photo Screen Saver Comic Edition\n";
            banner += "This software is free and open source\n";
            banner += "http://code.google.com/p/yetanotherphotoscreensavercomicedition/\n";
            banner += "http://shrinkster.com/sxt";
            using (Font font = new Font("Georgia", 8.25F)) {
                SizeF sizef = bufferedGraphics.Graphics.MeasureString(banner, font);
                bufferedGraphics.Graphics.DrawString(banner, font, Brushes.DarkGray, Math.Max(xOff, yOff), Height - sizef.Height - Math.Max(xOff, yOff));
            }
            Invalidate(Bounds);

            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e) {
            CloseOutProcs();
            Log.Instance.Write("Closing Base");
            base.OnClosed(e);

        }

        private void CloseOutProcs() {

            Log.Instance.Write("Window Close Called");
            Log.Instance.Write("Showing Cursor");
            Cursor.Show();          
			
			if (trans !=null)
			{
            	Log.Instance.Write("Canceling Transition");
            	trans.Cancel();
            };
			if (bufferedGraphics !=null)
			{
            	Log.Instance.Write("Disposing BufferedGraphics");
            	lock (bufferedGraphics)
            	{
                	bufferedGraphics.Dispose();
            	}
			}
			//if (controller !=null)
			//{
			//	Log.Instance.Write("Setting Controller to null");
			//	controller = null;
			//}
        }

        private Bitmap OrigImage(Rectangle Area, Rectangle Screenbounds) {
			
            Bitmap result = new Bitmap(Area.Width, Area.Height);
            Graphics g = this.CreateGraphics();
            Size s = Area.Size;
            result = new Bitmap(s.Width,s.Height,g);
            Graphics memoryGraphics = Graphics.FromImage(result);
            memoryGraphics.CopyFromScreen(Area.X+Screenbounds.X, Area.Y+Screenbounds.Y, 0, 0, Area.Size);
            return result;
        }

        public void Draw(Instruction instruction) {
            //lock (bufferedGraphics) {
            
            Log.Instance.Write("Starting windows draw instruction");
            try
            {
                if (instruction.image == null) { Log.Instance.Write("Null Image"); };

                Rectangle targetAreaRect = new Rectangle(
                        xOff + instruction.x * xSize,
                        yOff + instruction.y * ySize,
                        instruction.w * xSize,
                        instruction.h * ySize
                        );
                targetarearect = targetAreaRect;
                if (imagestyle == ImageStyle.CenterFill)
                {
                    destRect = targetAreaRect;
                    double ratio = (double)instruction.image.Width / (double)instruction.image.Height;
                    double targetRatio = (double)instruction.w / (double)instruction.h;
                    if (ratio > targetRatio)
                    {
                        int diff = instruction.image.Width - (int)((double)instruction.image.Height * targetRatio);
                        sourceRect = new Rectangle(
                            diff / 2,
                            0,
                            instruction.image.Width - diff,
                            instruction.image.Height
                            );
                    }
                    else if (ratio < targetRatio)
                    {
                        int diff = instruction.image.Height - (int)((double)instruction.image.Width / targetRatio);
                        sourceRect = new Rectangle(
                            0,
                            diff / 2,
                            instruction.image.Width,
                            instruction.image.Height - diff
                            );
                    }
                    else
                    {
                        sourceRect = new Rectangle(Point.Empty, instruction.image.Size);
                    }
                }
                else
                {
                    float scale = Math.Min(
                        (float)(instruction.w * xSize) / (float)instruction.image.Width,
                        (float)(instruction.h * ySize) / (float)instruction.image.Height);
                    int w = (int)((float)instruction.image.Width * scale);
                    int h = (int)((float)instruction.image.Height * scale);
                    const int padding = 4;
                    destRect = new Rectangle(
                        xOff + instruction.x * xSize + instruction.w * xSize / 2 - w / 2 + padding,
                        yOff + instruction.y * ySize + instruction.h * ySize / 2 - h / 2 + padding,
                        w - padding * 2, h - padding * 2);
                    sourceRect = new Rectangle(Point.Empty, instruction.image.Size);
                }

                try {
                Log.Instance.Write("Start transistion");
                trans = new Org.Kuhn.Yapss.transitions.transition(this,bufferedGraphics, destRect, sourceRect, GraphicsUnit.Pixel,config);                
                Log.Instance.Write("transition Out");
                if (instruction.image == null) { Log.Instance.Write("Null Image"); };
               		trans.transitionout(OrigImage(targetAreaRect,Screen.AllScreens[instruction.screen].Bounds), targetAreaRect);
                	Log.Instance.Write("transition In");
                	trans.transitionin(instruction.image, targetAreaRect);//});
                	Log.Instance.Write("transitions Done");
                } catch(ThreadAbortException){//ignore}
                } catch (Exception Ex) {
                	Log.Instance.Write("Transition Error");
                	Log.Instance.Write(Ex.Message);
                } finally {
                	
                }
                
            }
            catch (Exception ex)
            {
                Log.Instance.Write("Draw Error");
                Log.Instance.Write(ex.Message);
            }
        }
        

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            lock (bufferedGraphics) {
                bufferedGraphics.Render(e.Graphics);
            }
        }


        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            Log.Instance.Write("Mouse Move =" + Convert.ToString(e.Location) + Convert.ToString(e.Button));
            if (DateTime.Now - lastMove > TimeSpan.FromSeconds(1)) {
                moveCount = 0;
                lastMove = DateTime.Now;
                return;
            }
            if (++moveCount == 40) {
                Log.Instance.Write("User termination event (mouse move)");
                
                End(this, EventArgs.Empty);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            Log.Instance.Write("User termination event (mouse down)");
            End(this, EventArgs.Empty);
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            Log.Instance.Write("User termination event (key down)");
            End(this, EventArgs.Empty);
        }

        public IController Controller {
            get { return controller; }
        }

        public event EventHandler End;

        private Config config;
        private int moveCount = 0;
        private DateTime lastMove = DateTime.Now;       
        private Rectangle bounds;
        private int xSize, ySize, xOff, yOff;
        private BackGroundStyle backgroundstyle;
        private ImageStyle imagestyle;
        //private Theme theme;
        private Rectangle targetarearect;
        private IImageSource imageSource;
        private IController controller;
        private BufferedGraphics bufferedGraphics;
        public transition trans;
        public Rectangle sourceRect;
        public Rectangle destRect;
    }
}