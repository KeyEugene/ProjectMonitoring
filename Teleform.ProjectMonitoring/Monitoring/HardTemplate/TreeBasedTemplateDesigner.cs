

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Teleform.ProjectMonitoring.HardTemplate;
using Teleform.Reporting;

namespace Teleform.ProjectMonitoring.Templates
{
    using System.Collections;
    using System.Data;
    using Teleform.ProjectMonitoring.HttpApplication;
    using Teleform.Reporting;
    using Teleform.Reporting.Reporting.Template;

    public class TreeBasedTemplateDesigner : GeneralTemplateDesigner
    {
        #region Field and Property
        public Template template
        {
            get { return ViewState["_TemplateDesigner"] as Template; }
            set
            {
                ViewState["_TemplateDesigner"] = value;
            }
        }
        public string EntityID { get; set; }
        public Template Template
        {
            get
            {
                try
                {

                    if (template == null)
                    {
                        if (!string.IsNullOrEmpty(TemplateID))
                        {

                            var t = Storage.Select<Template>(TemplateID);
                            template = t.Clone();                            

                        }
                        else
                        {
                            var entity = Storage.Select<Entity>(EntityID);
                            var content = new byte[0];
                            template = new Template(string.Empty, entity, "screenTree", content);
                        }
                    }

                    return template;
                }
                catch (Exception ex)
                {
                    throw new Exception("Не удалось прочитать шаблон.");
                }
            }
        }
        public string selectedID
        {
            get { return (ViewState["_SelectedID"] == null) ? string.Empty : (string)ViewState["_SelectedID"]; }
            set { ViewState["_SelectedID"] = value; }
        }
        public string selectedAggregateFunction
        {
            get { return (ViewState["AggrFun"] == null) ? string.Empty : (string)ViewState["AggrFun"]; }
            set { ViewState["AggrFun"] = value; }
        }
        public bool isDialogShow
        {
            get
            {
                return (ViewState["isDialog"] == null) ? false : (bool)ViewState["isDialog"];
            }
            set { ViewState["isDialog"] = value; }
        }
        public string buttonAttributID
        {
            get { return ViewState["ButtonAttributID"] == null ? string.Empty : (string)ViewState["ButtonAttributID"]; }
            set { ViewState["ButtonAttributID"] = value; }
        }
        public Hashtable SortHashtable
        {
            get { return ViewState["SortDictionary"] == null ? new Hashtable() : (Hashtable)ViewState["SortDictionary"]; }
            set { ViewState["SortDictionary"] = value; }
        }
        public Phoenix.Web.UI.Dialogs.Form dialog { get; private set; }
        public bool templateIsNew = false;
        private List<TreeCell> treeControlsList;

        private FieldBox fieldBox;
        private Designer designer;
        public PlaceHolder collectionRightControls { get; private set; }

        public Button LeftButton { get; private set; }
        public Button RightButton { get; private set; }
        public Button IncludeButton { get; private set; }
        public Button IncludeNewLevelButton { get; private set; }
        public Button RemoveCell { get; private set; }
        public Button SaveTemplateButton { get; private set; }
        public Button CloseDesignerButton { get; private set; }

        private DropDownList TreeTypeDDL { get; set; }



        #endregion

        public TreeBasedTemplateDesigner()
        {
            new TreeBasedTemplateDesigner(null);
        }

        public TreeBasedTemplateDesigner(int? templateID = null)
        {
            //TemplateID = templateID.ToString();
            //template = Template;
        }

