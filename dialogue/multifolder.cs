using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Org.Kuhn.Yapss.dialogue
{
    public partial class multifolder : Form
    {
        public multifolder()
        {
            InitializeComponent();
        }

        public multifolder(string foldersString)
        {
            InitializeComponent();
            string[] folders = foldersString.Split(';');
            folderslistbox.Items.AddRange(folders);
        }

        private void multifolder_Load(object sender, EventArgs e)
        {

        }

        private void addbutton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                if (System.IO.Directory.Exists(fbd.SelectedPath))
                {
                    folderslistbox.Items.Add(fbd.SelectedPath);
                }

            }
        }

        private void removebutton_Click(object sender, EventArgs e)
        {
            if (folderslistbox.SelectedItem != null)
            {
                folderslistbox.Items.Remove(folderslistbox.SelectedItem);
            }
        }
        public DialogResult Result ()
        { 
            return result;
        }
        public string FileImageSourcePath()
        {
            string[] values = new string[folderslistbox.Items.Count];
            folderslistbox.Items.CopyTo(values, 0);
            return string.Join(";", values);
        
        }
        DialogResult result = DialogResult.Cancel;

        private void okbutton_Click(object sender, EventArgs e)
        {
            result = DialogResult.OK;
            this.Close();
        }

        private void cancelbutton_Click(object sender, EventArgs e)
        {
            result = DialogResult.Cancel;
            this.Close();
        }
    }

}
