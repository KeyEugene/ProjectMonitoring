using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Teleform.Office.Reporting.Placeholders
{
    public class OpenXMLBookmarkPlaceholder : IPlaceholder
    {
        private WordprocessingDocument Document { get; set; }
        private Dictionary<PlaceholderData, BookmarkStart> Bookmarks { get; set; }

        public OpenXMLBookmarkPlaceholder( WordprocessingDocument document )
        {
            this.Document = document;
        }

        public IEnumerable<PlaceholderData> GetPlaceholders()
        {
            if ( Bookmarks == null )
            {
                Bookmarks = new Dictionary<PlaceholderData, BookmarkStart>();

                foreach ( BookmarkStart bookmarkStart in Document.MainDocumentPart.RootElement.Descendants<BookmarkStart>() )
                {
                    // В документе могут присутствовать "скрытые" закладки с именами, начинающимися с "_".
                    if ( !bookmarkStart.Name.ToString().StartsWith( "_" ) )
                        Bookmarks.Add(
                            new PlaceholderData { Name = bookmarkStart.Name.ToString(), Tag = "", Text = "" }, bookmarkStart );
                }
            }
            return Bookmarks.Keys.ToList();
        }

        public void FillPlaceholders( IDictionary<string, string> data )
        {
            //foreach ( KeyValuePair<string, string> kv in data )
            //{
            //    if ( Bookmarks.ContainsKey( kv.Key ) )
            //    {
            //        Bookmarks[kv.Key].Parent.InsertAfter( new Run( new Text( kv.Value ) ), Bookmarks[kv.Key] );
            //    }
            //}
        }

        public void InsertCustomXml(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}
