using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Office.Reporting
{
    public class OpenXMLBookmarkPlaceholder : IPlaceholder
    {
        private WordprocessingDocument Document { get; set; }
        private Dictionary<string, BookmarkStart> Bookmarks { get; set; }

        public OpenXMLBookmarkPlaceholder( WordprocessingDocument document )
        {
            this.Document = document;
        }

        public IEnumerable<string> GetPlaceholders()
        {
            if ( Bookmarks == null )
            {
                Bookmarks = new Dictionary<string, BookmarkStart>();

                foreach ( BookmarkStart bookmarkStart in Document.MainDocumentPart.RootElement.Descendants<BookmarkStart>() )
                {
                    // В документе могут присутствовать "скрытые" закладки с именами, начинающимися с "_".
                    if ( !bookmarkStart.Name.ToString().StartsWith( "_" ) )
                        Bookmarks.Add( bookmarkStart.Name.ToString(), bookmarkStart );
                }
            }
            return Bookmarks.Keys.ToList();
        }

        public void FillPlaceholders( IDictionary<string, string> data )
        {
            foreach ( KeyValuePair<string, string> kv in data )
            {
                if ( Bookmarks.ContainsKey( kv.Key ) )
                {
                    Bookmarks[kv.Key].Parent.InsertAfter( new Run( new Text( kv.Value ) ), Bookmarks[kv.Key] );
                }
            }
        }
    }
}
