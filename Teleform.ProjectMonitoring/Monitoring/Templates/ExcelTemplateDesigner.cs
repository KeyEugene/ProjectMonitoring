//#define sheetbox

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Web.UI.WebControls;

namespace Teleform.ProjectMonitoring.Templates
{
    using Reporting;
    using Reporting.MicrosoftOffice;
    using DocumentFormat.OpenXml.Packaging;
    using System.IO;
    using DocumentFormat.OpenXml.Spreadsheet;
    using DocumentFormat.OpenXml;
    using Teleform.ProjectMonitoring.HttpApplication;

    public class ExcelTemplateDesigner : FileBasedTemplateDesigner
    {
        private static readonly string TemplateSheet = "templatesheet";
#if sheetbox
        private TextBox SheetBox;
#endif

        public ExcelTemplateDesigner()
        {
            AdmissableExtensions.Add(".xlsx");
        }

        public override bool IsFileBased
        { get { return true; } }

        protected override void CreateChildControls()
        {
            var table = CreateBasicControls();
#if sheetbox
            TableRow row;
            TableCell cell;

            row = new TableRow();
            cell = new TableCell();
            var label = new Label() { Text = "Лист" };
            cell.Controls.Add(label);
            row.Cells.Add(cell);

            cell = new TableCell();
            SheetBox = new TextBox();
            cell.Controls.Add(SheetBox);
            row.Cells.Add(cell);
            table.Rows.Add(row);

            this.Controls.Add(table);
#endif
            this.Controls.Add(table);
        }

        protected override Template RetrieveTemplate(string name, byte[] body, string TemplateID = null)
        {
#warning Order and for each
            Entity entity = null;
            object entityID, attributeID, formatID;

            Attribute attribute = null;

            var placeholders = GetExcelPlaceHolders(body);

            if (placeholders.Count == 0)
                throw new Exception("Указанный файл не содержит шаблон.");

            var fields = new List<TemplateField>();
            var creatorID = new Teleform.Reporting.UniqueIDCreator();

            creatorID.Split(placeholders.First().ID, out entityID, out attributeID, out formatID);

            entity = Storage.Select<Entity>(entityID);

            foreach (var holder in placeholders)
            {                

                creatorID.Split(holder.ID, out entityID, out attributeID, out formatID);

                attribute = entity.Attributes.FirstOrDefault(o => o.ID.ToString() == attributeID.ToString());
                
                if (attribute == null)
                    throw new InvalidOperationException(
                        string.Format("Сущность '{0}' не имеет атрибут с идентификатором {1}.",
                            entity.Name, attributeID));

                var field = new TemplateField(attribute);

                field.Format = attribute.Type.GetAdmissableFormats().FirstOrDefault(o => o.ID.ToString() == formatID.ToString());

                if(field.Format == null)
                    throw new InvalidOperationException(
                        string.Format("Тип '{0}' атрибута {1} не имеет формат с идентификатором {2}.",
                            attribute.Type.Name, attribute.Name, formatID));

                field.Name = holder.Alias;

                fields.Add(field);
            }
#if sheetbox
            var template = new ExcelTemplate(name, entity, "ExcelBased", body, fields, SheetBox.Text.Trim());
#else
            
            var template = new ExcelTemplate(name, entity, this.TemplateTypeCode, body, fields, string.Empty, TemplateID);

#endif
            return template;
        }

        private List<ExcelPlaceHolder> GetExcelPlaceHolders(byte[] body)
        {
            if (body == null)
                throw new ArgumentNullException("body",
                    "Параметр 'body' имеет значение null.");

            var placeholders = new List<ExcelPlaceHolder>();
            var stream = new MemoryStream(body);

            using (var doc = SpreadsheetDocument.Open(stream, true))
            {
                var wPart = doc.WorkbookPart;
                Sheet sh = wPart.Workbook.Descendants<Sheet>().
                    FirstOrDefault(x => x.Name.Value == TemplateSheet);

                if (sh == null)
                    throw new InvalidOperationException("Указанный файл не содержит шаблон.");

                var sPart = (WorksheetPart)(wPart.GetPartById(sh.Id));

                Row idRow = sPart.Worksheet.Descendants<Row>().First(x => x.RowIndex == 1);
                var idCellList = idRow.Descendants<Cell>();

                Row aliasRow = sPart.Worksheet.Descendants<Row>().First(x => x.RowIndex == 2);
                var aliasCellList = aliasRow.Descendants<Cell>();

                if (idCellList.Count() != aliasCellList.Count())
                    throw new InvalidOperationException("Указанный файл содержит недействительный шаблон.");
                
                for (int i = 0; i < idCellList.Count(); i++)
                {
                    var idCell = idCellList.ElementAt(i);
                    var aliasCell = aliasCellList.ElementAt(i);

                    var id = XLGetCellValue(idCell, wPart);
                    var alias = XLGetCellValue(aliasCell, wPart);

                    placeholders.Add(new ExcelPlaceHolder { ID = id, Alias = alias });
                }
            }
            return placeholders;
        }

        private static string XLGetCellValue(Cell idCell, WorkbookPart wbPart)
        {
            if (idCell.DataType != null)
            {
                switch (idCell.DataType.Value)
                {
                    case CellValues.SharedString:
                        var stringTable = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

                        if (stringTable == null)
                            throw new InvalidOperationException("Указанный файл повреждён, поскольку не содержит таблицу общих строк.");

                        return stringTable.SharedStringTable.ElementAt(int.Parse(idCell.InnerText)).InnerText;
                    case CellValues.String:
                        return idCell.CellValue.InnerText;
                    default:
                        return string.Empty;
                }
            }

            throw new InvalidOperationException("Ячейка имеет недопустимый тип данных.");
        }
    }

    internal class ExcelPlaceHolder
    {
        public string ID;

        public string Alias;
    }
}