using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting.WordExcelTemplateAddIns
{
    public class AddInEntity : IAddInElement
    {
        public string ID { get; set; }

        public string name { get; set; }

        public string Alias { get; set; }

        public List<AddInAttribute> Attributes { get; set; }
    }
}
