#define Alexj
#define Viktor


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teleform.Reporting;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Drawing;
using Phoenix.Web.UI.Dialogs;

namespace Teleform.ProjectMonitoring.Templates
{
    public class TableTemplateDesigner : GeneralTemplateDesigner, IPostBackEventHandler
    {
        public bool IsNotShowThis { get; set; }

        public event EventHandler CloseButtonClick;

        public string EntityID
        {
            get
            {
                var entID = ViewState["Eid"] == null ? null : ViewState["Eid"].ToString();
                return entID;
            }
            set
            {
                ViewState["Eid"] = value;
            }
        }

        public bool ShowSaveAsButton
        {
            get
            {
                return ViewState["ShowSaveAsButton"] == null ? false : (bool)ViewState["ShowSaveAsButton"];
            }
            set
            {
                ViewState["ShowSaveAsButton"] = value;
            }
        }

        public bool ShowSaveButton
        {
            get
            {
                return ViewState["ShowSaveButton"] == null ? false : (bool)ViewState["ShowSaveButton"];
            }
            set
            {
                ViewState["ShowSaveButton"] = value;
            }
        }

        public bool ShowCloseButton
        {
            get
            {
                return ViewState["ShowCloseButton"] == null ? false : (bool)ViewState["ShowCloseButton"];
            }
            set
            {
                ViewState["ShowCloseButton"] = value;
            }
        }

        public bool ShowCreateNewTemplateButton
        {
            get { return ViewState["newTemplate"] == null ? false : (bool)ViewState["newTemplate"]; }
            set { ViewState["newTemplate"] = value; }
        }

        public override bool IsFileBased
        {
            get { return false; }
        }

        private Table FieldTable;

        protected Designer designer;

        private List<FieldBox> FieldBoxList;

        public System.Web.UI.WebControls.CheckBox isTemplateDefault { get; set; }

        private int SelectedIndex
        {
            get
            {
                return ViewState["SelectedIndex"] == null ? -1 : (int)ViewState["SelectedIndex"];
            }

            set
            {
                ViewState["SelectedIndex"] = value;
            }
        }

        private TableItemStyle selectedRowStyle;

#warning сделать публичным???
        private TableItemStyle SelectedRowStyle
        {
            get
            {
                if (selectedRowStyle == null)
                    selectedRowStyle = new TableItemStyle { ForeColor = Color.Red };
                return selectedRowStyle;
            }
        }

        public Template template
        {
            get { return ViewState["_TemplateDesigner"] as Template; }
            set
            {
                ViewState["_TemplateDesigner"] = value;
            }
        }

        protected Template Template
        {
            get
            {
                try
                {
                    if (template == null)
                    {
                        if (!string.IsNullOrEmpty(TemplateID))
                        {
#if Viktor
                            template = Storage.Select<Template>(TemplateID);


#else
                            var t = Storage.Select<Template>(TemplateID);
                            template = t.Clone();
#endif
                        }
                        else
                        {
                            template = CreateTemplate();
                        }
                    }

                    return template;
                }
                catch
                {
                    throw new Exception("ошибка пришла из TableTemplateDesigner - в шаблоне есть атрибут, а в схеме нет");
                }
            }
        }


        protected override Reporting.Template GetTemplate()
        {
            if (FieldBoxList.Count < 1)
                throw new Exception("Шаблон должен содержать хотя бы одно поле.");

            //DuplicateAliasesChecker();

            Template.Name = TemplateNameBox.Text;
            Template.TemplateByDefault = isTemplateDefault.Checked;

            SaveFieldBoxList_To_Template();

            // проверка на то, что, если выбрана колонка списка была выбрана и списковая аггрегация
            var listAttributes = Template.Fields.Where(o => o.Attribute.IsListAttribute);
            var temp = listAttributes.Where(o => !string.IsNullOrEmpty(o.ListAttributeAggregation.ColumnName) && string.IsNullOrEmpty(o.ListAttributeAggregation.AggregateLexem));

            if (temp.Count() > 0)
            {
                var message = "В строках ";
                foreach (var field in temp)
                    message += string.Concat(field.Order + 1, ",");

                message = message.Remove(message.Length - 1);
                message += " выбрано значение в колонке списка, но не выбрано значение списковой аггрегации.";
                throw new Exception(message);
            }

            return Template;
        }

