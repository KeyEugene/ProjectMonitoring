#define Alexj



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Teleform.Reporting;
using Teleform.ProjectMonitoring.Templates;
using System.Web.UI;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using Teleform.ProjectMonitoring.HttpApplication;

namespace Teleform.ProjectMonitoring.admin
{
    using System.Web.UI.WebControls;
    public class PredicateBuilder : CompositeControl, IPostBackEventHandler
    {

        public CheckBox read = new CheckBox { ID = "read" };
#if Alexj
        public CheckBox create = new CheckBox { ID = "create", Visible = false, Checked = false };
#else
        public CheckBox create = new CheckBox { ID = "create" };
#endif
        public CheckBox update = new CheckBox { ID = "update" };
        public CheckBox delete = new CheckBox { ID = "delete" };
        public TextBox comment = new TextBox { ID = "comment" };

        private Entity entity { get; set; }

        public string EntityID
        {
            get { return ViewState["Eid"] == null ? null : ViewState["Eid"].ToString(); }
            set { ViewState["Eid"] = value; }
        }

        public Template template
        {
            get { return ViewState["_Template"] as Template; }
            set
            {
                ViewState["_Template"] = value;
            }
        }

        private Template Template
        {
            get
            {
                try
                {

                    if (template == null)
                    {
                        entity = Storage.Select<Entity>(EntityID);
                        var content = new byte[0];
                        template = new Template(string.Empty, entity, "TableBased", content);
                    }

                    return template;
                }
                catch
                {
                    return null;
                }
            }
        }

        private Designer Designer { get; set; }

        private Table FieldTable;

        private List<FieldBox> FieldBoxList;

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

        public string selectedID
        {
            get { return (ViewState["_SelectedID"] == null) ? string.Empty : (string)ViewState["_SelectedID"]; }
            set { ViewState["_SelectedID"] = value; }
        }

        private GridView gv;

