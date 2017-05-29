using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;

namespace Report
{
    public class WordBookmarkFiller : IBookmarkFiller
    {
        public byte[] GenerateDocument(IDictionary<string, string> values, string fileName)
        {
            if (values == null)
                throw new ArgumentException("Отсутствуют значения в словаре");
            if (!File.Exists(fileName))
                throw new ArgumentException("Файл \"" + fileName + "\" не существует");
            var tempFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".docx");
            return CreateFile(values, fileName, tempFileName);
        }

        private static byte[] CreateFile(IDictionary<string, string> values, string fileName, string tempFileName)
        {
            File.Copy(fileName, tempFileName);
            if (!File.Exists(tempFileName))
                throw new ArgumentException("Невозможно создать файл: " + tempFileName);

            using (var doc = WordprocessingDocument.Open(tempFileName, true))
            {
                if (doc.MainDocumentPart.HeaderParts != null)
                    foreach (var header in doc.MainDocumentPart.HeaderParts)
                        RenameBookmarks(values, DocumentSection.Header, header);

                RenameBookmarks(values, DocumentSection.Main, doc.MainDocumentPart);

                if (doc.MainDocumentPart.FooterParts != null)
                    foreach (var footer in doc.MainDocumentPart.FooterParts)
                        RenameBookmarks(values, DocumentSection.Footer, footer);
            }
            byte[] result = null;
            if (File.Exists(tempFileName))
            {
                result = File.ReadAllBytes(tempFileName);
                File.Delete(tempFileName);
            }
            return result;
        }

        private enum DocumentSection { Main, Header, Footer };
        private static void RenameBookmarks(IDictionary<string, string> values, DocumentSection documentSection, object section)
        {
            IEnumerable<BookmarkStart> bookmarks = null;

            switch (documentSection)
            {
                case DocumentSection.Main:
                    bookmarks = ((MainDocumentPart)section).Document.Body.Descendants<BookmarkStart>();
                    break;
                case DocumentSection.Header:
                    bookmarks = ((HeaderPart)section).RootElement.Descendants<BookmarkStart>();
                    break;
                case DocumentSection.Footer:
                    bookmarks = ((FooterPart)section).RootElement.Descendants<BookmarkStart>();
                    break;
            }

            if (bookmarks == null)
                return;

            foreach (var bmStart in bookmarks)
            {
                if (!values.ContainsKey(bmStart.Name))
                    continue;
                var bmText = values[bmStart.Name];
                BookmarkEnd bmEnd = null;

                switch (documentSection)
                {
                    case DocumentSection.Main:
                        bmEnd = (((MainDocumentPart)section).Document.Body.Descendants<BookmarkEnd>().Where(b => b.Id == bmStart.Id.ToString())).FirstOrDefault();
                        break;
                    case DocumentSection.Header:
                        bmEnd = (((HeaderPart)section).RootElement.Descendants<BookmarkEnd>().Where(b => b.Id == bmStart.Id.ToString())).FirstOrDefault();
                        break;
                    case DocumentSection.Footer:
                        bmEnd = (((FooterPart)section).RootElement.Descendants<BookmarkEnd>().Where(b => b.Id == bmStart.Id.ToString())).FirstOrDefault();
                        break;
                }

                if (bmEnd == null)
                    continue;

                var rProp = bmStart.Parent.Descendants<Run>().Where(rp => rp.RunProperties != null).Select(rp => rp.RunProperties).FirstOrDefault();
                if (bmStart.PreviousSibling<Run>() == null && bmEnd.ElementsAfter().Count(e => e.GetType() == typeof(Run)) == 0)
                {
                    bmStart.Parent.RemoveAllChildren<Run>();
                }
                else
                {
                    var list = bmStart.ElementsAfter().Where(r => r.IsBefore(bmEnd)).ToList();
                    var trRun = list.Where(rp => rp.GetType() == typeof(Run) && ((Run)rp).RunProperties != null).Select(rp => ((Run)rp).RunProperties).FirstOrDefault();
                    if (trRun != null)
                        rProp = (RunProperties)trRun.Clone();
                    for (var n = list.Count(); n > 0; n--)
                        list[n - 1].Remove();
                }

                bmText = values[bmStart.Name];
                if (!string.IsNullOrEmpty(bmText) && bmText.Contains(Environment.NewLine))
                {
                    var insertElement = bmStart.Parent.PreviousSibling();
                    var rows = bmText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    foreach (var row in rows)
                    {
                        var np = new Paragraph();
                        var nRun = new Run();
                        if (rProp != null)
                            nRun.RunProperties = (RunProperties)rProp.Clone();
                        nRun.AppendChild(new Text(row));
                        np.AppendChild(nRun);
                        if (insertElement.Parent != null)
                            insertElement.InsertAfterSelf(np);
                        else
                            insertElement.Append(np);
                        insertElement = np;
                    }
                }
                else
                {
                    var nRun = new Run();
                    if (rProp != null)
                        nRun.RunProperties = (RunProperties)rProp.Clone();
                    nRun.Append(new Text(bmText));
                    bmStart.InsertAfterSelf(nRun);
                }
            }
        }


