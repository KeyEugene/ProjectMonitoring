using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting.constraint
{
    [Serializable()]
    public class Constraint
    {


        public Constraint(string constraintID, string constraintName, string alias,
           string refTblName, string refTblID, bool isNullable, bool isIdentified,
           IEnumerable<Column> columns)
        {
            this.ConstraintObjID = constraintID;
            this.ConstraintName = constraintName;
            this.Alias = alias;
            this.RefTblName = refTblName;
            this.RefTblID = refTblID;
            this.IsNullable = isNullable;
            this.IsIdentified = isIdentified;
            this.Columns = columns = new List<Column>(columns);

        }

        public string ConstraintObjID { get; private set; }

        public string ConstraintName { get; private set; }

        public string Alias { get; private set; }
        /// <summary>
        /// Название таблици к которое имеет отношение данное Entity
        /// </summary>
        public string RefTblName { get; private set; }
        /// <summary>
        /// ID таблици к которое имеет отношение данное Entity
        /// </summary>
        public string RefTblID { get; private set; }
        /// <summary>
        /// Возвращает или задаёт значение, указывающее может ли текущий
        /// атрибут принимать null-значение.
        /// </summary>
        public bool IsNullable { get; private set; }

        public bool IsIdentified { get; set; }
        /// <summary>
        /// Список солонок ForeignKey 
        /// </summary>
        public IEnumerable<Column> Columns { get; private set; }

       

    }
}
