#define OXML1

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.IO.Compression;
//using Ionic.Zip;
//using Shell32;

#if OXML
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
#endif

namespace Report
{
    public class Report
    {
        public enum ReportType
        {
            ContractWord,
            ContractExcel,
            DivisionWord,
            DivisionExcel,
            PersonWord,
            PersonExcel
        }

        private static void FileWriteExcel(string[] colunmNames, string[] columnValues, string path, string templatePath, Microsoft.Office.Interop.Excel.Application oExcel)
        {

            Workbook wb=oExcel.Workbooks.Open(templatePath);
            int i = 1;
            try
            {
                for (i = 1; i < colunmNames.Length; i++)
                {
                    object cell = colunmNames[i];
                    oExcel.get_Range(cell).Value2 = columnValues[i];                    
                }
            }
            catch (Exception)
            {
                throw new Exception(string.Format("Шаблон не соответствует, отсутствует ячейка {0}", colunmNames[i]));
            }
            try
            {
                wb.SaveAs(path);
                wb.Close();
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Ошибка сохранения отчета {0}/n {1}", path,e.Message));
            }
        }
        private static void FileWriteWord(string[] colunmNames, string[] columnValues, string path, string templatePath, Microsoft.Office.Interop.Word.Application oWord)
        {
            
            Microsoft.Office.Interop.Word.Document oDoc = new Microsoft.Office.Interop.Word.Document();
            object oTemplate = templatePath;

            // Создание документа на основе .dotx шаблона
            oDoc = oWord.Documents.Add(ref oTemplate, Missing.Value,
                Missing.Value, Missing.Value);
            int i=1;
            try
            {
                for (i = 1; i < colunmNames.Length; i++)
                {
                    object oBookMark = colunmNames[i];  
                    oDoc.Bookmarks[oBookMark].Range.Text = columnValues[i];
                }
            }
            catch(Exception)
            {
                throw new Exception(string.Format("Шаблон не соответствует, отсутствует закладка {0}", colunmNames[i]));
            }

            try
            {
                throw new Exception("The code was commented,");
                //oDoc.SaveAs2(path);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Ошибка сохранения отчета {0}/n {1}", path, e.Message));
            }
            oDoc.Close();

            /*string[] lines = new string[20];
            for (int i = 0; i < colunmNames.Length;i++)
            {
                lines[i] = string.Format("{0,-20} {1}", colunmNames[i], columnValues[i]);
            }
            System.IO.File.WriteAllLines(path, lines, System.Text.Encoding.Default);*/

        }

        static string zipPath;
        public static void CreateReportsToSend(GridView grid, List<ReportType> typeList, SqlConnection connection,string rootPath, out string filePath)
        {
            CreateReports(grid, typeList, connection,rootPath);
            //byte[] emptyzip = new byte[]{80,75,5,6,0,0,0,0,0, 
            //      0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
            string destPath = Path.Combine(zipPath,"Report.zip");
            //FileStream fs = File.Create(destPath);
            /*fs.Write(emptyzip, 0, emptyzip.Length);
            fs.Flush();
            fs.Close();
            fs = null;
            ShellClass sc = new ShellClass();
            Folder SrcFlder = sc.NameSpace(zipPath);            
            Folder DestFlder = sc.NameSpace(destPath);
            FolderItems items = SrcFlder.Items();
            DestFlder.CopyHere(items,20);
            data = File.ReadAllBytes(destPath);*/

            //using (ZipFile zip = new ZipFile(destPath, System.Text.Encoding.GetEncoding("cp866")))
            //{
            //    zip.AlternateEncoding = System.Text.Encoding.GetEncoding("cp866");
            //    zip.AlternateEncodingUsage = ZipOption.Always;
            //    DirectoryInfo di = new DirectoryInfo(zipPath);
            //    foreach(FileInfo fi in di.GetFiles())
            //    {
            //        if(fi.Attributes.HasFlag(FileAttributes.Hidden)||fi.Extension=="zip")continue;
            //        zip.AddFile(fi.FullName,"");
            //    }
            //    zip.Save();
            //}
            filePath = destPath;
        }

