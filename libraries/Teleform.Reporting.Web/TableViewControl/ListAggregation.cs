using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Globalization;



namespace Teleform.Reporting.Web
{
    public static class ListAggregation
    {

        public static void ComputeAggrigate(Template template, DataTable refTbl)
        {
            var listAttributesArrg = template.Fields.Where(f => f.ListAttributeAggregation != null);

            if (listAttributesArrg.Count() > 0)
            {
                var entity = Storage.Select<Entity>(template.Entity.ID);
                foreach (var listAttrAggr in listAttributesArrg)
                {
                    if (!string.IsNullOrEmpty(listAttrAggr.ListAttributeAggregation.AggregateLexem) && !string.IsNullOrEmpty(listAttrAggr.ListAttributeAggregation.ColumnName))
                    {
                        var parentEntitySystemName = Storage.Select<Entity>(listAttrAggr.Attribute.EntityID).SystemName;

                        //получить колонку по которой будем группировать                  
                        var listConstr = entity.Lists.First(o => o.ConstraintName == listAttrAggr.Attribute.FPath);                        
                        var parentAttrGroupBy = listConstr.Key.ToString();

                        //получить колонку к которой будет применена агригационная функция                    
                        var parentAttrAggr = listAttrAggr.ListAttributeAggregation.ColumnName;

                        //получить колонку в которую будет записано значение агрегации
                        //var listAttrAggrName = listAttrAggr.Name;
                        var listAttrAggrHashName = listAttrAggr.HashName;

                        //получить лексему
                        var lexem = listAttrAggr.ListAttributeAggregation.AggregateLexem;
                        var parentTableQuery = string.Format("select {0} as parentAttrGroupBy, {1} from {2}", parentAttrGroupBy, parentAttrAggr, parentEntitySystemName);
                       
                        var parentTbl = Storage.GetDataTable(parentTableQuery);
                        var parentAttrAggrType = parentTbl.Columns[parentAttrAggr].DataType;

                        AddListAggregateColumn(parentTbl, parentAttrAggr, refTbl, listAttrAggrHashName, parentAttrAggrType, lexem);
                    }
                }
            }
        }

        private static void AddListAggregateColumn(DataTable parentTbl, string parentAttrAggr, DataTable refTbl, string listAttrAggrHashName, System.Type parentAttrAggrType, string lexem)
        {
            refTbl.Columns.Add(listAttrAggrHashName, parentAttrAggrType);

            foreach (DataRow dRow in refTbl.Rows)
            {
                var expression = string.Format("{0} ({1})", lexem, parentAttrAggr);                

                var filter = string.Format("{0} = {1}", "parentAttrGroupBy", dRow["objID"]);

                dRow[listAttrAggrHashName] = parentTbl.Compute(expression, filter);
            }
        }
    }
}
