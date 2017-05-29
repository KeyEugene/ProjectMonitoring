using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Excel = Microsoft.Office.Interop.Excel;
using Teleform.Reporting.WordExcelTemplateAddIns;


using Teleform.Reporting;

namespace Teleform.Office.DBSchemeWordAddIn
{

    public partial class SchemeForm : Form
    {
        //private Schema Schema;

        //private List<AddInEntity> addInEntities;

        private AddInSchema AddInSchema;

        private Excel.Application ExcelApplication;
        private dynamic TemplateSheet;
        private int EntitySelectedIndex;

        private bool TemplateSheetChanges;
        private bool EntityClear;

        // Адрес ячейки, хранящий идентификатор текущей сущности.
        private string EntityIDAddress = "$A$10";


        //public SchemeForm(Excel.Application excelApplication, Schema Schema)
        //{
        //    InitializeComponent();
        //    this.Schema = Schema;
        //    this.ExcelApplication = excelApplication;
        //    this.Load += SchemeForm_Load;

        //    if (!CheckTemplateSheet())
        //        CreateTemplateSheet();

        //    ExcelApplication.ActiveWorkbook.SheetChange += new Excel.WorkbookEvents_SheetChangeEventHandler(ActiveWorkbook_SheetChange);
        //    ExcelApplication.SheetChange += new Excel.AppEvents_SheetChangeEventHandler(ExcelApplication_SheetChange);

        //    EntitySelectedIndex = -1;
        //    TemplateSheetChanges = false;
        //    EntityClear = false;
        //}
        //public SchemeForm(Schema schema)
        //{
        //    InitializeComponent();
        //}

        //public SchemeForm(Excel.Application excelApplication, List<AddInEntity> addInEntities)
        //{
            
        //    InitializeComponent();
        //    this.addInEntities = addInEntities;
        //    this.ExcelApplication = excelApplication;
        //    this.Load += SchemeForm_Load;

        //    if (!CheckTemplateSheet())
        //        CreateTemplateSheet();

        //    ExcelApplication.ActiveWorkbook.SheetChange += new Excel.WorkbookEvents_SheetChangeEventHandler(ActiveWorkbook_SheetChange);
        //    ExcelApplication.SheetChange += new Excel.AppEvents_SheetChangeEventHandler(ExcelApplication_SheetChange);

        //    EntitySelectedIndex = -1;
        //    TemplateSheetChanges = false;
        //    EntityClear = false;
        //}



        public SchemeForm(Excel.Application excelApplication, AddInSchema addInSchema)
        {

            InitializeComponent();
            this.AddInSchema = addInSchema;
            this.ExcelApplication = excelApplication;
            this.Load += SchemeForm_Load;

            if (!CheckTemplateSheet())
                CreateTemplateSheet();

            ExcelApplication.ActiveWorkbook.SheetChange += new Excel.WorkbookEvents_SheetChangeEventHandler(ActiveWorkbook_SheetChange);
            ExcelApplication.SheetChange += new Excel.AppEvents_SheetChangeEventHandler(ExcelApplication_SheetChange);

            EntitySelectedIndex = -1;
            TemplateSheetChanges = false;
            EntityClear = false;
        }



        void SchemeForm_Load(object sender, EventArgs e)
        {
            //EntityBox.DisplayMember = "Name";
            // EntityBox.ValueMember = "Attributes";
            EntityBox.DisplayMember = "alias";
            EntityBox.ValueMember = "ID";
            EntityBox.DataSource = new List<AddInEntity>(AddInSchema.Entities.OrderBy(o => o.Alias));
            //EntityBox.DataSource = new List<Entity>(Schema.Entities.OrderBy(o => o.Name));
        }


        /// <summary>
        /// Изменение текущей сущности.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntityBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EntityBox.SelectedIndex == EntitySelectedIndex) return;

            var hasAttr = false;
            for (int i = 1; i <= 50; i++)
            {
                if (ExcelApplication.ActiveSheet.Cells(1, i).Value != null)
                { hasAttr = true; break; }
            }

            if (hasAttr && EntityBox.SelectedIndex != EntitySelectedIndex)
            {
                var result = MessageBox.Show(this, "В случае изменения типа объекта, для которого готовится отчёт, необходимо удалить все аттрибуты. Продолжить?", "Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    ClearEntityAttributes();
                }
                else if (result == DialogResult.No)
                    EntityBox.SelectedIndex = EntitySelectedIndex;
            }
            AttributeListBox.DataSource = new List<AddInAttribute>(AddInSchema.Entities.First(o => o.ID == EntityBox.SelectedValue).Attributes.OrderBy(o => o.Alias));                
            //AttributeListBox.DataSource = new List<Teleform.Reporting.Attribute>(Schema.Entities.First(o => o.ID == EntityBox.SelectedValue).Attributes.OrderBy(o => o.Name));
            AttributeListBox.ValueMember = "ID";
            AttributeListBox.DisplayMember = "alias";

            EntitySelectedIndex = EntityBox.SelectedIndex;
        }


