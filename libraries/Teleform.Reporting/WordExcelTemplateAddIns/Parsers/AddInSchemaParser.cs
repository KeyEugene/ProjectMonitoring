using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Teleform.Reporting.Parsers;

namespace Teleform.Reporting.WordExcelTemplateAddIns
{
    public class AddInSchemaParser 
    {
        public List<T> Parse<T>(XElement e) where T : IAddInElement
        {

            if (e.Name != "elements")
                throw new ArgumentException();

            List<T> elements = new List<T>();

            IParser parser;

            if (typeof(T) == typeof(AddInEntity))
            {
                parser = new AddInEntityParser();
                elements = e.Elements("element").Select(x => (T)parser.Parse(x)).ToList();
            }
            else if (typeof(T) == typeof(AddInAttribute))
            {
                parser = new AddInAttributeParser();
                elements = e.Elements("element").Select(x => (T)parser.Parse(x)).ToList();
            }

            return elements;
        }

        public AddInSchema Parse(XElement e)
        {
            if (e.Name != "schema")
                throw new ArgumentException();

            var schema = new AddInSchema();

            var typeParser = new TypeParser();
            var entityParser = new AddInEntityParser();

            schema.Types = new List<Type>(e.Element("types").Elements("type").Select(o => typeParser.Parse(o)));

            entityParser.types = schema.Types;

            schema.Entities = new List<AddInEntity>(e.Element("entities").Elements("entity").Select(x => (AddInEntity)entityParser.Parse(x)));



            return schema;

        }

    }
}
