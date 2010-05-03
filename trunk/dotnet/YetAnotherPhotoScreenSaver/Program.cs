using System;
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
                // just in case of abnormal termination
            }
        }

        public Program(Config config) {
            this.config = config;
        }

        public void Run() {
            Cursor.Hide();
        	Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
            Config myconfig = config;

            Log.Instance.IsEnabled = config.IsLoggingEnabled;
            Log.Instance.Write("Starting screen saver");
            Log.Instance.Write(Application.CommonAppDataPath);
            Random rnd = new Random();
            
            
            myconfig.BackGroundStyle = config.BackGroundStyle == BackGroundStyle.Random ?
                (rnd.Next(1,10) % 2 == 0 ? BackGroundStyle.Black : BackGroundStyle.White)
                : config.BackGroundStyle;
            myconfig.ImageStyle = config.ImageStyle == ImageStyle.Random ?
                (rnd.Next(1,10) % 2 == 0 ? ImageStyle.CenterFill : ImageStyle.Whole)
                : config.ImageStyle;


            List<IImageSource> imageSources = new List<IImageSource>();
            if (config.IsEnabledFileImageSource)
                imageSources.Add(new FileImageSource(config.FileImageSourcePath, config.Comicstyle));
            	
            if (config.IsEnabledFlickrImageSource)
                imageSources.Add(new FlickrImageSource(config.FlickrImageSourceTags, config.IsFlickrImageSourceTagAndLogic, config.FlickrImageSourceUserName, config.FlickrImageSourceText,  myconfig.ImageStyle == ImageStyle.CenterFill));
            IImageSource imageSource = new RoundRobinImageSource(imageSources, new ColorSquareImageSource());

            // base x size on width of primary screen
            int xSize = Screen.PrimaryScreen.Bounds.Width / config.XCount;

            // build window for each screen
            // order screens by x coordinates

            //int screenlimit=1;
            
            SortedList<int,Screen> orderedscreens = new SortedList<int,Screen>();
            foreach (Screen pscreen in Screen.AllScreens){
            //Screen pscreen = Screen.PrimaryScreen;
                orderedscreens.Add(pscreen.Bounds.X, pscreen);
                //if (orderedscreens.Count >= screenlimit){break;};

            }
			
            foreach (Screen screen in orderedscreens.Values) {
                //Log.Instance.Write(screen.DeviceName + "@ X:" + screen.Bounds.X + "Y:" + screen.Bounds.Y);
            	//Screen screen = Screen.PrimaryScreen;

                Window wnd = new Window(screen.Bounds, xSize, myconfig, imageSource);
                windows.Add(wnd);
				wnd.End += DisplayWindowEndEventHandler;
				
                wnd.Show();
            }
            Window wnd1 = windows[0];
            wnd1.Focus();
            wnd1.LostFocus+=DisplayWindowLostFocusEvenHandler;
            
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
            //stopcall = true;
            //thread.Abort();
            

            try
            {
                Log.Instance.Write("Starting Abort");
                thread.Abort();
                stopcall = true;
                Log.Instance.Write("Waiting to end");
                while (thread.IsAlive) { }
                Log.Instance.Write("rejoining thread");
                thread.Join();
                Log.Instance.Write("Join Complete");
            }
            catch (ThreadAbortException) { 
            }
            
            //int iwnd=0;
            Log.Instance.Write("Windows = " + Convert.ToString(windows.Count));
            for (int intwnd=windows.Count-1; intwnd>=0; intwnd--)
            {
	           	Log.Instance.Write("CLOSING " + intwnd);
    	        Window win = windows[intwnd];
            	win.Close();
            }

        }
        
		
        private Config config;
        private IList<Window> windows = new List<Window>();
        private Thread thread;
        private Boolean stopcall = false;
        private Boolean stopped = false;
        private AsyncMultiController controller;
        private void ThreadProc() {
            Log.Instance.Write("Drawing thread started");

            // build the multicontroller
            IList<IController> controllers = new List<IController>();
            for (int i = 0; i < windows.Count; ++i) {
                controllers.Add(windows[i].Controller);
            }

            // begin the controller loop, aborted by stop call

            int cachesize = 20;
            Log.Instance.Write("Cache Size = " + Convert.ToString(cachesize));
            using (controller = new AsyncMultiController(new MultiController(controllers), cachesize )) {
                while (stopcall ==false) {
                    try {
                        using (MultiControllerInstruction instruction = controller.GetInstruction()) {
                            instruction.screen = instruction.controllerIndex;
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
            controller.Quit();
        	End(this, EventArgs.Empty);
            
        }
        private void DisplayWindowLostFocusEvenHandler(object sender, EventArgs args) {
            controller.Quit();
            End(this, EventArgs.Empty);
        }
        
        public event EventHandler End;
	//	private int lostfocus = 0;
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;
        private const string TASKBAR_WINDOW = "Shell_TrayWnd";

        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);
        
        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);


         
        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            
        }
        
    }

	
}