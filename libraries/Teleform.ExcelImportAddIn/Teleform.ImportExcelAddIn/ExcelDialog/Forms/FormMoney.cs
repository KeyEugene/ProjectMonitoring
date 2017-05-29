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
    public partial class FormMoney : Form
    {
        internal long MaxValue { get; set; }
        private EntityDesigner entityDesigner { get; set; }

        public FormMoney()
        {
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            entityDesigner.SetCellValue(numeric.Value.ToString());
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        internal void ShowDialogMoney(EntityDesigner entityDesigner)
        {
            this.Show();
            this.entityDesigner = entityDesigner;

            NameColumn.Text = entityDesigner.NameColumn;
            labelMaxValue.Text = "Максимальное значение : " + entityDesigner.MaxValue.ToString();
            numeric.Maximum = entityDesigner.MaxValue;
        }

    }
}
