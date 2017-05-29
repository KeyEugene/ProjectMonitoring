using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Column = Teleform.Reporting.constraint.Column;

namespace Teleform.Reporting.Parsers
{
    public class ColumnParser : ObjectParser, IParser
    {
        private IParser parser;
        public TypeAccessor typeAccessor;

        public ColumnParser()
        {
            parser = this;
        }

        public ColumnParser(TypeAccessor typeAccessor)
            : this()
        {
            this.typeAccessor = typeAccessor;
            parser = this;
        }

        object IParser.Parse(XElement e)
        {
            string parentCol;
            var parentColAttribute = e.Attribute("parentCol");
            if (parentColAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "parentCol", e), "e");
            parentCol = parentColAttribute.Value;

            string refCol;
            var refColAttribute = e.Attribute("refCol");
            if (refColAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "refCol", e), "e");
            refCol = refColAttribute.Value;

            //bool isParentKey = false;
            //var isParentKeyAttribute = e.Attribute("isParentKey");
            //if (isParentKeyAttribute == null)
            //    throw new ArgumentException(Message.Get("Xml.NoAttribute", "isParentKey", e), "e");
            //isParentKey = refColAttribute.Value.Equals("1") ? true : false;

            bool isNullable;
            var isNullableAttribute = e.Attribute("isNullable");
            if (isNullableAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "isNullable", e), "e");
            isNullable = isNullableAttribute.Value.Equals("1") ? true : false;

            bool isParentKey;
            var isParentKeyAttribute = e.Attribute("isParentKey");
            if (isParentKeyAttribute == null)
                isParentKey = false;
            else
                isParentKey = isParentKeyAttribute.Value.Equals("1") ? true : false;

            return new Column(parentCol, refCol, isNullable, isParentKey);
            //return new Column(parentCol, refCol, isParentKey, isNullable, isReadOnly);
        }

        public Column Parse(XElement e)
        {
            return (Column)parser.Parse(e);
        }
    }
}
