using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting
{
    public class ExcelReport : GroupReport
    {
        public string Sheet { get; private set; }

        public ExcelReport(Template template, string sheet, IEnumerable<Instance> instances)
            : base(template, instances)
        {
            Sheet = sheet;
        }
    }
}
