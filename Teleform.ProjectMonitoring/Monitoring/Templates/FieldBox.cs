
#define AlexJSSelected

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Teleform.ProjectMonitoring.Templates
{
    using Teleform.Reporting;
    using Teleform.Reporting.Web;
    using Teleform.ProjectMonitoring.HttpApplication;
    using System.Reflection;

    public class FieldBox
    {
        public object ID { get; private set; }

        public Label orderLabel { get; private set; }

        public System.Web.UI.WebControls.CheckBox VisibleBox { get; private set; }

        public Label NameLabel { get; private set; }

        public TextBox AliasBox { get; private set; }

        public ListControl FormatList { get; private set; }

        public TextBox ViewUserPredicateBox { get; private set; }

        public CompositePredicateControl Predicate { get; private set; }

        public ListControl AggregationList { get; private set; }

        public ListControl RelatedColumnsList { get; private set; }

        public ListControl ListAggregationList { get; private set; }

        public TemplateField Field { get; private set; }

        public System.Web.UI.WebControls.CheckBox checkBox;

        public DropDownList CrossTableRoleList { get; set; }

        public FieldBox(TemplateField field, string TemplateType = null)
        {
            Field = field;
            var uniqueID = string.Format("{0}_{1}", field.Attribute.ID, ((string.IsNullOrEmpty(field.Level.ToString()) ? "1" : field.Level.ToString()) + field.Order));// +field.Attribute.SystemName; <-- Времененно

            VisibleBox = new System.Web.UI.WebControls.CheckBox { ID = "VisibleBox" + uniqueID, Checked = field.IsVisible };
#if AlexJSSelected
            orderLabel = new Label { ID = "NameLabel" + uniqueID + "lblAlex", Text = (Field.Order + 1).ToString() };

            checkBox = new System.Web.UI.WebControls.CheckBox { ID = "NameLabel" + uniqueID + "chAlex" };
            checkBox.Attributes.Add("hidden", "false");


            if (TemplateType == "crossReport")
            {
                CrossTableRoleList = new DropDownList
                {
                    ID = "CrossReportElementType" + uniqueID,
                    DataValueField = "objID",
                    DataTextField = "name",
                    AutoPostBack = true
                };
                var query = "SELECT objID, name FROM [model].[R$CrossTableRole] ORDER BY name";
                var dataSourceCrossReport = Global.GetDataTable(query);
                CrossTableRoleList.DataSource = dataSourceCrossReport;
                CrossTableRoleList.DataBind();
                CrossTableRoleList.SelectedValue = field.CrossTableRoleID.ToString();
            }

            NameLabel = new Label { ID = "NameLabel" + uniqueID, Text = field.Attribute.Name };
            NameLabel.Attributes.Add("class", "SelectedFieldAlex");
            NameLabel.Attributes.Add("onclick", "SelectedTheField(this);");
#else
                  NameLabel = new Label { ID = "NameLabel" + uniqueID, Text = field.Attribute.Name };
#endif
            AliasBox = new TextBox { ID = "AliasBox" + uniqueID, Text = field.Name };

            FormatList = new DropDownList { ID = "FormatList" + uniqueID, DataValueField = "ID", DataTextField = "Name" };
            FormatList.DataSource = GetFormats();
            FormatList.DataBind();
            FormatList.SelectedValue = field.Format.ID.ToString();

            Predicate = new CompositePredicateControl
            {
                ID = "PredicateControl" + uniqueID,
                AttributeID = "#a",
                
                //AttributeID = field.Attribute.ID.ToString(),
                CssClass = "PredicateControl",
                Field = field,
                PredicateInfo = field.PredicateInfo,
                UserPredicate = field.Predicate
            };
            ViewUserPredicateBox = new TextBox
            {
                ID = "ViewUserPredicateBox" + uniqueID,
                TextMode = TextBoxMode.MultiLine,
                Width = 150,
                Text = field.Predicate
            };

            if (field.Attribute.IsListAttribute)
            {
                RelatedColumnsList = new DropDownList { ID = "RelatedColumnsList" + uniqueID, AutoPostBack = true, AppendDataBoundItems = true };
                RelatedColumnsList.Items.Add(new ListItem("не выбрано", ""));
                //, AppendDataBoundItems = true, SkinID = "List" , DataTextField = "Name", DataValueField = "FullSystemName"
                RelatedColumnsList.SelectedIndexChanged += new EventHandler(RelatedColumnsList_SelectedIndexChanged);
                RelatedColumnsList.DataSource = GetRelatedColumns(field.Attribute);
                RelatedColumnsList.DataTextField = "Name";
                RelatedColumnsList.DataValueField = "FullSystemName";
                RelatedColumnsList.DataBind();
                RelatedColumnsList.SelectedValue = field.ListAttributeAggregation.ColumnName;


                ListAggregationList = new DropDownList { ID = "ListAggregationList" + uniqueID, DataTextField = "Name", DataValueField = "lexem", AppendDataBoundItems = true };
                ListAggregationList.Items.Add(new ListItem("не выбрано", ""));
                ListAggregationList.DataSource = GetListAggregate(field.Attribute, field.ListAttributeAggregation);
                ListAggregationList.DataBind();
                ListAggregationList.SelectedValue = field.ListAttributeAggregation.AggregateLexem;
            }

            AggregationList = new DropDownList { ID = "AggregationList" + uniqueID, DataValueField = "lexem", DataTextField = "name", AutoPostBack = true,  AppendDataBoundItems = true };
            AggregationList.Items.Add(new ListItem("не выбрано", ""));
            AggregationList.SelectedIndexChanged += new EventHandler(AggregationList_SelectedIndexChanged);

            AggregationList.DataSource = GetListAggregateFunction();

            AggregationList.DataBind();
            AggregationList.SelectedValue = field.Aggregation;
        }
        void AggregationList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Field.Aggregation = AggregationList.SelectedValue;
        }
        void RelatedColumnsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListAggregationList.ClearSelection();
            FormatList.ClearSelection();

            Field.Aggregation = AggregationList.SelectedValue;
            AggregationList.ClearSelection();

            Field.ListAttributeAggregation.ColumnName = RelatedColumnsList.SelectedValue;
            Field.ListAttributeAggregation.AggregateLexem = string.Empty;

            

            // ListAggregationList.DataBind();
            // FormatList.DataBind();
        }

        public TableRow CreateRow()
        {
            var row = new TableRow();
            TableCell cell;

            cell = new TableCell();

            cell.Controls.Add(checkBox); // <-- AlexJSSelected
            cell.Controls.Add(orderLabel);
            row.Cells.Add(cell);

            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(VisibleBox);

            cell = new TableCell();
            cell.Style.Add("cursor", "pointer");
            row.Cells.Add(cell);
            cell.Controls.Add(NameLabel);

            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(AliasBox);

            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(FormatList);

            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(ViewUserPredicateBox);
            cell.Controls.Add(Predicate);

            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(AggregationList);

            cell = new TableCell();
            row.Cells.Add(cell);

            if (RelatedColumnsList != null)
                cell.Controls.Add(RelatedColumnsList);

            cell = new TableCell();
            row.Cells.Add(cell);

            if (ListAggregationList != null)
                cell.Controls.Add(ListAggregationList);

            return row;
        }

        public TableRow CreateRowCrossTable()
        {
            var row = new TableRow();
            TableCell cell;

            cell = new TableCell();

            cell.Controls.Add(checkBox); // <-- AlexJSSelected
            cell.Controls.Add(orderLabel);
            row.Cells.Add(cell);

            //cell = new TableCell();
            //row.Cells.Add(cell);
            //cell.Controls.Add(VisibleBox);

            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(CrossTableRoleList);

            cell = new TableCell();
            cell.Style.Add("cursor", "pointer");
            row.Cells.Add(cell);
            cell.Controls.Add(NameLabel);

            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(AliasBox);

            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(FormatList);

            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(ViewUserPredicateBox);
            cell.Controls.Add(Predicate);

            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(AggregationList);

            cell = new TableCell();
            row.Cells.Add(cell);

            if (RelatedColumnsList != null)
                cell.Controls.Add(RelatedColumnsList);

            cell = new TableCell();
            row.Cells.Add(cell);

            if (ListAggregationList != null)
                cell.Controls.Add(ListAggregationList);

            return row;

        }

        public TableRow CreateRowPredicateBuilder()
        {
            var row = new TableRow();
            TableCell cell;

            cell = new TableCell();
            cell.Controls.Add(checkBox); // <-- AlexJS
            cell.Controls.Add(orderLabel);
            row.Cells.Add(cell);

            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(VisibleBox);

            cell = new TableCell();
            cell.Style.Add("cursor", "pointer");
            row.Cells.Add(cell);
            cell.Controls.Add(NameLabel);

            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(ViewUserPredicateBox);
            cell.Controls.Add(Predicate);

            cell = new TableCell();
            row.Cells.Add(cell);

            if (RelatedColumnsList != null)
                cell.Controls.Add(RelatedColumnsList);

            cell = new TableCell();
            row.Cells.Add(cell);

            if (ListAggregationList != null)
                cell.Controls.Add(ListAggregationList);

            return row;
        }

        #region For HardTemplate Alex

        public Table CreateDialogTable()
        {
            TableRow row;
            TableCell cell;
            Table tb = new Table();

            cell = new TableCell();
            cell.Controls.Add(AliasBox);
            row = new TableRow();
            row.Controls.Add(new TableHeaderCell { Text = "Псевдоним" });
            row.Controls.Add(cell);
            tb.Controls.Add(row);

            cell = new TableCell();
            cell.Controls.Add(Predicate);
            row = new TableRow();
            row.Controls.Add(new TableHeaderCell { Text = "Фильтр" });
            row.Controls.Add(cell);
            tb.Controls.Add(row);

            cell = new TableCell();
            cell.Controls.Add(FormatList);
            row = new TableRow();
            row.Controls.Add(new TableHeaderCell { Text = "Формат" });
            row.Controls.Add(cell);
            tb.Controls.Add(row);

            cell = new TableCell();
            cell.Controls.Add(AggregationList);
            row = new TableRow();
            row.Controls.Add(new TableHeaderCell { Text = "Агрегация" });
            row.Controls.Add(cell);
            tb.Controls.Add(row);

            if (RelatedColumnsList != null)
            {
                cell = new TableCell();
                if (RelatedColumnsList != null)
                    cell.Controls.Add(RelatedColumnsList);
                row = new TableRow();
                row.Controls.Add(new TableHeaderCell { Text = "Колонка списка" });
                row.Controls.Add(cell);
                tb.Controls.Add(row);

                cell = new TableCell();
                if (ListAggregationList != null)
                    cell.Controls.Add(ListAggregationList);
                row = new TableRow();
                row.Controls.Add(new TableHeaderCell { Text = "Списковая агрегация" });
                row.Controls.Add(cell);
                tb.Controls.Add(row);
            }
            return tb;
        }

        #endregion

        public void RejectFilters()
        {
            //var filterList = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);//.Where(o => o.FieldType is IFilterControl);

            //foreach (IFilterControl filter in filterList)
            //    filter.RejectFilter();
            ViewUserPredicateBox.Text = string.Empty;
            Predicate.RejectFilter();
        }

        private List<object> GetRelatedColumns(Attribute attribute)
        {
            //Изменен порядок именования спиковых атрибутов (теперь имя состоит из полного пути)

            var entity = Storage.Select<Entity>(attribute.EntityID);


            if (entity == null)
                throw new NullReferenceException("Нет такой сущности");
            else
            {
                var list = new List<object>();
#if true

                foreach (Attribute attr in entity.Attributes.Where(o => !o.FPath.Contains("/") && o.SType != "Table" &&
                    o.Type.GetAdmissableAggregateFunctions().Count() != 0 && o.Name != o.FPath))
                    list.Add(new { Name = attr.Name, FullSystemName = attr.FPath });


                return list;
            }
#else
             //временный вариант, пока не узнаем, что за свойство isBase, возможно будем опираться на него
              foreach (Attribute attr in entity.Attributes.Where(o => o.Description.ToString() == "Base" && !o.FPath.Contains("/") && o.Type.Name != "Table" &&
                    o.Type.GetAdmissableAggregateFunctions().Count() != 0))
                    list.Add(new { Name = attr.Name, FullSystemName = attr.FPath });

                return list;
            }
