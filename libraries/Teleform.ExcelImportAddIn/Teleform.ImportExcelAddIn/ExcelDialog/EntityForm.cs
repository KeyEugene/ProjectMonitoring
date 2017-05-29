using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Web;
using Teleform.Reporting;
using ExcelDialog.Forms;
using Teleform.ProjectMonitoring;
using Microsoft.Office.Interop.Excel;
using Teleform.ImportExcelAddIn;

namespace ExcelDialog
{

    public partial class EntityForm : Form
    {
        public bool isClose = false;

        private List<ListItem> listItems;
        internal EntityDesigner entityDesigner;

        public EntityForm()
        {
            InitializeComponent();
        }
        public EntityForm(Excel.Application excelApplication)
        {
            InitializeComponent();

            FillForm();
        }

        private void FillForm()
        {
            entityDesigner = new EntityDesigner();

            listItems = entityDesigner.GetData(Globals.ThisAddIn.Application);

            if (entityDesigner.fieldType != FieldType.Constraint)
            {
                isClose = true;

                ShowForm(entityDesigner.fieldType);
            }
            else if (listBox != null)
            {
                SetListBoxItems(listItems);

                nameColumn.Text = string.Concat("Колонка : ", entityDesigner.NameColumn);

                CountObjectToLabel();
            }
            else
            {
                isClose = true;
                return;
            }
        }

        private void ShowForm(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.Date:
                    var formDate = new FormDate();
                    formDate.ShowDialogDate(entityDesigner);
                    break;
                case FieldType.Bool:
                    var formBool = new FormBool();
                    formBool.ShowDialogBool(entityDesigner);
                    break;
                case FieldType.Number:
                case FieldType.Money:
                    var formMoney = new FormMoney();
                    formMoney.MaxValue = entityDesigner.MaxValue;
                    formMoney.ShowDialogMoney(entityDesigner);
                    break;
                case FieldType.String:
                    var formString = new FormString();
                    formString.ShowDialogString(entityDesigner);
                    break;
                case FieldType.None:
                    break;
                case FieldType.Constraint:
                    break;
                default:
                    break;
            }
        }


        private void SetListBoxItems(List<ListItem> items)
        {
            listBox.Items.Clear();

            for (int i = 0; i < items.Count; i++)
            {
                listBox.Items.Add(items[i].Text);
            }
        }
        private void button_Ok_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedItem == null)
                return;

            var listItem = listItems.FirstOrDefault(x => x.Text == listBox.SelectedItem.ToString());

            entityDesigner.SetCellValue(listItem.Text + "|" + listItem.Value);
            CountObjectToLabel();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var list = listItems.Where(x => x.Text.ToLower().Contains(filterTextBox.Text.ToLower())).ToList();
            SetListBoxItems(list);
            CountObjectToLabel();
        }

        private void CountObjectToLabel()
        {
            CountObjects.Text = listBox.Items.Count + " объекта";
        }

        private void CreateNew_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var href = string.Concat("Dynamics/XDynamicCard.aspx?entity=",this.entityDesigner.constraintLink.RefTblID,"&regime=insert");
            var webQuery = Environment.GetEnvironmentVariable("officeaddinserver");

            href = webQuery.Replace("get.schema.aspx", href);
            System.Diagnostics.Process.Start(href);

            this.Close();
        }
    }
}
