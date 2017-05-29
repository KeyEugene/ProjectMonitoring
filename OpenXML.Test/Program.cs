#define OpenXmlTest1
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Teleform.Office.Reporting;
using Teleform.Office.Reporting.Placeholders;


namespace OpenXML.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                List<PlaceholderData> bmz;
                //using (var template = new Report.Templates.WordTemplate(args[0]))
                //{
                //    bmz = template.GetBookmarksList();
                //    var bookmz = new Dictionary<string, string>();
                //    foreach (var item in bmz)
                //    {
                //        bookmz.Add(item, "42");
                //    }
                //    template.EvaluateBookmarks(bookmz);
                //    template.EvaluateBookmarks();
                //    template.SaveToDocument(@"c:\tmp", "filledDocument_test.docx");
                //}

                using (var template = new OpenXMLWordTemplate(args[0]))
                {
                    bmz = template.GetPlaceholders().ToList();
                    
                    //var bookmz = new Dictionary<string, string>();
                    //foreach ( var item in bmz )
                    //{
                    //    bookmz.Add( item, "42" );
                    //}

                    XDocument doc = XDocument.Load( @"c:\tmp\wordml\data.xml" );
                    var guid = template.AddCustomXml( XElement.Parse( doc.ToString() ) );
                    //template.BindSdtToXml(guid);
                    //template.FillPlaceholders( bookmz );
                    template.Save( @"c:\tmp\wordml\customxml.docx" );
                }
                Console.ReadKey(true);
            }
            else
                Console.WriteLine("Укажите имя файла.");
        }
    }
}
