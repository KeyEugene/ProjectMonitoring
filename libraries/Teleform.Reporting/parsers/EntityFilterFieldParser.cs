#define entityE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XElement = System.Xml.Linq.XElement;
//using Teleform.Reporting.EntityFilters;

namespace Teleform.Reporting.Parsers
{
    public class EntityFilterFieldParser : IParser
    {
        private IParser parser;
        private Entity entity;

        public EntityFilterFieldParser(Entity entity)
        {
            this.entity = entity;
            parser = this;
        }
        object IParser.Parse(XElement e)
        {
            var attributeID = e.Attribute("attributeID");
            if (string.IsNullOrEmpty(attributeID.Value))
                throw new ArgumentException(Message.Get("Xml.NoAttribute", e), "e");

            var attribute = entity.Attributes.First(o => o.ID.ToString() == attributeID.Value);
            if (attribute == null)
                throw new InvalidOperationException(string.Format(
                    "Не удалось найти атрибут c идентификатором '{0}', {1}, {2}, {3})",
                    attributeID.Value,
                    entity.Name,
                    entity.SystemName,
                    entity.ID
                ));

            var predicateInfo = e.Attribute("predicateInfo");
            string predicateInfoValue = null;
            if (predicateInfo != null)
                predicateInfoValue = predicateInfo.Value;

            var techPredicate = e.Attribute("techPredicate");
            string techPredicateValue = null;
            if (techPredicate != null)
                techPredicateValue = techPredicate.Value;

            var userPredicate = e.Attribute("userPredicate");
            string userPredicateValue = null;
            if (userPredicate != null)
                userPredicateValue = userPredicate.Value;

            var sequence = e.Attribute("sequence");
            if (string.IsNullOrEmpty(sequence.Value))
                throw new ArgumentException(Message.Get("XML.NoAttribute", e), "e");

            return new EntityFilterField(attributeID.Value, attribute, predicateInfoValue, techPredicateValue, userPredicateValue, Convert.ToInt32(sequence.Value));
        }

        public EntityFilterField Parse(XElement e)
        {
            return (EntityFilterField)parser.Parse(e);
        }

    }
}
