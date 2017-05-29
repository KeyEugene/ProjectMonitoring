using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleSHA
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click( object sender, EventArgs e )
        {
            try
            {
                lblSha1Hash.Text = "SHA1: " + tbInput.Text.GetHashString();
            }
            catch ( Exception ex )
            {
                MessageBox.Show( ex.Message );
            }
        }
    }
}
