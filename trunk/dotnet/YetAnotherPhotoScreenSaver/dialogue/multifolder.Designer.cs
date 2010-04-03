namespace Org.Kuhn.Yapss.dialogue
{
    partial class multifolder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.folderslistbox = new System.Windows.Forms.ListBox();
            this.removebutton = new System.Windows.Forms.Button();
            this.addbutton = new System.Windows.Forms.Button();
            this.cancelbutton = new System.Windows.Forms.Button();
            this.okbutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // folderslistbox
            // 
            this.folderslistbox.FormattingEnabled = true;
            this.folderslistbox.Location = new System.Drawing.Point(12, 12);
            this.folderslistbox.Name = "folderslistbox";
            this.folderslistbox.Size = new System.Drawing.Size(213, 147);
            this.folderslistbox.TabIndex = 5;
            // 
            // removebutton
            // 
            this.removebutton.Location = new System.Drawing.Point(231, 50);
            this.removebutton.Name = "removebutton";
            this.removebutton.Size = new System.Drawing.Size(110, 23);
            this.removebutton.TabIndex = 4;
            this.removebutton.Text = "Remove";
            this.removebutton.UseVisualStyleBackColor = true;
            this.removebutton.Click += new System.EventHandler(this.removebutton_Click);
            // 
            // addbutton
            // 
            this.addbutton.Location = new System.Drawing.Point(231, 21);
            this.addbutton.Name = "addbutton";
            this.addbutton.Size = new System.Drawing.Size(110, 23);
            this.addbutton.TabIndex = 3;
            this.addbutton.Text = "Add";
            this.addbutton.UseVisualStyleBackColor = true;
            this.addbutton.Click += new System.EventHandler(this.addbutton_Click);
            // 
            // cancelbutton
            // 
            this.cancelbutton.Location = new System.Drawing.Point(231, 107);
            this.cancelbutton.Name = "cancelbutton";
            this.cancelbutton.Size = new System.Drawing.Size(110, 23);
            this.cancelbutton.TabIndex = 6;
            this.cancelbutton.Text = "Cancel";
            this.cancelbutton.UseVisualStyleBackColor = true;
            this.cancelbutton.Click += new System.EventHandler(this.cancelbutton_Click);
            // 
            // okbutton
            // 
            this.okbutton.Location = new System.Drawing.Point(231, 136);
            this.okbutton.Name = "okbutton";
            this.okbutton.Size = new System.Drawing.Size(110, 23);
            this.okbutton.TabIndex = 7;
            this.okbutton.Text = "OK";
            this.okbutton.UseVisualStyleBackColor = true;
            this.okbutton.Click += new System.EventHandler(this.okbutton_Click);
            // 
            // multifolder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 179);
            this.Controls.Add(this.okbutton);
            this.Controls.Add(this.cancelbutton);
            this.Controls.Add(this.folderslistbox);
            this.Controls.Add(this.removebutton);
            this.Controls.Add(this.addbutton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "multifolder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Folders";
            this.Load += new System.EventHandler(this.multifolder_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox folderslistbox;
        private System.Windows.Forms.Button removebutton;
        private System.Windows.Forms.Button addbutton;
        private System.Windows.Forms.Button cancelbutton;
        private System.Windows.Forms.Button okbutton;
    }
}