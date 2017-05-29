using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Teleform.Reporting.Parsers;
using System.Xml.Linq;
using System.IO;

namespace Teleform.Reporting.Providers
{
    public class XmlTemplateProvider : IProvider
    {
        private IParser parser;
        private IProvider provider;


        public string Xml { get; private set; }


        public XmlTemplateProvider(IParser parser, string xml)
        {
            if (parser == null)
                throw new ArgumentNullException("parser",
                    string.Format("Параметр {0} имеет значение null.", "parser"));
            if (xml == null)
                throw new ArgumentNullException("xml",
                    string.Format("Параметр {0} имеет значение null.", "xml"));

            this.parser = parser;
            this.Xml = xml;
        }

        object IProvider.GetInstance()
        {
            TextReader reader = new StringReader(Xml);
            var document = XDocument.Load(reader);

            return parser.Parse(document.Root);
        }

        public Report GetInstance()
        {
            return (Report)provider.GetInstance();
        }


    }
}
