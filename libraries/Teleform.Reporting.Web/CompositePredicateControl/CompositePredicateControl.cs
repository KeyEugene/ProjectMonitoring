#define Alex
#define Attribute

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using IEnumerable = System.Collections.IEnumerable;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;

namespace Teleform.Reporting.Web
{
    [ToolboxData("<{0}:PredicateControl runat=\"server\"></{0}:PredicateControl>")]
    public partial class CompositePredicateControl : CompositeControl, IFilterControl
    {


        private TextBox TechPredicateBox, UserPredicateBox, JeysonBox;

        private DropDownList OperatorList;
        private TextBox ValueBox;
        private Button ApplyButton;

        private HtmlGenericControl addExpressionButton;
        private HtmlGenericControl startBlockButton;
        private HtmlGenericControl endBlockButton;
        private HtmlGenericControl addAndOperatorButton;
        private HtmlGenericControl addOrOperatorButton;

        private HtmlGenericControl clearButton;
        private HtmlGenericControl clearAllButton;

        private Table Table;
        private string techPredicate, userPredicate, column;



        public bool IsDatabasePredicate { get; set; }

        public Attribute Attribute { get; set; }

        public TemplateField Field { get; set; }

        public string PredicateInfo
        {
            get
            {
                EnsureChildControls();

                return JeysonBox.Text;
            }
            set
            {
                EnsureChildControls();

                JeysonBox.Text = value;
            }
        }
        public string AttributeID
        {
            get
            {
                if (Attribute != null)
                    return column ?? Attribute.ID.ToString();
                else                                    
                    return column ?? Field.Attribute.ID.ToString();
                
            }
            set { column = value; }
        }

        public string TechPredicate
        {
            get
            {
                EnsureChildControls();

                if (techPredicate == null)
                {
                    var expression = TechPredicateBox.Text;

                    if (expression != string.Empty)
                    {
                        //techPredicate = expression;
                        techPredicate = string.Concat("(", expression.Replace("#a", "[" + AttributeID + "]"), ")");
                    }
                }
                return techPredicate;
            }

            //set
            //{
            //    EnsureChildControls();

            //    TechPredicateBox.Text = value;
            //}
        }

        public string UserPredicate
        {
            get
            {
                EnsureChildControls();

                if (userPredicate == null)
                {
                    if (UserPredicateBox.Text != string.Empty)
                        userPredicate = UserPredicateBox.Text;
                }
                return userPredicate;
            }
            set
            {
                EnsureChildControls();

                UserPredicateBox.Text = value;
            }

        }




        public void RejectFilter()
        {
            //JeysonBox.Text = UserPredicateBox.Text = ValueBox.Text = null;
            JeysonBox.Text = TechPredicateBox.Text = UserPredicateBox.Text = ValueBox.Text = null;
            OperatorList.SelectedIndex = 0;
            //ItemList.ClearSelection();
        }



