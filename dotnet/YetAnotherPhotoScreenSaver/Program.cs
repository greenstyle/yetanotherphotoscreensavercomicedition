﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace Org.Kuhn.Yapss {
    class Program {
        [STAThread()]
        static void Main(string[] args) {
            try {
        		
                foreach (Process process in Process.GetProcesses())
                    if (process.Id != Process.GetCurrentProcess().Id && process.ProcessName.Equals("YetAnotherPhotoScreenSaverCE"))
                        return;
                if (args.Length > 0)
                    if (args[0].ToLower().Contains("/p"))
                        return;
                    else if (args[0].ToLower().Contains("/c")) {
                        Application.Run(new ConfigWindow());
                        return;
                    }

                Config config = new Config();
                Program program = new Program(config);
                program.End += (obj, e) => {
                    Log.Instance.Write("Stopping screen saver");
                    program.Stop();
                    Application.Exit();
                };
                program.Run();
                Application.Run();
            }
            catch (Exception ex) {
                Log.Instance.Write("Unhandled exception on main thread", ex);
            }
            finally {
                ShowTaskbar(); // just in case of abnormal termination
            }
        }

        public Program(Config config) {
            this.config = config;
        }

        public void Run() {
            Cursor.Hide();
        	Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
            Config myconfig = config;
            //HideTaskbar();
            Log.Instance.IsEnabled = config.IsLoggingEnabled;
            Log.Instance.Write("Starting screen saver");

            Random rnd = new Random();
            
            
            myconfig.BackGroundStyle = config.BackGroundStyle == BackGroundStyle.Random ?
                (rnd.Next(1,10) % 2 == 0 ? BackGroundStyle.Black : BackGroundStyle.White)
                : config.BackGroundStyle;
            myconfig.ImageStyle = config.ImageStyle == ImageStyle.Random ?
                (rnd.Next(1,10) % 2 == 0 ? ImageStyle.CenterFill : ImageStyle.Whole)
                : config.ImageStyle;


             //Theme theme = config.Theme == Theme.Random ?
            //    (DateTime.Now.Second % 2 == 0 ? Theme.Dark : Theme.Light)
            //    : config.Theme;

            List<IImageSource> imageSources = new List<IImageSource>();
            if (config.IsEnabledFileImageSource)
                //foreach (string fileimagesourcepath in config.FileImageSourcePath.Split(';'))
                //{
                imageSources.Add(new FileImageSource(config.FileImageSourcePath, config.Comicstyle));
                //}
            	
            if (config.IsEnabledFlickrImageSource)
                imageSources.Add(new FlickrImageSource(config.FlickrImageSourceTags, config.IsFlickrImageSourceTagAndLogic, config.FlickrImageSourceUserName, config.FlickrImageSourceText,  myconfig.ImageStyle == ImageStyle.CenterFill));
            IImageSource imageSource = new RoundRobinImageSource(imageSources, new ColorSquareImageSource());

            // base x size on width of primary screen
            int xSize = Screen.PrimaryScreen.Bounds.Width / config.XCount;

            // build window for each screen
            // order screens by x coordinates
            SortedList<int,Screen> orderedscreens = new SortedList<int,Screen>();
            foreach (Screen screen in Screen.AllScreens){
                orderedscreens.Add(screen.Bounds.X, screen);
            }


            foreach (Screen screen in orderedscreens.Values) {
                Log.Instance.Write(screen.DeviceName + "@ X:" + screen.Bounds.X + "Y:" + screen.Bounds.Y);
            	//Screen screen = Screen.PrimaryScreen;
                Window wnd = new Window(screen.Bounds, xSize, myconfig, imageSource);
                windows.Add(wnd);
                wnd.End += DisplayWindowEndEventHandler;
                wnd.Show();
            }
            
            // start the background drawing thread
            thread = new Thread(ThreadProc);
            thread.IsBackground = true;
            thread.Start();
        }

        public void Stop() {
            Cursor.Show();
            Log.Instance.Write("Windows = " + Convert.ToString(windows.Count));
            for (int intwnd = windows.Count - 1; intwnd >= 0; intwnd--)
            {
                Log.Instance.Write("Hiding Window " + intwnd);
                Window win = windows[intwnd];
                win.Hide();
            }
            stopcall = true;
            while (stopped == false) { }

            try
            {
                //thread.Abort();
                thread.Join();
            }
            catch (ThreadAbortException ex) { 
            }
            
            int iwnd=0;
            Log.Instance.Write("Windows = " + Convert.ToString(windows.Count));
            for (int intwnd=windows.Count-1; intwnd>=0; intwnd--)
            {
           	Log.Instance.Write("CLOSING " + intwnd);
            Window win = windows[intwnd];
            	win.Close();
            }
            //foreach (Window wnd in windows){
            //	Log.Instance.Print("CLOSING " + iwnd);
           // 	iwnd++;
            //	wnd.Close();}
        }
        

        private Config config;
        private IList<Window> windows = new List<Window>();
        private Thread thread;
        private Boolean stopcall = false;
        private Boolean stopped = false;

        private void ThreadProc() {
            Log.Instance.Write("Drawing thread started");

            // build the multicontroller
            IList<IController> controllers = new List<IController>();
            for (int i = 0; i < windows.Count; ++i) {
                controllers.Add(windows[i].Controller);
            }

            // begin the controller loop, aborted by thread termination only
            using (AsyncMultiController controller = new AsyncMultiController(new MultiController(controllers), 20)) {
                while (stopcall ==false) {
                    try {
                        using (MultiControllerInstruction instruction = controller.GetInstruction()) {
                            windows[instruction.controllerIndex].Draw(instruction);
                            Thread.Sleep(instruction.longPause ? config.LongInterval : config.ShortInterval);
                        }
                    }
                    catch (ThreadAbortException) {
                        // ignore
                    }
                    catch (Exception ex) {
                        Log.Instance.Write("Exception on drawing thread", ex);
                    }
                }
   
            }
            stopped = true;
        }

        private void DisplayWindowEndEventHandler(object sender, EventArgs args) {
            //Stop();
        	End(this, EventArgs.Empty);
            
        }

        public event EventHandler End;

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;
        private const string TASKBAR_WINDOW = "Shell_TrayWnd";

        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);
        
        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);

        private static void HideTaskbar() {
        }

        private static void ShowTaskbar() {
        }
         
        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            ShowTaskbar();
        }
        
    }

	
}