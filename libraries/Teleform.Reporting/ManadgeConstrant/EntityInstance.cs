using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using System.Data;
using Constraint = Teleform.Reporting.constraint.Constraint;
using Column = Teleform.Reporting.constraint.Column;
using System.Data.SqlClient;





namespace Teleform.Reporting
{
    [Serializable]
    public class EntityInstance
    {

        public EntityInstance(string entityInstanceID, string entityName, bool displayReferenceTableControl)
        {
            this.DisplayReferenceTableControl = displayReferenceTableControl;
            this.EntityInstanceID = entityInstanceID;
            this.EntityName = entityName;

        }

        public bool IsChanged { get; set; }

        /// <summary>
        ///true указывает, что instance был создан при работе DisplayReferenceTableControl, т.е. имеет constrain & RelationColumnsValue
        ///false указывает, что instance был создан вне DisplayReferenceTableControl, т.е имеет SelfColumnsValue и не имеет constrain & RelationColumnsValue
        /// </summary>
        public bool DisplayReferenceTableControl { get; set; }

        public IEnumerable<Constraint> Constraints { get; set; }

        public string EntityInstanceID { get; set; }

        public string EntityName { get; private set; }

        /// <summary>
        /// можно использовать для формирования XML, при сохранени объекта
        /// </summary>
        public List<RelationColumn> RelationColumnsValue { get; set; }

        public void SetRelationColumnsValue()
        {
            RelationColumnsValue = new List<RelationColumn>();

            var instanceColumnsValue = GetInstanceColumnsValue();

            var instanceColumnTitile = getInstanceColumnsTitle();

            foreach (var constraint in Constraints)
            {
                foreach (var column in constraint.Columns)
                {
                    RelationColumnsValue.Add(new RelationColumn
                    {
                        TitleAttribute = instanceColumnTitile[constraint.ConstraintName].ToString(),
                        ConstraintName = constraint.ConstraintName,
                        ParentCol = column.ParentColumn,
                        RefCol = column.RefColumn,
                        Value = instanceColumnsValue[column.ParentColumn],
                        ConstraintIsNullable = constraint.IsNullable,
                        ConstraintID = constraint.ConstraintObjID,
                        ConstraintColumnsCount = constraint.Columns.Count(),
                        EntityInstanceID = this.EntityInstanceID,
                    });
                }
            }
        }

        public Dictionary<string, string> GetInstanceColumnsValue()
        {
            string query = string.Empty;

            Dictionary<string, string> instanceColumnValueDict;

            if (!string.IsNullOrEmpty(EntityInstanceID) && EntityInstanceID != "-1")
            {
                var entity = Storage.SelectEntityByName(EntityName);


                var attribute = entity.Attributes.First(x => x.FPath == "objID");

                query = string.Format("SELECT * FROM [{0}] WHERE [{2}] = '{1}'", EntityName, EntityInstanceID, attribute.Col);

                var dt = Storage.GetDataTable(query);

                var row = dt.Rows[0];

                instanceColumnValueDict = row.Table.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => row[col.ColumnName].ToString());
            }
            else
            {
                var dt = Storage.GetDataTable(string.Concat("SELECT * FROM ", EntityName, " WHERE [objID] = -1"));

                instanceColumnValueDict = dt.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName.ToString(), col => string.Empty);
            }

            return instanceColumnValueDict;
        }

        public Dictionary<string, string> SelfColumnsValue { get; set; }

        private IDictionary<string, object> getInstanceColumnsTitle()
        {
            string query = string.Empty;

            IDictionary<string, object> instanceColumnTitleDict;

            if (!string.IsNullOrEmpty(EntityInstanceID) && EntityInstanceID != "-1")
            {

                query = string.Format("EXEC [report].[getBObjectdata] '{0}', NULL, @cyr=0, @flTitle=2, @instances = {1}", EntityName, EntityInstanceID);

                var dt = Storage.GetDataTable(query);


                var row = dt.Rows[0];

                instanceColumnTitleDict = row.Table.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => row[col.ColumnName]);
            }
            else
            {
                query = string.Format("EXEC [report].[getBObjectdata] '{0}', NULL, @cyr=0, @flTitle=2, @instances = -1", EntityName);

                var dt = Storage.GetDataTable(query);

                instanceColumnTitleDict = dt.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName.ToString(), col => (object)string.Empty);
            }

            return instanceColumnTitleDict;
        }
        
        public void RemoveAllValues()
        {
            for (byte i = 0; i < this.RelationColumnsValue.Count; i++)
            {
                this.RelationColumnsValue[i].Value = "";
            }
        }
    }
}
