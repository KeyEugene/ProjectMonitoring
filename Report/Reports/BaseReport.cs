using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Report.OXML.Templates;

namespace Report.Reports
{
    public abstract class BaseReport
    {
        private BaseTemplate _template;

        public BaseReport(BaseTemplate template)
        {
            this._template = template;
        }
    }
}
