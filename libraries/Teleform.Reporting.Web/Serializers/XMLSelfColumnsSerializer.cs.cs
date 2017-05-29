using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Teleform.Reporting.Web
{
    public class XMLSelfColumnsSerializer
    {
        private Dictionary<string, string> _selfColumnsDataDict;

        private Template _template;

        public XMLSelfColumnsSerializer(Template template, Dictionary<string, string> selfColumnsDataDict)
        {
            this._template = template;
            this._selfColumnsDataDict = selfColumnsDataDict;
        }

        public void Serialize(XElement attributesElement)
        {
            var fields = _template.Fields.Where(f => !f.Attribute.IsComputed && f.Format.ID.ToString() == "0" && f.Attribute.SType != "Table" && !f.Attribute.FPath.Contains("/"));

            var distinctFields = from fPatn in fields group fPatn by fPatn.Attribute.FPath into gFpath select gFpath.First();
            
            if (distinctFields.Count() > 0)
            {
                foreach (var field in distinctFields)
                {
                    var value = _selfColumnsDataDict[field.Attribute.FPath];




                    attributesElement.Add(new XElement
                      (
                         "attribute",
                          new XAttribute("name", field.Attribute.FPath),
                          new XAttribute("alias", field.Name),
                          new XAttribute("value", value),
                          new XAttribute("allowNulls", field.Attribute.IsNullable),
                          new XAttribute("type", field.Attribute.SType),
                          new XAttribute("IsEditable", !field.Attribute.IsComputed),
                          new XAttribute("IsComputed", field.Attribute.IsComputed)
                       ));
                }

            }
 
        }
    }
}
