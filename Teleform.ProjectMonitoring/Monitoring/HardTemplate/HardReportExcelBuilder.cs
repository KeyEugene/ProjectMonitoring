#define Alexj

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Teleform.Reporting;
using System.Globalization;
using Teleform.Reporting.MicrosoftOffice;

namespace Teleform.ProjectMonitoring.HardTemplate
{
    public class HardReportExcelBuilder : BaseExcelBuilder
    {
        public Template template;
        public DynamicQueryForHeardTemplate dynamicQuery;
        private int maxlevel;
        private int currentRow;
        private int nonAgrCount;
        private int lastAgr;
        private List<UInt32Value> styles;
        private List<CellValues> dataTypes;

        public void Create(Stream output, Template template)
        {
            sheetName = template.Name;
            var stream = new MemoryStream();
            CreateFromTree(stream);
            var array = stream.ToArray();
            output.Write(array, 0, array.Length);
        }

        private void CreateFromTree(Stream output)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(output, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                GenWorkbookPart(sheetName, workbookPart);

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>("rId1");
                GenWorksheetPart(worksheetPart);

                WorkbookStylesPart workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>("rId2");
                GenWorkbookStylesPart(workbookStylesPart);
            }
        }


        //создание и заполнение workSheet строками с данными
        private void GenWorksheetPart(WorksheetPart worksheetPart)
        {
            Worksheet workSheet = new Worksheet();
            SheetData sheetData = new SheetData();

            currentRow = 1;

            var rows = MethodBuildTreeToRows();

            foreach (var item in rows)
                sheetData.AppendChild(item);

            workSheet.Append(sheetData);
            worksheetPart.Worksheet = workSheet;
        }

        //создание строк с данными
        private List<Row> MethodBuildTreeToRows()
        {
            var rows = new List<Row>();
            maxlevel = template.Fields.Max(x => x.Level);

            var currentLevel = 1;
            rows.Add(BuildHeaderRow());

            var dt = dynamicQuery.GetData(currentLevel);

            //Если что-то пошло не так!
            if (dt.Rows.Count == 0)
                return rows;

            var LevelAgr = template.Fields.Where(x => x.Level == currentLevel).Count(o => o.Aggregation != string.Empty);

            foreach (DataRow item in dt.Rows)
            {
                lastAgr = 0;
                var row = CreateRow(currentLevel, 0, item);
                rows.Add(row);
                var newRows = BuildChildRows(item, currentLevel, item.ItemArray.Count() - LevelAgr);
                foreach (var newItem in newRows)
                    rows.Add(newItem);
            }

            return rows;
        }

