using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Teleform.Office.DBSchemeWordAddIn.XmlWebService;

using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools.Word;
using System.Runtime.InteropServices;

using System.Collections;
using System.Xml.Linq;

using Teleform.Reporting.WordExcelTemplateAddIns;


namespace Teleform.Office.DBSchemeWordAddIn
{
    using Reporting;

    public partial class SchemeForm : Form
    {
        private Word.Document document;

        private Document extendedDocument;

        private bool HasAttribute
        {
            get { return extendedDocument.Controls.Count > 0; }
        }

        static int i = 0;

        private void InsertAttribute(object sender, EventArgs e)
        {
#if f
            var o = AttributeListBox.SelectedValue as DataRowView;
            SelectedAttribute = new Attribute
            {
                Table = o[0].ToString(),
                Name = o[2].ToString(),
                Property = o[3].ToString().GetHashString()
                //FPath = o[3].ToString().GetHashString()
            };
             Word.Range currentRange = Globals.ThisAddIn.Application.Selection.Range;

            var attribute = SelectedAttribute;

            try
            {
                var placeholder = extendedDocument.Controls.AddPlainTextContentControl(currentRange, attribute.Name);

                placeholder.PlaceholderText = attribute.Name;
                placeholder.Tag = /*attribute.Table + @"\" + */attribute.Property;
                placeholder.LockContents = true;
            }
            catch (COMException ex)
            {
                MessageBox.Show(ex.Message);
            }
#else
            var attribute = (AddInAttribute)AttributeListBox.SelectedItem;
            Word.Range currentRange = Globals.ThisAddIn.Application.Selection.Range;

            try
            {
                var placeholder = extendedDocument.Controls.AddPlainTextContentControl(currentRange, (i++).ToString());

                placeholder.PlaceholderText = attribute.Alias;

                var entity = addInSchema.Entities.First(o => o.ID == EntityBox.SelectedValue);
                var attr = entity.Attributes.First(o => o.ID == AttributeListBox.SelectedValue);
                var type = addInSchema.Types.First(x => x.Name == attr.Type.Name);
                var format = type.GetAdmissableFormats().First(o => o.ID == FormatListBox.SelectedValue);
                //var format = Schema.GetType(attr.Type.Name).GetAdmissableFormats().First(o => o.ID == FormatListBox.SelectedValue);

                var idCreator = new UniqueIDCreator();
                placeholder.Tag = idCreator.Aggregate(entity, attr, format).ToString();

                placeholder.LockContents = true;
            }
            catch (COMException)
            {
                //MessageBox.Show("Не удаётся добавить выбранный аттрибут.", "Уведомление", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                MessageBox.Show(this, "Выберите позицию для вставки аттрибута.", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
#endif

        }

        private int entityBoxIndex;

        private Schema Schema { get; set; }

        private AddInSchema addInSchema { get; set; }

        private List<AddInEntity> Entities { get; set; }

        private List<Teleform.Reporting.Type> Types { get; set; }



        public SchemeForm(Schema schema)
        {
            InitializeComponent();

            document = Globals.ThisAddIn.Application.ActiveDocument;
            extendedDocument = Globals.Factory.GetVstoObject(document);

            Schema = schema;

            Load += new EventHandler(SchemeForm_Load);
        }

        public SchemeForm(AddInSchema schema)
        {
            InitializeComponent();

            document = Globals.ThisAddIn.Application.ActiveDocument;
            extendedDocument = Globals.Factory.GetVstoObject(document);

            addInSchema = schema;

            Load += new EventHandler(SchemeForm_Load);
        }

        public SchemeForm(List<AddInEntity> entities, List<Teleform.Reporting.Type> types)
        {
            InitializeComponent();

            document = Globals.ThisAddIn.Application.ActiveDocument;
            extendedDocument = Globals.Factory.GetVstoObject(document);

            this.Entities = entities;
            this.Types = types;

            Load += new EventHandler(SchemeForm_Load);
        }

        void SchemeForm_Load(object sender, EventArgs e)
        {
            EntityBox.DisplayMember = "Alias";
            // EntityBox.ValueMember = "Attributes";
            EntityBox.ValueMember = "ID";
            //EntityBox.DataSource = new List<Entity>(Schema.Entities.OrderBy(o => o.Name));
            EntityBox.DataSource = addInSchema.Entities;
        }

        private void BaseTableBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var bt = EntityBox.SelectedValue.ToString();

            if (HasAttribute && EntityBox.SelectedIndex != entityBoxIndex)
            {
                var result = MessageBox.Show(this, "В случае изменения типа объекта, для которого готовится отчёт, необходимо удалить все аттрибуты. Продолжить?", "Предупреждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    object control;

                    while (extendedDocument.Controls.Count > 0)
                    {
                        control = extendedDocument.Controls[0];
                        if (control is PlainTextContentControl)
                            extendedDocument.Controls.Remove(control);
                    }
                }
                else if (result == DialogResult.No)
                    EntityBox.SelectedIndex = entityBoxIndex;
            }
            entityBoxIndex = EntityBox.SelectedIndex;

            AttributeListBox.DataSource = new List<AddInAttribute>(addInSchema.Entities.First(o => o.ID == EntityBox.SelectedValue).Attributes.OrderBy(o => o.Alias));
            AttributeListBox.ValueMember = "ID";
            AttributeListBox.DisplayMember = "Alias";
        }

        private void AttributeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var attribute = (AddInAttribute)AttributeListBox.SelectedItem;
            var typeName = attribute.Type.Name;

            //var type = Schema.GetType(typeName);
            var type = addInSchema.Types.First(x => x.Name == typeName);

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
    }
}
