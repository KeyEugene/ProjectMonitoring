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
    public partial class FormBool : Form
    {
        public FormBool()
        {
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (comboBoxBool.SelectedIndex != -1)
                entityDesigner.SetCellValue(comboBoxBool.SelectedItem.ToString());
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBoxBool_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBoxBool.SelectedIndex != -1)
                entityDesigner.SetCellValue(comboBoxBool.SelectedItem.ToString());
            this.Close();
        }

        internal void ShowDialogBool(EntityDesigner entityDesigner)
        {
            this.entityDesigner = entityDesigner;

            NameColumn.Text = entityDesigner.NameColumn;
            comboBoxBool.Items.Add("Да");
            comboBoxBool.Items.Add("Нет");

            this.Show();
        }

        public EntityDesigner entityDesigner { get; set; }
    }
}
