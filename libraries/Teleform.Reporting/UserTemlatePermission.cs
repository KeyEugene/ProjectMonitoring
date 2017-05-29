#define Viktor
#define alexj

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Teleform.Reporting.constraint;
//using Teleform.Reporting.DynamicCard;

namespace Teleform.Reporting
{
    public class UserTemlatePermission
    {
        public static void SetFieldsTaboo(int userID, Template template)
        {
            if (userID == null)
                userID = 0;

            var deniedEntity = StorageUserObgects.Select<UserEntityPermission>(userID, userID).getReadDeniedEntities().AsEnumerable().Select(x => x["entity"].ToString()).ToList<string>();

            foreach (var field in template.Fields)
            {
                var entityName = deniedEntity.FirstOrDefault(e => e == field.NativeEntityName);
                if (entityName != null)
                    field.IsForbidden = true;                
                else
                    field.IsForbidden = false;
            }
        }

        private static Teleform.Reporting.constraint.Constraint GetConstraintByAttributeFpath(TemplateField field, Entity entity)
        {

            //найти констраин, по имени поля 
            Teleform.Reporting.constraint.Constraint constr = null;

            var fPath = field.Attribute.FPath;
            var indexOf = fPath.IndexOf("/");

            if (indexOf != -1)
            {
                var constrName = fPath.Substring(0, indexOf);
                constr = entity.Constraints.FirstOrDefault(x => x.ConstraintName == constrName);
            }

            return constr;

        }

        public static List<object> GetPermittedTemplates(string entityID, string userID, string lexem, string typeCode, List<Template> runtimeTeplates = null)
        {
            var ReportsTemplateslist = new List<object>();

            if (string.IsNullOrEmpty(userID))
                userID = "0";

            var query = string.Format(@"select CONVERT(varchar(200), p.[objID]) as [templateID], p.[name] [templateName], p.[type] [typeName]
                                      from [Permission].[IUTemplatePermission]({0}) p, model.BTables t
                                      where t.[name] = p.[baseTable] and t.[object_ID] = '{1}'  and p.[read] = 1 and p.[typeCode] {2} '{3}'
                                      order by p.[name]", userID, entityID, lexem, typeCode);

            var dt = Storage.GetDataTable(query);

#if alexj
            var deniedEntities = StorageUserObgects.Select<UserEntityPermission>(Convert.ToInt32(userID), Convert.ToInt32(userID)).getReadDeniedEntities().AsEnumerable().Select(x => x["entity"].ToString()).ToList<string>();

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                var template = Storage.Select<Template>(dt.Rows[i]["templateID"].ToString());

                if (template.TypeCode == Reporting.Template.EnumTypeCode.TableBased)
                {
                    foreach (var field in template.Fields)
                    {
                        var constraint = GetConstraintByAttributeFpath(field, template.Entity);

                        if (constraint != null && constraint.IsIdentified && field.Attribute.SType != "Table")
                        {
                            var fieldEntity = field.NativeEntityName;

                            if (deniedEntities.Contains(fieldEntity))
                            {
                                dt.Rows.RemoveAt(i);
                                break;
                            }

                        }
                    }
                }
                else
                {
                    foreach (var field in template.Fields)
                    {
                        var fieldEntity = field.NativeEntityName;

                        if (deniedEntities.Contains(fieldEntity))
                        {
                            dt.Rows.RemoveAt(i);
                            break;
                        }
                    }
                }

            }

        

            

#endif


            if (runtimeTeplates != null)
            {
                foreach (var template in runtimeTeplates)
                {
                    var newRow = dt.NewRow();
                    newRow[0] = template.ID;
                    newRow[1] = template.Name;
                    dt.Rows.Add(newRow);
                }
            }

            if (dt.Rows.Count > 0)
            {
                var sortedRows = dt.Select("", "templateID DESC");
                dt = sortedRows.CopyToDataTable();
            }


            foreach (DataRow row in dt.Rows)
            {
                var templateID = row["templateID"];
                var typeName = row["typeName"];
                var templateName = row["templateName"];

                var entityName = string.Empty;
                
                var text = string.Empty;
                if (runtimeTeplates != null)
                    text = string.Format("{0}", templateName);
                else
                    text = string.Format("({0}) {1}", typeName, templateName);

                ReportsTemplateslist.Add(new { Value = templateID, Text = text });

            }
            return ReportsTemplateslist;
        }

    }

}
