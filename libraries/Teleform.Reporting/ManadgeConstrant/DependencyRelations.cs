#define isCreate
#define Viktor

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Teleform.Reporting.constraint;


namespace Teleform.Reporting
{
    using Constraint = Teleform.Reporting.constraint.Constraint;
    using System.Collections;
    public class DependencyRelations
    {

        List<RelationColumn> newRelColumnsForCondition { get; set; }

        List<RelationColumn> relColumnsForCondition { get; set; }

        List<Column> constrantColumns { get; set; }

        EntityInstance entityInstance { get; set; }

        Constraint constraint;

        public string NewEntityInstanceID;

        public Dictionary<string, string> RelationTableTitleAttributes { get; set; }

        private RelationColumn relationColumn;

        private RelationColumn newRelationColumn;

        public string CleanableConstraint;

        public DependencyRelations(Constraint constraint, EntityInstance entityInstance)
        {
            constrantColumns = constraint.Columns.ToList();
            this.entityInstance = entityInstance;
            this.constraint = constraint;
        }

        private bool isRepeatColumn(IEnumerable<Column> columns)
        {
            var isRepeatColumn = false;

            foreach (var column in columns)
            {
                relationColumn = entityInstance.RelationColumnsValue.FirstOrDefault(x => x.ParentCol == column.ParentColumn && x.ConstraintID != constraint.ConstraintObjID);

                newRelationColumn = entityInstance.RelationColumnsValue.FirstOrDefault(x => x.ParentCol == column.ParentColumn && x.ConstraintID == constraint.ConstraintObjID);

                if (relationColumn != null)
                {
                    isRepeatColumn = true;
                    break;
                }

            }
            return isRepeatColumn;
        }

