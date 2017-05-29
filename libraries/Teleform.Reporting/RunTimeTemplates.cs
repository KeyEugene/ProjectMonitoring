using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Teleform.Reporting.Reporting.Template;
using System.Data;

namespace Teleform.Reporting
{
    public static class RunTimeTemplates
    {

        public static Template GetRuntimeTemplate(string templateID)
        {
            var index = templateID.IndexOf("_");

            var entityID = templateID.Substring(index + 1);

            var entity = Storage.Select<Entity>(entityID);

            var name = string.Empty;
            if (templateID.Contains("require"))
                name = string.Format("Создание и редактирование объектов_{0}", entity.SystemName);
            else
                name = string.Format("Титульный шаблон_{0}", entity.SystemName);

            var fileNameAttribute = "";
            var typeNameAttribute = "Табличная форма";
            var typeCodeAttribute = "TableBased";
            var templateByDefault = false;
            var treeType = EnumTreeType.Undefined;
            byte[] content = null;
            var fields = getCardTemplateFields(templateID, entity);

            var parameters = new Dictionary<string, string>();
            parameters.Add("sheet", "Лист1");

            var template = new Template(templateID, name, entity, fileNameAttribute, typeNameAttribute, typeCodeAttribute, templateByDefault, treeType, content, fields, parameters);

            return template;
        }

        private static List<Teleform.Reporting.TemplateField> getCardTemplateFields(string templateID, Entity entity)
        {
            var fields = new List<Teleform.Reporting.TemplateField>();


            var query = string.Empty;

            if (templateID.Contains("title"))
            {
                //query = string.Format("select hash, tbl from model.BObjectMap('{0}') where (title=1 or tbl='{0}' and sType<>'U') and attr<>nameA order by fPath", entity.SystemName);

                query = string.Format("select 	b.hash,b.tbl from (select m.hash,m.tbl from model.BObjectMap('{0}') m  join (select min(hash)hash, iif(tblID=parentTblID,constr,tbl)tbl, attr from model.BObjectMap('{0}') where sType<>'u' and title=1 group by iif(tblID=parentTblID,constr,tbl), attr) m0 on m.sType<>'u' and m.title=1 and m0.hash=m.hash)b", entity.SystemName);

            }
            else
            {
                query = string.Format("select hash, tbl from model.BObjectMapRequiredOnly('{0}') order by fPath", entity.SystemName);
            }

            var dt = Storage.GetDataTable(query);



            foreach (DataRow row in dt.Rows)
            {
                var hashCommon = row.ItemArray[0];

                var nativeEntity = row.ItemArray[1];

                var attr = entity.Attributes.First(a => a.ID.ToString() == hashCommon.ToString());

                var field = new Teleform.Reporting.TemplateField(attr);

                field.HashName = hashCommon.ToString();

                field.NativeEntityName = nativeEntity.ToString();

                fields.Add(field);

            }


            return fields;
        }

    }
}