        public static void CreateReports(GridView grid, List<ReportType> typeList, SqlConnection connection,string rootPath)
        {
            if (typeList.Count == 0) return;
            var conn = connection;
            string path;
            using (var cmd = new SqlCommand("select [value] from [Param] where code='ReportsDir'", conn))
            {
                conn.Open();
                path = Path.Combine(rootPath,(string)cmd.ExecuteScalar());

                string folderName;
                folderName = DateTime.Now.ToString();
                folderName = folderName.Replace(":", "_");
                Directory.CreateDirectory(Path.Combine(path, folderName));
                zipPath = Path.Combine(path, folderName);

                string tableName = "";
                string templateNameWord = "";
                string templateNameExcel = "";
                bool word=false;
                bool excel=false;

                Microsoft.Office.Interop.Word.Application oWord=null;
                Microsoft.Office.Interop.Excel.Application oExcel=null;

                if (typeList.Contains(ReportType.ContractWord))
                {
                    tableName = "VO__Contract_Report";
                    templateNameWord = "Contract.dotx";
                    word = true;
                }
                else if (typeList.Contains(ReportType.DivisionWord))
                {
                    tableName = "VO__Division_Report";
                    templateNameWord = "Division.dotx";
                    word = true;
                }
                else if (typeList.Contains(ReportType.PersonWord))
                {
                    tableName = "VO__Person_Report";
                    templateNameWord = "Person.dotx";
                    word = true;
                }
                if (typeList.Contains(ReportType.ContractExcel))
                {
                    tableName = "VO__Contract_Report";
                    templateNameExcel = "Contract.xltx";
                    excel = true;

                }
                else if (typeList.Contains(ReportType.DivisionExcel))
                {
                    tableName = "VO__Division_Report";
                    templateNameExcel = "Division.xltx";
                    excel = true;
                }
                else if (typeList.Contains(ReportType.PersonExcel))
                {
                    tableName = "VO__Person_Report";
                    templateNameExcel = "Person.xltx";
                    excel = true;
                }
                
                // Работа с документом
                if(word) oWord = new Microsoft.Office.Interop.Word.Application();
                if (excel)
                {
                    oExcel = new Microsoft.Office.Interop.Excel.Application();
                    oExcel.DisplayAlerts = false;
                }

                
                foreach (GridViewRow row in grid.Rows)
                {
                    int id = int.Parse(row.Cells[0].Text);
                    cmd.CommandText = string.Format("select * from [{0}] where objID='{1}'", tableName, id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    if (!reader.HasRows) { reader.Close(); continue; }
                    
                    string[] cellTexts = new string[reader.FieldCount];
                    string[] cellNames = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        cellTexts[i] = Convert.ToString(reader.GetValue(i));
                        cellNames[i] = reader.GetName(i);
                    }
                    if (word)
                    {
                        string fname = reader.GetString(1) + ".doc";
                        FileWriteWord(cellNames, cellTexts, Path.Combine(path, folderName, fname), Path.Combine(path, "Templates", templateNameWord), oWord);
                    }
                    if (excel)
                    {
                        string fname = reader.GetString(1) + ".xlsx";
                        FileWriteExcel(cellNames, cellTexts, Path.Combine(path, folderName, fname), Path.Combine(path, "Templates", templateNameExcel), oExcel);
                    }                        
                    reader.Close();
                }

                if(word)oWord.Quit();
                if(excel)oExcel.Quit();

            }
        }

#if OXML
        #region OpenXml FTW!
        public static void CreateOXmlTest(string docName)
        {
            using (WordprocessingDocument package = WordprocessingDocument.Create(docName, WordprocessingDocumentType.Template))
            {
                package.AddMainDocumentPart();

                package.MainDocumentPart.Document =
                    new DocumentFormat.OpenXml.Wordprocessing.Document(
                        new Body(
                            new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                                new Run(
                                    new Text("TEST TEST TEST")))));

                package.MainDocumentPart.Document.Save();
            }
        }

        #endregion OpenXml FTW!
#endif
        public static void TestWordDoc()
        {
            //var templatePath = @"c:\!!!Work_Projects\МинПромТорг\workable\MonitoringMinProm\MonitoringMinProm\Resources\Reports\Templates\Contract.dotx";
            //var name = @"42.docx";
            //var location = @"c:\!!!Work_Projects\МинПромТорг\workable\MonitoringMinProm\OpenXML.Test\bin\Debug";
            //var wd = new Documents.WordTemplate(templatePath, name, location);

            //var tmpDict = new Dictionary<string, string>();
            //tmpDict.Add("tmpExecutive", "Пупкин");
            //tmpDict.Add("startYear", "1999");
            //tmpDict.Add("name", "Болт 19х5 с резьбой");
            //wd.Bookmarks = tmpDict;
            //wd.EvaluateBookmarks();

            //var tmpList = new List<Dictionary<string, string>>();
            //tmpList.Add(tmpDict);

            //var excelTemplatePath = @"c:\!!!Work_Projects\МинПромТорг\workable\MonitoringMinProm\MonitoringMinProm\Resources\Reports\Templates\Contract.xltx";
            //var excelName = @"42.xlsx";
            //var excelLocation = @"c:\!!!Work_Projects\МинПромТорг\workable\MonitoringMinProm\OpenXML.Test\bin\Debug";

            //var ed = new Documents.ExcelDocument(excelTemplatePath, excelName, excelLocation);
            //ed.SheetData = tmpList;
            //ed.EvaluateData();
            //Entities.EntityGenerator.Connection = new SqlConnection(@"data source=stend\SQLEXPRESS; Initial Catalog = MinProm; User Id=sa; Password=345;");
            //var bar = new Entities.EntityGenerator();
            //var foo = Entities.EntityGenerator.GetEntities();
        }
    }

}