#define Dasha

using System;
using System.Collections.Generic;
using System.Linq;
using XElement = System.Xml.Linq.XElement;

namespace Teleform.Reporting.Parsers
{
    public class AttributeParser : ObjectParser, IParser
    {
        private IParser parser;
        private TypeAccessor typeAccessor;

        public AttributeParser()
        {
            parser = this;
        }

        public AttributeParser(TypeAccessor typeAccessor): this()
        {
            this.typeAccessor = typeAccessor;
            parser = this;
        }

        object IParser.Parse(XElement e)
        {
            object id;
            var idAttribute = e.Attribute("id");
            if (idAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "id", e), "e");
            else
                id = idAttribute.Value;

            string lPath;
            var lPathAttribute = e.Attribute("lPath");
            if (lPathAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "lPath", e), "e");
            lPath = lPathAttribute.Value;


            var descriptionAttribute = e.Attribute("description");
            if (descriptionAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", e), "e");

            var appTypeAttribute = e.Attribute("appType");
            if (appTypeAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", e), "e");

            var typeAttribute = e.Attribute("type");

            var fPathAttribute = e.Attribute("fPath");

            var colAttribute = e.Attribute("col");

            var namingAttribute = e.Attribute("naming");
            var isNullableAttribute = e.Attribute("isNullable");

            var entityIDAttribute = e.Attribute("entityID");

            bool naming = false, isNullable = false, isComputed = false, isIdentity = false;

            if (namingAttribute != null)
                naming = Convert.ToBoolean(namingAttribute.Value);

            if (isNullableAttribute != null)
                isNullable = Convert.ToBoolean(Convert.ToInt32((isNullableAttribute.Value)));


            var isComputedAttribute = e.Attribute("isComputed");
            if (isComputedAttribute != null)
                isComputed = Convert.ToBoolean(Convert.ToInt32((isComputedAttribute.Value)));

            var sTypeAttribute = e.Attribute("sType");
            if (sTypeAttribute == null)
                sTypeAttribute = e.Attribute("type");

            var isIdentityAttribute = e.Attribute("isIdentity");
            if (isIdentityAttribute != null)
                isIdentity = isIdentityAttribute.Value.Equals("1") ? true : false;


          //  var xxtype = typeAccessor(typeAttribute.Value);



            if (typeAttribute == null)
                throw new ArgumentNullException("e", "У атрибута не указан тип.");

            else return new Attribute(
                                      id,
                                      lPath,
                                      colAttribute.Value,
                                      fPathAttribute.Value,
                                      typeAccessor(typeAttribute.Value),
                                      sTypeAttribute.Value,
                                      int.Parse(descriptionAttribute.Value) / 4 == 1 ? true : false,
                                      (Description)int.Parse(descriptionAttribute.Value),
                                      GetAppTypeEnum(appTypeAttribute.Value),
                                      typeAttribute.Value == "Table" ? true : false,
                                      entityIDAttribute == null ? null : entityIDAttribute.Value,
                                      isIdentity,
                                      isNullable,
                                      naming,
                                      isComputed
                                      );

        }

        private AppType GetAppTypeEnum(string appTypeAttributeValue)
        {
            if (appTypeAttributeValue.Equals("objid"))
                return AppType.objid;
            else if (appTypeAttributeValue.Equals("title"))
                return AppType.title;
            else if (appTypeAttributeValue.Equals("parentid"))
                return AppType.parentid;
            else if (appTypeAttributeValue.Equals("info"))
                return AppType.info;
            else
                throw new ArgumentException();
        }

        public Attribute Parse(XElement e)
        {
            var attr = (Attribute)parser.Parse(e);
            return attr;
        }
    }
}
