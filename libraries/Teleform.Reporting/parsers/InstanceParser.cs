using System;
using System.Collections.Generic;
using System.Linq;
using XElement = System.Xml.Linq.XElement;

namespace Teleform.Reporting.Parsers
{
    public class InstanceParser : IParser
    {
        private IParser parser;
        public Template Template { get; private set; }
        private PropertyParser propertyParser;

        public InstanceParser(Template template)
        {
            parser = this;
            this.Template = template;
            propertyParser = new PropertyParser(GetField);
        }

        private TemplateField GetField(object id)
        {
            foreach (var field in Template.Fields)
#warning Явное приведение к ToString();
                if (field.ID.ToString() == id.ToString())
                    return field;

            return null;
        }

        object IParser.Parse(System.Xml.Linq.XElement e)
        {
            if (e == null)
                throw new ArgumentNullException("e", Message.Get("Common.NullArgument", "e"));

            var propertyElements = e.Elements("property");

            if (propertyElements.Count() == 0)
                throw new ArgumentException(Message.Get("Xml.NoElements", "property", e), "e");

            var entityAttributes = Template.Entity.Attributes;

            var properties = new List<Property>(propertyElements.Select(o => propertyParser.Parse(o)));

            if (properties.Count() == 0)
                throw new ArgumentNullException("properties", Message.Get("Common.NullArgument", "properties"));

            return new Instance(Template.Entity, properties);
        }

        public Instance Parse(XElement e)
        {
            return (Instance)parser.Parse(e);
        }
    }
}
