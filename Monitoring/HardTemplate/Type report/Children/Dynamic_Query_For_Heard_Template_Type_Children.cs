#define alexj1

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Teleform.ProjectMonitoring.HttpApplication;
using Teleform.Reporting;

namespace Teleform.ProjectMonitoring.HardTemplate.Type_report.Children
{
    public class Dynamic_Query_For_Heard_Template_Type_Children : DynamicQueryForHeardTemplate
    {


        public Dynamic_Query_For_Heard_Template_Type_Children(string sqlQuery, Template template, Hashtable sortHashTable) :
            base (sqlQuery, template, sortHashTable)
        {
            
        }
        /// <summary>
        /// Первый запрос где parentID is null
        /// example 
        /// select distinct jnt0.[objID] [objID]
        ///,jnt1.[name] [Организации/В составе/Название]
        ///,jnt0.[nameF] [Название поное]
        ///,jnt2.[count] [Список Организации]
        /// from [_Division] jnt0
        /// left join [_Division]jnt1 on jnt0.[parentID]=jnt1.[objID]
        /// left join (select [_Division].[parentID] objID,count(*)[count] from [_Division] group by [_Division].[parentID]) jnt2 on jnt2.objID=jnt0.objID
        /// where jnt0.parentID is null  <- вот эту часть меняем 
        /// </summary>
        public DataTable FirstQuery()
        {
            this.parent = "parentID is null";
            return Global.GetDataTable(SqlQuery);
        }

        protected override void PreRequestToSql()
        {
#if alexj
            select.Append(", [objID]");
            groupBy.Append(", [objID]");
#endif
            
        }

    }
}