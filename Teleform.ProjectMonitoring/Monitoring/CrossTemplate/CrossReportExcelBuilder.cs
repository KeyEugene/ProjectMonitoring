#define Viktor
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
    public class CrossReportExcelBuilder : IGroupReportBuilder
    {
        private string sheetName;
        public List<string> HeaderList;

        public void Create(Stream output, GroupReport report)
        {
            ExcelTemplate template;

            if (report.Template is ExcelTemplate)
                template = report.Template as ExcelTemplate;
            else template = new ExcelTemplate(report.Template);

            sheetName = template.Sheet;
            var stream = new MemoryStream();
#if Viktor
            CreateFromEmptyTemplate(stream, report);
#else

		if (report.Template.Content.Count() > 0)
				CreateFromTemplateWithBody(stream, report);
			else
                CreateFromEmptyTemplate(stream, report);
#endif


            var array = stream.ToArray();
            output.Write(array, 0, array.Length);
        }

        private void CreateFromTemplateWithBody(Stream output, GroupReport report)
        {
            var content = report.Template.Content;

            output.Write(content, 0, content.Length);

            SpreadsheetDocument document = SpreadsheetDocument.Open(output, true);

#if Force_Formula_Recalculations

            // Необходимо для принудительного пересчёта формул при загрузке.
            document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            document.WorkbookPart.Workbook.CalculationProperties.FullCalculationOnLoad = true;
#endif
            AddNewSheet(sheetName, document, output, report);

            WorkbookStylesPart workbookStylesPart = document.WorkbookPart.WorkbookStylesPart;
            GenWorkbookStylesPart(workbookStylesPart);

            document.Close();
        }

        private void CreateFromEmptyTemplate(Stream output, GroupReport report)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(output, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                GenWorkbookPart(sheetName, workbookPart);

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>("rId1");
                GenWorksheetPart(worksheetPart, report);

                WorkbookStylesPart workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>("rId2");
                GenWorkbookStylesPart(workbookStylesPart);
            }
        }

        // создает рабочую книгу Excel
        private void GenWorkbookPart(string sheetName, WorkbookPart workbookPart)
        {
            Workbook workbook = new Workbook();

            Sheets sheets = new Sheets();
            Sheet sheet = new Sheet() { Name = sheetName, SheetId = (UInt32Value)1U, Id = "rId1" };
            sheets.Append(sheet);
            workbook.Append(sheets);
            workbookPart.Workbook = workbook;
        }

        // создает рабочий лист Excel и запрлняет ячейкаи с данными из GroupReport
        private void AddNewSheet(string sheetName, SpreadsheetDocument spreadsheetDocument, Stream stream, GroupReport report)
        {
            if (spreadsheetDocument == null) throw new ArgumentNullException("spreadsheetDocument", string.Format("Параметр {0} имеет значение null.", "spreadsheetDocument"));
            if (report == null) throw new ArgumentNullException("report", string.Format("Параметр {0} имеет значение null.", "report"));

            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.Sheets;

            foreach (Sheet sheet in sheets)
            {
                if (sheetName == sheet.Name)
                {
                    WorksheetPart workSheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart.GetPartById(sheet.Id);

                    Worksheet workSheet = workSheetPart.Worksheet;

                    SheetData sheetData = workSheet.GetFirstChild<SheetData>();

                    Row headerRow = new Row() { RowIndex = 1 };
                    addHeaderRow(headerRow, report);
                    sheetData.AppendChild(headerRow);

                    //записать в Excel строки с данными
                    addDataRows(sheetData, report);

                    return;
                }
            }

            // добавить новый worksheet.
            WorksheetPart newWorksheetPart = spreadsheetDocument.WorkbookPart.AddNewPart<WorksheetPart>();
            SheetData newSheetData = new SheetData();

            Worksheet newWorkSheet = new Worksheet(newSheetData);
            newWorksheetPart.Worksheet = newWorkSheet;
            newWorksheetPart.Worksheet.Save();

            string relationshipId = spreadsheetDocument.WorkbookPart.GetIdOfPart(newWorksheetPart);

            // создать уникальный ID для worksheet.
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            Sheet newSheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            sheets.Append(newSheet);
            spreadsheetDocument.WorkbookPart.Workbook.Save();

            //записать в Excel заголовки полей
            Row newHeaderRow = new Row();
            addHeaderRow(newHeaderRow, report);
            newSheetData.AppendChild(newHeaderRow);

            //записать в Excel строки с данными
            addDataRows(newSheetData, report);

        }

        private void GenWorksheetPart(WorksheetPart worksheetPart, GroupReport report)
        {
            if (worksheetPart == null) throw new ArgumentNullException("worksheetPart", string.Format("Параметр {0} имеет значение null.", "worksheetPart"));
            if (report == null) throw new ArgumentNullException("template", string.Format("Параметр {0} имеет значение null.", "template"));

            Worksheet workSheet = new Worksheet();
            SheetData sheetData = new SheetData();

            //записать в Excel названия полей
            Row headerRow = new Row();
            addHeaderRow(headerRow, report);
            sheetData.AppendChild(headerRow);

            //записать в Excel строки с данными
            addDataRows(sheetData, report);

            workSheet.Append(sheetData);
            worksheetPart.Worksheet = workSheet;
        }

        private void addHeaderRow(Row headerRow, GroupReport report)
        {
            foreach (var head in HeaderList)
            {
                Cell cell = new Cell() { StyleIndex = (UInt32Value)0U };
                cell.DataType = CellValues.String;
                cell.CellValue = new CellValue(head);
                headerRow.AppendChild(cell);
            }
        }

        private void addDataRows(SheetData sheetData, GroupReport report)
        {

            // бежим по строкам
            foreach (Instance instance in report.Instances)
            {
                var row = new Row();

                //Instance.Property property;

                foreach (Instance.Property property in instance.BaseProperties) // TemplateField field in report.Template.Fields)
                {
                    var field = report.Template.Fields.FirstOrDefault(x => x.Attribute.FPath == property.Attribute.FPath);
                    //property = instance.BaseProperties.First(o => o.Attribute.ID.ToString() == field.Attribute.ID.ToString());

                    var cell = new Cell();
                    var cellValue = new CellValue();

                    if (field.Format.FormatString == "{0:T}") //"thousand")
                    {
                        if (property.Value != DBNull.Value)
                        {
                            decimal o = Convert.ToDecimal(property.Value);
                            o = o / 1000;
                            var a = o.ToString().Replace(',', '.');
                            cell.StyleIndex = (UInt32Value)2U;
                            cellValue.Text = a;
                        }
                        else
                            cellValue.Text = string.Empty;
                    }
                    else if (field.Attribute.Type.Name.Contains("date"))
                    {
                        switch (field.Format.FormatString)
                        {
                            case "{0:dd MMMMM yyyy года}":
                                // Формат № 1 для дат FormatCode = "[$-F800]dddd\\,\\ mmmm\\ dd\\,\\ yyyy" 
                                cell.StyleIndex = (UInt32Value)1U;
                                cell.DataType = CellValues.Date;
                                cellValue.Text = string.Format("{0:yyyy-MM-dd}", property.Value);
                                break;
                            default:
                                //формат № 3 для дат * 14.03.2015
                                cell.StyleIndex = (UInt32Value)3U;
                                cell.DataType = CellValues.Date;
                                cellValue.Text = string.Format("{0:yyyy-MM-dd}", property.Value);
                                break;
                        }
                    }
                    else if (field.Attribute.Type.Name.Contains("money"))
                    {
                        switch (field.Format.FormatString)
                        {
                            case "{0:### ### ### ### ### p}.":
                                //формат № 2 для денег FormatCode = "#,##0\"р.\"" 
                                cell.StyleIndex = (UInt32Value)7U;
                                cell.DataType = CellValues.Number;
                                cellValue.Text = string.Format(CultureInfo.InvariantCulture, "{0}", property.Value);
                                break;
                            case "{0}":
                                //для общего формата денег
                                cell.StyleIndex = (UInt32Value)8U;
                                //cell.DataType = CellValues.Number;
                                if (property.Value != DBNull.Value)
                                {
                                    var z = Convert.ToInt64(property.Value);
                                    cellValue.Text = string.Format(CultureInfo.InvariantCulture, "{0}", z);
                                }
                                else
                                    cellValue.Text = string.Empty;
                                break;
                            default:
                                // Формат № 0 по умолчанию общий
                                cell.StyleIndex = (UInt32Value)0U;
                                cell.DataType = CellValues.String;
                                cellValue.Text = string.Format(field.Format.Provider, field.Format.FormatString, property.Value);
                                break;
                        }
                    }
                    else
                    {
                        //ловим на int что бы потом преобразовать в Excel в число
                        if (property.Value is int)
                        {
                            int number = Convert.ToInt32(property.Value);
                            cell.DataType = CellValues.Number;
                            cell.StyleIndex = (UInt32Value)5U;
                            cellValue.Text = number.ToString();
                        }
                        else
                        {
                            // Формат № 0 по умолчанию общий
                            cell.StyleIndex = (UInt32Value)0U;
                            cell.DataType = CellValues.String;
                            cellValue.Text = string.Format(field.Format.Provider, field.Format.FormatString, property.Value);
                        }
                    }

                    cell.Append(cellValue);
                    row.AppendChild(cell);
                }
                sheetData.AppendChild(row);
            }
        }

        // создает ситили форматы
        private void GenWorkbookStylesPart(WorkbookStylesPart workbookStylesPart)
        {
            Stylesheet stylesheet = new Stylesheet();

            Fonts fonts1 = new Fonts();
            Font font1 = new Font();
            fonts1.Append(font1);

            Fills fills1 = new Fills();
            Fill fill1 = new Fill();
            Fill fill2 = new Fill();
            Fill fill3 = new Fill();
            PatternFill patternFill3 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor1 = new ForegroundColor() { Rgb = "FFFFFF00" };
            patternFill3.Append(foregroundColor1);
            fill3.Append(patternFill3);
            fills1.Append(fill1);
            fills1.Append(fill2);
            fills1.Append(fill3);

            Borders borders1 = new Borders();
            Border border1 = new Border();
            borders1.Append(border1);

            //Переопределяем номера формата 164U, 166U, 167U, 168U,
            NumberingFormats numberingFormats1 = new NumberingFormats() { Count = (UInt32Value)3U };
            NumberingFormat numberingFormat1 = new NumberingFormat() { NumberFormatId = (UInt32Value)164U, FormatCode = "[$-F800]dddd\\,\\ mmmm\\ dd\\,\\ yyyy" };
            NumberingFormat numberingFormat2 = new NumberingFormat() { NumberFormatId = (UInt32Value)166U, FormatCode = "#,##0.000\"р.\"" };
            NumberingFormat numberingFormat3 = new NumberingFormat() { NumberFormatId = (UInt32Value)167U, FormatCode = "#,##0\"р.\"" }; //"#,##0.00\"р.\"" };
            NumberingFormat numberingFormat4 = new NumberingFormat() { NumberFormatId = (UInt32Value)168U, FormatCode = "#,##0.00\"р.\"" };

            numberingFormats1.Append(numberingFormat1);
            numberingFormats1.Append(numberingFormat2);
            numberingFormats1.Append(numberingFormat3);
            numberingFormats1.Append(numberingFormat4);

            CellFormats cellFormats1 = new CellFormats() { Count = (UInt32Value)6U };
            CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U }; // Формат № 0 по умолчанию общий
            CellFormat cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)164U }; // Формат № 1 для дат FormatCode = "[$-F800]dddd\\,\\ mmmm\\ dd\\,\\ yyyy" 
            CellFormat cellFormat4 = new CellFormat() { NumberFormatId = (UInt32Value)166U }; //формат № 2 для денег по тысяч. ###,### <-- (number/ 1000)
            CellFormat cellFormat5 = new CellFormat() { NumberFormatId = (UInt32Value)14U }; //формат № 3 для дат * 14.03.2015
            CellFormat cellFormat6 = new CellFormat() { NumberFormatId = (UInt32Value)2U }; //формат № 4 для чисел
            CellFormat cellFormat7 = new CellFormat() { NumberFormatId = (UInt32Value)1U }; //более подходящий формат № 5U для чисел (5U)
            CellFormat cellFormat8 = new CellFormat() { NumberFormatId = (UInt32Value)165U };//формат №6U для денег по умолчанию 
            CellFormat cellFormat9 = new CellFormat() { NumberFormatId = (UInt32Value)167U };// формат №7U для денег(переопределенный) по три разряда ### ### р.
            CellFormat cellFormat10 = new CellFormat() { NumberFormatId = (UInt32Value)168U };// формат №8U для денег(переопределенный) ### ###,00 р.

            cellFormats1.Append(cellFormat2);
            cellFormats1.Append(cellFormat3);
            cellFormats1.Append(cellFormat4);
            cellFormats1.Append(cellFormat5);
            cellFormats1.Append(cellFormat6);
            cellFormats1.Append(cellFormat7);
            cellFormats1.Append(cellFormat8);
            cellFormats1.Append(cellFormat9);
            cellFormats1.Append(cellFormat10);

            stylesheet.Append(numberingFormats1);
            stylesheet.Append(fonts1);
            stylesheet.Append(fills1);
            stylesheet.Append(borders1);
            stylesheet.Append(cellFormats1);

            workbookStylesPart.Stylesheet = stylesheet;
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
    }
}
