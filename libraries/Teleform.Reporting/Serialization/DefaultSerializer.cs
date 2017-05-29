using System.Collections.Generic;
using System.Linq;

using XElement = System.Xml.Linq.XElement;
using XAttribute = System.Xml.Linq.XAttribute;
using Convert = System.Convert;

namespace Teleform.Reporting.Serialization
{
    public static class DefaultSerializer
    {
        public static string Serialize(this Template template)
        {
            var fields = new XElement("fields");
            var parameters = new XElement("parameters",
                template.Parameters.Select(pair => new XElement
                (
                    "parameter",
                    new XAttribute("name", pair.Key),
                    new XAttribute("value", pair.Value)
                )));

#warning Временно, проблема в процедуре если ей отправить <body> </body> то в БД запишется 0x, а нас это не устраивает
            XElement xml;

            if (template.Content.Count() != 0)
                xml = new XElement(
                    "template",
                    new XAttribute("type", template.TypeCodeString),
                    new XAttribute("name", template.Name),
                    new XAttribute("entityID", template.Entity.ID),
                    new XAttribute("fileName", template.FileName),
                    fields,
                    parameters,
                    new XElement("body", Convert.ToBase64String(template.Content))
                );
            else
                xml = new XElement(
                "template",
                new XAttribute("type", template.TypeCodeString),
                new XAttribute("name", template.Name),
                new XAttribute("entityID", template.Entity.ID),
                new XAttribute("fileName", template.FileName),
                new XAttribute("templateByDefault", (template.TemplateByDefault ? "1" : "0")),
                new XAttribute("treeTypeID", (int)template.TreeTypeEnum),
                fields,
                parameters
            );

            if (template.ID != null)
                xml.Add(new XAttribute("id", template.ID));

            foreach (var item in template.Fields)
                fields.Add(new XElement(
                    "field", //new XAttribute("id", item.ID),
                    new XAttribute("attributeID", item.Attribute.ID),
                    new XAttribute("formatID", item.Format.ID),
                    new XAttribute("alias", item.Name),
                  //  new XAttribute("filter", item.Filter),
                  //  new XAttribute("operation", item.Operation),
                    new XAttribute("order", item.Order),
                    new XAttribute("aggregate", item.Aggregation),
                    new XAttribute("predicateInfo", item.PredicateInfo),
                    new XAttribute("predicate", item.Predicate == null ? string.Empty : item.Predicate), // is new add Alex
                    new XAttribute("isVisible", item.IsVisible),
                    new XAttribute("level", item.Level),
                    new XAttribute("listCol", item.Attribute.IsListAttribute ? item.ListAttributeAggregation.ColumnName : string.Empty),
                    new XAttribute("listAggregate", item.Attribute.IsListAttribute ? item.ListAttributeAggregation.AggregateLexem : string.Empty),
                    new XAttribute("crossTableRoleID", item.CrossTableRoleID)
                ));

            return xml.ToString();
        }
    }
}