        protected override void OnInit(EventArgs e)
        {
            bool visibleSaveCancel = this.Parent is View ? true : false;

            LeftButton = new Button { ID = "_leftBtn", Text = "←" };
            RightButton = new Button { ID = "_rightBtn", Text = "→" };
            IncludeButton = new Button { ID = "_IncludeBtn", Text = "+" };
            IncludeNewLevelButton = new Button { ID = "_newLevelBtn", Text = "Новый уровень" };
            RemoveCell = new Button { ID = "_removeBtn", Text = "-" }; RemoveCell.Style.Add("display", "none");
            TreeTypeDDL = new DropDownList { ID = "_treeType", AutoPostBack = true };
            GetDropDownListFromTreeType();


            SaveTemplateButton = new Button { ID = "_saveBtn", Text = "Сохранить", BackColor = Color.Lavender, Visible = visibleSaveCancel };
            CloseDesignerButton = new Button { ID = "_closeDesigner", Text = "Закрыть", BackColor = Color.Lavender, Visible = visibleSaveCancel };
            TemplateNameBox = new TextBox { ID = "_nameTemplate" };

            treeControlsList = new List<TreeCell>();

            IncludeButton.Click += new EventHandler(IncludeButton_Click);
            IncludeNewLevelButton.Click += new EventHandler(IncludeNewLevelButton_Click);
            LeftButton.Click += new EventHandler(LeftButton_Click);
            RightButton.Click += new EventHandler(RightButton_Click);
            RemoveCell.Click += new EventHandler(RemoveCellButton_Click);
            SaveTemplateButton.Click += new EventHandler(SaveTemplate_Click);
            CloseDesignerButton.Click += new EventHandler(CloseDesigner_Click);
            TreeTypeDDL.SelectedIndexChanged += TreeTypeDDL_SelectedIndexChanged;
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            GetSortDictionary();

            base.OnLoad(e);
        }

        #region BildControl

        protected override void CreateChildControls()
        {
            template = Template;
            this.Controls.Clear();

            designer = new Designer() { userID = this.userID }; collectionRightControls = new PlaceHolder();

            designer.ID = "_designer";

            if (buttonAttributID != string.Empty)
                CreateDialog();

            if (templateIsNew)
                TemplateNameBox.Text = Template.Name;

            if (!string.IsNullOrEmpty(TemplateID))
                TemplateNameBox.Text = Template.Name;

            TreeTypeDDL.SelectedIndex = ((int)template.TreeTypeEnum - 1);

            //if (template.Entity.IsHierarchic) // Оставляем возможность добавлять атрибут parentID (Doc_Doc/name)
            //{
            //    var fPath = template.Entity.SystemName + "_" + template.Entity.SystemName + "/name";
            //    designer.FilterAttributeIDList = Template.Fields.Where(x => x.Attribute.FPath != fPath).Select(x => x.Attribute.ID.ToString()).ToList();
            //}
            //else

            GetTreeTypeFromDropDownList();

            if (Template.TreeTypeEnum == EnumTreeType.Children)
                TreeTypeDDL_SelectedIndexChanged(null, EventArgs.Empty);
            else
                designer.FilterAttributeIDList = Template.Fields.Select(x => x.Attribute.ID.ToString()).ToList();

            designer.EntityID = Template.Entity.ID.ToString();


            CreateTreeCellControls();
            BildRightSide();

            this.Controls.Add(ConstructionOfTheMainView());

            template = Template;
        }

        private Table ConstructionOfTheMainView()
        {
            designer.AttributeBoxStyle = new System.Web.UI.WebControls.Style
            {
                BorderStyle = System.Web.UI.WebControls.BorderStyle.Outset,
                BorderColor = System.Drawing.Color.CornflowerBlue,
                BackColor = Color.LightGray
            };

            var mainTable = new Table();
            var rowHewader = new TableRow(); var mainRow = new TableRow();
            var cellHeaderLeft = new TableCell(); var cellHeaderRight = new TableCell();
            var mainCellLeft = new TableCell(); var mainCellRight = new TableCell();

            mainCellRight.VerticalAlign = VerticalAlign.Top;

            cellHeaderLeft.Controls.Add(new Literal { Text = "Название шаблона : " }); cellHeaderLeft.Controls.Add(TemplateNameBox);
            cellHeaderLeft.Controls.Add(IncludeButton); cellHeaderLeft.Controls.Add(IncludeNewLevelButton); cellHeaderLeft.Controls.Add(RemoveCell);
            //cellHeaderLeft.Attributes.Add("align", "right");
            cellHeaderRight.Controls.Add(LeftButton); cellHeaderRight.Controls.Add(RightButton);
            cellHeaderRight.Controls.Add(SaveTemplateButton); cellHeaderRight.Controls.Add(CloseDesignerButton);
            cellHeaderRight.Controls.Add(TreeTypeDDL);


            rowHewader.Controls.Add(cellHeaderLeft); rowHewader.Controls.Add(cellHeaderRight);
            mainCellLeft.Controls.Add(designer);

            if (isDialogShow)
                mainCellRight.Controls.Add(dialog);

            mainCellRight.Controls.Add(new Literal { Text = "<br /><br />" });
            mainCellRight.Controls.Add(collectionRightControls);
            mainRow.Controls.Add(mainCellLeft); mainRow.Controls.Add(mainCellRight);
            mainTable.Controls.Add(rowHewader);
            mainTable.Controls.Add(mainRow);

            return mainTable;
        }

