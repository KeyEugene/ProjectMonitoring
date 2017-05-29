using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Teleform.Reporting.constraint;

namespace Teleform.Reporting.Parsers
{
    public class ListParser : ObjectParser, IParser
    {
        private IParser parser;
        private ColumnParser columnParser;
        public TypeAccessor typeAccessor { get; private set; }
        public ListParser()
        {
            parser = this;
            columnParser = new ColumnParser();
        }

        public ListParser(TypeAccessor typeAccessor): this()
        {
            this.typeAccessor = typeAccessor;
            columnParser = new ColumnParser();
            parser = this;
        }

        object IParser.Parse(XElement e)
        {
            string constrID, constrName, alias, refTblName, refTblID, parentTblName, parentTblID, key;

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

            var parentTblNameAttribute = e.Attribute("parentTbl");
            if (parentTblNameAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "parentTbl", e), "e");
            parentTblName = parentTblNameAttribute.Value;

            var parentTblIDAttribute = e.Attribute("parentTblID");
            if (parentTblIDAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "parentTblID", e), "e");
            parentTblID = parentTblIDAttribute.Value;

            var refTbleNameAttribute = e.Attribute("refTbl");
            if (refTbleNameAttribute == null)
                refTblName = "";// throw new ArgumentException(Message.Get("Xml.NoAttribute", "refTbl", e), "e");
            else
                refTblName = refTbleNameAttribute.Value;

            var refTblIDAttribute = e.Attribute("refTblID");
            if (refTblIDAttribute == null)
                //throw new ArgumentException(Message.Get("Xml.NoAttribute", "refTblID", e), "e");
                refTblID = "";
            else
                refTblID = refTblIDAttribute.Value;

            var keyAttribute = e.Attribute("key");
            if (keyAttribute == null)
                key = "";
            else
                key = keyAttribute.Value;

            var columnAttributs = e.Elements("column");

            return new ListConstraint(
                constrID,
                constrName,
                alias,
                refTblName,
                refTblID,
                parentTblName,
                parentTblID,
                key,
                columnAttributs.Select(x => columnParser.Parse(x)));
        }
        public ListConstraint Parse(System.Xml.Linq.XElement e)
        {
            return (ListConstraint)parser.Parse(e);
        }
    }
}
