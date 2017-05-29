
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace Teleform.Reporting.DynamicCard
{


    public static partial class XmlCardSerializer
    {


        public static Card Deserialize(int entityID)
        {
            var entity = Storage.Select<Entity>(entityID);

            var card = new Card(entity);

            foreach (var constraint in entity.Constraints)            
                card.Fields.Add(new CardRelationField(constraint, card));
            

            var selfAttributes = entity.Attributes.Where(a => a.SType != "Table" && !a.FPath.Contains("/"));
            foreach (var selfAttribute in selfAttributes)
            {
                //Костыл, пока не разберусь в иерархией
                if (selfAttribute.FPath != "parentID")
                    card.Fields.Add(new CardSelfField(selfAttribute, card));
            }

            foreach (var listConstraint in entity.Lists)
                card.Fields.Add(new CardListRelationField(listConstraint, card));

            
            return card;
        }

        public static string Serialize(Card card, Mode mode, int lastInsertedInstanceID)
        {
            var root = new XElement
            (
                "bObject",
                new XAttribute("entity", card.Entity.SystemName),
                new XAttribute("userID", 0)
            );

            if (mode == Mode.Edit)
            {
                root.Add(new XAttribute("objID", card.GetAnyField("objID").Value));
            }
            var attributesElement = new XElement("attributes");

            foreach (var selfField in card.Fields.Where(f => f is CardSelfField).OfType<CardSelfField>().Where(f => !f.IsForbidden))
            {
                if (selfField.TypeCode != CardSelfField.Type.FileName)
                {
                    attributesElement.Add(
                      new XElement
                      (
                          "attribute",
                          new XAttribute("name", selfField.SystemName),
                          new XAttribute("alias", selfField.Name),
                          new XAttribute("value", selfField.Value),
                          new XAttribute("allowNulls", selfField.IsNullable),
                          new XAttribute("type", selfField.SType),
                          new XAttribute("IsEditable", !selfField.IsReadOnly(mode)),
                          new XAttribute("IsComputed", selfField.IsComputed || selfField.IsIdentity)
                      ));
                }
                else
                {
                    var file = selfField.Value as File;

                    if (file != null && file.IsModified)
                    {
                        attributesElement.Add(
                            new XElement("file",
                                new XAttribute("fileName", file.FileName),
                                new XAttribute("mimeType", file.MimeType),
                                new XElement("content", Convert.ToBase64String(file.Content))
                            ));
                    }

                }
            }

            if (card.EntityInstance != null)
            {
                if (card.EntityInstance.RelationColumnsValue != null)
                    new XMLRelationColumnsSerializer(card.EntityInstance.RelationColumnsValue).Serialize(attributesElement, card.Entity.AncestorID, lastInsertedInstanceID);
            }
            root.Add(attributesElement);
            return root.ToString();
        }

    }
}
