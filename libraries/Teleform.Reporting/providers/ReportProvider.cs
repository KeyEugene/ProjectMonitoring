using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Teleform.Reporting.Parsers;
using System.IO;
using System.Xml.Linq;

namespace Teleform.Reporting.Providers
{
    public class ReportProvider<T> : IProvider
    {
        private IProvider provider;
        private IParser parser;

        public string Xml { get; private set; }

        public ReportProvider(IParser parser, string xml)
        {
            if (parser == null)
                throw new ArgumentNullException("parser",
                    string.Format("Параметр {0} имеет значение null.", "parser"));

            if (xml == null)
                throw new ArgumentNullException("xml",
                    string.Format("Параметр {0} имеет значение null.", "xml"));

            this.provider = this;
            this.parser = parser;
            this.Xml = xml;
        }

        object IProvider.GetInstance()
        {
            var reader = new StringReader(Xml);
            var document = XDocument.Load(reader);

            return parser.Parse(document.Root);
        }

        public T GetInstance()
        {
            return (T) provider.GetInstance();
        } 
    }

}