        public void RaisePostBackEvent(string argument)
        {
            if (argument.StartsWith("s"))
                SelectedIndex = int.Parse(argument.Substring(1));

#warning костыль
            foreach (TableRow row in FieldTable.Rows)
                row.ForeColor = Color.Black;

            FieldTable.Rows[SelectedIndex + 1].ApplyStyle(SelectedRowStyle);
        }

        protected override void CreateChildControls()
        {
            if (IsNotShowThis)
                return;

            if (string.IsNullOrEmpty(EntityID) && string.IsNullOrEmpty(TemplateID))
                return;
            //       throw new ArgumentNullException("При создании табличного шаблона должен быть задан либо EntityID, либо TemplateID.");

            this.Controls.Clear();

            this.Controls.Add(ViewPage());

        }

        private Table ViewPage()
        {
            var table = new Table();
            TableRow row;
            TableCell cell;

            #region label and Textbox Name Template

            row = new TableRow();
            cell = new TableCell();
            cell.Controls.Add(new Literal { Text = "<br />" });

            var label = new Label { Text = "Имя шаблона" };
            cell.Controls.Add(label);

            TemplateNameBox = new TextBox { ID = "NameBox" };
            cell.Controls.Add(TemplateNameBox);

            if (!string.IsNullOrEmpty(TemplateID) && Template != null)
                TemplateNameBox.Text = Template.Name;

            #endregion

            row.Cells.Add(cell);

            #region Create button's

            var cell2 = new TableCell();
            cell2.Controls.Add(new Literal { Text = "<br />" });
            //cell2.VerticalAlign = VerticalAlign.Top;
            var IncludeButton = new Button() { ID = "includeButton", Text = "+" };
            IncludeButton.Click += new EventHandler(includeButton_Click);
            cell2.Controls.Add(IncludeButton);

            var excludeButton = new Button() { Text = "-" };
            excludeButton.Click += new EventHandler(excludeButton_Click);
            cell2.Controls.Add(excludeButton);

            var upButton = new Button() { Text = "\u2191" };
            upButton.Click += new EventHandler(upButton_Click);
            cell2.Controls.Add(upButton);

            var downButton = new Button() { Text = "\u2193" };
            downButton.Click += new EventHandler(downButton_Click);
            cell2.Controls.Add(downButton);
            cell2.Controls.Add(new Literal { Text = "&nbsp&nbsp&nbsp" });

            var rejectFilterButton = new Button { Text = "Сброс фильтров" };
            rejectFilterButton.Click += new EventHandler(rejectFilterButton_Click);
            cell2.Controls.Add(rejectFilterButton);
            cell2.Controls.Add(new Literal { Text = "&nbsp&nbsp&nbsp" });

            isTemplateDefault = new System.Web.UI.WebControls.CheckBox { ID = "CheckBoxTemplateDefault", Text = "Сделать шаблоном по умолчанию ? " };
            isTemplateDefault.Checked = Template.TemplateByDefault;
            cell2.Controls.Add(isTemplateDefault);
            cell2.Controls.Add(new Literal { Text = "&nbsp&nbsp&nbsp" });

            if (ShowSaveAsButton)
            {
                var saveAsButton = new Button { ID = "SaveAsButton", Text = "Сохранить как" };
                saveAsButton.Click += new EventHandler(saveAsButton_Click);
                cell2.Controls.Add(saveAsButton);
            }

            if (ShowSaveButton)
            {
                var saveButton = new Button { ID = "SaveButton", Text = "Сохранить" };
                saveButton.Click += new EventHandler(saveButton_Click);
                cell2.Controls.Add(saveButton);
            }

            if (ShowCloseButton)
            {
                var closeButton = new Button { ID = "CloseButton", Text = "Закрыть", BackColor = Color.FromArgb(205, 205, 205) };
                closeButton.Click += new EventHandler(closeButton_Click);
                cell2.Controls.Add(closeButton);
            }

            cell2.Controls.Add(new Literal { Text = "&nbsp&nbsp&nbsp" });

            if (ShowCreateNewTemplateButton)
            {
                var newButton = new Button { ID = "CreateTemplate", Text = "Новый", BackColor = Color.FromArgb(218, 226, 245) };
                newButton.OnClientClick = "return confirm(' Создать новый шаблон ? Несохраненные данные в текущем шаблоне будут потеряны.');";
                newButton.Click += new EventHandler(createNewTemplate_Click);
                cell2.Controls.Add(newButton);
            }

            #endregion

            row.Cells.Add(cell2);
            table.Rows.Add(row);

            row = new TableRow();

            #region Designer

            cell = new TableCell();
            cell.VerticalAlign = VerticalAlign.Top;

            if (string.IsNullOrEmpty(EntityID))
                EntityID = (Template as Template).Entity.ID.ToString();

            designer = new Designer() { userID = this.userID };
            InitializationDesigner();

            #endregion

            cell.Controls.Add(designer);

            #region Build fields

            cell2 = new TableCell();
            cell2.VerticalAlign = VerticalAlign.Top;
            cell2.Controls.Add(new Literal { Text = "<br /><br />" });

            FieldTable = new Table { ID = "FieldTable" };
            var headerRow = CreateHeaderRow();
            FieldTable.Rows.Add(headerRow);
            CreateFieldTableRows();

            #endregion

            cell2.Controls.Add(FieldTable);

            row.Cells.Add(cell);
            row.Cells.Add(cell2);

            table.Rows.Add(row);

            return table;
        }


