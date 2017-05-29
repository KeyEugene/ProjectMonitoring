using System;
using XElement = System.Xml.Linq.XElement;
using System.Linq;

namespace Teleform.Reporting.Parsers
{
    public class PropertyParser : IParser
    {
        private FieldAccessor accessor;
        private IParser parser;

        public PropertyParser(FieldAccessor accessor)
        {
            this.accessor = accessor;
            parser = this;
        }

        object IParser.Parse(XElement e)
        {
            if (e == null)
                throw new ArgumentNullException("e", Message.Get("Common.NullArgument", "e"));

            var valueAttribute = e.Attribute("value");

            if (valueAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoElement", "value", e), "e");

            var fieldIDAttribute = e.Attribute("fieldID");

            if (fieldIDAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoElement", "fieldID", e), "e");

            return new Property(accessor(fieldIDAttribute.Value), valueAttribute.Value);
        }

        public Property Parse(XElement e)
        {
            return (Property)parser.Parse(e);
        }
    }
}