        //создает строку заголовка
        private Row BuildHeaderRow()
        {
            var hRow = new Row();
            styles = new List<UInt32Value>();
            dataTypes = new List<CellValues>();

            var aFields = template.Fields.Where(x => x.Aggregation != string.Empty).OrderBy(o => o.Level);
            var nFields = template.Fields.OrderBy(x => x.Level).Except(aFields);
            var fields = nFields.Concat(aFields).ToList();

            nonAgrCount = fields.Where(x => x.Aggregation == "").Count();

            //заполняем строку заголовка и запоминаем форматы столбцов
            for (int i = 0; i < fields.Count; i++)
            {
                var cell = new Cell()
                {
                    CellReference = string.Format("{0}{1}", (Char)('A' + i), currentRow.ToString()),
                    StyleIndex = (UInt32Value)0U,
                    DataType = CellValues.String
                };
                var cellValue = new CellValue();
                if (fields[i].AggregationFunction != null)
                    cellValue.Text = string.Format("{0} ({1})", fields[i].Name, fields[i].AggregationFunction.Name);
                else
                    cellValue.Text = fields[i].Name;
                cell.Append(cellValue);
                hRow.Append(cell);

                if (fields[i].Format.FormatString == "{0:T}")
                {
                    styles.Add((UInt32Value)2U);
                    dataTypes.Add(CellValues.Number);
                }
                else if (fields[i].Attribute.Type.Name.Contains("date"))
                {
                    switch (fields[i].Format.FormatString)
                    {
                        case "{0:dd MMMMM yyyy года}":
                            // Формат № 1 для дат FormatCode = "[$-F800]dddd\\,\\ mmmm\\ dd\\,\\ yyyy"
                            styles.Add((UInt32Value)1U);
                            dataTypes.Add(CellValues.Date);
                            break;
                        default:
                            //формат № 3 для дат * 14.03.2015
                            styles.Add((UInt32Value)3U);
                            dataTypes.Add(CellValues.Date);
                            break;
                    }
                }
                else if (fields[i].Attribute.Type.Name.Contains("money"))
                {
                    switch (fields[i].Format.FormatString)
                    {
                        case "{0:### ### ### ### ### p}.":
                            //формат № 2 для денег FormatCode = "#,##0\"р.\"" 
                            styles.Add((UInt32Value)7U);
                            dataTypes.Add(CellValues.Number);
                            break;
                        case "{0}":
                            //для общего формата денег
                            styles.Add((UInt32Value)8U);
                            dataTypes.Add(CellValues.Number);
                            break;
                        default:
                            // Формат № 0 по умолчанию общий
                            styles.Add((UInt32Value)0U);
                            dataTypes.Add(CellValues.String);
                            break;
                    }
                }
                else
                {
                    //Если никуда не попали, то формат общий
                    styles.Add((UInt32Value)0U);
                    dataTypes.Add(CellValues.String);
                }
            }

            currentRow++;   //на следующую строку
            return hRow;
        }
        //создает дочерние строки (по аналогии с TreeViewer.BuildChildNodes)
        private List<Row> BuildChildRows(DataRow drow, int currentLevel, int shift)
        {
            if (maxlevel == currentLevel)
                return new List<Row>();

            var LevelAgr = template.Fields.Where(x => x.Level == currentLevel).Count(o => o.Aggregation != string.Empty);
            var rows = new List<Row>();
            var dt = dynamicQuery.GetData(++currentLevel, drow);

            if (maxlevel != currentLevel)
                foreach (DataRow item in dt.Rows)
                {
                    lastAgr = LevelAgr;

                    var row = CreateRow(currentLevel, shift, item);
                    var newRows = BuildChildRows(item, currentLevel, shift + item.ItemArray.Count() - LevelAgr);
                    rows.Add(row);

                    foreach (var newItem in newRows)
                        rows.Add(newItem);
#if Alexj
                    var adRow = new Row() { Hidden = true, OutlineLevel = (ByteValue)(currentLevel - 1) };

                    rows.Add(adRow);
                    currentRow++;
#endif
                }
            else
                foreach (DataRow item in dt.Rows)
                    rows.Add(CreateRow(currentLevel, shift, item));

            return rows;
        }

