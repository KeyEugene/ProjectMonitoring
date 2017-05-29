using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting.constraint
{
    [Serializable()]
    public class Column
    {
        /// <summary>
        /// Название колонки у этого объекта(entity), которое ссылается на родителя
        /// </summary>
        public string ParentColumn { get; private set; }
        /// <summary>
        /// Назваине колонки в родительской табице 
        /// </summary>
        public string RefColumn { get; private set; }
        public bool IsNullable { get; private set; }
        public bool IsParentKey { get; private set; }

        public Column(string parentColumn, string refColumn, bool isNullable, bool isParentKey)
        {
            this.ParentColumn = parentColumn;
            this.RefColumn = refColumn;
            this.IsNullable = isNullable;
            this.IsParentKey = isParentKey; 
        }

    }
}
