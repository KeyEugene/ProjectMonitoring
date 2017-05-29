#define MultiSort

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Teleform.Reporting.Web
{
    partial class TableViewControl
    {
        private Style sortingStyle, filterStyle, predicateControlStyle, activeStyle;

        public Style ActiveStyle
        {
            get
            {
                if (activeStyle == null)
                    activeStyle = new Style();

                return activeStyle;
            }
        }

        public Style FilterStyle
        {
            get
            {
                if (filterStyle == null)
                    filterStyle = new Style();

                return filterStyle;
            }
        }

        public Style PredicateControlStyle
        {
            get
            {
                if (predicateControlStyle == null)
                    predicateControlStyle = new Style();

                return predicateControlStyle;
            }
        }

        public Style SortingStyle
        {
            get
            {
                if (sortingStyle == null)
                    sortingStyle = new Style();

                return sortingStyle;
            }
        }

    }
}
