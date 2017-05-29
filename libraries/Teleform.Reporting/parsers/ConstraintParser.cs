using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Teleform.Reporting.Parsers;

namespace Teleform.Reporting.constraint
{
    public class ConstraintParser : ObjectParser, IParser
    {
        private IParser parser;
        private ColumnParser columnParser;
        public TypeAccessor typeAccessor { get; private set; }
        public ConstraintParser()
        {
            parser = this;
            columnParser = new ColumnParser();
        }

        public ConstraintParser(TypeAccessor typeAccessor)
            : this()
        {
            this.typeAccessor = typeAccessor;
            columnParser = new ColumnParser();
            parser = this;
        }

        object IParser.Parse(XElement e)
        {
            string constrID, constrName, alias, refTblName, refTblID;
            bool isNullable, isIdentified;

            var constrIDAttribute = e.Attribute("objID");
            if (constrIDAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "objID", e), "e");
            constrID = constrIDAttribute.Value;

            var constrNameAttribute = e.Attribute("name");
            if (constrNameAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "name", e), "e");
            constrName = constrNameAttribute.Value;

            var aliasAttribute = e.Attribute("alias");
            if (aliasAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "alias", e), "e");
            alias = aliasAttribute.Value;

            var refTbleNameAttribute = e.Attribute("refTbl");
            if (refTbleNameAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "refTbl", e), "e");
            refTblName = refTbleNameAttribute.Value;

            var refTblIDAttribute = e.Attribute("refTblID");
            if (refTblIDAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "refTblID", e), "e");
            refTblID = refTblIDAttribute.Value;
            
            var isNullableAttribute = e.Attribute("isNullable");
            if (isNullableAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "isNullable", e), "e");
            isNullable = isNullableAttribute.Value.Equals("1") ? true : false;


            var isIdentifiedAttribute = e.Attribute("isIdentified");
            if (isIdentifiedAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "isIdentified", e), "e");
            isIdentified = isIdentifiedAttribute.Value.Equals("1") ? true : false;

            var columnAttributs = e.Elements("column");

            return new Constraint(constrID,
                constrName,
                alias,
                refTblName,
                refTblID,
                isNullable,
                isIdentified,
                columnAttributs.Select(x => columnParser.Parse(x)));
        }
        public Constraint Parse(System.Xml.Linq.XElement e)
        {
            return (Constraint)parser.Parse(e);
        }
    }
}
