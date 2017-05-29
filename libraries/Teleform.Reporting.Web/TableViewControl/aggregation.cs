using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Web.UI.WebControls;

namespace Teleform.Reporting.Web
{
    partial class TableViewControl
    {
        private void CreateAggregationRow()
        {
#if DEPRECATED
            var fields = Template.Fields.Where(f => !string.IsNullOrWhiteSpace(f.Aggregation));

            if (fields.Count() > 0)
            {
                var row = new TableRow();
                var table = Data.ToTable();

                foreach (DataColumn column in Data.Table.Columns)
                {
                    var cell = new TableCell();

                    if (column.ColumnName.ToLower() != "objid")
                    {
                        var field = fields.FirstOrDefault(f => f.Attribute.ID.ToString() == column.ColumnName);

                        if (field != null)
                        {
                            var function = field.AggregationFunction;
                            cell.Text = function.Name + ": " + table.Compute(string.Format("{0}([{1}])", function.Lexem, column.ColumnName), null).ToString();
                            cell.ForeColor = System.Drawing.Color.DarkOrange;
                            cell.Attributes["style"] = "background-color: AliceBlue";
                        }
                    }

                    row.Cells.Add(cell);
                }

                this.table.Rows.Add(row);
            }
#else
            if (Template.Fields.Any(f => !string.IsNullOrWhiteSpace(f.Aggregation)))
            {
                var row = new TableRow();
               // var table = SessionContent.DataView.ToTable();
                var table = DataView.ToTable();

                //var table = Storage.GetDataTable("select _workID, value  from _WorkCost");

                foreach (var field in Template.Fields)
                {
                    var cell = new TableCell();

                    cell.Attributes["style"] = "background-color: AliceBlue";
                    row.Cells.Add(cell);

                    if (!string.IsNullOrWhiteSpace(field.Aggregation))
                    {
                        var function = field.AggregationFunction;
                        var format = field.Attribute.Type.DefaultFormat;

                        var attributeID = field.Attribute.ID.ToString();
                        if (field.ListAttributeAggregation != null)
                        {
                            if (!string.IsNullOrEmpty(field.ListAttributeAggregation.ColumnName) && !string.IsNullOrEmpty(field.ListAttributeAggregation.AggregateLexem))
                                attributeID = field.HashName;
                        }

                        cell.Text = string.Concat
                        (
                            function.Name,
                            ": ",
                            string.Format
                            (
                                format.Provider,
                                format.FormatString,
                                table.Compute(string.Format("{0}([{1}])", function.Lexem, attributeID), null)
                            )
                        );
                        cell.ForeColor = System.Drawing.Color.Black;
                        cell.Font.Bold = true;
                    }
                }

                this.table.Rows.Add(row);
            }
#endif
        }
    }
}
