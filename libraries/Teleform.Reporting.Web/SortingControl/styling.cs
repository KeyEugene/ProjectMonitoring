using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

using SortDirection = System.Web.UI.WebControls.SortDirection;

namespace Teleform.Reporting.Web
{
    partial class SortingControl
    {
        public bool Active
        {
            get
            {
                //EnsureChildControls();

                return !string.IsNullOrWhiteSpace(AttributeID);
            }
        }

        public Style ActiveStyle { get; set; }





        private TableItemStyle sortedAscendingHeaderStyle, sortedDescendingHeaderStyle;

        /// <summary>
        /// Получает или задает стиль CSS, применяемый к заголовку столбца данного
        /// элемента управления, когда столбец отсортирован по возрастанию.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public TableItemStyle SortedAscendingHeaderStyle
        {
            get
            {
                if (sortedAscendingHeaderStyle == null)
                {
                    sortedAscendingHeaderStyle = new TableItemStyle();
                }

                return sortedAscendingHeaderStyle;
            }
        }

        /// <summary>
        /// Получает или задает стиль CSS, применяемый к заголовку столбца данного
        /// элемента управления, когда столбец отсортирован по убыванию.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public TableItemStyle SortedDescendingHeaderStyle
        {
            get
            {
                if (sortedDescendingHeaderStyle == null)
                    sortedDescendingHeaderStyle = new TableItemStyle();

                return sortedDescendingHeaderStyle;
            }
        }
  

    }
}
