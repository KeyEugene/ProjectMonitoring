#define Force_Formula_Recalculations
#define Alex

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Globalization;

namespace Teleform.Reporting.MicrosoftOffice
{
    public sealed class ReportViewExcelBuilder : BaseExcelBuilder
    {
        private void CreateFromTemplateWithBody(Stream output, GroupReport report)
        {
            var content = report.Template.Content;

            output.Write(content, 0, content.Length);

            SpreadsheetDocument document = SpreadsheetDocument.Open(output, true);

            // Необходимо для принудительного пересчёта формул при загрузке.
            document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            document.WorkbookPart.Workbook.CalculationProperties.FullCalculationOnLoad = true;

            AddNewSheet(sheetName, document, output, report);

            WorkbookStylesPart workbookStylesPart = document.WorkbookPart.WorkbookStylesPart;
            GenWorkbookStylesPart(workbookStylesPart);

            document.Close();
        }

        //отправляет данные в Excel без форматов
        [Obsolete("Подумать над переходом на новый интерфейс.")]
        public void CreateExcel(Stream output, DataSet report)
        {
            if (output == null) throw new ArgumentNullException("output", string.Format("Параметр {0} имеет значение null.", "output"));
            if (report == null) throw new ArgumentNullException("report", string.Format("Параметр {0} имеет значение null.", "report"));

            using (var workbook = SpreadsheetDocument.Create(output, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();

                workbook.WorkbookPart.Workbook = new Workbook();

                workbook.WorkbookPart.Workbook.Sheets = new Sheets();

                foreach (DataTable table in report.Tables)
                {
                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    sheetPart.Worksheet = new Worksheet(sheetData);

                    Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                    string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                    uint sheetId = 1;
                    if (sheets.Elements<Sheet>().Count() > 0)
                    {
                        sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
                    sheets.Append(sheet);

                    Row headerRow = new Row();

                    List<String> columns = new List<string>();
                    foreach (DataColumn column in table.Columns)
                    {
                        columns.Add(column.ColumnName);

                        Cell cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);
                    foreach (DataRow dsrow in table.Rows)
                    {
                        Row newRow = new Row();
                        foreach (String col in columns)
                        {
                            Cell cell = new Cell();
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue(dsrow[col].ToString());
                            newRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(newRow);
                    }

                }
            }
        }

        public override void addDataRows(SheetData sheetData, GroupReport report)
        {
            // бежим по строкам
            foreach (Instance instance in report.Instances)
            {
                var row = new Row();                               

                foreach (Instance.Property property in instance.OwnProperties)
                {
                    var field = report.Template.Fields.FirstOrDefault(x => x.Attribute.FPath == property.Attribute.FPath);
                       
                    var cell = new Cell();
                        var cellValue = new CellValue();

                        ApplyStyle(property, field, ref cell, ref cellValue);

                        cell.Append(cellValue);
                        row.AppendChild(cell);                    
                }
                sheetData.AppendChild(row);
            }
        }
       
    }
}
