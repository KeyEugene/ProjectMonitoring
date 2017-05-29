using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Teleform.Reporting.MicrosoftOffice.ImportFile
{
    public class ExcelParser
    {
        private byte[] fileBytes;
        private string fileName;
        private Template template;

        public ExcelParser(byte[] fileBytes, string fileName)
        {
            this.fileBytes = fileBytes;
            this.fileName = fileName;
            ParseFile();
        }

        private void ParseFile()
        {
            var stream = new MemoryStream(fileBytes);

            using (var doc = SpreadsheetDocument.Open(stream, false))
            {
                var wPart = doc.WorkbookPart;
                var T = wPart.Workbook.FileSharing;
                
                Sheet sheet = (Sheet)wPart.Workbook.Sheets.FirstChild;

                GetTemplate(sheet.Name.Value);

                var workPart = (WorksheetPart)wPart.GetPartById(sheet.Id);
                var z = workPart.Worksheet.Descendants();
                var zz = workPart.TableDefinitionParts;
                var zzz = workPart.RootElement;
                var zzzzzzzz = workPart.Worksheet.Descendants<SharedStringItem>().ToArray();
                var rows = workPart.Worksheet.Descendants<Row>().ToList();

                ValidationTemplateFields(rows[0]);

                var pp = rows[0].Descendants<Cell>();

                foreach (var item in sheet.Descendants<Row>())
                {
                    var i = item;
                }


                //Sheet sheet = wPart.Workbook.Descendants<Sheet>().FirstOrDefault(x => x.Name.Value == "FullInputExcelBasedTest");

            }
        }

        private void ValidationTemplateFields(Row row)
        {
            for (int i = 0; i < template.Fields.Count; i++)
            {
                var z = row.Descendants<Cell>().FirstOrDefault();

                if (template.Fields[i].Attribute.Name == row.ChildElements[i].ToString() || template.Fields[i].Name == z.InnerText)
                    continue;
            }
            
        }

        private void GetTemplate(string value)
        {
            var dt = BaseParseFile.GetDataTable(string.Concat("SELECT [objID] FROM [R_Template] WHERE [name] like '", value, "'"));

            if (dt.Rows.Count == 0)
                new Exception("Шаблона с таким иминем не существует : " + value);

            template = Storage.Select<Template>(dt.Rows[0][0].ToString());
        }
    }
}
