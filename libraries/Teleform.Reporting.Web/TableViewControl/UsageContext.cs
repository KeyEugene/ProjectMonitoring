#define Viktor
#define isNotDeprecated

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web;

namespace Teleform.Reporting.Web
{
    public class UsageContext
    {
        public Dictionary<string, object> SortingData { get; private set; }

        public Dictionary<string, object> FilterData { get; private set; }

        public string ExpressionForSelectCheckBoxItems { get; set; }

        public SortDirection? SortDirection { get; set; }

        public int SelectedRowIndex { get; set; }

        public int CurrentPage { get; set; }

        public int LeftPageIndex { get; set; }

        public UsageContext()
        {          
            SortingData = new Dictionary<string, object>();
            FilterData = new Dictionary<string, object>();
        }
    }
}
