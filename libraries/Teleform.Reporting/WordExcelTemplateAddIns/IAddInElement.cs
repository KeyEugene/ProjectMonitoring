using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting.WordExcelTemplateAddIns
{
    public interface IAddInElement
    {
        string ID { get; set; }

        string Alias { get; set; }
    }
}