        void createNewTemplate_Click(object sender, EventArgs e)
        {
            if (EntityID != null)
            {
                TemplateID = null;
                template = null;
                this.DataBind();
            }
            else
                throw new Exception(" EntityID не выбран.");
        }

        private TableHeaderRow CreateHeaderRow()
        {
            var headerRow = new TableHeaderRow();
            TableHeaderCell th;

            th = new TableHeaderCell { Text = "№" };
            headerRow.Cells.Add(th);

            th = new TableHeaderCell { Text = "Вкл." };
            headerRow.Cells.Add(th);

            th = new TableHeaderCell { Text = "Атрибут" };
            headerRow.Cells.Add(th);

            th = new TableHeaderCell { Text = "Псевдоним" };
            headerRow.Cells.Add(th);

            th = new TableHeaderCell { Text = "Формат" };
            headerRow.Cells.Add(th);

            th = new TableHeaderCell { Text = "Фильтр" };
            headerRow.Cells.Add(th);

            th = new TableHeaderCell { Text = "Агрегация" };
            headerRow.Cells.Add(th);

            th = new TableHeaderCell { Text = "Колонка списка" };
            headerRow.Cells.Add(th);

            th = new TableHeaderCell { Text = "Списковая агрегация" };
            headerRow.Cells.Add(th);

            return headerRow;
        }


        /// <summary>
        /// Вызываем, чтобы перестроить только ряды таблицы и не делать лишний postBack
        /// </summary>
        private void ReCreateFieldTableRows()
        {
            for (int i = FieldTable.Rows.Count - 1; i > 0; i--)
                FieldTable.Rows.Remove(FieldTable.Rows[i]);

            DuplicateAliasesChecker();

            SaveFieldBoxList_To_Template();
            CreateFieldTableRows();
        }

        /// <summary>
        /// проверка на то, чтобы не было атрибутов с одинаковыми alias
        /// </summary>
        private void DuplicateAliasesChecker()
        {
            var isRepeatID = FieldBoxList.GroupBy(c => c.Field.Attribute.ID).Where(grp => grp.Count() > 1);

            if (isRepeatID.Count() > 0)
            {
                var message = string.Empty;

                foreach (var group in isRepeatID)
                {
                    var isRepeatAlias = FieldBoxList.Where(o => o.Field.Attribute.ID == group.Key).GroupBy(c => c.AliasBox.Text).Where(grp => grp.Count() > 1);
                    if (isRepeatAlias.Count() > 0)
                    {
                        foreach (var gr in isRepeatAlias)
                            message += string.Concat(gr.Key, ",");
                    }
                }

                if (!string.IsNullOrEmpty(message))
                {
                    message = string.Concat("В шаблоне встречаются атрибуты с одинаковыми псевдонимами: ", message);
                    message = message.Remove(message.Length - 1);
                    throw new Exception(message);
                }
            }
        }

