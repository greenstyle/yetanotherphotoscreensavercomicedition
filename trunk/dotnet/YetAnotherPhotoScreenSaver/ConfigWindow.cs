using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace Org.Kuhn.Yapss {
    public partial class ConfigWindow : Form {
        public ConfigWindow() {
            InitializeComponent();
        }

        private void CopyConfig() {
            config.XCount = (int)xCountNumUpDown.Value;
            switch (speedDropdown.SelectedIndex) {
                case 0:
                    config.LongInterval = 10000;
                    config.ShortInterval = 3000;
                    break;
                case 1:
                    config.LongInterval = 5000;
                    config.ShortInterval = 1000;
                    break;
                case 2:
                    config.LongInterval = 1;
                    config.ShortInterval = 1;
                    break;
            }
            config.BackGroundStyle = (BackGroundStyle)Enum.Parse(typeof(BackGroundStyle), bgColorDropdown.Text);
            config.ImageStyle = (ImageStyle)Enum.Parse(typeof(ImageStyle), ImageStyledropdown.Text);
            config.TransitionIn = (TransitionStyle)Enum.Parse(typeof(TransitionStyle), transitionindropdown.Text);
            config.TransitionOut = (TransitionStyle)Enum.Parse(typeof(TransitionStyle), transitionoutdropdown.Text);
            //config.Theme = (Theme)Enum.Parse(typeof(Theme), bgColorDropdown.Text);
            config.Comicstyle = (Comicstyle)Enum.Parse(typeof(Comicstyle), comicstyleDropDown.Text);
            config.IsEnabledFlickrImageSource = flickrCheckbox.Checked;
            config.FlickrImageSourceTags = tagsTextBox.Text;
            config.IsFlickrImageSourceTagAndLogic = tagLogicDropdown.SelectedIndex == 1;
            config.FlickrImageSourceUserName = userIdTextBox.Text;
            config.FlickrImageSourceText = textTextBox.Text;
            config.IsEnabledFileImageSource = diskCheckBox.Checked;
            config.FileImageSourcePath = pathTextBox.Text;
            config.IsLoggingEnabled = DebugCheckBox.Checked;
        }

        private void ConfigWindow_Load(object sender, EventArgs e) {
            xCountNumUpDown.Value = config.XCount;
            if (config.LongInterval == 10000) {
                speedDropdown.SelectedIndex = 0;
            }
            else if (config.LongInterval == 5000) {
                speedDropdown.SelectedIndex = 1;
            }
            else {
                speedDropdown.SelectedIndex = 2;
            }
            bgColorDropdown.Text = Enum.GetName(typeof(BackGroundStyle), config.BackGroundStyle);
            ImageStyledropdown.Text = Enum.GetName(typeof(ImageStyle), config.ImageStyle);
            comicstyleDropDown.Text = Enum.GetName(typeof(Comicstyle), config.Comicstyle);
            transitionindropdown.Text = Enum.GetName(typeof(TransitionStyle), config.TransitionIn);
            transitionoutdropdown.Text = Enum.GetName(typeof(TransitionStyle), config.TransitionOut);
            flickrCheckbox.Checked = flickrPanel.Enabled = config.IsEnabledFlickrImageSource;
            tagsTextBox.Text = config.FlickrImageSourceTags;
            tagLogicDropdown.SelectedIndex = config.IsFlickrImageSourceTagAndLogic ? 1 : 0;
            userIdTextBox.Text = config.FlickrImageSourceUserName;
            textTextBox.Text = config.FlickrImageSourceText;
            diskCheckBox.Checked = diskPanel.Enabled = config.IsEnabledFileImageSource;
            pathTextBox.Text = folderBrowserDialog.SelectedPath = config.FileImageSourcePath;
            DebugCheckBox.Checked = config.IsLoggingEnabled;
        }

        private void browseButton_Click(object sender, EventArgs e) {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                pathTextBox.Text = folderBrowserDialog.SelectedPath;
        }

        private void saveButton_Click(object sender, EventArgs e) {
            CopyConfig();
            config.Save();
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            Close();
        }

        private void previewButton_Click(object sender, EventArgs e) {
            CopyConfig();
            Program program = new Program(config);
            program.End += (obj, evt) => {
                Log.Instance.Write("Stopping screen saver preview");
                program.Stop();
            };
            program.Run();
        }

        private Config config = new Config();

        private void flickrCheckbox_CheckedChanged(object sender, EventArgs e) {
            flickrPanel.Enabled = flickrCheckbox.Checked;
        }

        private void diskCheckBox_CheckedChanged(object sender, EventArgs e) {
            diskPanel.Enabled = diskCheckBox.Checked;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            try {
                System.Diagnostics.Process.Start("http://code.google.com/p/yetanotherphotoscreensavercomicedition/");
            }
            catch (Exception ex) {
                MessageBox.Show("Error launching web browser.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Instance.Write("Failed launching web browser", ex);
            }
        }

        private void OpenLogButton_Click(object sender, EventArgs e)
        {
         ProcessStartInfo procStart = new ProcessStartInfo(Log.Instance.LogFilePath);
         procStart.UseShellExecute = true;
         Process proc = new Process();
         proc.StartInfo = procStart;
         proc.Start();
    
        }


    }
}
