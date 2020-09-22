namespace GtaCasinoHackerPackGenerator
{
    partial class FilePickerForm
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
            this.openFilePickerBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // openFilePickerBtn
            // 
            this.openFilePickerBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.openFilePickerBtn.Location = new System.Drawing.Point(12, 55);
            this.openFilePickerBtn.Name = "openFilePickerBtn";
            this.openFilePickerBtn.Size = new System.Drawing.Size(356, 23);
            this.openFilePickerBtn.TabIndex = 0;
            this.openFilePickerBtn.Text = "Open File Picker";
            this.openFilePickerBtn.UseVisualStyleBackColor = true;
            this.openFilePickerBtn.Click += new System.EventHandler(this.openFilePickerBtn_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(356, 39);
            this.label1.TabIndex = 1;
            this.label1.Text = "Open an in-game screenshot to begin.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FilePickerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 90);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.openFilePickerBtn);
            this.Name = "FilePickerForm";
            this.Text = "FilePickerForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button openFilePickerBtn;
        private System.Windows.Forms.Label label1;
    }
}