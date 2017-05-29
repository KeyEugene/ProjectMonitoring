using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting.DynamicCard
{
    public static class QueryBuilderExtensions
    {
        public static string MakeQuery(this CardListRelationField listRelation)
        {
            var list = new List<string>();

            foreach (var column in listRelation.ListRelationColumns)
            {                
                if (!listRelation.Card.FieldsValuesFromDB.ContainsKey(column.RefColumn))
                    throw new Exception(column.RefColumn);

list.Add(string.Format("{0}.[{1}] = {2}", listRelation.Entity.SystemName, column.ParentColumn, listRelation.Card.FieldsValuesFromDB[column.RefColumn]
                ));

            }            
            return string.Join(" AND ", list);
        }
    }
}
