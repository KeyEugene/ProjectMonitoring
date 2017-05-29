using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Microsoft.Office.Interop.Word;
using Report.enums;


namespace Report.Interop.Templates
{
    public class WordTemplate : BaseTemplate
    {
        private static Application oWord = null;
        private static Document oDoc = null;
        private static Microsoft.Office.Interop.Word.Bookmarks oBookmarks = null;

        private List<string> _bookmarks;
        public List<string> Bookmarks
        {
            get
            {
                return _bookmarks;
            }
            private set
            {
                value = _bookmarks;
            }
        }

        private object _missing = Missing.Value;

        //private Dictionary<string, string> Bookmarks = null;

        public WordTemplate(string templatePath)
        {
            this.Type = TemplateType.WordTemplate;
            this.TemplatePath = templatePath;

            oWord = new Application();
            oDoc = new Document();

            object readOnly = false;
            object isVisible = false;
            object oTemplate = TemplatePath;

            Microsoft.Office.Interop.Word.Documents oDocTmp = oWord.Documents;
            oWord.Visible = false;

            try
            {
                oDoc = oDocTmp.Open(ref oTemplate,
                                    ref _missing,
                                    ref readOnly,
                                    ref _missing,
                                    ref _missing,
                                    ref _missing,
                                    ref _missing,
                                    ref _missing,
                                    ref _missing,
                                    ref _missing,
                                    ref _missing,
                                    ref isVisible,
                                    ref _missing,
                                    ref _missing,
                                    ref _missing,
                                    ref _missing);
            }
            catch (Exception ex)
            {
                this.Dispose();
                throw new Exception("Не удалось открыть документ.\r\n" + ex.Message);
            }
            oBookmarks = oDoc.Bookmarks;
        }

        public override List<string> GetBookmarksList()
        {
            var bmList = new List<string>();

            for (int i = 1; i <= oBookmarks.Count; i++)
            {
                object oI = i;
                try
                {
                    var bm = oBookmarks.get_Item(ref oI).Name;
                    bmList.Add(bm);
                }
                catch (Exception ex)
                {
                    bmList.Add("Не удалось считать закладку.\r\n" + ex.Message);
                }

            }

            this._bookmarks = bmList;
            return bmList;
        }

        public override void EvaluateBookmarks(Dictionary<string, string> values)
        {
            try
            {
                foreach (var bm in values)
                {
                    object oBookmark = bm.Key;
                    oBookmarks[oBookmark].Range.Text = bm.Value;
                }
            }
            catch (Exception)
            {
                throw new Exception("Ошибка заполнения данных.");
            }
        }

        public override void EvaluateBookmarks()
        {
            try
            {
                foreach ( var bm in _bookmarks )
                {
                    object oBookmark = bm;
                    oBookmarks[oBookmark].Range.Text = bm;
                }
            }
            catch ( Exception )
            {
                throw new Exception( "Ошибка заполнения данных." );
            }
        }

        public override void SaveToDocument(string destination, string documentName)
        {
            var fullPath = Path.Combine(destination, documentName);

            try
            {
                object o1 = Missing.Value,
                    o2 = Missing.Value,
                    o3 = Missing.Value,
                    o4 = Missing.Value,
                    o5 = Missing.Value,
                    o6 = Missing.Value,
                    o7 = Missing.Value,
                    o8 = Missing.Value,
                    o9 = Missing.Value,
                    o10 = Missing.Value,
                    o11 = Missing.Value,
                    o12 = Missing.Value,
                    o13 = Missing.Value,
                    o14 = Missing.Value;

                oDoc.SaveAs(fullPath,
                             WdSaveFormat.wdFormatDocumentDefault,
                             ref o1,
                             ref o2,
                             false,
                             ref o3,
                             ref o4,
                             ref o5,
                             ref o6,
                             ref o7,
                             ref o8,
                             ref o9,
                             ref o10,
                             ref o11,
                             ref o12,
                             ref o13
                           );
            }
            catch (Exception ex)
            {
                this.Dispose();
                throw new Exception("Не удалось сохранить документ.\r\n" + ex.Message);
            }
        }

        #region Disposing
        public override void Dispose()
        {
            Cleanup(true);
            GC.SuppressFinalize(this);
        }

        private void Cleanup(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (disposing)
                {
                    // Освобождение управляемых ресурсов.
                }
                // Очистка неуправляемых ресурсов.
                if (oDoc != null)
                {
                    oDoc.Close(WdSaveOptions.wdDoNotSaveChanges);
                    oWord.Application.Quit(WdSaveOptions.wdDoNotSaveChanges);
                    oDoc = null;
                }
            }
            IsDisposed = true;
        }

        ~WordTemplate()
        {
            Cleanup(false);
        }
        #endregion
    }
}
