using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting
{
    public class RelationColumn
    {
        public string ConstraintName { get; set; }

        public string ConstraintID { get; set; }

        public bool ConstraintIsNullable { get; set; }

        public int ConstraintColumnsCount { get; set; }

        
        public string ParentCol { get; set; }

        public string RefCol { get; set; }

        public object Value { get; set; }

        public string TitleAttribute { get; set; }

        
        public string EntityInstanceID { get; set; }
        
    }
}
