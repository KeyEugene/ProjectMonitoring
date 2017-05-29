using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Teleform.Reporting.Parsers;

namespace Teleform.Reporting.WordExcelTemplateAddIns
{
    public class AddInEntityParser : IParser
    {
        public List<Teleform.Reporting.Type> 
            types { get; set; }

        public object Parse(XElement e)
        {

            //id
            string entityID;
            var idAttribute = e.Attribute("id");
            if (idAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "id", e), "e");
            else
                entityID = idAttribute.Value;

            //name
            string entityName;
            var nameAttribute = e.Attribute("name");
            if (nameAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "name", e), "e");
            else
                entityName = nameAttribute.Value;

            //alias
            string entityAlias;
            var aliasAttribute = e.Attribute("alias");
            if (aliasAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "alias", e), "e");
            else
                entityAlias = aliasAttribute.Value;

            //attributes
            var attributeElements = e.Elements("attribute");
            if (attributeElements.Count() == 0)
                throw new ArgumentException(Message.Get("Xml.NoElements", "attribute", e), "e");
            var attributeParser = new AddInAttributeParser();
            attributeParser.types = this.types;
            var entityAttributes = new List<AddInAttribute>(attributeElements.Select(x => (AddInAttribute)attributeParser.Parse(x)));


            return new AddInEntity() { ID = entityID, name = entityName, Alias = entityAlias, Attributes = entityAttributes };
        }

    }
}
