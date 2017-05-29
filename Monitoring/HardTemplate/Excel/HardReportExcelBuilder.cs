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
using Teleform.ProjectMonitoring.HardTemplate.Type_report.Children;

namespace Teleform.ProjectMonitoring.HardTemplate
{
    public partial class HardReportExcelBuilder : BaseExcelBuilder
    {
        public Template template;
        public DynamicQueryForHeardTemplate dynamicQuery;
        public Dynamic_Query_For_Heard_Template_Type_Children dynamicQueryChildren;
        private int maxlevel;
        private int currentRow;
        private int nonAgrCount;
        private int lastAgr;
        private List<UInt32Value> styles;
        private List<CellValues> dataTypes;

        public void Create(Stream output, Template template)
        {
            sheetName = template.Name.Length > 30 ? "Лист 1" : template.Name;// Если навзание больше чем 30 символов - выдается ошибка

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

            sheetData.AppendChild(BuildHeaderRow());
            var rows = CreateRowsFromTree();

            for (int i = 0; i < rows.Count; i++)
                sheetData.AppendChild(rows[i]);

            workSheet.Append(sheetData);
            worksheetPart.Worksheet = workSheet;
        }

        //создание строк с данными
        private List<Row> CreateRowsFromTree()
        {
            #region Инициализация переменных
            List<Row> rows = new List<Row>(); int currentLevel = 1;
            int LevelAgr = template.Fields.Where(x => x.Level == 1).
                Count(o => o.Aggregation != string.Empty);

            maxlevel = template.Fields.Max(x => x.Level);

            DataTable dt = GetFirstData();
            #endregion

            if (template.TreeTypeEnum == Reporting.Reporting.Template.EnumTreeType.Undefined ||
                   template.TreeTypeEnum == Reporting.Reporting.Template.EnumTreeType.General)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    lastAgr = 0;
                    rows.Add(CreateRowFor_General(currentLevel, 0, dt.Rows[i]));

                    var childRow = BuildChildRows_General(dt.Rows[i], currentLevel, dt.Rows[i].ItemArray.Count() - LevelAgr);

                    for (int j = 0; j < childRow.Count; j++)
                        rows.Add(childRow[j]);
                }
            }
            else if (template.TreeTypeEnum == Reporting.Reporting.Template.EnumTreeType.Children)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    rows.Add(CreateRowFor_Children(currentLevel, dt.Rows[i]));

                    var childRows = BuildChildRows_Children(dt.Rows[i], currentLevel);

