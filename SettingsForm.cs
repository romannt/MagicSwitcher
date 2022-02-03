using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MagicSwitcher
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            Text = Program.ProgramTitle() + " Settings";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The settings have been saved");
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            ActiveControl = edtOption1;
        }
    }
}