        /// <summary>
        /// Изменение любой ячейки на любом листе книги.
        /// </summary>
        /// <param name="Sh"></param>
        /// <param name="Target"></param>
        void ExcelApplication_SheetChange(object Sh, Excel.Range Target)
        {
            if (this.EntityClear) return;

            if (((dynamic)Sh).Name.Contains("templatesheet"))
            {
                if (this.TemplateSheetChanges)
                {
                    this.TemplateSheetChanges = false;
                    return;
                }
            }
            else
            {
                // Если данные нулевые или пробелы - очищаем привязанный идентификатор и алиас.
                if (string.IsNullOrWhiteSpace(Convert.ToString(Target.Value)))
                {
                    this.TemplateSheetChanges = true;
                    this.TemplateSheet.Range(Target.Address).Clear();
                    this.TemplateSheet.Cells(Target.Row + 1, Target.Column).Value = null;
                }
                else
                {
                    // Если данные введены не в первой строке - очищаем ячейку.
                    if (Target.Row != 1)
                        Target.Value = null;
                    else
                    {
                        this.TemplateSheetChanges = true;
                        this.TemplateSheet.Cells(Target.Row + 1, Target.Column).Value = Target.Value;
                    }
                }
            }
        }


        void ActiveWorkbook_SheetChange(object Sh, Excel.Range range)
        {
            //MessageBox.Show(range.Address);
        }


        /// <summary>
        /// Проверка книги на наличие скрытого системного листа.
        /// </summary>
        private bool CheckTemplateSheet()
        {
            var sheets = this.ExcelApplication.ActiveWorkbook.Sheets;
            for (int i = 1; i <= sheets.Count; i++)
            {
                if (sheets[i].Name == "templatesheet")
                {
                    this.TemplateSheet = sheets[i];
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Добавление скрытого системного листа в книгу.
        /// </summary>
        private void CreateTemplateSheet()
        {
            this.TemplateSheet = this.ExcelApplication.ActiveWorkbook.Sheets.Add();
            this.TemplateSheet.Visible = Excel.XlSheetVisibility.xlSheetVeryHidden;
            this.TemplateSheet.Name = "templatesheet";
        }


        /// <summary>
        /// Доабвление атрибута сущности в документ.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertAttribute(object sender, EventArgs e)
        {
            if (!CheckTemplateSheet())
                MessageBox.Show("Нет сислиста!");



            var r = ExcelApplication.ActiveCell;
            var addressCoords = r.Address.Split('$').Where(x => x != "").ToArray();
            var insertCellAddress = addressCoords[0].Trim(new char[] { ':', ' ' }) + "1";
            var templateAliasCellAddress = addressCoords[0].Trim(new char[] { ':', ' ' }) + "2";


            //var attribute = (Teleform.Reporting.Attribute)AttributeListBox.SelectedItem;
            //var entity = Schema.Entities.First(o => o.ID == EntityBox.SelectedValue);
            //var attr = entity.Attributes.First(o => o.ID == AttributeListBox.SelectedValue);
            //var format = Schema.GetType(attr.Type.Name).GetAdmissableFormats().First(o => o.ID == FormatListBox.SelectedValue);

            var attribute = (AddInAttribute)AttributeListBox.SelectedItem;

            var uidCreator = new UniqueIDCreator();

            var entity = AddInSchema.Entities.First(o => o.ID == EntityBox.SelectedValue);
            
            var attr = entity.Attributes.First(o => o.ID == AttributeListBox.SelectedValue);
            
            var format = AddInSchema.Types.First(o => o.Name == attr.Type.Name).GetAdmissableFormats().First(o => o.ID == FormatListBox.SelectedValue);

            var uid = uidCreator.Aggregate(entity, attr, format);

            ExcelApplication.ActiveSheet.Range(insertCellAddress).Value = attribute.Alias;
            this.TemplateSheet.Range(templateAliasCellAddress).Value = attribute.Alias;
            this.TemplateSheet.Range(insertCellAddress).Value = uid.ToString();
        }


        private void ClearEntityAttributes()
        {
            this.EntityClear = true;
#warning Удаляется информация с активной, на момент работы, страницы.
            // Удаление всех записей из заголовка.
            ExcelApplication.ActiveSheet.Range("1:1").Clear();
            // Удаление всех идентификаторов из системной страницы.
            this.TemplateSheet.Range("1:1").Clear();
            // Удаление алиасов из системной таблицы.
            this.TemplateSheet.Range("2:2").Clear();
            this.EntityClear = false;
        }


        private void AttributeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
           // var attribute = (Teleform.Reporting.Attribute)AttributeListBox.SelectedItem;
            //var type = Schema.GetType(typeName);

            var attribute = (AddInAttribute)AttributeListBox.SelectedItem;            
            var type = AddInSchema.Types.First(t => t.Name == attribute.Type.Name);

            FormatListBox.DataSource = type.GetAdmissableFormats();
            FormatListBox.DisplayMember = "Name";
            FormatListBox.ValueMember = "ID";
        }


        private void FormatListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var format = (Format)FormatListBox.SelectedItem;

            if (!string.IsNullOrEmpty(format.Description))
                DescriptionLabel.Text = format.Description;
            else DescriptionLabel.Text = "Отсутствует описание текущего формата.";

            if (!string.IsNullOrEmpty(format.Example))
                ExampleLabel.Text = format.Example;
            else ExampleLabel.Text = string.Empty;
        }


        private void SchemeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            var address = ExcelApplication.ActiveCell.Address.Split(new char[] { '$', ':', ' ' }).Where(x => x != "").ToArray();
            
            var row = ExcelApplication.ActiveCell.Row;
            var col = ExcelApplication.ActiveCell.Column;

            ExcelApplication.ActiveSheet.Cells(row, col).Value = null;
        }
    }
}
