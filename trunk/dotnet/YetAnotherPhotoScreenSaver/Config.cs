using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Text;
using System.Drawing;

namespace Org.Kuhn.Yapss {
    public class Config {
        public Config() {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(KEY);
            if (reg == null)
                return;
            xCount = (int)reg.GetValue("xCount", xCount);
            longInterval = (int)reg.GetValue("longInterval", longInterval);
            shortInterval = (int)reg.GetValue("shortInterval", shortInterval);
            isEnabledFileImageSource = (int)reg.GetValue("isEnabledFileImageSource", Convert.ToInt32(isEnabledFileImageSource)) == 1;
            fileImageSourcePath = (string)reg.GetValue("fileImageSourcePath", fileImageSourcePath);
            isEnabledFlickrImageSource = (int)reg.GetValue("isEnabledFlickrImageSource", Convert.ToInt32( isEnabledFlickrImageSource)) == 1;
            flickrImageSourceTags = (string)reg.GetValue("flickrImageSourceTags", flickrImageSourceTags);
            isFlickrImageSourceTagAndLogic = (int)reg.GetValue("isFlickrImageSourceTagAndLogic", isFlickrImageSourceTagAndLogic) == 1;
            flickrImageSourceUserName = (string)reg.GetValue("flickrImageSourceUserName", flickrImageSourceUserName);
            flickrImageSourceText = (string)reg.GetValue("flickrImageSourceText", flickrImageSourceText);
            backgroundstyle = (BackGroundStyle)Enum.Parse(typeof(BackGroundStyle), (string)reg.GetValue("backgroundstyle", Enum.GetName(typeof(BackGroundStyle), backgroundstyle)));
            imagestyle = (ImageStyle)Enum.Parse(typeof(ImageStyle), (string)reg.GetValue("imagestyle", Enum.GetName(typeof(ImageStyle), imagestyle)));
            transitionin = (TransitionStyle)Enum.Parse(typeof(TransitionStyle), (string)reg.GetValue("transitionin", Enum.GetName(typeof(TransitionStyle), transitionin)));
            transitionout = (TransitionStyle)Enum.Parse(typeof(TransitionStyle), (string)reg.GetValue("transitionout", Enum.GetName(typeof(TransitionStyle), transitionout)));
            //theme = (Theme)Enum.Parse(typeof(Theme), (string)reg.GetValue("theme", Enum.GetName(typeof(Theme), theme)));
            comicstyle = (Comicstyle)Enum.Parse(typeof(Comicstyle), (string)reg.GetValue("comicstyle", Enum.GetName(typeof(Comicstyle), comicstyle)));
            isLoggingEnabled = (int)reg.GetValue("isLoggingEnabled", Convert.ToInt32(isLoggingEnabled)) == 1;
            filesearchfilter = (string)reg.GetValue("filesearchfilter", filesearchfilter);
            enablefilesearchfilter = (int)reg.GetValue("enablefilesearchfilter", Convert.ToInt32(enablefilesearchfilter)) == 1;
            reg.Close();
        }
        public void Save() {
            RegistryKey reg = Registry.CurrentUser.CreateSubKey(KEY);
            reg.SetValue("xCount", xCount, RegistryValueKind.DWord);
            reg.SetValue("longInterval", longInterval,RegistryValueKind.DWord);
            reg.SetValue("shortInterval", shortInterval, RegistryValueKind.DWord);
            reg.SetValue("isEnabledFileImageSource", isEnabledFileImageSource, RegistryValueKind.DWord);
            reg.SetValue("fileImageSourcePath", fileImageSourcePath, RegistryValueKind.String);
            reg.SetValue("isEnabledFlickrImageSource", isEnabledFlickrImageSource, RegistryValueKind.DWord);
            reg.SetValue("flickrImageSourceTags", flickrImageSourceTags, RegistryValueKind.String);
            reg.SetValue("isFlickrImageSourceTagAndLogic", isFlickrImageSourceTagAndLogic, RegistryValueKind.DWord);
            reg.SetValue("flickrImageSourceUserName", flickrImageSourceUserName, RegistryValueKind.String);
            reg.SetValue("flickrImageSourceText", flickrImageSourceText, RegistryValueKind.String);
            reg.SetValue("backgroundstyle", Enum.GetName(typeof(BackGroundStyle), backgroundstyle), RegistryValueKind.String);
            reg.SetValue("imagestyle", Enum.GetName(typeof(ImageStyle), imagestyle), RegistryValueKind.String);
            reg.SetValue("transitionin", Enum.GetName(typeof(TransitionStyle), transitionin), RegistryValueKind.String);
            reg.SetValue("transitionout", Enum.GetName(typeof(TransitionStyle), transitionout), RegistryValueKind.String);
            //reg.SetValue("theme", Enum.GetName(typeof(Theme), theme), RegistryValueKind.String);
            reg.SetValue("comicstyle", Enum.GetName(typeof(Comicstyle), Comicstyle), RegistryValueKind.String);
            reg.SetValue("isLoggingEnabled", isLoggingEnabled, RegistryValueKind.DWord);
            reg.SetValue("filesearchfilter", filesearchfilter, RegistryValueKind.String);
            reg.SetValue("enablefilesearchfilter", enablefilesearchfilter, RegistryValueKind.DWord);
            reg.Close();
        }
        public int XCount {
            get { return xCount; }
            set { xCount = value; }
        }
        public int LongInterval {
            get { return longInterval; }
            set { longInterval = value; }
        }
        public int ShortInterval {
            get { return shortInterval; }
            set { shortInterval = value; }
        }
        public bool IsEnabledFileImageSource {
            get { return isEnabledFileImageSource; }
            set { isEnabledFileImageSource = value; }
        }
        public string FileImageSourcePath {
            get { return fileImageSourcePath; }
            set { fileImageSourcePath = value; }
        }
        public bool IsEnabledFlickrImageSource {
            get { return isEnabledFlickrImageSource; }
            set { isEnabledFlickrImageSource = value; }
        }
        public string FlickrImageSourceTags {
            get { return flickrImageSourceTags; }
            set { flickrImageSourceTags = value; }
        }
        public bool IsFlickrImageSourceTagAndLogic {
            get { return isFlickrImageSourceTagAndLogic; }
            set { isFlickrImageSourceTagAndLogic = value; }
        }
        public string FlickrImageSourceUserName {
            get { return flickrImageSourceUserName; }
            set { flickrImageSourceUserName = value; }
        }
        public string FlickrImageSourceText {
            get { return flickrImageSourceText; }
            set { flickrImageSourceText = value; }
        }
		public Brush BackBrush{
			get{
    	        	switch (backgroundstyle) {
        	    	case BackGroundStyle.Black:
            			backbrush = Brushes.Black;
            			break;
            		case BackGroundStyle.White:
            			backbrush = Brushes.White;
            			break;
            		case BackGroundStyle.Random:
            			Random rndbrsh = new Random{};
            			if (rndbrsh.Next(1,2)==1) {backbrush = Brushes.White;}
            			break;          		
				}
				return backbrush;
			}
		}
		public Color BackColor{
			get{
				Color color = backgroundstyle == BackGroundStyle.Black ? Color.Black : Color.White;
				return color;
			}
		}
		//public Theme Theme {
        //    get { return theme; }
        //    set { theme = value; }
        //}
        public BackGroundStyle BackGroundStyle{
            get { return backgroundstyle; }
            set { backgroundstyle = value; }
        }
        public ImageStyle ImageStyle
        {
            get { return imagestyle; }
            set { imagestyle = value; }
        }
        
