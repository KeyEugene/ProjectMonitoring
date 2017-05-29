using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting
{
    [Serializable()]
    public class ListAttributeAggregation
    {
        /// <summary>
        /// имя колонки по которой осуществляется аггрегация
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// лексема аггрегации
        /// </summary>
        public string AggregateLexem { get; set; }


        public ListAttributeAggregation(string columnName, string aggregateLexem)
        {
            ColumnName = columnName;
            AggregateLexem = aggregateLexem;
        }
    }
}
