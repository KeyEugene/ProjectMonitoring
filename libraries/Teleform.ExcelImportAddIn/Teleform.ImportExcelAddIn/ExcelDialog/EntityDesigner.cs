using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Teleform.Reporting;
using Teleform.Reporting.Providers;
using Excel = Microsoft.Office.Interop.Excel;
using TemplateAndShemaData = Teleform.ImportExcelAddIn.SaveShemaAndTemplate.Serialize;
using Constraint = Teleform.Reporting.constraint.Constraint;
using Teleform.ImportExcelAddIn;

namespace ExcelDialog
{

    public partial class EntityDesigner
    {
        internal static EntityForm entityForm;
        internal static Schema Schema { get; private set; }
        internal Template template { get; private set; }
        #region General Form
        internal FieldType fieldType { get; private set; }
        internal long MaxValue;
        internal long MinValue;
        #endregion
        private List<EntityInstance> collectionEntityInstance { get; set; }
        private DependencyRelations dependencyRelation;

        #region Excel data
        public string NameColumn { get; private set; }
        public Constraint constraintLink { get; private set; }
        private string templateName;
        private string adressCell;
        private int numberRow;
        private int numberColumn;
        private object oldValueCell;
        #endregion

        internal List<ListItem> GetData(Excel.Application excelApplication)
        {
            #region get Schema
            if (TemplateAndShemaData.schema == null)
            {
                FillSchema();
                TemplateAndShemaData.schema = Schema;
            }
            else
                Schema = (Schema)TemplateAndShemaData.schema;

            #endregion

            ParsersExcel();

            #region get Template
            if (TemplateAndShemaData.template == null)
            {
                GetTemplate();
                TemplateAndShemaData.template = template;
            }
            else
                template = (Template)TemplateAndShemaData.template;
            #endregion

            #region get CollectionEntityInstance

            if (TemplateAndShemaData.collectionEntityInstance != null)
                collectionEntityInstance = (List<EntityInstance>)TemplateAndShemaData.collectionEntityInstance;
            else
                TemplateAndShemaData.collectionEntityInstance = collectionEntityInstance = new List<EntityInstance>();

            while (collectionEntityInstance.Count <= numberRow)
            {
                var entityInstance = new EntityInstance(string.Empty, template.Entity.SystemName, false);
                entityInstance.Constraints = template.Entity.Constraints;
                entityInstance.SetRelationColumnsValue();
                collectionEntityInstance.Add(entityInstance);

            }

            #endregion

            var field = FindConstraint(NameColumn);
            var dt = new DataTable();

            if (field is Constraint)
            {
                dt = GetDataTableForListItem(constraintLink = field as Constraint);
                fieldType = FieldType.Constraint;
            }
            else if (field is TemplateField)
            {
                GetFieldType(field as TemplateField);
                return null;
            }

            return CreateListItems(dt);
        }

        private void GetFieldType(TemplateField field)
        {
            switch (field.Attribute.Type.RuntimeType)
            {
                case "System.String":
                    fieldType = FieldType.String;
                    this.MaxValue = (long)field.Attribute.Type.Length;
                    break;
                case "System.Boolean":
                    fieldType = FieldType.Bool;
                    break;
                case "System.DateTime":
                    fieldType = FieldType.Date;
                    break;
                case "System.Decimal":
                    fieldType = FieldType.Money;
                    MaxValue = field.Attribute.Type.MaxValue;
                    MinValue = field.Attribute.Type.MinValue;
                    break;
                case "System.Byte":
                    fieldType = FieldType.Number;
                    MaxValue = field.Attribute.Type.MaxValue;
                    MinValue = field.Attribute.Type.MinValue;
                    break;
                case "System.Double":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.UInt16":
                case "System.UInt32":
                    fieldType = FieldType.Number;
                    MaxValue = field.Attribute.Type.MaxValue;
                    break;
                default:
                    break;
            }

        }

        private DataTable GetDataTableForListItem(Constraint constraint)
        {
            CheckValidationObject(constraint);

            dependencyRelation = new DependencyRelations(constraint, collectionEntityInstance[numberRow]);

            return dependencyRelation.SetRelationColumnsValue_GetReferenceTable(true, true); //ReferenceTableDataSource;
        }

