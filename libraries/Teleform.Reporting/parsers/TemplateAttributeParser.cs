using System;
using XElement = System.Xml.Linq.XElement;

namespace Teleform.Reporting.Parsers
{
    internal class TemplateAttributeParser : AttributeParser, IParser
    {
        private IParser parser;

        public TemplateAttributeParser()
        {
            parser = this;
        }

        object IParser.Parse(XElement e)
        {
            var attribute = base.Parse(e);

            if (attribute == null)
                throw new InvalidOperationException("Отсутствует объект атрибута.");

            var attributeValue = e.Attribute("value");

            if (attributeValue == null)
                throw new ArgumentNullException("e", "xxxx");

            return attribute;
        }
    }
}
