using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting
{
    public class SingleReport: Report
    {
        public Instance Instance {get; private set;}

        public SingleReport(Template template, Instance instance): base(template)
        {
            this.Instance = instance;
        }

        public static implicit operator GroupReport(SingleReport report)
        {
            throw new NotImplementedException();
        }
    }
}
