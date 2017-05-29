using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;



namespace Teleform.Reporting.Web
{
    public partial class TableViewControl
    {


        private TableHeaderRow CreateHeaderRow(ref int tableWidth)
        {
            if (HeaderRow == null || isColResizable == true)
            {
                TemplateFieldIndices = new SortedDictionary<TemplateField, int>();
                HeaderRow = new TableHeaderRow();

                foreach (var field in Template.Fields.Where(f => f.IsForbidden == false))
                {
                    if (field.IsVisible)
                    {
                        var cell = new TableHeaderCell();

                        var viewLength = field.Attribute.Type.ViewLength.Value;
                        var fieldLength = field.Name.Length;
                        int[] i = { viewLength, fieldLength };
                        int maxValue = i.Max();

                        int cellLength = 0;

                        if (maxValue >= 1024)
                            cellLength = maxValue / 2;
                        else if (maxValue > 100 && maxValue <= 255)
                            cellLength = maxValue;
                        else if (maxValue >= 50 && maxValue <= 100)
                            cellLength = maxValue * 3;
                        else if (maxValue > 25 && maxValue < 50)
                            cellLength = maxValue * 5;
                        else if (maxValue < 25)
                            cellLength = maxValue * 10;

                        tableWidth += cellLength;
                        cell.Width = cellLength;

                        cell.Attributes.Add("HeaderColumnAlias", field.HashName);

                        var sessionKey = string.Concat(Template.ID, "fieldsSizeDict");

                        if (SessionContent.FieldsSize.Keys.Contains(sessionKey))
                        {
                            var fieldsSizeDict = (IDictionary<string, string>)SessionContent.FieldsSize[sessionKey];

                            if (fieldsSizeDict != null && fieldsSizeDict.Count > 0)
                            {
                                string s = String.Empty;
                                fieldsSizeDict.TryGetValue(field.HashName, out s);
                                if (!string.IsNullOrEmpty(s))
                                {
                                    var cellWidth = s;
                                    if (s.Contains('.'))
                                        cellWidth = s.Remove(s.IndexOf('.'), s.Length - s.IndexOf('.'));
                                    cell.Width = Unit.Parse(cellWidth);
                                }
                            }
                        }

                        var attributeID = field.Attribute.ID.ToString();

                        if (field.ListAttributeAggregation != null)
                        {
                            //если есть списковая агрегация, то в HashName подставляется рассчетное значение агрегации
                            if (!string.IsNullOrEmpty(field.ListAttributeAggregation.ColumnName) && !string.IsNullOrEmpty(field.ListAttributeAggregation.AggregateLexem))
                                attributeID = field.HashName;
                        }

                        TemplateFieldIndices.Add(field, DataView.Table.Columns.IndexOf(attributeID));


                        var listBox = CreateFilterControl<FilterControl>(attributeID, FilterStyle, field);


                        FillFilterBox(attributeID, listBox);
                        sessionKey = MakeKey(attributeID, listBox.GetType());
                        if (SessionContent.FilterData.ContainsKey(sessionKey))
                            listBox.FilterData = SessionContent.FilterData[sessionKey];

                        var predicateBox = CreateFilterControl<CompositePredicateControl>(attributeID, PredicateControlStyle, field);
                        sessionKey = MakeKey(attributeID, predicateBox.GetType());
                        if (SessionContent.FilterData.ContainsKey(sessionKey))
                            predicateBox.FilterData = SessionContent.FilterData[sessionKey];

                        var sortingControl = CreateSortingControl(attributeID, SortingStyle, field);
                        sessionKey = MakeKey(attributeID, sortingControl.GetType());
                        if (SessionContent.SortingData.ContainsKey(sessionKey))
                            sortingControl.SortingData = SessionContent.SortingData[sessionKey];


                        var tbl = new Table();
                        var trow = new TableRow();
                        var trow2 = new TableRow();
                        var tFilters = new TableCell();
                        var tHeaderText = new TableCell();
                        var tSorting = new TableCell();
#if trueWWW
                        var headerText = new TextBox();
                        headerText.Text = field.Name;
                        headerText.ReadOnly = true;
                        headerText.TextMode = TextBoxMode.MultiLine;
                        headerText.ForeColor = Color.White;
                        headerText.Style.Add("background", "#424242 repeat-x top;");
                        headerText.Style.Add("overflow", "hidden");
                        headerText.BorderWidth = Unit.Pixel(0);
                        headerText.Width = 100;
                        tHeaderText.Controls.Add(headerText);
                        trow.Cells.Add(tHeaderText);
#endif
                        tFilters.BorderStyle = System.Web.UI.WebControls.BorderStyle.None;
                        tFilters.Style.Add("vertical-align", "bottom");
                        tHeaderText.BorderStyle = System.Web.UI.WebControls.BorderStyle.None;
                        tSorting.BorderStyle = System.Web.UI.WebControls.BorderStyle.None;
                        tbl.BorderStyle = System.Web.UI.WebControls.BorderStyle.None;


                        tSorting.Controls.Add(sortingControl);
                        tFilters.Controls.Add(predicateBox);
                        tFilters.Controls.Add(listBox);
                        trow.Cells.Add(tSorting);
                        trow.Cells.Add(tFilters);


                        tbl.Rows.Add(trow);
                        cell.Controls.Add(tbl);
                        HeaderRow.Cells.Add(cell);
                    }

                }
            }
            var ccount = HeaderRow.Cells.Count;

            return HeaderRow;
        }


    }
}
