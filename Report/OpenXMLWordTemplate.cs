using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Teleform.Office.Reporting.Placeholders;
using System.Xml.Linq;
using System.Xml;
using Teleform.Office.Reporting.ExtensionMethods;

namespace Teleform.Office.Reporting
{
    public class OpenXMLWordTemplate : BaseWordTemplate
    {
        private static XNamespace w=
            "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        private static XName r=w+"r";
        private static XName ins=w+"ins";
        private static XNamespace ds=
            "http://schemas.openxmlformats.org/officeDocument/2006/customXml";


        private WordprocessingDocument document;
        private MemoryStream memory;

        //private Dictionary<string, BookmarkStart> Bookmarks { get; set; }

        public OpenXMLWordTemplate( string path )
            : base( path )
        {
            OpenInMemory( Body );
        }

        public OpenXMLWordTemplate( byte[] body )
            : base( body )
        {
            OpenInMemory( Body );
        }

        private void OpenInMemory( byte[] bytes )
        {
            memory=new MemoryStream();
            memory.Write( bytes, 0, (int)bytes.Length );

            try
            {
                document=WordprocessingDocument.Open( memory, true );
                document.ChangeDocumentType( DocumentFormat.OpenXml.WordprocessingDocumentType.Document );
            }
            catch ( Exception ex )
            {
                throw new ArgumentException( "Файл поврежден или не является документом Word(2007-10)", ex );
            }
            AttachPlaceholders();
        }

        public Guid AddCustomXml( XElement customXml, string xmlns = "" )
        {
            var partGuid=Guid.NewGuid();

            // Создаем новую custom XML часть.
            var customXmlPart=document.MainDocumentPart.AddCustomXmlPart( CustomXmlPartType.CustomXml );

            using ( Stream s=customXmlPart.GetStream( FileMode.Create, FileAccess.ReadWrite ) )
            using ( XmlWriter xw=XmlWriter.Create( s ) )
                customXml.Save( xw );

            // Создаем блок свойств.
            XDocument propertyPartXDoc=new XDocument(
                new XElement( ds+"datastoreItem",
                    new XAttribute( ds+"itemID", "{"+partGuid.ToString().ToUpper()+"}" ),
                    new XAttribute( XNamespace.Xmlns+"ds", ds.NamespaceName ),
                    new XElement( ds+"schemaRefs",
                        new XElement( ds+"schemaRef",
                            new XAttribute( ds+"uri", xmlns )
                        )
                    )
                )
            );

            // Добавляем в документ.
            var customXmlPropertyPart=customXmlPart.AddNewPart<CustomXmlPropertiesPart>();

            using ( Stream s=customXmlPropertyPart.GetStream( FileMode.Create, FileAccess.ReadWrite ) )
            using ( XmlWriter xw=XmlWriter.Create( s ) )
                propertyPartXDoc.Save( xw );

            return partGuid;
        }

        public void BindSdtToXml( Guid guid, string prefixMapping, string xPath, string sdtTag )
        {
            var sdtList=document.ContentControls();

            foreach ( var sdt in sdtList )
            {
                SdtProperties property=sdt.Elements<SdtProperties>().FirstOrDefault();
                var tag = property.Elements<Tag>().FirstOrDefault();

                if ( tag.Val == sdtTag )
                {
                    var binding = new DataBinding();
                    binding.StoreItemId = "{" + guid.ToString().ToUpper() + "}";
                    binding.PrefixMappings = prefixMapping;
                    binding.XPath = xPath;
                    property.AppendChild<DataBinding>( binding );
                }
            }
        }

        protected override void AttachPlaceholders()
        {
            switch ( this.PlaceholderType )
            {
                case enums.PlaceholderType.Bookmark:
                this._placeholder=new OpenXMLBookmarkPlaceholder( document );
                break;
                case enums.PlaceholderType.ContentControl:
                this._placeholder=new OpenXMLSDTPlaceholder( document );
                break;
            }
        }

        public override void Save( string path )
        {
            MainDocumentPart mainPart=document.MainDocumentPart;
            DocumentSettingsPart documentSettingsPart1=mainPart.DocumentSettingsPart;

#if templates_old
            AttachedTemplate attachedTemplate1=new AttachedTemplate() { Id="relationId1" };
            documentSettingsPart1.Settings.Append( attachedTemplate1 );

            // На заметку: узнать подробнее про External Relationships в структуре OpenXML.
            // Пэ Эс: можно (и даже нужно) заполнять фигней. Если подставлять реальный URI, может повесить ворд.
            documentSettingsPart1.AddExternalRelationship(
                "http://schemas.openxmlformats.org/officeDocument/2006/relationships/attachedTemplate",
                new Uri( @"c:\foo\bar", UriKind.Absolute ),
                "relationId1" );
#endif

            mainPart.Document.Save();

            document.Close();

            using ( FileStream fs=new FileStream( path, FileMode.Create ) )
            {
                memory.WriteTo( fs );
            }
            memory.Close();
            IsDisposed=true;
        }

        protected override void Cleanup( bool disposing )
        {
            if ( !this.IsDisposed )
            {
                if ( disposing )
                {
                    // Освобождение управляемых ресурсов.
                    if ( document!=null ) document.Close();
                    if ( memory!=null ) memory.Close();
                }
            }
            IsDisposed=true;
        }

#if BookmarksOnly
        public override IEnumerable<string> GetBookmarks()
        {
            if (Bookmarks == null)
            {

                Bookmarks = new Dictionary<string, BookmarkStart>();

                foreach (BookmarkStart bookmarkStart in document.MainDocumentPart.RootElement.Descendants<BookmarkStart>())
                {
                    // В документе могут присутствовать "скрытые" закладки с именами, начинающимися с "_".
                    if (!bookmarkStart.Name.ToString().StartsWith("_"))
                        Bookmarks.Add(bookmarkStart.Name.ToString(), bookmarkStart);
                }
            }
            return Bookmarks.Keys.ToList();
        }

        public override void FillBookmarks(IDictionary<string, string> data)
        {
            foreach (KeyValuePair<string, string> kv in data)
            {
                if (Bookmarks.ContainsKey(kv.Key))
                {
                    Bookmarks[kv.Key].Parent.InsertAfter(new Run(new Text(kv.Value)), Bookmarks[kv.Key]);
                }
            }
        }
#endif
    }
}