        protected override void CreateChildControls()
        {
            // base.CreateChildControls();

            if (Field != null || Attribute != null)
            {
                Table = new Table { ID = "Table" };

                var row = new TableRow();
                var cell = new TableCell();

                JeysonBox = new TextBox { ID = "JeysonBox" };
                JeysonBox.Style.Add("display", "none");
                JeysonBox.TextMode = TextBoxMode.MultiLine;
                cell.Controls.Add(JeysonBox);

                TechPredicateBox = new TextBox { ID = "TechPredicateBox" };
                TechPredicateBox.Style.Add("display", "none");
                TechPredicateBox.TextMode = TextBoxMode.MultiLine;
                cell.Controls.Add(TechPredicateBox);

                UserPredicateBox = new TextBox { ID = "UserPredicateBox" };
                UserPredicateBox.Style.Add("width", "306px");
                UserPredicateBox.ReadOnly = true;
                UserPredicateBox.TextMode = TextBoxMode.MultiLine;
                cell.Controls.Add(UserPredicateBox);

                row.Cells.Add(cell);
                Table.Rows.Add(row);

#if FOR_EXCLUDE
                row = new TableRow();
                cell = new TableCell();

                addExpressionButton = new HtmlGenericControl("input");
                addExpressionButton.Attributes.Add("type", "Button");
                addExpressionButton.Attributes.Add("value", "+");
                addExpressionButton.ID = "addExpressionButton";
                cell.Controls.Add(addExpressionButton);

                startBlockButton = new HtmlGenericControl("input");
                startBlockButton.Attributes.Add("type", "Button");
                startBlockButton.Attributes.Add("value", "(");
                startBlockButton.ID = "startBlockButton";
                cell.Controls.Add(startBlockButton);

                endBlockButton = new HtmlGenericControl("input");
                endBlockButton.Attributes.Add("type", "Button");
                endBlockButton.Attributes.Add("value", ")");
                endBlockButton.ID = "endBlockButton";
                endBlockButton.Attributes.Add("disabled", "true");
                cell.Controls.Add(endBlockButton);

                addAndOperatorButton = new HtmlGenericControl("input");
                addAndOperatorButton.Attributes.Add("type", "Button");
                addAndOperatorButton.Attributes.Add("value", "И");
                addAndOperatorButton.ID = "addAndOperatorButton";
                addAndOperatorButton.Attributes.Add("disabled", "true");
                cell.Controls.Add(addAndOperatorButton);

                addOrOperatorButton = new HtmlGenericControl("input");
                addOrOperatorButton.Attributes.Add("type", "Button");
                addOrOperatorButton.Attributes.Add("value", "ИЛИ");
                addOrOperatorButton.ID = "addOrOperatorButton";
                addOrOperatorButton.Attributes.Add("disabled", "true");
                cell.Controls.Add(addOrOperatorButton);

                cell.HorizontalAlign = HorizontalAlign.Center;
                row.Cells.Add(cell);
                Table.Rows.Add(row);
#endif
                row = new TableRow();
                cell = new TableCell();

#if Alex
                Attribute attr;
                if (Attribute != null)
                    attr = Attribute;
                else
                    attr = Field.Attribute;

                if (Field != null && Field.ListAttributeAggregation != null)
                {
                    if (!string.IsNullOrEmpty(Field.ListAttributeAggregation.ColumnName))
                    {
                        var entity = Storage.Select<Entity>(Field.Attribute.EntityID);
                        attr = entity.Attributes.FirstOrDefault(x => x.FPath == Field.ListAttributeAggregation.ColumnName);
                    }
                }


                OperatorList = new DropDownList
                {
                    ID = "OperatorList",
                    DataTextField = "Name",
                    DataValueField = "Lexem"
                };

                OperatorList.DataSource = attr.GetAccessibleOperators();
                OperatorList.DataBind();

                ValueBox = new TextBox { ID = "ValueBox" };

                ValueBox.ApplyType(attr.Type);
#else



                OperatorList = new DropDownList
                {
                    ID = "OperatorList",
                    DataTextField = "Name",
                    DataValueField = "Lexem"
                };


                if (Attribute != null)
                    OperatorList.DataSource = Attribute.GetAccessibleOperators();
                else
                    OperatorList.DataSource = Field.Attribute.GetAccessibleOperators();
                OperatorList.DataBind();
               

                ValueBox = new TextBox { ID = "ValueBox" };
                if (Attribute != null)
                    ValueBox.ApplyType(Attribute.Type);
                else
                    ValueBox.ApplyType(Field.Attribute.Type);
#endif
                cell.Controls.Add(OperatorList);
                cell.Controls.Add(ValueBox);

                row.Cells.Add(cell);
                Table.Rows.Add(row);

                row = new TableRow();
                cell = new TableCell();
#if FOR_EXCLUDE
                clearButton = new HtmlGenericControl("input");
                clearButton.Attributes.Add("type", "Button");
                clearButton.Attributes.Add("value", "Очистить");
                clearButton.Attributes.Add("ID", "clearButton");
                cell.Controls.Add(clearButton);

                clearAllButton = new HtmlGenericControl("input");
                clearAllButton.Attributes.Add("type", "Button");
                clearAllButton.Attributes.Add("value", "Очистить все");
                clearAllButton.Attributes.Add("ID", "clearAllButton");
                cell.Controls.Add(clearAllButton);
#endif
                ApplyButton = new Button { ID = "ApplyButton", Text = "Применить", Enabled = true };
                ApplyButton.Click += new EventHandler(ApplyFilter_Click);

                cell.Controls.Add(ApplyButton);

                cell.BackColor = System.Drawing.Color.FromArgb(212, 212, 212);

                row.Cells.Add(cell);
                Table.Rows.Add(row);

                Controls.Add(Table);
            }
        }

        //public void SetFilter(string predicate)
        //{
        //    if (!string.IsNullOrWhiteSpace(predicate))
        //    {
        //        EnsureChildControls();

        //        var variableName = new Regex(@"#\w{1}");
        //        var valueName = new Regex(@"((?:\d*\.)?\d+)|(['|""](.*?)['|""])");
        //        var infix = new Regex(@"\b([Aa][Nn][Dd]|[Oo][Rr])\b");
        //        var op = new Regex(@"(\B<>\B|\B>=\B|\B<=\B)|(>|<|=)|(\blike\b)|(\bnot like\b)", RegexOptions.IgnoreCase);

        //        var leftValue = valueName.Match(predicate).Value.Trim(new char[] { '\'', '\"', '%' });
        //        var rightValue = valueName.Match(predicate).NextMatch().Value
        //            .Trim(new char[] { '\'', '\"', '%' }); ;


        //        var leftOperator = op.Match(predicate).Value;
        //        var rightOperator = op.Match(predicate).NextMatch().Value;

        //        var infixOp = infix.Match(predicate).Value;
        //        OperatorList.SelectedValue = Attribute.GetAccessibleOperators().First(o => o.Lexem.ToLower() == leftOperator.ToLower()).ID.ToString();

        //    }
        //}


    }
}
