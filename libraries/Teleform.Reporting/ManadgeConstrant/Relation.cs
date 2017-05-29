#define Viktor

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using System.Data;
using Constraint = Teleform.Reporting.constraint.Constraint;

namespace Teleform.Reporting
{
    public class Relation
    {

        public Relation(Constraint constraint, string referenceEntityInstanceID)
        {
            this._constraint = constraint;
            this._referenceEntityInstanceID = referenceEntityInstanceID;
        }


        public List<RelationColumn> RelationColumns { get; private set; }

        private string _referenceEntityInstanceID { get; set; }

        private Constraint _constraint { get; set; }



        public List<RelationColumn> GetParentColumnsValue()
        {
            var relationColumns = new List<RelationColumn>();

            var constrColumns = _constraint.Columns;

            if (constrColumns.Count() > 0)
            {
                var refColumns = new StringBuilder();

                foreach (var column in constrColumns)
                    refColumns.Append(string.Format("[{0}],", column.RefColumn));

                refColumns.Remove(refColumns.Length - 1, 1);

                

                var query = string.Format("SELECT {0} FROM [{1}] where objID = {2}", refColumns, _constraint.RefTblName, _referenceEntityInstanceID);
                var dt = Storage.GetDataTable(query);

                var dtColumnsName = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();

                foreach (var colName in dtColumnsName)
                {
                    var constrColumn = constrColumns.First(col => col.RefColumn == colName);

                   
#if Viktor
                    string value = string.Empty;
                    if (dt.Rows.Count > 0)                    
                        value = dt.Rows[0][colName].ToString();                    

#else
                    var value = dt.Rows[0][colName];
#endif

                    relationColumns.Add(new RelationColumn
                        {
                            ConstraintName = _constraint.ConstraintName,
                            ParentCol = constrColumn.ParentColumn,
                            RefCol = constrColumn.RefColumn,
                            Value = value,
                            ConstraintIsNullable = constrColumn.IsNullable,
                            ConstraintID = _constraint.ConstraintObjID,
                            ConstraintColumnsCount = _constraint.Columns.Count(),
                            EntityInstanceID = this._referenceEntityInstanceID

                        });
                }

                return relationColumns;
            }
            throw new ArgumentNullException("в связанной таблице все колнки только для чтения");
        }

    }

}
