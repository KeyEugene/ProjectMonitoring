
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Data;
using System.Data.SqlClient;
using Teleform.Reporting.DynamicCard;

namespace Teleform.Reporting.Web
{
    partial class TableViewControl
    {
        public string Serialize(string userID, EntityInstance EntityInstance)
        {
            var entity = Storage.Select<Entity>(Template.Entity.ID);

            var root = new XElement("bObject");

            if (EntityInstance.EntityInstanceID == "-1")
            {
                root = new XElement
                (
                    "bObject",
                    new XAttribute("entity", entity.SystemName),
                    new XAttribute("userID", userID)
                );
            }
            else
            {
                root = new XElement
              (
                  "bObject",
                  new XAttribute("entity", entity.SystemName),
                  new XAttribute("userID", userID),
                  new XAttribute("objID", EntityInstance.EntityInstanceID)
              );
            }

            var attributesElement = new XElement("attributes");

            if (EntityInstance.SelfColumnsValue != null)
                new XMLSelfColumnsSerializer(Template, EntityInstance.SelfColumnsValue).Serialize(attributesElement);

            if (EntityInstance.RelationColumnsValue != null)
                new XMLRelationColumnsSerializer(EntityInstance.RelationColumnsValue).Serialize(attributesElement, -1, -1);



            root.Add(attributesElement);

            return root.ToString();
        }
    }
}
