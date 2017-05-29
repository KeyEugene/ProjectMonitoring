using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Teleform.Reporting.Parsers;

namespace Teleform.Reporting.WordExcelTemplateAddIns
{
    public class AddInAttributeParser : IParser
    {
        public List<Type> types { get; set; }

        public object Parse(XElement e)
        {

            string attrID;
            var idAttribute = e.Attribute("id");
            if (idAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "id", e), "e");
            else
                attrID = idAttribute.Value;

            Type attrType;
            var typeAttribute = e.Attribute("type");
            if (typeAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "type", e), "e");
            else
                attrType = types.AsEnumerable().FirstOrDefault(x=>x.Name == typeAttribute.Value);

            string attrAlias;
            var aliasAttribute = e.Attribute("alias");
            if (aliasAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "alias", e), "e");
            else
                attrAlias = aliasAttribute.Value;


            return new AddInAttribute() { ID = attrID, Type = attrType, Alias = attrAlias };
        }
    }
}