        // использование:
        //var bookMarks = FindBookmarks(doc.MainDocumentPart.Document);

        //foreach( var end in bookMarks )
        //{
        //    var textElement = new Text("asdfasdf");
        //    var runElement = new Run(textElement);

        //    end.Value.InsertAfterSelf(runElement);
        //}
        private static Dictionary<string, BookmarkEnd> FindBookmarks(OpenXmlElement documentPart, Dictionary<string, BookmarkEnd> results = null, Dictionary<string, string> unmatched = null)
        {
            results = results ?? new Dictionary<string, BookmarkEnd>();
            unmatched = unmatched ?? new Dictionary<string, string>();

            foreach (var child in documentPart.Elements())
            {
                if (child is BookmarkStart)
                {
                    var bStart = child as BookmarkStart;
                    unmatched.Add(bStart.Id, bStart.Name);
                }

                if (child is BookmarkEnd)
                {
                    var bEnd = child as BookmarkEnd;
                    foreach (var orphanName in unmatched)
                    {
                        if (bEnd.Id == orphanName.Key)
                            results.Add(orphanName.Value, bEnd);
                    }
                }
                FindBookmarks(child, results, unmatched);
            }
            return results;
        }

        private static void InsertAfterBookmarks(OpenXmlElement documentPart, Dictionary<string,string> values)
        {
            var bookmarks = FindBookmarks(documentPart);

            foreach (var end in bookmarks)
            {
                var textElement = new Text("");
                var runElement = new Run(textElement);
                end.Value.InsertAfterSelf(runElement);
            }
        }

        private void CreateDocFromTemplate()
        {
            //string srcFile = template.TemplatePath;
            //string destFile = Path.Combine(Environment.CurrentDirectory, DocumentName);

            //try
            //{
            //    File.Copy(srcFile, destFile, true);

            //    using (WordprocessingDocument document =
            //        WordprocessingDocument.Open(destFile, true))
            //    {
            //        document.ChangeDocumentType(DocumentFormat.OpenXml.WordprocessingDocumentType.Document);
            //        MainDocumentPart mainPart = document.MainDocumentPart;
            //        DocumentSettingsPart documentSettingsPart1 = mainPart.DocumentSettingsPart;

            //        AttachedTemplate attachedTemplate1 = new AttachedTemplate() { Id = "relationId1" };
            //        documentSettingsPart1.Settings.Append(attachedTemplate1);

            //        documentSettingsPart1.AddExternalRelationship(
            //            "http://schemas.openxmlformats.org/officeDocument/2006/relationships/attachedTemplate",
            //            new Uri(srcFile, UriKind.Absolute),
            //            "relationId1");

            //        mainPart.Document.Save();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}

        }
    }
}
