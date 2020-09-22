using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GtaCasinoHacker
{
    public partial class DiagnoseForm : Form
    {
        public DiagnoseForm(GtaReaderSession session)
        {
            InitializeComponent();
            this.session = session;

            LoadChallenge(0, 0, answerInfo11, answerView11);
            LoadChallenge(1, 0, answerInfo21, answerView21);
            LoadChallenge(0, 1, answerInfo12, answerView12);
            LoadChallenge(1, 1, answerInfo22, answerView22);
            LoadChallenge(0, 2, answerInfo13, answerView13);
            LoadChallenge(1, 2, answerInfo23, answerView23);
            LoadChallenge(0, 3, answerInfo14, answerView14);
            LoadChallenge(1, 3, answerInfo24, answerView24);
            SetImageOnView(session.matches[0].ReadDataAsBitmap(), matchView1);
            SetImageOnView(session.matches[1].ReadDataAsBitmap(), matchView2);
            SetImageOnView(session.matches[2].ReadDataAsBitmap(), matchView3);
            SetImageOnView(session.matches[3].ReadDataAsBitmap(), matchView4);
            SetImageOnView(session.screenshot.GetCloneTarget().ReadDataAsBitmap(), screenFingerprint);
            SetImageOnView(session.file.finger.ReadDataAsBitmap(), matchedFingerprint);

            //Make info text
            string infoText = $"Is 16:9:\n{session.screenshot.is169.ToString()}\n\nIs Windowed:\n{session.screenshot.isWindowed}\n\nImage Resized:\n{session.screenshot.imageResized.ToString()}\n\nFinger Packs Loaded:\n";
            foreach (var p in session.registeredFingers)
                infoText += $">{p.name} (modified {p.modified_utc.ToShortDateString()} {p.modified_utc.ToLongTimeString()})\n";
            infoLabel.Text = infoText;
        }

        private GtaReaderSession session;

        private void LoadChallenge(int left, int top, Label viewClick, PictureBox viewImg)
        {
            //Load
            GtaFingerprintChallenge challenge = null;
            foreach(var c in session.screenChallenges)
            {
                if (c.fingerX == left && c.fingerY == top)
                    challenge = c;
            }

            //Check if this was a match
            int matchIndex = session.matches.IndexOf(challenge);

            //Set
            if(matchIndex == -1)
            {
                viewClick.Text = "Not Correct";
            } else
            {
                viewClick.Text = "MATCH " + (matchIndex + 1);
                viewClick.BackColor = Color.LimeGreen;
            }
            SetImageOnView(challenge.ReadDataAsBitmap(), viewImg);
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
    }
}
