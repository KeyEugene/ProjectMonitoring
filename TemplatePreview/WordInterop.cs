using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Reflection;

namespace Teleform.Office.TemplatePreview
{
    public interface IPreview : IDisposable 
    {
        /// <summary>
        /// пересохраняет файл с расширением html
        /// </summary>
        /// <param name="path">
        /// путь к документу в папке с расширением как в базе данных 
        /// </param>
        void SaveWithHtmlExtension(string path);
    }

    public class WordInterop : IPreview
    {
        private object missing = Missing.Value;
        private Word.Application ap = null;
        private Word.Document doc = null;

        public void SaveWithHtmlExtension(string path)
        {
            try
            {
                ap = new Word.Application();
                doc = new Word.Document();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException + ex.Message);
            }
            try
            {
                doc = ap.Documents.Open(path, ReadOnly: false, Visible: true);
            }
            catch(Exception ex) 
            {
                this.Dispose();
                throw new Exception("Не удалось открыть файл в исходном формате" + ex.Message);
            }

            try
            {
                doc.SaveAs(Path.ChangeExtension(path, "html"), Word.WdSaveFormat.wdFormatHTML, true, missing, true, missing, false, missing, missing, missing,
                missing, missing, missing, missing, missing, missing);
            }
            catch (Exception ex) 
            {
                this.Dispose();
                throw new Exception("Не удалось сохранить файл в формате .html" + ex.Message);
            }
        }

        private void Cleanup()
        {
            doc.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
            ap.Application.Quit(Word.WdSaveOptions.wdDoNotSaveChanges);
        }

        public void Dispose()
        {
            Cleanup();
            GC.SuppressFinalize(this);
        }
    }

    public class ExcelInterop : IPreview
    {

        private object missing = Missing.Value;
        private Excel.Application ap = null;
        private Excel.Workbook wb = null;

        public void SaveWithHtmlExtension(string path) 
        {
            try
            {
                ap = new Excel.Application();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException + ex.Message);
            }
            try
            {
                wb = ap.Workbooks.Open(path, ReadOnly: false);
            }
            catch (Exception ex)
            {
                this.Dispose();
                throw new Exception("Не удалось откырть файл в исходном формате" + ex.Message);
            }


            try 
            {
                wb.SaveAs(Path.ChangeExtension(path, "html"), Excel.XlFileFormat.xlHtml, missing, missing, missing, missing,  Excel.XlSaveAsAccessMode.xlNoChange, missing, missing, missing, missing, missing);
            }
            catch (Exception ex)
            {
                this.Dispose();
                throw new Exception("Не удалось сохранить файл в формате .html" + ex.Message);
            }
        }

        private void Cleanup()
        {
            wb.Close(missing, missing, missing);
            ap.Application.Quit();
        }

        public void Dispose()
        {
            Cleanup();
            GC.SuppressFinalize(this);
        }
    }
}