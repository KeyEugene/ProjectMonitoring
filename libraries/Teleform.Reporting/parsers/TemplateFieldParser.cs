#define Dasha

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XElement = System.Xml.Linq.XElement;

namespace Teleform.Reporting.Parsers
{
    public class TemplateFieldParser : IParser
    {
        private IParser parser;
        private Entity entity;

        public TemplateFieldParser(Entity entity)
        {
            this.entity = entity;
            parser = this;
        }

        object IParser.Parse(XElement e)
        {
            if (e == null)
                throw new ArgumentNullException("e", Message.Get("Common.NullArgument", "e"));

            if (e.Name != "field")
                throw new ArgumentException(Message.Get("Xml.NoElement", "field", e), "e");

            var NativeEntityName = e.Attribute("NativeEntityName");

            if (NativeEntityName == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "NativeEntityName", e), "e");   


            var attributeID = e.Attribute("attributeID");

            if (attributeID == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "attributeID", e), "e");

            var formatID = e.Attribute("formatID");

            if (formatID == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "formatID", e), "e");

            var alias = e.Attribute("alias");
            if (alias == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "alias", e), "e");

            var hashName = e.Attribute("hashName");
            if (hashName == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute" , "hashName", e), "e");

            var id = e.Attribute("ID");

            if (id == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "ID", e), "e");                   

            var aggregate = e.Attribute("aggregate");

            if (aggregate == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "aggregation", e), "e");

            var order = e.Attribute("order");

            if (order == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "order", e), "e");

            var level = e.Attribute("level");

            if (level == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "level", e), "e");

            var predicateInfoAttribute = e.Attribute("predicateInfo");
            string predicateInfo = null;

            if (predicateInfoAttribute != null)
                predicateInfo = predicateInfoAttribute.Value;

            var predicateAttribute = e.Attribute("predicate");

            if (predicateAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "predicate", e), "e");

            var IsVisible = e.Attribute("isVisible");
            bool isVisible = true;
            if (IsVisible == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "isVisible", e), "e");
            else
                if (IsVisible.Value == "0")
                    isVisible = false;

            var listCol = e.Attribute("listCol");
            if (listCol == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "listCol", e), "e");

            var listAggregate = e.Attribute("listAggregate");
            if (listAggregate == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "listAggregateAttribute", e), "e");

            var templateIDAttribute = e.Attribute("templateID");
            string templateID = null;
            if (templateIDAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "templateID", e), "e");
            else
                templateID = templateIDAttribute.Value;

            var attribute = entity.Attributes.FirstOrDefault(o => o.ID.ToString() == attributeID.Value);

            if (attribute == null)
            {
                  throw new ArgumentException(Message.Get(string.Format("Не удалось найти атрибут c идентификатором '{0}'. Возможно шаблон устарел.\nEntity = ({1}, {2}, {3})", attributeID.Value, entity.Name, entity.SystemName, entity.ID), e), "e");
            }

           // if (attribute == null)
                //throw new InvalidOperationException(string.Format("Не удалось найти атрибут c идентификатором '{0}'. Возможно шаблон устарел.\nEntity = ({1}, {2}, {3})", attributeID.Value, entity.Name, entity.SystemName, entity.ID));

            var crossTableRoleID = (int)e.Attribute("crossTableRoleID");

            return new TemplateField
                (
                id.Value,
                NativeEntityName.Value,
                alias.Value,
                hashName.Value,
                attribute,
                attribute.IsListAttribute ? attribute.GetAttributeByColumnName(listCol.Value).Type.GetAdmissableFormats().First(o => o.ID.ToString() == formatID.Value) : attribute.Type.GetAdmissableFormats().First(o => o.ID.ToString() == formatID.Value),
       
               // operation.Value,
                Convert.ToInt32(level.Value),
                Convert.ToInt32(order.Value),
                aggregate.Value,
                predicateAttribute.Value,
                predicateInfo,
                isVisible,
                attribute.IsListAttribute ? new ListAttributeAggregation(listCol.Value, listAggregate.Value) : null,
                crossTableRoleID,
                templateID
                );
        }

        public TemplateField Parse(XElement e)
        {
            return (TemplateField)parser.Parse(e);
        }
    }
}