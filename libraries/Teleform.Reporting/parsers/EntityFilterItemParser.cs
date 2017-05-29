using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XElement = System.Xml.Linq.XElement;
//using Teleform.Reporting.EntityFilters;

namespace Teleform.Reporting.Parsers
{
    public class EntityFilterItemParser : IParser
    {
        private IParser parser;

        public EntityFilterItemParser()
        {
            parser = this;
        }

        object IParser.Parse(XElement e)
        {
            e.Element("filterItem");

            var predicate = e.Attribute("predicate").Value;
            if (string.IsNullOrEmpty(predicate))
                throw new ArgumentException(Message.Get("Xml.NoAttribute", e), "e");

            var hash = e.Attribute("hash").Value;
            if (string.IsNullOrEmpty(hash))
                throw new ArgumentException(Message.Get("Xml.NoAttribute", e), "e");

            return new EntityFilterItem(hash, predicate);
        }

        public EntityFilterItem Parse(XElement e)
        {
            return (EntityFilterItem)parser.Parse(e);
        }

    }
}
