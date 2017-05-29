using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Teleform.Reporting.Web
{
    partial class CompositePredicateControl
    {
        public bool Active
        {
            get
            {
                EnsureChildControls();

                return !string.IsNullOrWhiteSpace(UserPredicateBox.Text) && !string.IsNullOrWhiteSpace(ValueBox.Text);
            }
        }

        public Style ActiveStyle
        { get; set; }
    }
}
