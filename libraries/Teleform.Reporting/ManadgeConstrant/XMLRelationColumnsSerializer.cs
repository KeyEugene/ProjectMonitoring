
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Teleform.Reporting
{
    public class XMLRelationColumnsSerializer
    {
        List<RelationColumn> _relationColumnsValue;

        public XMLRelationColumnsSerializer(List<RelationColumn> relationColumnsValue)
        {
            _relationColumnsValue = new List<RelationColumn>(relationColumnsValue);

            //удалить все objID для не иерахических таблиц
            _relationColumnsValue.RemoveAll(col => col.ParentCol == "objID" && col.ParentCol != col.RefCol);


            var repeatingColumns = _relationColumnsValue.GroupBy(c => c.ParentCol).Where(grp => grp.Count() > 1);

            foreach (var repCol in repeatingColumns)
            {
                var repeatedItems = _relationColumnsValue.Where(c => c.ParentCol == repCol.Key && c.Value == "");
                if (repeatedItems != null)
                    _relationColumnsValue.RemoveAll(c => c.ParentCol == repCol.Key && c.Value == "");

            }
        }


        public void Serialize(XElement attributesElement, int ancestorID, int lastInsertedInstanceID)
        {
            var distinctColumnsValue = from colValue in _relationColumnsValue group colValue by colValue.ParentCol into gColValue select gColValue.First();

            foreach (var columnValue in distinctColumnsValue)
            {

                if (!columnValue.ConstraintIsNullable && ancestorID != -1 && string.IsNullOrEmpty(columnValue.TitleAttribute))
                {
                    var ancestorEntity = Storage.Select<Entity>(ancestorID);
                    var query = string.Format("select {0} from {1} where objID = {2}", columnValue.ParentCol, ancestorEntity.SystemName, lastInsertedInstanceID);
                    var table = Storage.GetDataTable(query);
                    columnValue.Value = table.Rows[0][0];
                }


                attributesElement.Add(new XElement
                      (
                         "attribute",
                          new XAttribute("name", columnValue.ParentCol),
                          new XAttribute("alias", columnValue.ParentCol),
                          new XAttribute("value", columnValue.Value),
                          new XAttribute("allowNulls", columnValue.ConstraintIsNullable),
                          new XAttribute("type", "INT"),
                          new XAttribute("IsEditable", true),
                          new XAttribute("IsComputed", false)
                       ));
            }

        }





    }
}