		public Comicstyle Comicstyle{
			get { return comicstyle;}
			set { comicstyle = value;}
		}
        public TransitionStyle TransitionIn {
            get { return transitionin; }
            set { transitionin = value; }
        }
        public TransitionStyle TransitionOut
        {
            get { return transitionout; }
            set { transitionout = value; }
        }

        public bool IsLoggingEnabled {
            get { return isLoggingEnabled; }
            set { isLoggingEnabled = value; }
        }
        public string FileSearchFilter
        {
            get { return filesearchfilter; }
            set { filesearchfilter = value; }
        }
        public bool Enablefilesearchfilter
        {
            get { return enablefilesearchfilter; }
            set { enablefilesearchfilter = value; }
        }
        public string[] FileNameSearchParms {
            get { 
                string[] results;
                if(enablefilesearchfilter){
                    results = filesearchfilter.Split(';');
                    }
                else
                   {
                    results = new string[1];
                    results.SetValue("*",0);                    
                    }
                return results;
            }
        }

        public readonly int  maxInterval = 10000;

        private int xCount = 9;
        private int longInterval = 5000;
        private int shortInterval = 1000;
        private bool isEnabledFileImageSource = false;
        private string fileImageSourcePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        private bool isEnabledFlickrImageSource = true;
        private string flickrImageSourceTags = "";
        private bool isFlickrImageSourceTagAndLogic = false;
        private string flickrImageSourceUserName = "";
        private string flickrImageSourceText = "";
        private BackGroundStyle backgroundstyle = BackGroundStyle.Black;
        private ImageStyle imagestyle = ImageStyle.Whole;
        private TransitionStyle transitionin = TransitionStyle.Fade;
        private TransitionStyle transitionout = TransitionStyle.Fade;
        //private Theme theme = Theme.Dark;
        private Comicstyle comicstyle = Comicstyle.CoversOnly;
        private bool isLoggingEnabled = false;
        private Brush backbrush;
        private string filesearchfilter = "*";
        private bool enablefilesearchfilter = false;



        private static readonly string KEY = "Software\\YetAnotherPhotoScreenSaverCE";  
    }

    //public enum Theme {
    //    Light,
    //    Dark,
    //    Random
    //} 
    public enum BackGroundStyle { 
        Black,
        White,
        Random
    }
    public enum ImageStyle { 
        Whole,
        CenterFill,
        Random
    }

}
