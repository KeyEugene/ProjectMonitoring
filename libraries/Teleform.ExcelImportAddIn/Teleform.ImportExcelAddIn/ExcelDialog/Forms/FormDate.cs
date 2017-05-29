using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelDialog.Forms
{
    public partial class FormDate : Form
    {
        public FormDate()
        {
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            entityDesigner.SetCellValue(dateTimePicker1.Value.ToShortDateString());
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        internal void ShowDialogDate(EntityDesigner entityDesigner)
        {
            this.entityDesigner = entityDesigner;
            NameColumn.Text = entityDesigner.NameColumn;
            this.Show();
        }

        public EntityDesigner entityDesigner { get; set; }
    }
}