                    for (int j = 0; j < childRows.Count; j++)
                    {
                        childRows[j].Hidden = true;
                        childRows[j].OutlineLevel = (ByteValue)1;
                        rows.Add(childRows[j]);
                    }
                }
            }
            else if (template.Entity.IsHierarchic && template.TreeTypeEnum == Reporting.Reporting.Template.EnumTreeType.Branch)
            { }

            return rows;
        }

        private DataTable GetFirstData()
        {
            var dt = new DataTable();

            if (template.TreeTypeEnum == Reporting.Reporting.Template.EnumTreeType.Undefined ||
                 template.TreeTypeEnum == Reporting.Reporting.Template.EnumTreeType.General)
                dt = dynamicQuery.GetData(1);
            else if (template.Entity.IsHierarchic && template.TreeTypeEnum == Reporting.Reporting.Template.EnumTreeType.Children)
                dt = dynamicQueryChildren.FirstQuery();
            else if (template.Entity.IsHierarchic && template.TreeTypeEnum == Reporting.Reporting.Template.EnumTreeType.Branch)
            {

            }

            return dt;//?? new DataTable();
        }

        #region создаем строку заголовка
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
        #endregion

        //создает дочерние строки (по аналогии с TreeViewer.BuildChildNodes)
        private List<Row> BuildChildRows_General(DataRow drow, int currentLevel, int shift)
        {
            if (maxlevel == currentLevel)
                return new List<Row>();

            var LevelAgr = template.Fields.Where(x => x.Level == currentLevel).Count(o => o.Aggregation != string.Empty);
            var rows = new List<Row>();
            DataTable dt = new DataTable();

            dt = dynamicQuery.GetData(++currentLevel, drow);
            
            if (maxlevel != currentLevel)
                foreach (DataRow item in dt.Rows)
                {
                    lastAgr = LevelAgr;

                    var row = CreateRowFor_General(currentLevel, shift, item);
                    var newRows = BuildChildRows_General(item, currentLevel, shift + item.ItemArray.Count() - LevelAgr);
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
                    rows.Add(CreateRowFor_General(currentLevel, shift, item));

            return rows;
        }

        private List<Row> BuildChildRows_Children(DataRow drow, int currentLevel)
        {
            var rows = new List<Row>();
            DataTable dt = new DataTable();

            dynamicQueryChildren.parent = "parentID = " + drow["objID"].ToString();
            dt = dynamicQueryChildren.GetData(1, drow);

            foreach (DataRow row in dt.Rows)
            {
                var newCurrentLevel = currentLevel;
                var newRow = CreateRowFor_Children(1, row);
                var newRows = BuildChildRows_Children(row, ++newCurrentLevel); 
                rows.Add(newRow);

                for (int i = 0; i < newRows.Count; i++)
                {
                    newRows[i].Hidden = true;
                    newRows[i].OutlineLevel = (ByteValue)(currentLevel + 1);

                    rows.Add(newRows[i]);
                }
                currentRow++;
            }
            return rows;
        }


        //строит Excel строку 
        private Row CreateRowFor_General(int currentLevel, int shift, DataRow drow)
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

                    if (styles.Count < nonAgrCount + i)
                    {
                        cell.StyleIndex = styles[nonAgrCount + i];
                        cell.DataType = dataTypes[nonAgrCount + i];
                    }
                    else
                    {
                        cell.StyleIndex = styles[0];
                        cell.DataType = dataTypes[0];
                    }
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

        private Row CreateRowFor_Children(int currentLevel, DataRow drow)
        {
            var row = new Row();


            var fields = template.Fields.OrderBy(x => x.Level).Where(o => o.Level == currentLevel).ToList();
            //var LevelAgr = fields.Where(x => x.Aggregation != "").Count();
            //Добавляем все до агрегации
            var count = fields.Where(x => x.Aggregation == string.Empty).Count();

            for (int i = 0; i < fields.Count; i++)
            {
                var cell = new Cell()
                {
                    //CellReference = string.Format("{0}{1}", (Char)('A' + i), currentRow.ToString()),
                    StyleIndex = styles[i],
                    DataType = dataTypes[i]
                };

                #region full cell value
                var cellValue = new CellValue();

                if (cell.StyleIndex == (UInt32Value)2U)     //по тысячам
                {
                    if (drow[fields[i].Name] != DBNull.Value)
                    {
                        decimal o = Convert.ToDecimal(drow[fields[i].Name]);
                        o = o / 1000;
                        var a = o.ToString().Replace(',', '.');
                        cellValue.Text = a;
                    }
                    else
                        cellValue.Text = string.Empty;
                }
                else if (cell.StyleIndex == 1)                  //Формат № 1 для дат FormatCode = "[$-F800]dddd\\,\\ mmmm\\ dd\\,\\ yyyy"
                    cellValue.Text = string.Format("{0:yyyy-MM-dd}", drow[fields[i].Name]);
                else if (cell.StyleIndex == 3)                  //формат № 3 для дат * 14.03.2015
                    cellValue.Text = string.Format("{0:yyyy-MM-dd}", drow[fields[i].Name]);
                else if (cell.StyleIndex == 7)                  //формат № 2 для денег FormatCode = "#,##0\"р.\"" 
                    cellValue.Text = string.Format(CultureInfo.InvariantCulture, "{0}", drow[fields[i].Name]);
                else if (cell.StyleIndex == 8)                  //для общего формата денег
                    if (drow[fields[i].Name] != DBNull.Value)
                    {
                        var z = Convert.ToInt64(drow[fields[i].Name]);
                        cellValue.Text = string.Format(CultureInfo.InvariantCulture, "{0}", z);
                    }
                    else
                        cellValue.Text = string.Empty;
                else if (drow[fields[i].Name] is int)              //если int
                {
                    int number = Convert.ToInt32(drow[fields[i].Name]);
                    cell.DataType = CellValues.Number;
                    cell.StyleIndex = (UInt32Value)5U;
                    cellValue.Text = number.ToString();
                }
                else if (drow[fields[i].Name] is bool)
                {
                    if (drow[fields[i].Name].ToString().ToLower() == "true")
                        cellValue.Text = "Да";
                    else if (drow[fields[i].Name].ToString().ToLower() == "false")
                        cellValue.Text = "Нет";
                }
                else
                    cellValue.Text = drow[fields[i].Name].ToString();  //все остальное

                cell.Append(cellValue);
                row.AppendChild(cell);
            }

                #endregion

            ////Добавляем оставшиеся агрегации
            //var agrCount = drow.ItemArray.Count() - count;
            ////var nowAgr = 0;
            //for (var i = 0; i < agrCount; i++)
            //{
            //    var cell = new Cell();

            //    if (i < LevelAgr)
            //    {
            //        cell.CellReference = string.Format("{0}{1}", (Char)('A' + nonAgrCount + lastAgr + i), currentRow.ToString());
            //        cell.StyleIndex = styles[nonAgrCount + lastAgr + i];
            //        cell.DataType = dataTypes[nonAgrCount + lastAgr + i];
            //    }
            //    else
            //    {
            //        cell.CellReference = string.Format("{0}{1}", (Char)('A' + nonAgrCount + i), currentRow.ToString());

            //        if (styles.Count < nonAgrCount + i)
            //        {
            //            cell.StyleIndex = styles[nonAgrCount + i];
            //            cell.DataType = dataTypes[nonAgrCount + i];
            //        }
            //        else
            //        {
            //            cell.StyleIndex = styles[0];
            //            cell.DataType = dataTypes[0];
            //        }
            //    }

            //    var cellValue = new CellValue();


            //    if (cell.StyleIndex == 2)                   //по тысячам
            //        if (drow.ItemArray[count + i] != DBNull.Value)
            //        {
            //            decimal o = Convert.ToDecimal(drow.ItemArray[count + i]);
            //            o = o / 1000;
            //            var a = o.ToString().Replace(',', '.');
            //            cellValue.Text = a;
            //        }
            //        else
            //            cellValue.Text = string.Empty;
            //    else if (cell.StyleIndex == 1)              //Формат № 1 для дат FormatCode = "[$-F800]dddd\\,\\ mmmm\\ dd\\,\\ yyyy"
            //        cellValue.Text = string.Format("{0:yyyy-MM-dd}", drow.ItemArray[count + i]);
            //    else if (cell.StyleIndex == 3)              //формат № 3 для дат * 14.03.2015
            //        cellValue.Text = string.Format("{0:yyyy-MM-dd}", drow.ItemArray[count + i]);
            //    else if (cell.StyleIndex == 7)              //формат № 2 для денег FormatCode = "#,##0\"р.\"" 
            //        cellValue.Text = string.Format(CultureInfo.InvariantCulture, "{0}", drow.ItemArray[count + i]);
            //    else if (cell.StyleIndex == 8)              //для общего формата денег
            //        if (drow.ItemArray[count + i] != DBNull.Value)
            //        {
            //            var z = Convert.ToInt64(drow.ItemArray[count + i]);
            //            cellValue.Text = string.Format(CultureInfo.InvariantCulture, "{0}", z);
            //        }
            //        else
            //            cellValue.Text = string.Empty;
            //    else if (drow.ItemArray[count + i] is int)          //если int
            //    {
            //        int number = Convert.ToInt32(drow.ItemArray[count + i]);
            //        cell.DataType = CellValues.Number;
            //        cell.StyleIndex = (UInt32Value)5U;
            //        cellValue.Text = number.ToString();
            //    }
            //    else
            //        cellValue.Text = drow.ItemArray[count + i].ToString();  //все остальное

            //    cell.Append(cellValue);
            //    row.AppendChild(cell);
            //}

            //for (var i = 0; i < count; i++)
            //{


            //}
            //lastAgr = nowAgr;
            currentRow++;
            return row;

        }


        public override void addDataRows(SheetData sheetData, GroupReport report)
        {
        }
    }
}