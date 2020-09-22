using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GtaCasinoHacker.UI
{
    public partial class HelpDialog : Form
    {
        public HelpDialog(GtaReaderSession session)
        {
            InitializeComponent();
            this.session = session;
            btnDiag.Enabled = session != null;
        }

        private GtaReaderSession session;

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnDiag_Click(object sender, EventArgs e)
        {
            new DiagnoseForm(session).ShowDialog();
        }
    }
}
