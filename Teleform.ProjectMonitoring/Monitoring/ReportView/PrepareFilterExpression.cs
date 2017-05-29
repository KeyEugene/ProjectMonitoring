using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Teleform.Reporting;
using System.Data;
using Teleform.ProjectMonitoring.HttpApplication;

namespace Teleform.ProjectMonitoring
{
    public partial class ReportView
    {
        protected string GetNavigatFilterExpression()
        {
            var constraintID = Request.QueryString["constraint"];
            var instanceID = Request.QueryString["id"];
            var entityID = Request.QueryString["entity"];

            string navigationFilterExpression = string.Empty;

            if (!string.IsNullOrEmpty(constraintID) && !string.IsNullOrEmpty(instanceID))
            {
                var query = string.Format("EXEC report.getListAttributeInstances {0}, {1}", constraintID, instanceID);
                var instanceIDDataTable = Global.GetDataTable(query);

                if (instanceIDDataTable.Rows.Count > 0)
                {
                    int[] instanceIDArray = instanceIDDataTable.AsEnumerable().Select(s => s.Field<int>("objID")).ToArray<int>();
                    var instanceIDStr = string.Join(", ", instanceIDArray);
                    navigationFilterExpression = string.Concat("objID in (", instanceIDStr, ")");
                }
                else
                {                    
                    navigationFilterExpression = "no instances";
                }
                
            }

            return navigationFilterExpression;
        }

       

    }
}