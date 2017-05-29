using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Teleform.Reporting.Parsers
{
    public class OperatorParser : ObjectParser, IParser
    {
        private IParser parser;

        public OperatorParser()
        {
            parser = this;
        }

        public Operator Parse(XElement e)
        {
            return (Operator)parser.Parse(e);
        }

        object IParser.Parse(XElement e)
        {
            if (e.Name != "operator")
                throw new ArgumentException();

            string name;
            object id;

            ParseObject(e, out id, out name);

            string lexem = string.Empty;
            int order = 255;

            var lexemAttribute = e.Attribute("lexem");
            var orderAttribute = e.Attribute("order");

            if (lexemAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "lexem", e), "e");

            if (orderAttribute != null)
                order = int.Parse(orderAttribute.Value);
            
            lexem = lexemAttribute.Value;

            return new Operator(id, name, lexem, order);
        }
    }
}
