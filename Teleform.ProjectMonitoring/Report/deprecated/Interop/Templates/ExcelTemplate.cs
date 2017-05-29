using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace Report.Interop.Templates
{
    public class ExcelTemplate : BaseTemplate
    {
        public List<Dictionary<string,string>> SheetData = new List<Dictionary<string,string>>();

        public ExcelTemplate(string templatePath, string name, string location)
        {
            this.DocumentName = name;
            this.Location = location;
            this.TemplatePath = templatePath;

            CreateSpreadsheetFromTemplate(templatePath);
        }

        private void CreateSpreadsheetFromTemplate(string templatePath)
        {
            var oExcel = new Microsoft.Office.Interop.Excel.Application();
            var oWorkbook = oExcel.Workbooks.Open(templatePath);

            //object oTemplate = templatePath;
            //oWorkbook = oExcel.Workbooks.Add(templatePath);

            try
            {
                oWorkbook.SaveAs(FullPath);
            }
            catch (Exception)
            {
                throw new Exception("Не удалось сформировать таблицу из шаблона.");
            }

            oWorkbook.Close();
            oExcel.Application.Quit();
        }

        public void EvaluateData()
        {
            var oExcel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook oWorkbook;
            object isVisible = false;
            //object fileName = FullPath;
            object saveFile;

            try
            {
                oWorkbook = oExcel.Workbooks.Open(FullPath);
                Worksheet oSheet = (Worksheet)oWorkbook.ActiveSheet;
                Range oRange = oSheet.UsedRange;

                int Row = 0;
                int Col = 0;

                //Заголовки
                foreach (var sheetDataUnit in SheetData[Row])
                {
                    Col++;
                    oExcel.Cells[1,Col] = sheetDataUnit.Key;
                }

                foreach (var item in SheetData)
                {
                    Row++;
                    Col = 0;
                    //foreach (var cell in item[Row])
                }
            }
            catch (Exception)
            {
                throw new Exception("Не удалось открыть таблицу.");
            }

            try
            {
                oWorkbook.SaveAs(FullPath);
            }
            catch (Exception)
            {
                throw new Exception("Ошибка сохранения отчета.");
            }

        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
