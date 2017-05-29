using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Teleform.Reporting;

namespace Teleform.Reporting.WordExcelTemplateAddIns
{
    public class AddInAttribute : IAddInElement
    {
        public string ID { get; set; }

        public string Alias { get; set; }

        public Type Type { get; set; }

    }
}
