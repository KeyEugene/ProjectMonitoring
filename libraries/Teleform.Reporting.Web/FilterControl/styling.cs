using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Teleform.Reporting.Web
{
    partial class FilterControl
    {
        public bool Active
        {
            get
            {
                EnsureChildControls();
                return CheckBoxList.SelectedIndex != -1;
            }
        }

        public Style ActiveStyle
        { get; set; }
    }
}
