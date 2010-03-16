using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using Org.Kuhn.Yapss.transitions;

namespace Org.Kuhn.Yapss {
    class Window : Form {
        public Window(Rectangle bounds, int xSize, BackGroundStyle backgroundstyle, ImageStyle imagestyle, IImageSource imageSource) {
            Log.Instance.Write("Creating screen saver window at bounds " + bounds);
            this.bounds = bounds;
            this.xSize = xSize;
            this.backgroundstyle = backgroundstyle;
            this.imagestyle = imagestyle;
            //this.theme = theme;
            this.imageSource = imageSource;
        }

        protected override void OnLoad(EventArgs e) {
            // appearance
            Bounds = bounds;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            TopMost = true;
            Cursor.Hide();
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

            // draw the start state
            bufferedGraphics.Graphics.FillRectangle(backgroundstyle == BackGroundStyle.Black ? Brushes.Black : Brushes.White, Bounds);
            string banner = "Yet Another Photo Screen Saver Comic Edition\n";
            banner += "This software is free and open source\n";
            banner += "http://code.google.com/p/yetanotherphotoscreensavercomicedition/\n";
            banner += "http://shrinkster.com/sxt";
            using (Font font = new Font("Georgia", 8.25F)) {
                SizeF sizef = bufferedGraphics.Graphics.MeasureString(banner, font);
                bufferedGraphics.Graphics.DrawString(banner, font, Brushes.DarkGray, Math.Max(xOff, yOff), Height - sizef.Height - Math.Max(xOff, yOff));
            }
            Invalidate();

            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e) {
            bufferedGraphics.Dispose();
            Cursor.Show(); 
            base.OnClosed(e);
        }

        public void Draw(Instruction instruction) {
            //lock (bufferedGraphics) {
                Rectangle targetAreaRect = new Rectangle(
                        xOff + instruction.x * xSize,
                        yOff + instruction.y * ySize,
                        instruction.w * xSize,
                        instruction.h * ySize
                        );
                targetarearect = targetAreaRect;
                if (imagestyle == ImageStyle.CenterFill) {
                    destRect = targetAreaRect;
                    double ratio = (double)instruction.image.Width / (double)instruction.image.Height;
                    double targetRatio = (double)instruction.w / (double)instruction.h;
                    if (ratio > targetRatio) {
                        int diff = instruction.image.Width - (int)((double)instruction.image.Height * targetRatio);
                        sourceRect = new Rectangle(
                            diff / 2,
                            0,
                            instruction.image.Width - diff,
                            instruction.image.Height
                            );
                    }
                    else if (ratio < targetRatio) {
                        int diff = instruction.image.Height - (int)((double)instruction.image.Width / targetRatio);
                        sourceRect = new Rectangle(
                            0,
                            diff / 2,
                            instruction.image.Width,
                            instruction.image.Height - diff
                            );
                    }
                    else {
                        sourceRect = new Rectangle(Point.Empty, instruction.image.Size);
                    }
                }
                else {
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

                trans = new Org.Kuhn.Yapss.transitions.transition(bufferedGraphics);
                trans.set(instruction.image, destRect, sourceRect, GraphicsUnit.Pixel);
                //trans.transitionout(this, targetAreaRect, TransitionStyle.None);
                //ParameterizedThreadStart ts = new ParameterizedThreadStart(Fadein);

                //Thread transitionthread = new Thread(delegate() { Fadein(instruction.image); });
                //transitionthread.Start();

                //Thread transitionthread = new Thread(delegate(){
                trans.transitionin(this, targetAreaRect, TransitionStyle.None, backgroundstyle);//});

                //transitionthread.Start();
                //bufferedGraphics.Graphics.DrawImage(
                //        instruction.image,
                //        destRect,
                //        sourceRect,
                //        GraphicsUnit.Pixel
                //        );
                //Invalidate(targetAreaRect);                
            //}
        }
        private void Fadein(Image image ) {
            trans.transitionin(this, targetarearect, TransitionStyle.Fade, backgroundstyle);
            
        }

        public delegate void ThreadStart();

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            lock (bufferedGraphics) {
                bufferedGraphics.Render(e.Graphics);
            }
        }


        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
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