        private IEnumerable<Teleform.Reporting.TemplateField> AddField()
        {
            var items = Designer.AttributeListBox.Items.OfType<ListItem>().Where(x => x.Selected).Select(o => o.Value);
            return Template.Entity.Attributes.Where(x => items.Contains(x.ID.ToString())).Select(a => new Teleform.Reporting.TemplateField(a)).ToArray();
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



        protected override void CreateChildControls()
        {
            if (string.IsNullOrEmpty(EntityID))
                return;

            this.Controls.Clear();
            this.Controls.Add(ViewPage());

        }

        public DataView GetDataView()
        {
            return new DataView(FillfilterResult(), Template.GetFilterExpressionByAttrAlias(), "", DataViewRowState.CurrentRows);
        }

     
        private void showButton_Click(object sender, EventArgs e)
        {

           var GV = this.Parent.FindControl("GV");

            if (template.Fields.Count > 0)
            {
                template.Fields.Clear();
                for (int i = 0; i < FieldBoxList.Count; i++)
                {
                    var field = FieldBoxList[i].SaveToField();
                    template.Fields.Add(field);
                }

                gv = new GridView();
                gv.ID = "showFilterResult";
                
                var dvSource = GetDataView();
                dvSource.Table.Columns.Remove("objID");
                foreach (var field in Template.Fields)
                {
                    if (!field.IsVisible)
                        dvSource.Table.Columns.Remove(field.Name);
                }
                gv.DataSource = dvSource;
                gv.DataBind();
                this.Controls.Add(gv);

                //this.Controls.Add(CreateDataTable(FillfilterResult()));


            }
        }

        private DataTable FillfilterResult()
        {
            using (SqlConnection conn = new SqlConnection(Global.ConnectionString))
            {
                var cmd = new SqlCommand(string.Format(@"SELECT [representationN] FROM [Model].BObjectMap('{0}') where title = 1", template.Entity.SystemName), conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                var titleAttrList = new List<string>();
                if (reader.HasRows)
                {
                    while (reader.Read())
                        titleAttrList.Add((string)reader[0]);
                }
                conn.Close();

                cmd = new SqlCommand("EXEC [report].[getBObjectData] @baseTable=@baseTable, @cyr= 1", conn);
                cmd.Parameters.AddRange(
                     new SqlParameter[]
                    {                                             
                        new SqlParameter { ParameterName = "baseTable", DbType = DbType.Int32, Value = Designer.EntityID},
                        
                    });

                var adapter = new SqlDataAdapter(cmd);
                var dtSource = new DataTable();

                adapter.Fill(dtSource);

                var excludeListAttr = template.Entity.Attributes.Where(a => a.IsListAttribute == true);
                foreach (var item in excludeListAttr)
                {
                    if (!titleAttrList.Contains(item.Name))
                        dtSource.Columns.Remove(item.Name);
                }

                var excludeAttr = Designer.AttributeListBox.Items;
                foreach (var item in excludeAttr)
                {
                    if (!titleAttrList.Contains(item.ToString()))
                        dtSource.Columns.Remove(item.ToString());
                }


                foreach (var item in titleAttrList)
                    dtSource.Columns[item.ToString()].SetOrdinal(0);

                return dtSource;

            }
        }

        private Table CreateDataTable(DataTable dtSource)
        {
            Dictionary<Teleform.Reporting.TemplateField, int> FieldIndices = new Dictionary<Teleform.Reporting.TemplateField, int>();
            var HeaderRow = new TableHeaderRow();
            var dvSource = new DataView(dtSource, "", "", DataViewRowState.CurrentRows);
            foreach (var field in template.Fields)
            {
                FieldIndices.Add(field, dvSource.Table.Columns.IndexOf(field.Name));
                var cell = new TableHeaderCell();
                cell.Attributes.Add("HeaderColumnAlias", field.Name);
                HeaderRow.Cells.Add(cell);
            }

            var table = new Table();
            table.Rows.Add(HeaderRow);

            IEnumerable<DataRowView> list;
            list = dvSource.OfType<DataRowView>();

            foreach (var data in list)
            {
                var row = new TableRow();

                foreach (var field in Template.Fields)
                {
                    var tBox = new TextBox();
                    tBox.Text = string.Format(field.Format.Provider, field.Format.FormatString, data.Row.ItemArray[FieldIndices[field]]);
                    var cell = new TableCell();
                    cell.Controls.Add(tBox);
                    row.Cells.Add(cell);
                }
                table.Rows.Add(row);
            }

            return table;
        }

        private Table ViewPage()
        {
            var table = new Table();
            var row = new TableRow();

            #region Build Designer
            var cell1 = new TableCell();
            cell1.VerticalAlign = VerticalAlign.Top;

            

            Designer = new Designer() { ID = "Designer", EntityID = EntityID };

            Designer.isFromAdministration = true;

            Designer.FilterAttributeIDList = Template.Fields.Select(a => a.Attribute.ID.ToString()).ToList();

            cell1.Controls.Add(Designer);
            #endregion


            #region Build button's
            var cell2 = new TableCell();
            cell2.VerticalAlign = VerticalAlign.Top;

            var IncludeButton = new Button() { ID = "includeButton", Text = "+" };
            IncludeButton.Click += new EventHandler(includeButton_Click);

            var excludeButton = new Button() { Text = "-" };
            excludeButton.Click += new EventHandler(excludeButton_Click);

            var showButton = new Button() { Text = "Показать" };
            showButton.Click += new EventHandler(showButton_Click);

            cell2.Controls.Add(IncludeButton);
            cell2.Controls.Add(new Literal() { Text = " " });
            cell2.Controls.Add(excludeButton);
            cell2.Controls.Add(new Literal() { Text = " " });
            cell2.Controls.Add(showButton);
            cell2.Controls.Add(new Literal() { Text = " " });


            #endregion

            #region Build checbox

            cell2.Controls.Add(new Literal { Text = "Чтение" });
            cell2.Controls.Add(read);
#if Alexj
            cell2.Controls.Add(new Literal { Text = "Создание", Visible = false });
            cell2.Controls.Add(create);
#else
            cell2.Controls.Add(new Literal { Text = "Создание" });
            cell2.Controls.Add(create);
#endif
            cell2.Controls.Add(new Literal { Text = "Изменение" });
            cell2.Controls.Add(update);

            cell2.Controls.Add(new Literal { Text = "Удаление" });
            cell2.Controls.Add(delete);

            comment.Attributes.Add("placeholder", "комментарий");
            cell2.Controls.Add(comment);



            #endregion


            #region Build fields
            FieldTable = new Table { ID = "FieldTable" };
            var headerRow = CreateHeaderRow();
            FieldTable.Rows.Add(headerRow);
            CreateFieldTableRows();

            cell2.VerticalAlign = VerticalAlign.Top;
            cell2.Controls.Add(FieldTable);

            #endregion


            row.Cells.Add(cell1);
            row.Cells.Add(cell2);
            table.Rows.Add(row);


            return table;
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

            th = new TableHeaderCell { Text = "Фильтр" };
            headerRow.Cells.Add(th);

            th = new TableHeaderCell { Text = "Колонка списка" };
            th.Attributes.Add("style", "display:none");
            headerRow.Cells.Add(th);

            th = new TableHeaderCell { Text = "Списковая агрегация" };
            th.Attributes.Add("style", "display:none");
            headerRow.Cells.Add(th);

            return headerRow;
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
            var s = sender as DropDownList;
            var z = s.ID;

            Template.Fields.Clear();

            for (int i = 0; i < FieldBoxList.Count; i++)
            {
                if (FieldBoxList[i].RelatedColumnsList != null && FieldBoxList[i].RelatedColumnsList.ID == z)
                {
                    FieldBoxList[i].Field.Predicate = string.Empty;
                    FieldBoxList[i].Field.PredicateInfo = string.Empty;

                    FieldBoxList[i] = new FieldBox(FieldBoxList[i].Field);
                }
                Template.Fields.Add(FieldBoxList[i].Field);
            }
            DataBind();
        }

        private void CreateFieldTableRows()
        {
            FieldBoxList = new List<FieldBox>();

            foreach (var field in Template.Fields.OrderBy(x => x.Order))
            {
                var fieldBox = new FieldBox(field);

                if (fieldBox.RelatedColumnsList != null)
                    fieldBox.RelatedColumnsList.SelectedIndexChanged += new EventHandler(RelatedColumnsList_SelectedIndexChanged);

                if (fieldBox.ViewUserPredicateBox != null)
                    fieldBox.Predicate.FilterApplied += new EventHandler(Predicate_FilterApplied);

                var fieldRow = fieldBox.CreateRowPredicateBuilder();

                if (field.Order == SelectedIndex)
                {
                    fieldBox.checkBox.Checked = true;
                    fieldRow.ApplyStyle(SelectedRowStyle);
                }

                FieldTable.Rows.Add(fieldRow);
                FieldBoxList.Add(fieldBox);
            }
        }


        void includeButton_Click(object sender, EventArgs e)
        {
            GetSelectedItem();

            if (Designer.AttributeListBox.SelectedItem == null)
                return;
            Template.Fields.AddRange(AddField());

            DataBind();
        }

        void excludeButton_Click(object sender, EventArgs e)
        {
            GetSelectedItem();

            if (SelectedIndex == -1)
                return;

            Template.Fields.Remove(Template.Fields[SelectedIndex]);

            DataBind();
        }



        public override void DataBind()
        {
            CreateChildControls();
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


       
    }
}