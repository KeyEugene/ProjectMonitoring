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
    public partial class FormString : Form
    {
        public FormString()
        {
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            entityDesigner.SetCellValue(textBox.Text);
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        internal void ShowDialogString(EntityDesigner entityDesigner)
        {
            this.entityDesigner = entityDesigner;
            NameColumn.Text = entityDesigner.NameColumn;
            textBox.MaxLength = (int)entityDesigner.MaxValue;
            label1.Text = "Максимальная длина  " + entityDesigner.MaxValue.ToString() + " символов.";
            this.Show();
        }

        public EntityDesigner entityDesigner { get; set; }
    }
}
