using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GtaCasinoHackerPackGenerator
{
    public partial class FilePickerForm : Form
    {
        public FilePickerForm()
        {
            InitializeComponent();
        }

        private void openFilePickerBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Title = "Choose file to open.";
            fd.Filter = "BMP files|*.bmp";
            if(fd.ShowDialog() == DialogResult.OK)
            {
                Form1 form = new Form1((Bitmap)System.Drawing.Bitmap.FromFile(fd.FileName));
                Hide();
                form.ShowDialog();
                Close();
            }
        }
    }
}