        private void SaveFieldBoxList_To_Template()
        {
            foreach (var field in Template.Fields.OrderBy(o => o.Order))
            //            foreach (var field in Template.Fields.OrderBy(o => o.Order).Where(f => !f.IsDenied))
            {
                var i = field.Order;
#warning порядок неверен
                var fieldBox = FieldBoxList[i];

                var type = field.Attribute.Type;
                if (field.Attribute.IsListAttribute)
                {
                    field.ListAttributeAggregation.ColumnName = FieldBoxList[i].RelatedColumnsList.SelectedValue;
                    field.ListAttributeAggregation.AggregateLexem = FieldBoxList[i].ListAggregationList.SelectedValue;

                    type = field.Attribute.GetAttributeByColumnName(FieldBoxList[i].RelatedColumnsList.SelectedValue).Type;
                }

                field.Format = type.GetAdmissableFormats().First(f => f.ID.ToString() == FieldBoxList[i].FormatList.SelectedValue);

                field.PredicateInfo = FieldBoxList[i].Predicate.PredicateInfo;
                field.Predicate = FieldBoxList[i].Predicate.TechPredicate;

                field.Name = FieldBoxList[i].AliasBox.Text.Trim();
                field.Aggregation = FieldBoxList[i].AggregationList.SelectedValue;
                field.IsVisible = FieldBoxList[i].VisibleBox.Checked;
            }
        }

        /// <summary>
        /// Создает все ряды таблицы, кроме заголовка
        /// </summary>
        private void CreateFieldTableRows()
        {
            //GetSelectedItem();

            FieldBoxList = new List<FieldBox>();

            foreach (var field in Template.Fields.OrderBy(x => x.Order))           
            {
                var fieldBox = new FieldBox(field);

                if (fieldBox.RelatedColumnsList != null)
                    fieldBox.RelatedColumnsList.SelectedIndexChanged += new EventHandler(RelatedColumnsList_SelectedIndexChanged);

                if (fieldBox.ViewUserPredicateBox != null)
                    fieldBox.Predicate.FilterApplied += new EventHandler(Predicate_FilterApplied);

                var fieldRow = fieldBox.CreateRow();
        
                if (field.Order == SelectedIndex)
                {
                    fieldBox.checkBox.Checked = true;
                    fieldRow.ApplyStyle(SelectedRowStyle);
                }

                if (!field.IsForbidden)
                    FieldTable.Rows.Add(fieldRow);
                
                FieldBoxList.Add(fieldBox);

            }

        }

        void Predicate_FilterApplied(object sender, EventArgs e)
        {
            Template.Fields.Clear();

            for (int i = 0; i < FieldBoxList.Count; i++)
            {
                Template.Fields.Add(FieldBoxList[i].SaveToField());
            }

            FieldTable = new Table();
            DataBind();
            //CreateFieldTableRows();
        }

        void RelatedColumnsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var control = sender as DropDownList;
            var controlID = control.ID;

            Template.Fields.Clear();

            foreach (var fieldBox in FieldBoxList)
            {
                if (fieldBox.RelatedColumnsList != null && fieldBox.RelatedColumnsList.ID == controlID)
                {
                    fieldBox.Field.Predicate = string.Empty;
                    fieldBox.Field.PredicateInfo = string.Empty;
                    fieldBox.Field.Name = fieldBox.AliasBox.Text;
                }
                //fieldBox.Field.CrossTableRoleID = Int32.Parse(fieldBox.CrossTableRoleList.SelectedValue);
                Template.Fields.Add(fieldBox.Field);
            }
            DataBind();
        }

        public override void DataBind()
        {
            CreateChildControls();
            //base.DataBind();
        }

        void excludeButton_Click(object sender, EventArgs e)
        {
            GetSelectedItem();

            if (SelectedIndex == -1) return;

            var field = this.Template.Fields[SelectedIndex];
            this.Template.Fields.Remove(field);

            FieldBoxList.Remove(FieldBoxList[SelectedIndex]);

#if Alexj
            if (SelectedIndex >= FieldBoxList.Count)
                SelectedIndex = -1;
#endif

            ReCreateFieldTableRows();
        }

        void upButton_Click(object sender, EventArgs e)
        {
            GetSelectedItem();

            if (SelectedIndex == -1 || SelectedIndex == 0) return;

            this.Template.Fields.Permute(SelectedIndex, SelectedIndex - 1);

            var fieldBox = FieldBoxList[SelectedIndex];
            FieldBoxList[SelectedIndex] = FieldBoxList[SelectedIndex - 1];
            FieldBoxList[SelectedIndex - 1] = fieldBox;

            SelectedIndex--;

            ReCreateFieldTableRows();
        }

