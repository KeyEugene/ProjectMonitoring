using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Teleform.Reporting.MicrosoftOffice.ImportFile.Excel
{
    public class ExcelParser
    {
        private byte[] fileBytes;
        private string fileName;
        private Template template;
        private static WorkbookPart workbookPart;

        public ExcelParser(byte[] fileBytes, string fileName)
        {
            this.fileBytes = fileBytes;
            this.fileName = fileName;
        }

        public List<List<string>> StartParse(out Template template1)
        {
            List<List<string>> excelList = ParseFile();
            workbookPart = null;
            template1 = this.template;

            return excelList;
        }

        /// <summary>
        /// Несколько правил для парсинга Excel файла, 
        /// 1-работаем всегда с 1 листом(Sheet)
        /// 2-название 1 листа == названию темплейта в R$Template, так как бегаем по Field, а не по названем голонок
        /// 3-в начале проверяем равны ли название колонок с филдами, если да, то продолжаем, если нет то выводим ошибку : о не соответсвии шаблона и файла
        /// </summary>
        private List<List<string>> ParseFile()
        {
            var stream = new MemoryStream(fileBytes);

            using (var doc = SpreadsheetDocument.Open(stream, false))
            {
                workbookPart = doc.WorkbookPart;
                Sheet sheet = (Sheet)workbookPart.Workbook.Sheets.FirstChild;

                GetTemplate(sheet.Name.Value);

                var workPartBySheet = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                var rows = workPartBySheet.Worksheet.Descendants<Row>().ToList();

                ValidationTemplateFields(rows[0]);

                return FillExcelList(rows); ;
            }
        }

        private List<List<string>> FillExcelList(List<Row> rows)
        {
            var excelList = new List<List<string>>();
            var countColumn = template.Fields.Count;

            for (short i = 1; i < rows.Count; i++)
            {
               var rowIndex = rows[i].RowIndex.Value;
                    var cells = rows[i].Descendants<Cell>().ToList();
                    var listRow = new List<string>(cells.Count());

                    for (byte j = 0; j < countColumn; j++)
                    {
                        if (cells.Count == 0)
                        {
                            listRow.Add("");
                            continue;
                        }

                        var up = GetTheNameOfTheUpperColumn(j);
                        var cell = cells.ElementAt(0);
                        var adressCell = cell.CellReference.Value;

                        if ((up + (rowIndex).ToString()) == adressCell)//Это делаем из-за того, что в cells только те ячейки в которых есть данные
                        {
                            var cellValue = XLGetCellValue(cell);
                            listRow.Add(cellValue);
                            cells.Remove(cell);
                        }
                        else
                            listRow.Add("");
                    }
                    excelList.Add(listRow);
            }
            return excelList;
        }

        private void ValidationTemplateFields(Row row)
        {
            var cells = row.Descendants<Cell>();
            for (byte i = 0; i < template.Fields.Count; i++)
            {
                var cellValue = XLGetCellValue(cells.ElementAt(i));
                if (template.Fields[i].Attribute.Name == cellValue || template.Fields[i].Name == cellValue)
                    continue;
                else
                    new Exception(string.Concat("Поля в шаблоне ", template.Name, " не совпадают с названием колонок в файле."));
            }
        }

        private void GetTemplate(string value)
        {
            var dt = BaseParseFile.GetDataTable(string.Concat("SELECT [objID] FROM [model].[R$Template] WHERE [name] like '", value, "'"));

            if (dt.Rows.Count == 0)
                new Exception("Шаблона с таким иминем не существует : " + value);

            template = Storage.Select<Template>(dt.Rows[0][0].ToString());
        }

        private static string XLGetCellValue(Cell idCell)
        {
            if (idCell.DataType != null)
            {
                switch (idCell.DataType.Value)
                {
                    case CellValues.SharedString:
                        var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

                        if (stringTable == null)
                            throw new InvalidOperationException("Указанный файл повреждён, поскольку не содержит таблицу общих строк.");

                        return stringTable.SharedStringTable.ElementAt(int.Parse(idCell.InnerText)).InnerText;
                    case CellValues.String:
                        return idCell.CellValue.InnerText;
                    default:
                        return string.Empty;
                }
            }
            else
                return idCell.CellValue != null ? idCell.CellValue.InnerText : idCell.InnerText;

            throw new InvalidOperationException("Ячейка имеет недопустимый тип данных.");
        }

        public static string GetTheNameOfTheUpperColumn(byte j)
        {
            switch (++j)
            {
                case 1: return "A";
                case 2: return "B";
                case 3: return "C";
                case 4: return "D";
                case 5: return "E";
                case 6: return "F";
                case 7: return "G";
                case 8: return "H";
                case 9: return "I";
                case 10: return "J";
                case 11: return "K";
                case 12: return "L";
                case 13: return "M";
                case 14: return "N";
                case 15: return "O";
                case 16: return "P";
                case 17: return "Q";
                case 18: return "R";
                case 19: return "S";
                case 20: return "T";
                case 21: return "U";
                case 22: return "V";
                case 23: return "W";
                case 24: return "X";
                case 25: return "Y";
                case 26: return "Z";
                case 27: return "AA";
                case 28: return "AB";
                case 29: return "AC";
                case 30: return "AD";
                case 31: return "AE";
                case 32: return "AF";
                case 33: return "AG";
                case 34: return "AH";
                case 35: return "AI";
                case 36: return "AJ";
                case 37: return "AK";
                case 38: return "AL";
                case 39: return "AM";
                case 40: return "AN";
                case 41: return "AO";
                case 42: return "AP";
                case 43: return "AQ";
                case 44: return "AR";
                case 45: return "AS";
                case 46: return "AT";
                case 47: return "AU";
                case 48: return "AV";
                case 49: return "AW";
                case 50: return "AX";
                case 51: return "AY";
                case 52: return "AZ";
                default: new Exception("Не поддерживается.");
                    break;
            }
            return null;
        }
    }
}