        private System.Web.UI.Control GetDropDownListFromTreeType()
        {
            if (ViewState["treeType"] == null)
            {
                var dt = Global.GetDataTable("SELECT [objID], [name], [code] FROM [model].[R$TemplateTreeType]");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    TreeTypeDDL.Items.Add(new ListItem
                    {
                        Text = dt.Rows[i]["name"].ToString(),
                        Value = dt.Rows[i]["objID"].ToString()
                    });
                }
                ViewState.Add("treeType", TreeTypeDDL);
            }
            else
            {
                TreeTypeDDL = (DropDownList)ViewState["treeType"];
            }

            return TreeTypeDDL;
        }
        private EnumTreeType GetTreeTypeFromDropDownList()
        {
            if (TreeTypeDDL.SelectedValue == "1")
                return EnumTreeType.General;
            else if (TreeTypeDDL.SelectedValue == "2")
                return EnumTreeType.Branch;
            else if (TreeTypeDDL.SelectedValue == "3")
                return EnumTreeType.Children;

            return EnumTreeType.Undefined;
        }

        private void CreateTreeCellControls()
        {
            Color color;

            treeControlsList.Clear();

            foreach (var field in Template.Fields)
            {
                color = Color.Gainsboro;

                color = isSelectedAttributeListOrAggregation(field);

                var t = SortHashtable[field.Attribute.ID.ToString()];

                var treeCell = new TreeCell(field, color, t == null ? "" : t.ToString());
                treeCell.isCheck.Checked = field.Attribute.ID.ToString() == selectedID ? true : false;
                treeCell.labelFieldAlias.ForeColor = field.Attribute.ID.ToString() == selectedID ? Color.Red : Color.Black;
                treeCell.ID = field.Attribute.ID.ToString();

                treeCell.dialogButton.Click += new EventHandler(ButtonClick_ViewDialog);

                if (!field.IsForbidden)
                    treeControlsList.Add(treeCell);
            }
        }

        private Color isSelectedAttributeListOrAggregation(TemplateField field)
        {
            if (field.Aggregation != "")
            {
                return Color.LightBlue;
            }

            //if (field.AttributeListAggregation != null)
            //{
            //    if (field.AttributeListAggregation.AggregateLexem != "")
            //        return Color.LightBlue;
            //}
            return Color.Gainsboro;
        }

        private void BildRightSide()
        {
            int z;
            if (treeControlsList.Count == 0)
                return;
            else
                z = treeControlsList.Max(x => x.Field.Level);

            int level = 1;
            string s = "╚═══════";

            do
            {
                var tb = new Table();
                var row = new TableRow();

                var cell1 = new TableCell();
                cell1.Controls.Add(new Literal { Text = level == 1 ? "" : s });
                row.Controls.Add(cell1);

                foreach (TreeCell tree in GetCollectionForNLevel(level))
                {
                    var cell = new TableCell();

                    var cellBentween = new TableCell();

                    // var removeCell = new Button { ID = "removeButton" + tree.Field.Attribute.ID };
                    var name = "removeButton" + tree.Field.Attribute.ID;
                    var removeCell = new LinkButton { ID = name };
                    removeCell.Attributes.Add("name", name);
                    removeCell.Attributes.Add("class", "removeButton");
                    removeCell.Attributes.Add("onclick", "SelectedFieldHardTemplate('" + tree.labelFieldAlias.ID + "');");
                    removeCell.Click += removeCell_Click;

                    cellBentween.Text = "═══";

                    cell.Attributes.Add("class", "treeCellControl");
                    cell.Attributes.Add("onmouseover", "onmouseoverTreeCell('" + name + "');");
                    cell.Attributes.Add("onmouseout", "onmouseoutTreeCell('" + name + "');");

                    cell.Controls.Add(tree);
                    cell.Controls.Add(removeCell);

                    row.Controls.Add(cell);
                    row.Controls.Add(cellBentween);
                }
                tb.Controls.Add(row);

                collectionRightControls.Controls.Add(tb);

                s = s.Insert(0, "········");

            } while ((level++) <= z);
        }

