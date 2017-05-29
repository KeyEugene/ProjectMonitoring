using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;


namespace Report.Documents
{
    public class WordDocument : BaseDocument
    {
        public Dictionary<string, string> Bookmarks = new Dictionary<string, string>();

        public WordDocument(string templatePath, string name, string location)
        {
            this.DocumentName = name;
            this.Location = location;
            //this.FullPath = Path.Combine(Location, DocumentName);
            this.TemplatePath = templatePath;

            CreateDocumentFromTemplate(templatePath);
        }


        // Создаем документ из шаблона.
        private void CreateDocumentFromTemplate(string templatePath)
        {
            var oWord = new Microsoft.Office.Interop.Word.Application();
            var oDoc = new Microsoft.Office.Interop.Word.Document();

            object oTemplate = templatePath;
            oDoc = oWord.Documents.Add(ref oTemplate,
                                        Missing.Value,
                                        Missing.Value);
            try
            {
                oDoc.SaveAs2(FullPath);
            }
            catch (Exception)
            {
                throw new Exception("Не удалось сформировать документ из шаблона.");
            }

            oDoc.Close();
            oWord.Application.Quit();
        }

        public void EvaluateBookmarks()
        {
            var oWord = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document oDoc;
            object isVisible = false;
            object fileName = FullPath;
            object saveFile;

            try
            {
                oDoc = oWord.Documents.Open(ref fileName);
                oDoc.Activate();
            }
            catch (Exception)
            {
                throw new Exception("Не удалось открыть документ.");
            }

            try
            {
                foreach (var bm in Bookmarks)
                {
                    object oBookmark = bm.Key;
                    oDoc.Bookmarks[oBookmark].Range.Text = bm.Value;
                }
            }
            catch (Exception)
            {
                throw new Exception("Ошибка заполнения данных.");
            }

            try
            {
                oDoc.SaveAs2(FullPath);
            }
            catch (Exception)
            {
                throw new Exception("Ошибка сохранения отчета.");
            }

            oDoc.Close();
            oWord.Application.Quit();
        }

    }
}
