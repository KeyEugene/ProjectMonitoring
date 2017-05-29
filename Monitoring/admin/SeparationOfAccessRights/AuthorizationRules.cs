using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teleform.ProjectMonitoring.HttpApplication;
using System.Data;
using Teleform.Reporting;

namespace Teleform.ProjectMonitoring.admin.SeparationOfAccessRights
{
    public class AuthorizationRules
    {

        public static DataTable EntityInstancesResolution(DataTable table, Entity entity, string userID)
        {
            if (table.Rows.Count == 0)
                return new DataTable();

            var query = string.Format("SELECT * FROM Permission.UserPermission({0}, '{1}') WHERE [objID] IS NOT NULL", userID, entity.SystemName);

            var InstanceNoPermission = Storage.GetDataTable(query).AsEnumerable().Where(x => !Convert.ToBoolean(x["read"])).Select(o => o["objID"].ToString()).ToList<string>();

            DataRow[] rows = table.AsEnumerable().Where(x => !InstanceNoPermission.Contains(x["objID"].ToString())).ToArray();

            return rows.CopyToDataTable();
        }


        public static bool TemplateResolution(ActionType actionEnum, string userID, string objIDTemplate = null)
        {
            var dt = new System.Data.DataTable();
            var query = string.Empty;

            if (actionEnum == ActionType.create)
            {
                query = string.Concat(@"SELECT [" + actionEnum.ToString() + "] FROM [Permission].[UserPermissionForObject] (" + userID + ",'R$Template', NULL)");
                dt = Global.GetDataTable(query);            
            }
            else
            {
                //query = @"SELECT [" + actionEnum.ToString() + "] FROM [Permission].[UserPermissionForObject] (" + userID + ",'R$Template', " + objIDTemplate + ")";
                query = string.Concat(@"SELECT [" + actionEnum.ToString() + "] FROM [Permission].[IUTemplatePermission] (" + userID + ") where objID = " + objIDTemplate + "");
                dt = Global.GetDataTable(query);
                   
            }
            if (dt.Rows.Count == 0)
                return false;
            else
            {
                bool result = false;
                Boolean.TryParse(dt.Rows[0][0].ToString(), out result);

                return result;
            }
        }

    }
}