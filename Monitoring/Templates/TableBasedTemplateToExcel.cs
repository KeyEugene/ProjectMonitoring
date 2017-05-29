using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using Teleform.Reporting;
using Teleform.Reporting.MicrosoftOffice;
using DocumentFormat.OpenXml;
using Teleform.Reporting.constraint;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using Teleform.ProjectMonitoring.HttpApplication;

namespace Teleform.ProjectMonitoring.Templates
{
    public class TableBasedTemplateToExcel : BaseExcelBuilder
    {
        private List<Reporting.Attribute> Fields;
        public List<Entity> Entitys;

        protected override void CreateFromEmptyTemplate(Stream output, GroupReport report)
        {
            sheetName = report.Template.Name;

            using (SpreadsheetDocument document = SpreadsheetDocument.Create(output, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                GenWorkbookPart(sheetName, workbookPart);

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>("rId0");
                GenWorksheetPart(worksheetPart, report);
                WorksheetPart worksheetPart2 = workbookPart.AddNewPart<WorksheetPart>("rId1");
                GenWorksheetPartAddConnectionData(worksheetPart2, report);


                WorkbookStylesPart workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>("rId15");
                GenWorkbookStylesPart(workbookStylesPart);
            }

        }

        /// <summary>
        /// add connection data @data source=stend\sqlexpress_12; and  Initial Catalog = PMonitor;"
        /// </summary>
        /// <param name="worksheetPart2"></param>
        /// <param name="report"></param>
        private void GenWorksheetPartAddConnectionData(WorksheetPart worksheetPart2, GroupReport report)
        {
            if (worksheetPart2 == null) throw new ArgumentNullException("worksheetPart", "Параметр worksheetPart имеет значение null.");
            if (report == null) throw new ArgumentNullException("template", "Параметр template имеет значение null.");

            Worksheet workSheet = new Worksheet();
            SheetData sheetData = new SheetData();

            //записать в Excel названия полей
            Row headerRow = new Row();
            Cell headerCellDataSource = new Cell() { StyleIndex = (UInt32Value)0U };
            headerCellDataSource.DataType = CellValues.String;
            headerCellDataSource.CellValue = new CellValue("Server name");

            Cell headerCellInitialCatalog = new Cell() { StyleIndex = (UInt32Value)0U };
            headerCellInitialCatalog.DataType = CellValues.String;
            headerCellInitialCatalog.CellValue = new CellValue("Database name");

            headerRow.Append(headerCellDataSource);
            headerRow.Append(headerCellInitialCatalog);

            var sql = new System.Data.SqlClient.SqlConnection(Global.ConnectionString);

            Row row = new Row();
            Cell cellDataSource = new Cell() { StyleIndex = (UInt32Value)0U };
            cellDataSource.DataType = CellValues.String;
            cellDataSource.CellValue = new CellValue(sql.DataSource);

            Cell cellInitialCatalog = new Cell() { StyleIndex = (UInt32Value)0U };
            cellInitialCatalog.DataType = CellValues.String;
            cellInitialCatalog.CellValue = new CellValue(sql.Database);

            row.Append(cellDataSource);
            row.Append(cellInitialCatalog);

            sheetData.AppendChild(headerRow);
            sheetData.AppendChild(row);

            //записать в Excel строки с данными
            addDataRows(sheetData, report);
            workSheet.Append(sheetData);
            worksheetPart2.Worksheet = workSheet;

        }

        private void GetGroupReportByConstraint(out GroupReport groupReport, Constraint constraint)
        {
            var nameTable = constraint.RefTblName;

            var fields = Entitys.FirstOrDefault(x => x.SystemName == nameTable)
                .Attributes.Where(x => x.AppType == AppType.title || x.FPath == "objID")
                .Select(x => new TemplateField(x));

            var tmpTemplate = new Template(constraint.Alias, Entitys.FirstOrDefault(x => x.SystemName == nameTable), "TableBased", null, fields);

            var dt = Global.GetDataTable(string.Concat("EXEC [report].[getBObjectdata] '", nameTable, "', @flTitle=1, @flHeader=0"));
            //Делаем для того , что бы медот GroupReport.Make() вернул нам объекты вместе с objID, если этого не сдалать он обрежет колонку objID
            dt.Columns["objID"].ColumnName = tmpTemplate.Fields.FirstOrDefault(x => x.Attribute.FPath == "objID").Attribute.ID.ToString();

            groupReport = GroupReport.Make(tmpTemplate, dt);
        }

        protected override void GenWorkbookPart(string sheetName, WorkbookPart workbookPart)
        {
            Workbook workbook = new Workbook();
            Sheets sheets = new Sheets();

            Sheet sheet = new Sheet() { Name = sheetName, SheetId = (UInt32Value)1U, Id = "rId0" };
            Sheet sheet2 = new Sheet() { Name = "ConnectData", SheetId = (UInt32Value)2U, Id = "rId1" };
            sheets.Append(sheet);
            sheets.Append(sheet2);

            workbook.Append(sheets);
            workbookPart.Workbook = workbook;
        }

        protected override void addHeaderRow(Row headerRow, GroupReport report)
        {
            Fields = new List<Reporting.Attribute>(report.Template.Fields.OrderBy(o => o.Order).Select(x => x.Attribute)); //Добавили Атрибуты из Template

            //Формируем основной sheet или же sheet's with Constraint's
            var b = this.sheetName == report.Template.Name;

            for (int i = 0; i < Fields.Count; i++)
                headerRow.AppendChild(BuildHeaderCell(Fields[i].Name));
        }

        private OpenXmlElement BuildHeaderCell(string name)
        {
            Cell cell = new Cell() { StyleIndex = (UInt32Value)0U };
            cell.DataType = CellValues.String;
            cell.CellValue = new CellValue(name);

            return cell;
        }

        public override void addDataRows(SheetData sheetData, GroupReport report)
        {
            // бежим по строкам
            foreach (Instance instance in report.Instances)
            {
                var row = new Row();

                Instance.Property property;

                foreach (TemplateField field in report.Template.Fields)
                {
                    if (field.IsVisible)
                    {
                        property = instance.OwnProperties.First(o => o.Attribute.ID.ToString() == field.Attribute.ID.ToString());

                        var cell = new Cell();
                        var cellValue = new CellValue();

                        ApplyStyle(property, field, ref cell, ref cellValue);

                        cell.Append(cellValue);
                        row.AppendChild(cell);
                    }
                }
                sheetData.AppendChild(row);
            }
        }
    }
}