        private object FindConstraint(string nameColumn)
        {
            for (byte q = 0; q < template.Fields.Count; q++)
            {
                if (template.Fields[q].Name == nameColumn || template.Fields[q].Attribute.Name == nameColumn)
                {
                    var fPath = template.Fields[q].Attribute.FPath;

                    var ParentName = template.Entity.SystemName + "_" + template.Entity.SystemName + "/name";

                    if (fPath.Contains("objID") || fPath.Contains("objid") || fPath.Contains(ParentName))
                        return GetConstraint(fPath);
                    else
                        return template.Fields[q];
                }
            }
            return null;
        }

        private void CheckValidationObject(Constraint constraint)
        {
            var entityInstanceID = collectionEntityInstance[numberRow].RelationColumnsValue.FirstOrDefault(x => x.ConstraintID == constraint.ConstraintObjID).EntityInstanceID;
            var olderValueCell = oldValueCell == null ? "" : oldValueCell.ToString();
            var CleanValue = !string.IsNullOrEmpty(olderValueCell) ? GetObjID(olderValueCell) : "";

            if (entityInstanceID == null) entityInstanceID = "";

            if (entityInstanceID != CleanValue) //Проверяем соответсвует ли запись в Excel и в объекте collectionEntityInstance[numberRow]
                UpdateValueFromRow();
        }

        private void UpdateValueFromRow()
        {
            for (byte i = 1; i < (template.Fields.Count + 1); i++)
            {
                var valueCell = Globals.ThisAddIn.Application.ActiveSheet.Cells(numberRow, i).Value;
                var nameColumn = Globals.ThisAddIn.Application.ActiveSheet.Cells(1, i).Value;

                var constraint = FindConstraint(nameColumn);

                if (constraint is TemplateField)
                    continue;
                else if (constraint is Constraint)
                    constraint = constraint as Constraint;

                var relationColumnsValue = collectionEntityInstance[numberRow].RelationColumnsValue;

                for (byte q = 0; q < relationColumnsValue.Count; q++)
                {
                    if (relationColumnsValue[q].ConstraintID == constraint.ConstraintObjID)
                    {
                        if (string.IsNullOrEmpty(valueCell))
                        {
                            relationColumnsValue[q].Value = "";
                            relationColumnsValue[q].EntityInstanceID = "";
                            continue;
                        }

                        valueCell = GetObjID(valueCell);

                        if (relationColumnsValue[q].EntityInstanceID != valueCell)
                        {
                            relationColumnsValue[q].EntityInstanceID = "";
                            relationColumnsValue[q].Value = "";
                            Globals.ThisAddIn.Application.ActiveSheet.Cells(numberRow, i).Value = "";
                        }
                    }
                }
            }

        }

        // Запись в определенную ячейку информацию
        public void SetCellValue(string value)
        {
            try
            {
                Globals.ThisAddIn.Application.ActiveSheet.Cells(numberRow, numberColumn).Value = value;

                if (fieldType == FieldType.Constraint)
                {
                    dependencyRelation.NewEntityInstanceID = GetObjID(value);
                    dependencyRelation.SetRelationColumnsValue_GetReferenceTable(false, true);

                    if (!String.IsNullOrEmpty(dependencyRelation.CleanableConstraint))
                        CleareValueConstraint();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось сохранить значение, попробуйте снова.");
            }
        }

        private void CleareValueConstraint()
        {
            var constraint = template.Entity.Constraints.FirstOrDefault(x => x.ConstraintObjID == dependencyRelation.CleanableConstraint);

            for (int i = 0; i < template.Fields.Count; i++)
            {
                if (!template.Fields[i].Attribute.FPath.Contains("objID"))
                    continue;

                if (GetConstraint(template.Fields[i].Attribute.FPath).ConstraintName == constraint.ConstraintName)
                {
                    Globals.ThisAddIn.Application.ActiveSheet.Cells(numberRow, ++i).Value = string.Empty;
                    i--;
                }
            }
        }

    }
}
