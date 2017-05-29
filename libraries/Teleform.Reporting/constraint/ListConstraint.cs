using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting.constraint
{
    [Serializable()]
    public class ListConstraint
    {

        

        public string ConstraintID { get; private set; }
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
        public string ParentTblName { get; private set; }
        public string ParentTblID { get; private set; }

        public string Key { get; private set; }

        //public bool IsBackRef { get; private set; }
        //public bool isIdentified { get; private set; }
        //public bool isBasic { get; private set; }
        //public bool isTerminal { get; private set; }
        //public bool isHierarchic { get; private set; }


        /// <summary>
        /// Список солонок ForeignKey 
        /// </summary>
        public IEnumerable<Column> Columns { get; private set; }

        public ListConstraint(string constraintID, string constraintName, string alias,
            string refTblName, string refTblID, string parentTblName, string parentTblID, string key,
            IEnumerable<Column> columns)
        {
            this.ConstraintID = constraintID;
            this.ConstraintName = constraintName;
            this.Alias = alias;
            this.RefTblName = refTblName;
            this.RefTblID = refTblID;
            this.ParentTblName = parentTblName;
            this.ParentTblID = parentTblID;
            this.Key = key;
            this.Columns = columns = new List<Column>(columns);
        }
    }
}