        private IEnumerable<TreeCell> GetCollectionForNLevel(int level)
        {
            return treeControlsList.Where(x => x.Field.Level == level).OrderBy(x => x.Field.Order);
        }

        #endregion

        public override void DataBind()
        {
            CreateChildControls();

            //base.DataBind();
        }

        public override bool IsFileBased
        {
            get { return false; }
        }

        protected override Template GetTemplate()
        {
            Template.TreeTypeEnum = GetTreeTypeFromDropDownList();
            RidSameViewState();

            Template.Name = TemplateNameBox.Text;

            return Template;
        }

        private void GetSelecteItem()
        {
            GetSortDictionary();

            foreach (var item in treeControlsList)
            {
                if (item.isCheck.Checked)
                {
                    selectedID = item.Field.Attribute.ID.ToString();
                    return;
                }
            }
            selectedID = null;
        }

        private void GetSortDictionary()
        {
            var t = new Hashtable();

            foreach (var cell in treeControlsList)
            {
                if (cell.textBoxSort == null)
                    return;
                t.Add(cell.Field.Attribute.ID.ToString(), cell.textBoxSort.Text);
            }

            SortHashtable = t;
        }

        #region Event's

        void IncludeButton_Click(object sender, EventArgs e)
        {
            GetSelecteItem();

            if (designer.AttributeListBox.SelectedItem == null || selectedID == string.Empty)
                return;

            var field = Template.Fields.FirstOrDefault(x => x.Attribute.ID.ToString() == selectedID);
            var maxOrder = Template.Fields.Where(x => x.Level == field.Level).Max(x => x.Order);

            if (maxOrder == field.Order)
                Template.Fields.AddRange(AddField(), field.Level);
            else
                Template.Fields.InsertRange(field.Level, ++field.Order, AddField());

            DataBind();
        }

        private IEnumerable<TemplateField> AddField()
        {
            var items = designer.AttributeListBox.Items.OfType<ListItem>().Where(x => x.Selected).Select(o => o.Value);
            return Template.Entity.Attributes.Where(x => items.Contains(x.ID.ToString())).Select(a => new TemplateField(a)).ToArray();
        }

        void IncludeNewLevelButton_Click(object sender, EventArgs e)
        {
            if (designer.AttributeListBox.SelectedItem == null)
                return;
            int a;
            if (Template.Fields.Count == 0)
                a = 1;
            else
                a = Template.Fields.Max(x => x.Level) + 1;

            var newFields = AddField();

            Template.Fields.AddRange(AddField(), a);

            selectedID = newFields.LastOrDefault().Attribute.ID.ToString();

            DataBind();
        }

        void LeftButton_Click(object sender, EventArgs e)
        {
            GetSelecteItem();

            if (selectedID == string.Empty)
                return;

            var field = Template.Fields.FirstOrDefault(x => x.Attribute.ID.ToString() == selectedID);

            var z = field.Order;

            if (z == 0)
                return;

            Template.Fields.Permute(field.Level, z, z - 1);

            DataBind();
        }

        void RightButton_Click(object sender, EventArgs e)
        {
            GetSelecteItem();

            if (selectedID == string.Empty)
                return;

            var field = Template.Fields.FirstOrDefault(x => x.Attribute.ID.ToString() == selectedID);

            var z = field.Order;
            var max = Template.Fields.Where(x => x.Level == field.Level).Max(x => x.Order);

            if (z == max)
                return;

            Template.Fields.Permute(field.Level, z, z + 1);

            DataBind();
        }

        void RemoveCellButton_Click(object sender, EventArgs e)
        {
            GetSelecteItem();

            if (selectedID == string.Empty)
            {
                GetSortDictionary();
                return;
            }

            var field = Template.Fields.FirstOrDefault(x => x.Attribute.ID.ToString() == selectedID);

            var order = field.Order;
            if (order != 0)
                selectedID = Template.Fields.FirstOrDefault(x => x.Level == field.Level && x.Order == (order - 1)).Attribute.ID.ToString();
            else selectedID = null;

            Template.Fields.Remove(field, true);

            DataBind();
        }