        public DataTable SetRelationColumnsValue_GetReferenceTable(bool isGetReferenceTable, bool isCreate = false)
        {
            // Этот флаг говорит о том,что проходили ли мы "Наш алгоритм"
            bool isOne = false;
            SetNewInstanceID();

            var dt = new DataTable();

            var columns = constrantColumns.Where(conCol => conCol.ParentColumn != "objID");

            if (isRepeatColumn(columns) == true)
            {
                //входящий не обязательный (isNullable="1"), native обязательный (isNullable="0"), нужен condition
                if (!relationColumn.ConstraintIsNullable && newRelationColumn.ConstraintIsNullable)
                {
                    if (isGetReferenceTable)
                        GetReferenceTableWithCondition(ref dt);
                    else
                        SetNewValueSelfOptionConstraint(isCreate);
                }//входящий обязательный, native не обязательный, не нужен condition, native будет обнулен
                else if (relationColumn.ConstraintIsNullable && !newRelationColumn.ConstraintIsNullable)
                {
                    if (isGetReferenceTable)
                        GetReferenceTable(ref dt);
                    else
                        SetNewValueRequiredConstraint_EmptyOptionalConstraint(isCreate);
                }

                 //оба равноправные т.е. (значения isNullable равны )
                else if ((relationColumn.ConstraintIsNullable && newRelationColumn.ConstraintIsNullable || (!relationColumn.ConstraintIsNullable && !newRelationColumn.ConstraintIsNullable)))
                {


                    if (isGetReferenceTable)
                    {
                        if (string.IsNullOrEmpty(newRelationColumn.EntityInstanceID) && string.IsNullOrEmpty(relationColumn.EntityInstanceID))
                            GetReferenceTable(ref dt);
                        else
                            GetReferenceTableWithCondition(ref dt);
                    }
                    else
                        SetNewValueSelfOptionConstraint(isCreate);

                }
                isOne = true;
            }
            //если колонка не повторяется в других констраинт
            else if (dt.Columns.Count == 0)
            {
                if (isGetReferenceTable)
                {
#if Viktor
                    var isParentKeyColumn = constraint.Columns.FirstOrDefault(col => col.IsParentKey == true);
                    GetReferenceTable(ref dt);
                    return dt;

#else

                    if (isCreate)
                    {
                        GetReferenceTable(ref dt);
                        return dt;
                    }
                    else
                    {
                        var isParentKeyColumn = constraint.Columns.FirstOrDefault(col => col.IsParentKey == true);

                        if (isParentKeyColumn == null)
                        {
                            GetReferenceTable(ref dt);
                            return dt;
                        }
                    }
#endif


                }
                else
                    SetNewValueSelfOptionConstraint(isCreate);

                isOne = true;
            }

            if (isCreate && !isOne)
            {
                if (AllIsReadOnly())
                    if (isGetReferenceTable)
                        GetReferenceTable(ref dt);
                    else
                    {
                        SetNewValueSelfOptionConstraint(true);
                        CheckRepeatColumns();
                    }
            }

            return dt;
        }
        /// <summary>
        /// Обнулить необязательный коснтрейнт, поменять Value в обязательном
        /// </summary>
        private void SetNewValueRequiredConstraint_EmptyOptionalConstraint(bool isCreate = false)
        {
            //в выбраном (во входящем) констрейнте получить список relationColumns и их value находящиеся
            var newColumnsValue = new Relation(constraint, NewEntityInstanceID).GetParentColumnsValue();
            var optionalConstraintID = string.Empty;
            var requiredConstraintID = string.Empty;
            foreach (var newColValue in newColumnsValue)
            {
                foreach (var allColValue in entityInstance.RelationColumnsValue)
                {
                    if (newColValue.ParentCol == allColValue.ParentCol && newColValue.ConstraintID != allColValue.ConstraintID)
                    {
                        optionalConstraintID = allColValue.ConstraintID;
                        requiredConstraintID = newColValue.ConstraintID;
                    }
                }
            }

            foreach (var allColValue in entityInstance.RelationColumnsValue)
            {
                //Обнулить необязательный коснтрейнт
                if (allColValue.ConstraintID == optionalConstraintID)
                {
                    allColValue.Value = string.Empty;
                    allColValue.TitleAttribute = string.Empty;
                    CleanableConstraint = optionalConstraintID;
                }
                //в выбраном обязатеьном (во входящем) констрейнте поменять Value
                else if (allColValue.ConstraintID == requiredConstraintID)
                {
                    allColValue.Value = newColumnsValue.First(cv => cv.ParentCol == allColValue.ParentCol).Value;
                    allColValue.EntityInstanceID = NewEntityInstanceID;

                    if (RelationTableTitleAttributes != null)
                    {
                        foreach (var item in RelationTableTitleAttributes)
                        {
                            if (allColValue.ConstraintName == item.Key)
                                allColValue.TitleAttribute = item.Value;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// поменять value в не обязательном констраинт, или в собственном констраинт
        /// </summary>
        private void SetNewValueSelfOptionConstraint(bool isCreate = false)
        {
            var newColumnsValue = new Relation(constraint, NewEntityInstanceID).GetParentColumnsValue();



            var optionalConstraintID = string.Empty;

            foreach (var newColValue in newColumnsValue)
            {
                foreach (var allColValue in entityInstance.RelationColumnsValue)
                {
                    if (newColValue.ParentCol == allColValue.ParentCol && newColValue.ConstraintID == allColValue.ConstraintID)
                        optionalConstraintID = allColValue.ConstraintID;
                }
            }


            foreach (var allColValue in entityInstance.RelationColumnsValue)
            {
                //в выбраном не обязатеьном (во входящем) констрейнте поменять Value
                if (allColValue.ConstraintID == optionalConstraintID)
                {
                    allColValue.Value = newColumnsValue.First(cv => cv.ParentCol == allColValue.ParentCol).Value;

                    if (RelationTableTitleAttributes != null)
                    {
                        foreach (var item in RelationTableTitleAttributes)
                        {
                            if (allColValue.ConstraintName == item.Key)
                                allColValue.TitleAttribute = item.Value;
                        }
                    }
                }
            }



        }
        /// <summary>
        /// Получить ссылочную таблицу 
        /// </summary>
        /// <param name="dt"></param>
        private void GetReferenceTable(ref DataTable dt)
        {

            var query = string.Concat("EXEC [report].[getBObjectdata] '", constraint.RefTblName, "', @flTitle=1, @cyr=1, @parent='", entityInstance.EntityName, "'");
            dt = Storage.GetDataTable(query);

            //фильтр по объектам на циклические ссылки в иерархических таблицах
            if (entityInstance.EntityName == constraint.RefTblName)
            {
                query = string.Format("select objID  from {0} where objID not in (select objID from report.TreeList('{0}','{1}'))",
                  entityInstance.EntityName, entityInstance.EntityInstanceID);

                var objIdTable = Storage.GetDataTable(query);

                var objIDList = new StringBuilder();

                foreach (DataRow row in objIdTable.Rows)
                    objIDList.Append(row["objID"] + ",");

                if (objIDList.Length != 0)
                {
                    objIDList.Length--;
                    var filteredRows = dt.Select(string.Format("objID in ({0})", objIDList.ToString()));
                    dt = filteredRows.CopyToDataTable();
                }
                else
                {
                    dt = null;
                }
            }

        }
        /// <summary>
        /// Получить ссылочную таблицу используя Condition
        /// </summary>
        /// <param name="dt"></param>
        private void GetReferenceTableWithCondition(ref DataTable dt)
        {
            var condition = string.Empty;
            if (!string.IsNullOrEmpty(relationColumn.Value.ToString()))
                condition = string.Format("@condition='{0}.[{1}] = {2}'", constraint.RefTblName, newRelationColumn.RefCol, relationColumn.Value.ToString());
            else
                condition = string.Format("@condition=''");

            var query = string.Format("EXEC [report].[getBObjectdata] '{0}', {1}, @flTitle=1, @cyr=1, @parent='{2}'", constraint.RefTblName, condition, entityInstance.EntityName);

            dt = Storage.GetDataTable(query);
        }

        public void SetRelationColumnsValueFromExcel_GetReferenceTable()
        {
            var allIsReadOnly = AllIsReadOnly();

            var newColumnsValue = new Relation(constraint, NewEntityInstanceID).GetParentColumnsValue();

            for (byte i = 0; i < entityInstance.RelationColumnsValue.Count(); i++)
            {
                for (byte j = 0; j < newColumnsValue.Count; j++)
                {
                    if (entityInstance.RelationColumnsValue[i].ParentCol == newColumnsValue[j].ParentCol && entityInstance.RelationColumnsValue[i].ConstraintID == newColumnsValue[j].ConstraintID)
                        entityInstance.RelationColumnsValue[i].Value = newColumnsValue[j].Value;
                }

            }
        }

        //Проверяем есть ли в повторяющиеся parent columns с другими constraintObjID
        private void CheckRepeatColumns()
        {
            for (byte i = 0; i < entityInstance.RelationColumnsValue.Count(); i++)
            {
                for (byte j = 0; j < constraint.Columns.Count(); j++)
                {
                    if (entityInstance.RelationColumnsValue[i].ConstraintID != constraint.ConstraintObjID
                        && entityInstance.RelationColumnsValue[i].ParentCol == constraint.Columns.ElementAt(j).ParentColumn)
                    {
                        CleanableConstraint = entityInstance.RelationColumnsValue[i].ConstraintID;
                        i = (byte)(entityInstance.RelationColumnsValue.Count() - 1); //Досрочно выйти из двух циклов
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Проверяет все ли колонки в констрейнте IsReadOnly = true
        /// Если да, возвращаем true
        /// </summary>
        /// <returns></returns>
        private bool AllIsReadOnly()
        {
            bool allIsReadOnly = false;
            for (int q = 0; q < constrantColumns.Count; q++)
            {
                if (constrantColumns[q].IsParentKey)
                {
                    allIsReadOnly = true;
                    break;
                }
            }
            return allIsReadOnly;
        }

        private void SetNewInstanceID()
        {
            for (int i = 0; i < entityInstance.RelationColumnsValue.Count; i++)
            {
                if (entityInstance.RelationColumnsValue[i].ConstraintID == constraint.ConstraintObjID)
                    entityInstance.RelationColumnsValue[i].EntityInstanceID = NewEntityInstanceID;
            }
        }
    }
}