        void downButton_Click(object sender, EventArgs e)
        {
            GetSelectedItem();
            if (SelectedIndex == -1 || SelectedIndex == FieldTable.Rows.Count - 2) return;

            this.Template.Fields.Permute(SelectedIndex, SelectedIndex + 1);

            var fieldBox = FieldBoxList[SelectedIndex];
            FieldBoxList[SelectedIndex] = FieldBoxList[SelectedIndex + 1];
            FieldBoxList[SelectedIndex + 1] = fieldBox;

            SelectedIndex++;

            ReCreateFieldTableRows();
        }

        void includeButton_Click(object sender, EventArgs e)
        {
            GetSelectedItem();
            if (designer.AttributeListBox.SelectedItem != null)
            {
                var items = designer.AttributeListBox.Items.OfType<ListItem>().Where(o => o.Selected).Select(o => o.Value);

                var test = Template.Entity.Attributes.Where(o => items.Contains(o.ID.ToString()));
                var fields = Template.Entity.Attributes.Where(o => items.Contains(o.ID.ToString())).Select(a => new Teleform.Reporting.TemplateField(a));

                // проверка на наличие повоторяющихся alias
                foreach (var field in fields)
                {
                    var repeated = FieldBoxList.Where(o => o.Field.Attribute.ID == field.Attribute.ID && o.AliasBox.Text == field.Name).Count();
                    if (repeated != 0)
                        throw new Exception(string.Format("Добавление невозможно, в списке уже есть атрибут {0} с псевдонимом {1}", field.Attribute.Name, field.Name));
                }

                if (SelectedIndex == -1)
                {
                    var maxOrder = Template.Fields.Count - 1;
                    Template.Fields.AddRange(fields);
                    var fieldBoxes = Template.Fields.Where(o => o.Order > maxOrder).Select(a => new FieldBox(a));
                    FieldBoxList.AddRange(fieldBoxes);
                }
                else
                {
                    Template.Fields.InsertRange(SelectedIndex + 1, fields);
                    var fieldBoxes = Template.Fields.Where(o => o.Order > SelectedIndex && o.Order <= SelectedIndex + fields.Count()).Select(a => new FieldBox(a));
                    FieldBoxList.InsertRange(SelectedIndex + 1, fieldBoxes);
                }

                ReCreateFieldTableRows();
            }
        }

        void closeButton_Click(object sender, EventArgs e)
        {
            if (CloseButtonClick != null)
                CloseButtonClick(this, e);
        }

        void saveButton_Click(object sender, EventArgs e)
        {
            this.Save();

            if (TemplateID != null)
                Storage.CleareTypeByEntityID(typeof(Template), template.Entity.ID.ToString());

            Storage.ClearBusinessContents();
        }

        void saveAsButton_Click(object sender, EventArgs e)
        {
            this.Save(true);

            if (TemplateID != null)
                Storage.CleareTypeByEntityID(typeof(Template), template.Entity.ID.ToString());
            //Storage.ClearInstanceCache(typeof(Template), TemplateID);

            Storage.ClearBusinessContents();
        }

        void rejectFilterButton_Click(object sender, EventArgs e)
        {
            foreach (var fieldBox in FieldBoxList)
                fieldBox.RejectFilters();

            ReCreateFieldTableRows();
        }

        private int GetSelectedItem()
        {
            SelectedIndex = -1;

            foreach (TableRow row in FieldTable.Rows)
            {
                if (row is TableHeaderRow)
                    continue;

                var lbl = row.Cells[0].Controls[1] as Label;
                var checkBox = row.Cells[0].Controls[0] as System.Web.UI.WebControls.CheckBox;
                if (checkBox.Checked)
                {
                    SelectedIndex = Convert.ToInt32(lbl.Text) - 1;
                    return SelectedIndex;
                }
            }
            return -1;
        }

        protected virtual Template CreateTemplate()
        {
            var entity = Storage.Select<Entity>(EntityID);
            var content = new byte[0];
            return new Template(string.Empty, entity, "TableBased", content);
        }

        protected virtual void InitializationDesigner()
        {

            designer.ID = "Designer";
            designer.EntityID = EntityID;
        }

    }
}