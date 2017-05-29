using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Word;
using System.Reflection;

namespace Teleform.Office.Reporting
{
    public class MSWordTemplate : BaseWordTemplate
    {
        protected Application WordApplication { get; set; }

        protected Document Document { get; set; }

        private List<string> bookmarkNames;

        public MSWordTemplate(byte[] body)
            : base(body)
        {
            throw new NotImplementedException("В процессе разработки.");
        }

        public MSWordTemplate(string path)
            : base(path)
        {
            WordApplication = new Application();

            object oPath = path, readOnly = false, isVisible = false;
            object missing = Missing.Value;

            try
            {
                Document = WordApplication.Documents.Open(
                    ref oPath,
                    ref missing,
                    ref readOnly,
                    ref missing,
                    ref missing,
                    ref missing,
                    ref missing,
                    ref missing,
                    ref missing,
                    ref missing,
                    ref missing,
                    ref isVisible,
                    ref missing,
                    ref missing,
                    ref missing,
                    ref missing);
            }
            catch (Exception ex)
            {
                this.Dispose();
                throw new Exception("Не удалось открыть документ.\r\n" + ex.Message);
            }
        }

        [Obsolete("Зачем-то создаётся Document.")]
        public override IEnumerable<string> GetBookmarks()
        {
            if (bookmarkNames == null)
            {
                bookmarkNames = new List<string>();

                //Document = new Microsoft.Office.Interop.Word.Document();
                var bookmarks = Document.Bookmarks;

                for (int i = 1; i <= bookmarks.Count; i++)
                    try
                    {
                        object oI = i;
                        bookmarkNames.Add(bookmarks.get_Item(ref oI).Name);
                    }
                    catch (Exception ex) { throw ex; }
            }

            return bookmarkNames;
        }

        public override void FillBookmarks(IDictionary<string, string> data)
        {
            try
            {
                var bookmarks = Document.Bookmarks;

                foreach (var o in data)
                {
                    object oBookmark = o.Key;
                    bookmarks[oBookmark].Range.Text = o.Value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка заполнения данных.\r\n" + ex.Message);
            }
        }

        public override void Save(string path)
        {
            var path1 = path;   // На всякий случай, т.к. Interop работает со ссылочными переменными.

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

                Document.SaveAs(path1,
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
                    ref o13);
            }
            catch (Exception ex)
            {
                this.Dispose();
                throw new Exception("Не удалось сохранить документ.\r\n" + ex.Message);
            }
        }

        protected override void Cleanup(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (Document != null)
                {
                    Document.Close(WdSaveOptions.wdDoNotSaveChanges);
                    WordApplication.Application.Quit(WdSaveOptions.wdDoNotSaveChanges);
                }
            }

            IsDisposed = true;
        }

        ~MSWordTemplate()
        {
            Cleanup(false);
        }
    }
}
