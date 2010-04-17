using System;
using System.IO;
using System.Text;

namespace Org.Kuhn.Yapss {
    class Log {
        private Log() {
			System.Diagnostics.Debug.Print(LogFilePath);
			System.IO.Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
        }

        public static Log Instance {
            get { return instance; }
        }

        public bool IsEnabled {
            get { return enabled; }
            set { enabled = value; }
        }

        public void Write(String message) {
            Write(message, null);
        }

        public void Write(String message, Exception ex) {
            if (enabled) {
                lock (this) {
                    StreamWriter writer = null;
                    try {
                        writer = File.AppendText(logFilePath);
                        writer.Write(DateTime.Now.ToString());
                        writer.Write(" ");
                        if (System.Diagnostics.Debugger.IsAttached) { System.Diagnostics.Debug.Print(message); }
                        writer.WriteLine(message);
                        
                        if (ex != null) {
                            writer.WriteLine(ex.ToString());
                        }
                        writer.Flush();
                    }
                    catch {
                        // ignore
                    }
                    finally {
                        if (writer != null) {
                            writer.Dispose();
                        }
                    }
                }

            }
        }

        private static Log instance = new Log();
        private bool enabled = false;
        private string logFilePath = Path.Combine(
                    			Path.Combine(
                    			System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),"YAPSSCE\\"),
                    	       "yapssce.log");
        
        
		public string LogFilePath {
			get { return logFilePath; }
		}
    }
}
