#define Alex

using System;
using System.Collections.Generic;
using XDocument = System.Xml.Linq.XDocument;
using StringReader = System.IO.StringReader;
using System.Linq;
using System.Xml.Linq;

namespace Teleform.Reporting.Parsers
{
    public class SchemaParser : IParser
    {
        private IParser parser;
        private TypeParser typeParser;
        private EntityParser entityParser;
        private IEnumerable<Type> types;

        public SchemaParser()
        {
            parser = this;
            typeParser = new TypeParser();            
            entityParser = new EntityParser(new AttributeParser(GetType));
        }

        private Type GetType(string name)
        {
            return types.First(o => o.Name == name);
        }

        object IParser.Parse(XElement e)
        {
            if (e.Name != "schema")
                throw new ArgumentException();

            types = new List<Type>(e.Element("types").Elements("type").Select(o => typeParser.Parse(o)));
            var entities = new List<Entity>(e.Elements("entity").Select(o => entityParser.Parse(o)));

            return new Schema(entities, types);
        }

        public Schema Parse(XElement e)
        {
            return (Schema)parser.Parse(e);
        }

        public Schema Parse(string xmlElement)
        {
            var reader = new StringReader(xmlElement);

            return Parse(XElement.Load(reader));
        }
    }
}
