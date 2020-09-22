using GtaCasinoHacker.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace GtaCasinoHacker
{
    public partial class Form1 : Form
    {
        public Form1(Process gtaProcess)
        {
            InitializeComponent();
            this.gtaProcess = gtaProcess;
            gtaProcess.Exited += GtaProcess_Exited;
            cells = new FingerprintViewCell[2, 4]
            {
                {
                    new FingerprintViewCell(cellPicture11, cellFrame11),
                    new FingerprintViewCell(cellPicture12, cellFrame12),
                    new FingerprintViewCell(cellPicture13, cellFrame13),
                    new FingerprintViewCell(cellPicture14, cellFrame14)
                },
                {
                    new FingerprintViewCell(cellPicture21, cellFrame21),
                    new FingerprintViewCell(cellPicture22, cellFrame22),
                    new FingerprintViewCell(cellPicture23, cellFrame23),
                    new FingerprintViewCell(cellPicture24, cellFrame24)
                }
            };

            //Hide busy dialog
            waitingPanel.Visible = false;

            //Complain if we haven't loaded any fingers
            if(Program.registeredFingers.Count == 0)
                MessageBox.Show("No GTA finger packs were found. Make sure they exist in the folder this program is in.", "No Fingers", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //Bind keybinds
            BindKeybind(0, System.Windows.Input.ModifierKeys.Control, Key.S);
            BindKeybind(1, System.Windows.Input.ModifierKeys.Control, Key.X);
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeys fsModifiers, int vk);

        public void BindKeybind(int id, ModifierKeys modifiers, Key key)
        {
            if (!RegisterHotKey(this.Handle, id, modifiers, KeyInterop.VirtualKeyFromKey(key)))
                throw new Exception("Error registering!");
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312 && !isBusy)
            {
                int id = m.WParam.ToInt32();
                if(id == 0)
                {
                    ScanPressed();
                } else if (id == 1)
                {
                    HidePressed();
                }
            }
            base.WndProc(ref m);
        }

        private void GtaProcess_Exited(object sender, EventArgs e)
        {
            Hide();
            new ProcessFinder().ShowDialog();
            Close();
        }

        private Process gtaProcess;
        private GtaReaderSession session;
        private FingerprintViewCell[,] cells;
        private bool hasAspectWarned;
        private bool isBusy;

        public void ScanPressed()
        {
            //Clear and set to busy so we don't confuse the user
            SetBusy(true);
            Update();

            //Capture screenshot
            Bitmap screenshot = ScreenshotCaptureTool.AltCaptureScreenshot(gtaProcess);

            //Open session
            session = new GtaReaderSession(Program.registeredFingers, screenshot, !isFullscreenCheck.Checked);

            //Warn if this is not a 16:9 screen
            if (!session.screenshot.is169 && !hasAspectWarned)
            {
                MessageBox.Show("Your game is not running in a 16:9 aspect ratio (1280x720, 1920x1080). This program will likely not work. This dialog won't appear again until you restart this program.", "Bad Screen Aspect Ratio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                hasAspectWarned = true;
            }

            //Update each cell
            foreach (var challenge in session.screenChallenges)
            {
                //Get cell
                var cell = cells[challenge.fingerX, challenge.fingerY];

                //Check if this was a match
                int matchIndex = session.matches.IndexOf(challenge);

                //Set
                cell.UpdateActive(matchIndex != -1);
                SetImageOnView(challenge.ReadDataAsBitmap(), cell.picture);
            }

            //Show all
            SetBusy(false);
        }
        
        public void HidePressed()
        {
            if (WindowState == FormWindowState.Normal)
                WindowState = FormWindowState.Minimized;
            else
                WindowState = FormWindowState.Normal;
        }

        public void SetBusy(bool busy)
        {
            //Set
            this.isBusy = busy;
            
            //Set busy dialog
            waitingPanel.Visible = busy;

            //Clear each of the frames so we don't confuse the user
            foreach (var f in cells)
            {
                f.panel.Visible = !busy;
            }

            //Set buttons
            btnScan.Enabled = !busy;
            btnHide.Enabled = !busy;
            btnMore.Enabled = !busy;
        }

        public static void SetImageOnView(Bitmap image, PictureBox view)
        {
            //Resize
            Bitmap result = new Bitmap(view.Width, view.Height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(image, 0, 0, view.Width, view.Height);
            }

            //Apply
            view.Image = result;
        }

        class FingerprintViewCell
        {
            public PictureBox picture;
            public Panel panel;

            public FingerprintViewCell(PictureBox picture, Panel panel)
            {
                this.picture = picture;
                this.panel = panel;
            }

            public void UpdateActive(bool active)
            {
                if (active)
                    panel.BackColor = Color.LimeGreen;
                else
                    panel.BackColor = Color.Gray;
            }
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            ScanPressed();
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            HidePressed();
        }

        private void btnMore_Click(object sender, EventArgs e)
        {
            new HelpDialog(session).ShowDialog();
        }
    }
}
