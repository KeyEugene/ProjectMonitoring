using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Teleform.Reporting;

namespace Teleform.ProjectMonitoring.Templates
{
    public class InputExcelTemplateDesigner : TableTemplateDesigner
    {
        public TemplateFieldCollection collectionFields
        {
            get { return ViewState["collectionFields"] as TemplateFieldCollection; }
            set
            {
                ViewState["collectionFields"] = value;
            }
        }
        protected override Template CreateTemplate()
        {
            #region Добавляем обязательные поля для заполнения
            Entity entity;
            GetNotNullFields(out entity);
            #endregion

            var content = new byte[0];
            var template = new Template(string.Empty, entity, "InputExcelBased", content);

            template.Fields.AddRange(collectionFields);

            return template;
        }

        private void GetNotNullFields(out Entity entity)
        {
            entity = Storage.Select<Entity>(EntityID);

            var fieldCollection = entity.Attributes.Where(x => !x.FPath.Contains("/") && !x.IsNullable && x.FPath.ToLower() != "objid").Distinct().Select(x => new TemplateField(x)).AsEnumerable();
            collectionFields = new TemplateFieldCollection(fieldCollection);

            var constraint = entity.Constraints.Where(x => !x.IsNullable).ToArray();
            List<TemplateField> list = new List<TemplateField>();

            for (int i = 0; i < constraint.Count(); i++)
            {
                var field = entity.Attributes.FirstOrDefault(x => x.FPath.ToLower() == (constraint[i].ConstraintName + "/objID").ToLower());
                if (field != null)
                    list.Add(new TemplateField(field));
            }

            collectionFields.AddRange(list);
        }

        protected override void InitializationDesigner()
        {
            designer.EntityID = EntityID;
            designer.userID = this.userID;
            //designer.FilterAttributeIDList = Template.Fields.Select(x => x.Attribute.ID.ToString()).ToList();
            designer.isInputExcelBased = true;
        }

        public void CheckTemplateForRequiredFields()
        {
            if (collectionFields == null || collectionFields.Count == 0)
            {
                Entity entity;
                GetNotNullFields(out entity);
            }

            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < collectionFields.Count; j++)
            {
                bool isHad = false;
                for (int i = 0; i < template.Fields.Count; i++)
                {
                    if (template.Fields[i].Attribute.FPath == collectionFields[j].Attribute.FPath)
                    {
                        isHad = true;
                        break;
                    }
                }

                if (!isHad)
                {
                    sb.Append(collectionFields[j].Name + ", ");
                    isHad = false;
                }
            }

            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                throw new Exception(sb.ToString() + " - это'и поле'я не могут быть пустым'и.");
            }
        }
    }
}