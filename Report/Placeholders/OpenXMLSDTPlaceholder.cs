using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Teleform.Office.Reporting.ExtensionMethods;

namespace Teleform.Office.Reporting.Placeholders
{
    public class OpenXMLSDTPlaceholder : IPlaceholder
    {
        private WordprocessingDocument Document { get; set; }
        private Dictionary<PlaceholderData, SdtElement> SDTList { get; set; }

        public OpenXMLSDTPlaceholder( WordprocessingDocument document )
        {
            this.Document = document;
        }

#warning Элементы без тегов никуда не записываются!
        public IEnumerable<PlaceholderData> GetPlaceholders()
        {
            if ( SDTList == null )
            {
                SDTList = new Dictionary<PlaceholderData, SdtElement>();

                var sdtList = Document.ContentControls();
                foreach ( var cc in sdtList )
                {                    
                    SdtProperties props = cc.Elements<SdtProperties>().FirstOrDefault();
                    Tag tag = props.Elements<Tag>().FirstOrDefault();

                    if ( tag != null )
                        SDTList.Add( new PlaceholderData { Text = cc.InnerText, Name = "", Tag = tag.Val.ToString() }, 
                            ( cc as SdtElement ) );

                        //SDTList.Add( tag.Val.ToString(), ( cc as SdtElement ) );
                }
            }
            return SDTList.Keys.ToList();
        }

        //public void FillPlaceholders( IDictionary<PlaceholderData, string> data )
        //{
        //    foreach ( KeyValuePair<PlaceholderData, string> kv in data )
        //    {
        //        if ( SDTList.ContainsKey( kv.Key ) )
        //        {
        //            if ( SDTList[kv.Key].Descendants<Text>().FirstOrDefault().Text != null )
        //                SDTList[kv.Key].Descendants<Text>().FirstOrDefault().Text = kv.Value;
        //        }
        //    }
        //}

        public void FillPlaceholders( IDictionary<string, string> data )
        {
            var dict = new Dictionary<string, SdtElement>();
            foreach ( KeyValuePair<PlaceholderData, SdtElement> kv in SDTList )
            {
                dict.Add( kv.Key.Tag, kv.Value );
            }

            foreach ( KeyValuePair<string, string> kv in data )
            {
                if ( dict.ContainsKey( kv.Key ) )
                {
                    if ( dict[kv.Key].Descendants<Text>().FirstOrDefault().Text != null )
                        dict[kv.Key].Descendants<Text>().FirstOrDefault().Text = string.IsNullOrEmpty( kv.Value) ? "Пусто" : kv.Value;
                }
            }
        }

        public void InsertCustomXml( XElement element )
        {
            //var mainPart = Document.MainDocumentPart;
            //var customXmlPartsCount = mainPart.GetPartsCountOfType<CustomXmlPart>();

            //if ( customXmlPartsCount == 0 )
            //{

            //}
            //var customXmlPart = mainPart.AddCustomXmlPart( CustomXmlPartType.CustomXml );

            //using (StreamWriter sw = new StreamWriter(customXmlPart.GetStream()))
            //{
            //    sw.Write( element.ToString() );
            //    sw.Close();
            //}

            //Guid idGuid = Guid.NewGuid();

            //CustomXmlPropertiesPart customXmlPropertyPart = customXmlPart.AddNewPart<CustomXmlPropertiesPart>();
            //using ( StreamWriter sw = new StreamWriter( customXmlPropertyPart.GetStream() ) )
            //{
            //    using (XmlWriter xw = XmlWriter.Create(sw))

            //}
        }
    }

    public class PlaceholderData
    {
        public string Text { get; set; }

        // Не используется.
        public string Name { get; set; }

        public string Tag { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
