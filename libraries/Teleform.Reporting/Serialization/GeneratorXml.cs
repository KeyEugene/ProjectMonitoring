using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Teleform.Reporting
{
    public class GeneratorXml
    {
        [Obsolete("ОБРАТИТЬ ОСОБОЕ ВНИМАНИЕ!")]
        public string GenerateXmlUplate(Template template)
        {
            var xml = new XElement("template",
                new XAttribute("type", template.TypeCodeString),
                new XAttribute("name", template.Name),
                new XAttribute("entityID", template.Entity.ID),
                new XElement("fields"),
                new XAttribute("fileName", template.FileName),
                new XElement("body", Convert.ToBase64String(template.Content)));

            if (template.ID != null)
                xml.Add(new XAttribute("id", template.ID));

            if (template.Parameters.ContainsKey("sheet"))
                xml.Add(new XAttribute("sheet", template.Parameters["sheet"]));

            foreach (var item in template.Fields)
            {
                var field = new XElement("field", //new XAttribute("id", item.ID),
                    new XAttribute("attributeID", item.Attribute.ID),
                    new XAttribute("formatID", item.Format.ID),
                    new XAttribute("alias", item.Name),
                    //new XAttribute("filter", item.Filter),
                    //new XAttribute("operation", item.Operation),
                    new XAttribute("order", item.Order),
                    new XAttribute("aggregate", item.Aggregation),
                    new XAttribute("predicate", item.Predicate == null ? string.Empty : item.Predicate),
                    new XAttribute("attributeType", item.Attribute.Type.Name),
                    new XAttribute("predicateInfo", item.PredicateInfo == null ? string.Empty : item.PredicateInfo),
                    new XAttribute("isVisible", Convert.ToInt16(item.IsVisible)),
                    new XAttribute("listCol", item.Attribute.IsListAttribute ? item.ListAttributeAggregation.ColumnName : string.Empty),
                    new XAttribute("listAggregate", item.Attribute.IsListAttribute ? item.ListAttributeAggregation.AggregateLexem : string.Empty));
                xml.Element("fields").Add(field);
            }
            return xml.ToString();
        }


        public string GenerateXmlUpdate(EntityFilter entityFilter)
        {
            var xml = new XElement("entityFilter",                 
                 new XAttribute("id", entityFilter.ID),
                 new XAttribute("name", entityFilter.Name),
                 new XAttribute("entityID", entityFilter.Entity.ID)
                 );

            foreach (var item in entityFilter.Fields)
            {
                var field = new XElement("field",
                    new XAttribute("attributeID", item.Attribute.ID),                   
                    new XAttribute("predicateInfo", item.PredicateInfo == null ? string.Empty : item.PredicateInfo),
                    new XAttribute("techPredicate", item.TechPredicate == null ? string.Empty : item.TechPredicate),
                    new XAttribute("userPredicate", item.UserPredicate == null ? string.Empty : item.UserPredicate),
                    new XAttribute("sequence", item.Sequence)
                    );
                xml.Add(field);
            }
            return xml.ToString();          
        }

      

    }

}