        void ButtonClick_ViewDialog(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var id = buttonAttributID = button.ID.Remove(0, 7);

            DataBind();
        }

        private List<Phoenix.Web.UI.Dialogs.ButtonItem> CrateButtonsForDialog()
        {
            List<Phoenix.Web.UI.Dialogs.ButtonItem> listBtn = new List<Phoenix.Web.UI.Dialogs.ButtonItem>();

            Phoenix.Web.UI.Dialogs.ButtonItem btn = new Phoenix.Web.UI.Dialogs.ButtonItem();
            Phoenix.Web.UI.Dialogs.ButtonItem btn2 = new Phoenix.Web.UI.Dialogs.ButtonItem();

            btn.ControlID = "okBtn";
            btn.Text = "Ok";
            btn.Click += new EventHandler(ButtonClick_OkDialog);

            btn2.ControlID = "cancelBtn";
            btn2.Text = "Cancel";
            btn2.Click += new EventHandler(ButtonClick_CancelDialog);

            listBtn.Add(btn); listBtn.Add(btn2);

            return listBtn;
        }

        private void CreateDialog()
        {
            TemplateField field;
            if (fieldBox != null)
                field = fieldBox.Field;
            else
                field = Template.Fields.FirstOrDefault(x => x.Attribute.ID.ToString() == buttonAttributID);

            fieldBox = new FieldBox(field);

            if (fieldBox.RelatedColumnsList != null)
                fieldBox.RelatedColumnsList.SelectedIndexChanged += new EventHandler(RelatedColumnsList_SelectedIndexChanged);

            DialogTemplateField content = new DialogTemplateField(fieldBox.CreateDialogTable());

            dialog = new Phoenix.Web.UI.Dialogs.Form();
            dialog.ID = "dialog";
            dialog.ContentTemplate = content;
            dialog.Caption = string.Concat("Атрибут : ", field.Name);
            dialog.Buttons = CrateButtonsForDialog();
            dialog.ButtonsAlign = HorizontalAlign.Center;
            dialog.Show();
            isDialogShow = true;
        }

        void RelatedColumnsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataBind();

