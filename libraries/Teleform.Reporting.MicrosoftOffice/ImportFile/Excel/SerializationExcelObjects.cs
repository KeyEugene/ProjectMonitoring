
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Teleform.Reporting.constraint;

namespace Teleform.Reporting.MicrosoftOffice.ImportFile.Excel
{
    public class SerializationExcelObjects
    {
        private Template template;
        private string userID;
        private XElement root;
        private List<object> column;
        private EntityInstance entityInstance;

        public SerializationExcelObjects(Template template, string userID)
        {
            this.template = template;
            this.userID = userID;

            entityInstance = new EntityInstance("", template.Entity.SystemName, false);
            entityInstance.Constraints = template.Entity.Constraints;
            entityInstance.SetRelationColumnsValue();

            GetColumn();
        }

        private void GetColumn()
        {
            column = new List<object>();

            for (byte i = 0; i < template.Fields.Count; i++)
            {
                var field = template.Fields[i];

                var ParentName = template.Entity.SystemName + "_" + template.Entity.SystemName + "/name";

                if (field.Attribute.FPath.Contains("/objID") || field.Attribute.FPath.Contains("/objid"))
                {
                    var fPathWithoutObjID = template.Fields[i].Attribute.FPath.Remove(template.Fields[i].Attribute.FPath.Count() - 6, 6);
                    var constraint = template.Entity.Constraints.FirstOrDefault(x => x.ConstraintName == fPathWithoutObjID);
                    column.Add(constraint);
                }else if (field.Attribute.FPath == ParentName)
                {
                    var fPathWithoutName = template.Fields[i].Attribute.FPath.Remove(template.Fields[i].Attribute.FPath.Count() - 5, 5);
                    var constraint = template.Entity.Constraints.FirstOrDefault(x => x.ConstraintName == fPathWithoutName);
                    column.Add(constraint);
                }
                else
                      column.Add(field);
            }
        }

        private void HeadXml()
        {
            root = new XElement
         (
             "bObject",
             new XAttribute("entity", template.Entity.SystemName),
             new XAttribute("userID", userID)
         );
        }
        public string Serialization(List<string> excelList)
        {
            var attributesElement = new XElement("attributes");
            entityInstance.RemoveAllValues();
            HeadXml();

            for (byte i = 0; i < column.Count; i++)
            {
                if (column[i] is TemplateField)
                {
                    var field = column[i] as TemplateField;
                    attributesElement.Add(
                            new XElement
                            (
                                "attribute",
                                new XAttribute("name", field.Attribute.FPath),
                                new XAttribute("alias", field.Attribute.Name),
                                new XAttribute("value", excelList[i]),
                                new XAttribute("allowNulls", field.Attribute.IsNullable),
                                new XAttribute("type", field.Attribute.SType),
                                new XAttribute("IsEditable", true),
                                new XAttribute("IsComputed", field.Attribute.IsComputed)
                            ));
                }
                else if (column[i] is Constraint)
                {
                    var value = excelList[i];
                    var constraint = column[i] as Constraint;
                    var dependencyRelations = new DependencyRelations(constraint, entityInstance);

                    dependencyRelations.NewEntityInstanceID = value;
                    dependencyRelations.SetRelationColumnsValueFromExcel_GetReferenceTable();
                }
            }

            new XMLRelationColumnsSerializer(entityInstance.RelationColumnsValue).Serialize(attributesElement, -1, -1);



            root.Add(attributesElement);

            return root.ToString();
        }

    }
}
