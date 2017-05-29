

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Teleform.Reporting.Web;

namespace Teleform.Reporting.Web
{
    public partial class CompositePredicateControl
    {
        
        public event EventHandler FilterApplied;
        public event EventHandler FilterCanceled;

        void ApplyFilter_Click(object sender, EventArgs e)
        {
            if (FilterApplied != null)
                FilterApplied(this, EventArgs.Empty);           
        }

        void CancelFilter_Click(object sender, EventArgs e)
        { 
            if (FilterCanceled != null)
                FilterCanceled(this, EventArgs.Empty);

            ApplyFilter_Click(this, EventArgs.Empty);
        }
    }
}