            //CreateDialog();
            // base.DataBind();
        }

        void ButtonClick_CancelDialog(object sender, EventArgs e)
        {
            dialog.Close();
            isDialogShow = false;
            buttonAttributID = string.Empty;
            DataBind();
        }

        void ButtonClick_OkDialog(object sender, EventArgs e)
        {
            UpdateFieldToTemplate();

            ButtonClick_CancelDialog(null, EventArgs.Empty);
        }

        private void UpdateFieldToTemplate()
        {
            if (fieldBox == null)
                return;

            for (int i = 0; i < Template.Fields.Count; i++)
            {
                if (Template.Fields[i].Attribute.ID.ToString() == buttonAttributID)
                {
                    var type = Template.Fields[i].Attribute.Type;

                    Template.Fields[i].Name = fieldBox.AliasBox.Text;
                    Template.Fields[i].Predicate = fieldBox.Predicate.TechPredicate;
                    Template.Fields[i].PredicateInfo = fieldBox.Predicate.PredicateInfo;
                    Template.Fields[i].Aggregation = fieldBox.AggregationList.SelectedValue;

                    if (Template.Fields[i].Attribute.IsListAttribute)
                    {
                        Template.Fields[i].ListAttributeAggregation.ColumnName = fieldBox.RelatedColumnsList.SelectedValue;
                        Template.Fields[i].ListAttributeAggregation.AggregateLexem = fieldBox.ListAggregationList.SelectedValue;

                        type = Template.Fields[i].Attribute.GetAttributeByColumnName(fieldBox.RelatedColumnsList.SelectedValue).Type;

                    }
                    Template.Fields[i].Format = type.GetAdmissableFormats().First(f => f.ID.ToString() == fieldBox.FormatList.SelectedValue);
                    return;
                }
            }
        }

        void SaveTemplate_Click(object sender, EventArgs e)
        {
            //проверяем имеются ли в уровне только поля с аггригацией
            CheckFieldsWithAggrIntoLevel(Template);

            this.Save();

            if (template.ID != null)
                Storage.ClearInstanceCache(typeof(Template), template.ID);

            CloseDesigner_Click(null, EventArgs.Empty);
        }

        public static void CheckFieldsWithAggrIntoLevel(Template currentTemplate)
        {
            var maxLvl = currentTemplate.Fields.Max(x => x.Level);

            for (int i = 1; i <= maxLvl; i++)
            {
                var z = currentTemplate.Fields.Where(x => x.Level == i && x.Aggregation != "").ToList();
                var countFild = currentTemplate.Fields.Count(o => o.Level == i);
                if (countFild == z.Count)
                    throw new InvalidOperationException("Сохранение невозможно, т.к. в одном уровне не может быть только поля с агрегацией.");
            }
        }

        public void CloseDesigner_Click(object sender, EventArgs e)
        {
            RidSameViewState();

            View b = null; MultiView c = null;

            b = Parent as View;

            if (b != null)
            {
                c = b.Parent as MultiView;
                c.ActiveViewIndex = 0;

                collectionRightControls = null;
                designer = null;
            }
        }

        void TreeTypeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            Template.TreeTypeEnum = GetTreeTypeFromDropDownList();

            if (Template.TreeTypeEnum == EnumTreeType.Children)
                designer.FilterAttributeIDList = GetFilterAttributeIDListForTypeFildren(GetHierarchicList());
            else  //if (Template.TreeType == )
                designer.FilterAttributeIDList = Template.Fields.Select(x => x.Attribute.ID.ToString()).ToList();

            if (sender != null)
                designer.DataBind();
        }

        private List<string> GetHierarchicList()
        {
            if (ViewState["HierarchicList"] == null)
            {
                var list = new List<string>();
                var dt = Global.GetDataTable("select pc.[parent], pc.[const],  isNULL(b.isHierarchic,0) isHierarchicParent from [model].[vo_ParentChildEntity] pc join model.BTables b on b.name=pc.parent where b.isHierarchic != 0");

                for (int i = 0; i < dt.Rows.Count; i++)
                    list.Add(dt.Rows[i]["const"].ToString());

                ViewState.Add("HierarchicList", list);
            }

            return (List<string>)ViewState["HierarchicList"];
        }

        /// <summary>
        /// Предоставляет список id(hash) attribute для фильтарции в Disaigner
        /// </summary>
        /// <param name="HierarchicList">Иерархический список</param>
        /// <returns>Filter attribute(id) for disaiger</returns>
        private List<string> GetFilterAttributeIDListForTypeFildren(List<string> HierarchicList)
        {
            var list = new List<string>();
            bool isFilter;

            for (int i = 0; i < template.Entity.Attributes.Count(); i++)
            {
                isFilter = false;
                for (int j = 0; j < HierarchicList.Count; j++)
                {
                    if (template.Entity.Attributes.ElementAt(i).FPath.Contains(HierarchicList[j]) || !template.Entity.Attributes.ElementAt(i).FPath.Contains("/"))
                        isFilter = true;
                }

                if (!isFilter)
                    list.Add(template.Entity.Attributes.ElementAt(i).ID.ToString());
            }

            if (Template.Fields.Count() != 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    for (int j = 0; j < Template.Fields.Count; j++)
                    {
                        if (list[i] == Template.Fields[j].Attribute.ID.ToString())
                        {
                            Template.Fields.Remove(Template.Fields[j]);
                            j--;
                        }
                    }
                }

                for (int i = 0; i < Template.Fields.Count; i++)
                {
                    list.Add(Template.Fields[i].Attribute.ID.ToString());
                }
            }


            return list;
        }

        #endregion

        void removeCell_Click(object sender, EventArgs e)
        {
            RemoveCellButton_Click(null, EventArgs.Empty);
        }

        /// <summary>
        /// Освобождаем не нужный ViewState при выходе из редактора
        /// </summary>
        private void RidSameViewState()
        {
            ViewState["treeType"] = null; //Очищаем ViewState от DropDownList with (Select * from [model].[R$TemplateTreeType]
            ViewState["HierarchicList"] = null; // Очищаем иерархический список (служит флагом о том первый раз мы в редакторе шаблоне(IsPostBack) или нет
        }

    }
}