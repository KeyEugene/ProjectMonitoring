
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XElement = System.Xml.Linq.XElement;
using Teleform.Reporting.Parsers;

namespace Teleform.Reporting.Parsers
{
    public class EntityFilterParser : ObjectParser, IParser
    {
        private IParser parser;
        private EntityFilterFieldParser entityFilterFieldParser;

        public EntityFilterParser()
        {
             parser = this;
        }               

        object IParser.Parse(XElement e)
        {  
            object id;
            string name;
            string userID;
            Entity entity;

            ParseObject(e, out id, out name);

            userID = e.Attribute("userID").Value;
            if (userID == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "userID", e), "e");

            var entityID = e.Attribute("entityID");
            if (entityID == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "entityID", e), "e");

            entity = Storage.Select<Entity>(entityID.Value);
            entityFilterFieldParser = new EntityFilterFieldParser(entity);
            
            var Fields = e.Elements("field");
            var fields = Fields.Select(o => entityFilterFieldParser.Parse(o));            

            return new EntityFilter(id, name, userID, entity, fields);
        }

        public EntityFilter Parse(XElement e)
        {
            return (EntityFilter)parser.Parse(e);
        }
    }
}