#endif
        }

        private IEnumerable<AggregateFunction> GetListAggregate(Attribute attribute, ListAttributeAggregation listAttributeAggregation)
        {
            var columnName = listAttributeAggregation.ColumnName;
            if (!string.IsNullOrEmpty(columnName))
                return attribute.GetAttributeByColumnName(columnName).Type.GetAdmissableAggregateFunctions();
            else
                return null;
        }

        private IEnumerable<AggregateFunction> GetListAggregateFunction()
        {
            if (Field.Attribute.IsListAttribute)
            {
                IEnumerable<AggregateFunction> aggregationFunctions;
                var columnName = Field.ListAttributeAggregation.ColumnName;

                //если выбрана колонка списковой агрегации, то представление получет тип == колонке списковой агрегации, в противном случае тип int
                if (string.IsNullOrEmpty(columnName))
                {
                    var type = Type.GetType("int");
                    Field.Attribute.Type = type;
                    aggregationFunctions = type.GetAdmissableAggregateFunctions();
                }
                else
                {
                    var type = Field.Attribute.GetAttributeByColumnName(columnName).Type;
                    Field.Attribute.Type = type;

                    aggregationFunctions = Field.Attribute.GetAttributeByColumnName(columnName).Type.GetAdmissableAggregateFunctions();
                }

                return aggregationFunctions;
            }
            else
                return Field.Attribute.Type.GetAdmissableAggregateFunctions();
        }


        private object GetFormats()
        {
            if (Field.Attribute.IsListAttribute)
            {
                var columnName = Field.ListAttributeAggregation.ColumnName;
                return Field.Attribute.GetAttributeByColumnName(columnName).Type.GetAdmissableFormats();
            }
            return Field.Attribute.Type.GetAdmissableFormats();
        }

        public TemplateField SaveToField()
        {
            var type = Field.Attribute.Type;

            Field.IsVisible = VisibleBox.Checked;
            Field.Name = AliasBox.Text;
            Field.Predicate = Predicate.TechPredicate;
            Field.PredicateInfo = Predicate.PredicateInfo;
            Field.Aggregation = AggregationList.SelectedValue;

            if (Field.Attribute.IsListAttribute)
            {
                Field.ListAttributeAggregation.ColumnName = RelatedColumnsList.SelectedValue;
                Field.ListAttributeAggregation.AggregateLexem = ListAggregationList.SelectedValue;

                type = Field.Attribute.GetAttributeByColumnName(RelatedColumnsList.SelectedValue).Type;
            }
            Field.Format = type.GetAdmissableFormats().First(f => f.ID.ToString() == FormatList.SelectedValue);

            return Field;
        }


       
    }

}