using GtaCasinoHacker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GtaCasinoHackerPackGenerator
{
    public partial class Form1 : Form
    {
        public Form1(Bitmap screenshot)
        {
            InitializeComponent();
            LoadScreenshot(screenshot);
        }

        private GtaScreenshot screenshot;
        private GtaFingerprintTarget finger;
        private List<ViewChallengeData> challenges;

        public void LoadScreenshot(Bitmap bmp)
        {
            //Open GTAScreenshot
            screenshot = new GtaScreenshot(bmp, false);

            //Load target
            finger = screenshot.GetCloneTarget();
            SetImageOnView(finger.ReadDataAsBitmap(), fingerprintChallenge);

            //Load challenges
            challenges = new List<ViewChallengeData>();
            LoadChallenge(0, 0, answerPicker11, answerView11);
            LoadChallenge(1, 0, answerPicker21, answerView21);
            LoadChallenge(0, 1, answerPicker12, answerView12);
            LoadChallenge(1, 1, answerPicker22, answerView22);
            LoadChallenge(0, 2, answerPicker13, answerView13);
            LoadChallenge(1, 2, answerPicker23, answerView23);
            LoadChallenge(0, 3, answerPicker14, answerView14);
            LoadChallenge(1, 3, answerPicker24, answerView24);
        }

        private void LoadChallenge(int left, int top, CheckBox viewClick, PictureBox viewImg)
        {
            //Load
            GtaFingerprintChallenge challenge = screenshot.GetComponent(left, top);

            //Set
            SetImageOnView(challenge.ReadDataAsBitmap(), viewImg);
            var data = new ViewChallengeData
            {
                top = top,
                left = left,
                challenge = challenge,
                viewImg = viewImg,
                viewCheck = viewClick
            };
            viewClick.Tag = data;
            challenges.Add(data);
        }

        private void SetImageOnView(Bitmap image, PictureBox view)
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

        private void answerPicker_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Find all challenges
            List<GtaFingerprintChallenge> fileChalleneges = new List<GtaFingerprintChallenge>();
            foreach(var d in challenges)
            {
                if (d.viewCheck.Checked)
                    fileChalleneges.Add(d.challenge);
            }

            //Create GTA finger package file
            GtaFingerPackFile file = new GtaFingerPackFile
            {
                modified_utc = DateTime.UtcNow,
                finger = finger,
                screen_height = (ushort)screenshot.height,
                screen_width = (ushort)screenshot.width,
                challenges = fileChalleneges.ToArray(),
                name = fingerNameBox.Text
            };

            //Open file
            using (FileStream fs = new FileStream(fingerNameBox.Text + ".gtafpak", FileMode.Create))
                file.SaveFile(fs);

            //Confirm
            MessageBox.Show("Finger file was saved successfully.", "Saved", MessageBoxButtons.OK);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FilePickerForm f = new FilePickerForm();
            Hide();
            f.ShowDialog();
            Close();
        }
    }
}
