using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Teleform.Reporting.Web;

namespace Teleform.Reporting.Web
{
    partial class SortingControl
    {
        public event EventHandler SortingApplied;

        void ApplySorting_Click(object sender, EventArgs e)
        {
            if (SortingApplied != null)
                SortingApplied(this, EventArgs.Empty);            
        }

   }
}
