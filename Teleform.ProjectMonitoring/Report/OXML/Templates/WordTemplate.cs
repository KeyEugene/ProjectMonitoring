using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Report.enums;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Report.OXML.Templates
{
    public class WordTemplate : BaseTemplate
    {
        private Dictionary<string, BookmarkStart> _bookmarks;
        public List<string> Bookmarks
        {
            get
            {
                if (_bookmarks != null)
                    return _bookmarks.Keys.ToList();
                else
                    return new List<string>() { "empty" };
            }
        }

        private WordprocessingDocument document;
        private static MemoryStream memory;

        public WordTemplate(string templatePath)
        {
            this.Type = TemplateType.WordTemplate;
            this.TemplatePath = templatePath;
            _bookmarks = new Dictionary<string, BookmarkStart>();

            byte[] docArray;
            // 
            try
            {
                docArray = File.ReadAllBytes(templatePath);
            }
            catch (Exception ex)
            {
                this.Dispose();
                throw new Exception(ex.Message);
            }

            memory = new MemoryStream();
            memory.Write(docArray, 0, (int)docArray.Length);

            document = WordprocessingDocument.Open(memory, true);
            document.ChangeDocumentType(DocumentFormat.OpenXml.WordprocessingDocumentType.Document);
        }

        public override List<string> GetBookmarksList()
        {
            var bmList = new List<string>();

            foreach (BookmarkStart bookmarkStart in document.MainDocumentPart.RootElement.Descendants<BookmarkStart>())
            {
                // В документе могут присутствовать "скрытые" закладки с именами, начинающимися с "_".
                if (!bookmarkStart.Name.ToString().StartsWith("_"))
                    _bookmarks.Add(bookmarkStart.Name.ToString(), bookmarkStart);
            }
            
            return Bookmarks;
        }

        public override void EvaluateBookmarks(Dictionary<string, string> values)
        {
            foreach (KeyValuePair<string, string> kv in values)
            {
                if (_bookmarks.ContainsKey(kv.Key))
                {
                    _bookmarks[kv.Key].Parent.InsertAfter(new Run(new Text(kv.Value)), _bookmarks[kv.Key]);
                }
            }
        }

        public override void EvaluateBookmarks()
        {
            foreach (var item in _bookmarks)
            {
                if (_bookmarks.ContainsKey(item.Key))
                {
                    _bookmarks[item.Key].Parent.InsertAfter(new Run(new Text(item.Key)), _bookmarks[item.Key]);
                }
            }
        }

        public override void SaveToDocument(string destination, string documentName)
        {
            MainDocumentPart mainPart = document.MainDocumentPart;
            DocumentSettingsPart documentSettingsPart1 = mainPart.DocumentSettingsPart;

            // 
            AttachedTemplate attachedTemplate1 = new AttachedTemplate() { Id = "relationId1" };
            documentSettingsPart1.Settings.Append(attachedTemplate1);

            documentSettingsPart1.AddExternalRelationship(
                "http://schemas.openxmlformats.org/officeDocument/2006/relationships/attachedTemplate",
                new Uri(TemplatePath, UriKind.Absolute),
                "relationId1");
            mainPart.Document.Save();

            document.Close();

            using (FileStream fs = new FileStream(Path.Combine(destination, documentName),FileMode.Create))
            {
#warning См. комментарий
                // Цитата со StackOverflow:
                // Use CopyTo instead, there is a bug in WriteTo which makes it fail to write the entire content of the buffer when the target stream does not support writing everything in one go.
                memory.WriteTo(fs);
            }

            memory.Close();
            IsDisposed = true;
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
                    if (document != null) document.Close();
                    if (memory != null) memory.Close();
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
