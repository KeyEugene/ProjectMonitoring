using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Teleform.Reporting.Parsers
{
    public class AggregateFunctionParser : ObjectParser, IParser
    {
        private IParser parser;

        public AggregateFunctionParser()
        {
            parser = this;
        }

        public AggregateFunction Parse(XElement e)
        {
            return (AggregateFunction)parser.Parse(e);
        }

        object IParser.Parse(XElement e)
        {
            if (e.Name != "aggregateFunction")
                throw new ArgumentException();

            string name;
            object id;

            ParseObject(e, out id, out name);

            string lexem = string.Empty;

            var lexemAttribute = e.Attribute("lexem");

            if (lexemAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "lexem", e), "e");

            lexem = lexemAttribute.Value;

            return new AggregateFunction(id, name, lexem);
        }
    }
}
