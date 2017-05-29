
using System;
using System.Collections.Generic;
using System.Linq;
using Teleform.Reporting.constraint;
using XElement = System.Xml.Linq.XElement;

namespace Teleform.Reporting.Parsers
{
    /// <summary>
    /// Представляет функциональность для создания сущности на основе xml.
    /// </summary>
    public class EntityParser : ObjectParser, IParser
    {

        private IParser parser;

        public TypeAccessor TypeAccessor { get; private set; }
        private ConstraintParser constraintParser;
        private ListParser listParser;


        /// <summary>
        /// Инициализирует объект типа Teleform.Reporting.Parsers.EntityParser.
        /// </summary>
        private AttributeParser attributeParser;
        public EntityParser(AttributeParser attributeParser)
        {
            if (attributeParser == null)
                throw new ArgumentNullException("attributeParser", Message.Get("Common.NullArgument", "attributeParser"));

            this.attributeParser = attributeParser;
            this.constraintParser = new ConstraintParser();
            this.listParser = new ListParser();
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

            string alias;
            var aliasAttribute = e.Attribute("alias");
            if (aliasAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "alias", e), "e");
            alias = aliasAttribute.Value;


            var attributeElements = e.Elements("attribute");


            if (attributeElements.Count() == 0)
                throw new ArgumentException(Message.Get("Xml.NoElements", "attribute", e), "e");

            var systemNameAttribute = e.Attribute("name");

            if (systemNameAttribute == null || systemNameAttribute.Value == null)
                throw new InvalidOperationException(alias);

            var mainAttribute = e.Attribute("main");

            if (mainAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "main", e), "e");

            var ancestorIDAttribute = e.Attribute("ancestorID");
            var ancestorID = -1;
            if (ancestorIDAttribute != null)
                ancestorID = int.Parse(ancestorIDAttribute.Value);



            var isHierarchicAttribute = e.Attribute("isHierarchic");
            bool isHierarchic;
            if (isHierarchicAttribute == null)
                isHierarchic = false;
            else isHierarchic = Convert.ToBoolean(int.Parse(isHierarchicAttribute.Value));

            var isEnumerationAttribute = e.Attribute("isEnumeration");
            bool isEnumeration;
            if (isEnumerationAttribute == null)
                isEnumeration = false;
            else isEnumeration = Convert.ToBoolean(int.Parse(isEnumerationAttribute.Value));

            var constraintElements = e.Elements("constraint");

            var listElements = e.Elements("list");

            return new Entity
            (
                id,               
                alias,
                systemNameAttribute.Value,
                Convert.ToBoolean(int.Parse(mainAttribute.Value)),
                ancestorID,
                attributeElements.Select(o => attributeParser.Parse(o)),
                constraintElements.Select(x => constraintParser.Parse(x)),
                listElements.Select(x => listParser.Parse(x)),
                isHierarchic,
                isEnumeration
            );
        }

        /// <summary>
        /// Преобразует элемент xml в сущность.
        /// </summary>
        /// <param name="e">Элемент xml.</param>
        /// <exception cref="ArgumentNullException">Параметр e равен null.</exception>
        /// <exception cref="ArgumentException">Парсер не может обработать указанный элемент xml.</exception>
        public Entity Parse(XElement e)
        {
            return (Entity)parser.Parse(e);
        }
    }
}