        //строит Excel строку 
        private Row CreateRow(int currentLevel, int shift, DataRow drow)
        {
            var row = new Row();

            #region группировка и свертка строк

#if Alexj
            if (currentLevel > 1)
            {
                row.OutlineLevel = (ByteValue)(currentLevel - 1);
                row.Hidden = true;
            }
#endif

            #endregion

            var fields = template.Fields.OrderBy(x => x.Level).Where(o => o.Level == currentLevel).ToList();
            var LevelAgr = fields.Where(x => x.Aggregation != "").Count();
            //Добавляем все до агрегации
            var count = fields.Where(x => x.Aggregation == string.Empty).Count();

            for (var i = 0; i < count; i++)
            {
                var cell = new Cell()
                {
                    CellReference = string.Format("{0}{1}", (Char)('A' + shift + i), currentRow.ToString()),
                    StyleIndex = styles[shift + i],
                    DataType = dataTypes[shift + i]
                };
                var cellValue = new CellValue();

                if (cell.StyleIndex == (UInt32Value)2U)     //по тысячам
                {
                    if (drow.ItemArray[i] != DBNull.Value)
                    {
                        decimal o = Convert.ToDecimal(drow.ItemArray[i]);
                        o = o / 1000;
                        var a = o.ToString().Replace(',', '.');
                        cellValue.Text = a;
                    }
                    else
                        cellValue.Text = string.Empty;
                }
                else if (cell.StyleIndex == 1)                  //Формат № 1 для дат FormatCode = "[$-F800]dddd\\,\\ mmmm\\ dd\\,\\ yyyy"
                    cellValue.Text = string.Format("{0:yyyy-MM-dd}", drow.ItemArray[i]);
                else if (cell.StyleIndex == 3)                  //формат № 3 для дат * 14.03.2015
                    cellValue.Text = string.Format("{0:yyyy-MM-dd}", drow.ItemArray[i]);
                else if (cell.StyleIndex == 7)                  //формат № 2 для денег FormatCode = "#,##0\"р.\"" 
                    cellValue.Text = string.Format(CultureInfo.InvariantCulture, "{0}", drow.ItemArray[i]);
                else if (cell.StyleIndex == 8)                  //для общего формата денег
                    if (drow.ItemArray[i] != DBNull.Value)
                    {
                        var z = Convert.ToInt64(drow.ItemArray[i]);
                        cellValue.Text = string.Format(CultureInfo.InvariantCulture, "{0}", z);
                    }
                    else
                        cellValue.Text = string.Empty;
                else if (drow.ItemArray[i] is int)              //если int
                {
                    int number = Convert.ToInt32(drow.ItemArray[i]);
                    cell.DataType = CellValues.Number;
                    cell.StyleIndex = (UInt32Value)5U;
                    cellValue.Text = number.ToString();
                }
                else if (drow.ItemArray[i] is bool)
                {
                    //cellValue.Text = string.Format
                    //           (
                    //               fields[i].Format.Provider,
                    //               fields[i].Format.FormatString,
                    //               drow.ItemArray[i]
                    //            );
                    if (drow.ItemArray[i].ToString().ToLower() == "true")
                        cellValue.Text = "Да";
                    else if (drow.ItemArray[i].ToString().ToLower() == "false")
                        cellValue.Text = "Нет";
                }
                else
                    cellValue.Text = drow.ItemArray[i].ToString();  //все остальное

                cell.Append(cellValue);
                row.AppendChild(cell);
            }

            //Добавляем оставшиеся агрегации
            var agrCount = drow.ItemArray.Count() - count;
            //var nowAgr = 0;
            for (var i = 0; i < agrCount; i++)
            {
                var cell = new Cell();

                if (i < LevelAgr)
                {
                    cell.CellReference = string.Format("{0}{1}", (Char)('A' + nonAgrCount + lastAgr + i), currentRow.ToString());
                    cell.StyleIndex = styles[nonAgrCount + lastAgr + i];
                    cell.DataType = dataTypes[nonAgrCount + lastAgr + i];
                }
                else
                {
                    cell.CellReference = string.Format("{0}{1}", (Char)('A' + nonAgrCount + i), currentRow.ToString());
                    cell.StyleIndex = styles[nonAgrCount + i];
                    cell.DataType = dataTypes[nonAgrCount + i];

                }

                var cellValue = new CellValue();


                if (cell.StyleIndex == 2)                   //по тысячам
                    if (drow.ItemArray[count + i] != DBNull.Value)
                    {
                        decimal o = Convert.ToDecimal(drow.ItemArray[count + i]);
                        o = o / 1000;
                        var a = o.ToString().Replace(',', '.');
                        cellValue.Text = a;
                    }
                    else
                        cellValue.Text = string.Empty;
                else if (cell.StyleIndex == 1)              //Формат № 1 для дат FormatCode = "[$-F800]dddd\\,\\ mmmm\\ dd\\,\\ yyyy"
                    cellValue.Text = string.Format("{0:yyyy-MM-dd}", drow.ItemArray[count + i]);
                else if (cell.StyleIndex == 3)              //формат № 3 для дат * 14.03.2015
                    cellValue.Text = string.Format("{0:yyyy-MM-dd}", drow.ItemArray[count + i]);
                else if (cell.StyleIndex == 7)              //формат № 2 для денег FormatCode = "#,##0\"р.\"" 
                    cellValue.Text = string.Format(CultureInfo.InvariantCulture, "{0}", drow.ItemArray[count + i]);
                else if (cell.StyleIndex == 8)              //для общего формата денег
                    if (drow.ItemArray[count + i] != DBNull.Value)
                    {
                        var z = Convert.ToInt64(drow.ItemArray[count + i]);
                        cellValue.Text = string.Format(CultureInfo.InvariantCulture, "{0}", z);
                    }
                    else
                        cellValue.Text = string.Empty;
                else if (drow.ItemArray[count + i] is int)          //если int
                {
                    int number = Convert.ToInt32(drow.ItemArray[count + i]);
                    cell.DataType = CellValues.Number;
                    cell.StyleIndex = (UInt32Value)5U;
                    cellValue.Text = number.ToString();
                }
                else
                    cellValue.Text = drow.ItemArray[count + i].ToString();  //все остальное

                cell.Append(cellValue);
                row.AppendChild(cell);
            }
            //lastAgr = nowAgr;
            currentRow++;
            return row;
        }

        public override void addDataRows(SheetData sheetData, GroupReport report)
        {
            throw new NotImplementedException();
        }
    }
}