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
	public class ExcelReportBuilder : IGroupReportBuilder
	{
		private string sheetName;
		
		public void Create(Stream output, GroupReport report)
		{
            ExcelTemplate template;

            if (report.Template is ExcelTemplate)
                template = report.Template as ExcelTemplate;
            else template = new ExcelTemplate(report.Template);

            sheetName = template.Sheet;

			var stream = new MemoryStream();

			if (report.Template.Content.Count() > 0)
				CreateFromTemplateWithBody(stream, report);
			else
				CreateFromEmptyTemplate(stream, report);

			var array = stream.ToArray();
			output.Write(array, 0, array.Length);
		}

		private void CreateFromTemplateWithBody(Stream output, GroupReport report)
		{
			var content = report.Template.Content; 
			output.Write(content, 0, content.Length);

			SpreadsheetDocument document = SpreadsheetDocument.Open(output, true);
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
		private void AddNewSheet(string sheetName, SpreadsheetDocument spreadsheetDocument, Stream stream, GroupReport gRreport)
		{
			if (spreadsheetDocument == null) throw new ArgumentNullException("spreadsheetDocument", string.Format("Параметр {0} имеет значение null.", "spreadsheetDocument"));
			if (gRreport == null) throw new ArgumentNullException("template", string.Format("Параметр {0} имеет значение null.", "template"));

			// добавить новый worksheet.
			WorksheetPart newWorksheetPart = spreadsheetDocument.WorkbookPart.AddNewPart<WorksheetPart>();
			SheetData sheetData = new SheetData();
			newWorksheetPart.Worksheet = new Worksheet(sheetData);
			newWorksheetPart.Worksheet.Save();

			Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>();
			string relationshipId = spreadsheetDocument.WorkbookPart.GetIdOfPart(newWorksheetPart);

			// создать уникальный ID для worksheet.
			uint sheetId = 1;
			if (sheets.Elements<Sheet>().Count() > 0)
			{
				sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;	
			}

			string	sheetN = sheetName;
			//sheetN = "Лист" + sheetId;			

			// Добавить новый worksheet и связать его с workbook
			Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetN };
			sheets.Append(sheet);
			spreadsheetDocument.WorkbookPart.Workbook.Save();

			//записать в Excel заголовки полей
			Row headerRow = new Row();
			addHeaderRow(headerRow, gRreport);
			sheetData.AppendChild(headerRow);

			//записать в Excel строки с данными
			addDataRows(sheetData, gRreport);
		}

		private void GenWorksheetPart( WorksheetPart worksheetPart, GroupReport gRreport)
		{
			if (worksheetPart == null) throw new ArgumentNullException("worksheetPart", string.Format("Параметр {0} имеет значение null.", "worksheetPart"));
			if (gRreport == null) throw new ArgumentNullException("template", string.Format("Параметр {0} имеет значение null.", "template"));

			Worksheet workSheet = new Worksheet();
			SheetData sheetData = new SheetData();

			//записать в Excel названия полей
			Row headerRow = new Row();
			addHeaderRow(headerRow, gRreport);
			sheetData.AppendChild(headerRow);

			//записать в Excel строки с данными
			addDataRows(sheetData, gRreport);

			workSheet.Append(sheetData);
			worksheetPart.Worksheet = workSheet;
		}

		private void addHeaderRow(Row headerRow, GroupReport gRreport)
		{
			foreach (TemplateField headerField in gRreport.Template.Fields.OrderBy(o => o.Order))
			{
				Cell cell = new Cell() { StyleIndex = (UInt32Value)0U };
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(headerField.Name);
				headerRow.AppendChild(cell);
			}

		}

		private void addDataRows(SheetData sheetData, GroupReport gRreport)
		{
			// бежим по строкам
			foreach (Instance instance in gRreport.Instances)
			{
				var row = new Row();

				Instance.Property property;

				foreach (TemplateField field in gRreport.Template.Fields)
				{
					property = instance.BaseProperties.First(o => o.Attribute.ID.ToString() == field.Attribute.ID.ToString());

					var cell = new Cell();
					var cellValue = new CellValue();

					if (field.Attribute.Type.Name.Contains("date"))
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
								//формат № 2 для денег FormatCode = "#,##0.00\"р.\""
								cell.StyleIndex = (UInt32Value)2U;  
								cell.DataType = CellValues.Number;
								cellValue.Text = string.Format(CultureInfo.InvariantCulture, "{0}", property.Value);
								break;
							case "{0}":
								//формат № 4 для чисел
								cell.StyleIndex = (UInt32Value)4U; 
								cell.DataType = CellValues.Number;
								cellValue.Text = string.Format(CultureInfo.InvariantCulture, "{0}", property.Value);
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
						// Формат № 0 по умолчанию общий
						cell.StyleIndex = (UInt32Value)0U; 
						cell.DataType = CellValues.String;
						cellValue.Text = string.Format(field.Format.Provider, field.Format.FormatString, property.Value);
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

			NumberingFormats numberingFormats1 = new NumberingFormats() { Count = (UInt32Value)2U };
			NumberingFormat numberingFormat1 = new NumberingFormat() { NumberFormatId = (UInt32Value)164U, FormatCode = "[$-F800]dddd\\,\\ mmmm\\ dd\\,\\ yyyy" };
			NumberingFormat numberingFormat2 = new NumberingFormat() { NumberFormatId = (UInt32Value)166U, FormatCode = "#,##0\"р.\"" }; 

			numberingFormats1.Append(numberingFormat1);
			numberingFormats1.Append(numberingFormat2);

			CellFormats cellFormats1 = new CellFormats() { Count = (UInt32Value)5U };
			CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U }; // Формат № 0 по умолчанию общий
			CellFormat cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)164U }; // Формат № 1 для дат FormatCode = "[$-F800]dddd\\,\\ mmmm\\ dd\\,\\ yyyy" 
			CellFormat cellFormat4 = new CellFormat() { NumberFormatId = (UInt32Value)166U }; //формат № 2 для денег FormatCode = "#,##0.00\"р.\"" 
			CellFormat cellFormat5 = new CellFormat() { NumberFormatId = (UInt32Value)14U }; //формат № 3 для дат * 14.03.2015
			CellFormat cellFormat6 = new CellFormat() { NumberFormatId = (UInt32Value)2U }; //формат № 4 для чисел

			cellFormats1.Append(cellFormat2);
			cellFormats1.Append(cellFormat3);
			cellFormats1.Append(cellFormat4);
			cellFormats1.Append(cellFormat5);
			cellFormats1.Append(cellFormat6);

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
