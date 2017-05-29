using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Office.Reporting
{
    public interface IPlaceholder
    {
        IEnumerable<string> GetPlaceholders();

        void FillPlaceholders(IDictionary<string, string> data);
    }
}
