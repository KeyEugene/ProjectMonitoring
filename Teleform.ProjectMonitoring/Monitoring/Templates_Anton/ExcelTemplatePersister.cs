using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Teleform.ProjectMonitoring.HttpApplication;
using Teleform.ProjectMonitoring.Templates;
using Teleform.Reporting;
using Teleform.Reporting.MicrosoftOffice;
using System.Xml.Linq;

namespace Teleform.ProjectMonitoring.Templates_Anton
{
    public class ExcelTemplatePersister : ITemplatePersister<ExcelTemplate>
    {
        private static readonly string TemplateSheet = "templatesheet";
        private string name;
        private byte[] body;

        public ExcelTemplatePersister(string name, byte[] body)
        {
            this.name = name;
            this.body = body;
        }

        public ExcelTemplate Create()
        {
            var template = RetreiveTemplate(name, body);

            return template;
        }

        public object Insert(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                throw new ArgumentNullException("xml", "Получено недействительное xml-представление шаблона.");

            using (var c = new SqlConnection(Kernel.ConnectionString))
            {
                var cmd = new SqlCommand { CommandText = "EXEC [model].[R$TemplateCreate] @xml", Connection = c };

                c.Open();

                cmd.Parameters.Add("xml", SqlDbType.Xml).Value = xml;

                var newId = cmd.ExecuteScalar();

                return newId;
            }
        }

        public void Update(string xml, object id)
        {
            if (string.IsNullOrWhiteSpace(xml))
                throw new ArgumentNullException("xml", "Получено недействительное xml-представление шаблона.");

            //dirty hack goes here!
            var xTemplate = XDocument.Parse(xml);
            xTemplate.Root.Add(
                new XAttribute("id", id)
                );
            //

            using (var c = new SqlConnection(Kernel.ConnectionString))
            {
                var cmd = new SqlCommand { CommandText = "EXEC [model].[R$TemplateUpdate] @xml", Connection = c };
                cmd.Parameters.Add("xml", SqlDbType.Xml).Value = xTemplate.ToString();
                c.Open();
                cmd.ExecuteNonQuery();
            }
        }

        #region Private Methods
        private ExcelTemplate RetreiveTemplate(string name, byte[] body)
        {
#warning Order and for each
            Entity entity = null;
            object entityID, attributeID, formatID;

            Teleform.Reporting.Attribute attribute = null;

            var placeholders = GetExcelPlaceHolders(body);

            if (placeholders.Count == 0)
                throw new Exception("Указанный файл не содержит шаблон.");

            var fields = new List<TemplateField>();
            var creatorID = new Teleform.Reporting.UniqueIDCreator();

            creatorID.Split(placeholders.First().ID, out entityID, out attributeID, out formatID);

            entity = Global.Schema.Entities.FirstOrDefault(o => o.ID.ToString() == entityID.ToString());

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

                if (field.Format == null)
                    throw new InvalidOperationException(
                        string.Format("Тип '{0}' атрибута {1} не имеет формат с идентификатором {2}.",
                            attribute.Type.Name, attribute.Name, formatID));

                field.Name = holder.Alias;

                fields.Add(field);
            }

#warning sheetName
            var template = new Teleform.Reporting.MicrosoftOffice.ExcelTemplate(name, entity, "ExcelBased", body, fields, "Лист1");
            return template;
        }

        private List<ExcelPlaceHolder> GetExcelPlaceHolders(byte[] body)
        {
            if (body == null)
                throw new ArgumentNullException("body",
                    "Параметр 'body' имеет значение null.");

            var placeholders = new List<ExcelPlaceHolder>();
            var stream = new MemoryStream(body);

            using (var doc = SpreadsheetDocument.Open(stream, false))
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
        #endregion Private Methods
    }
}