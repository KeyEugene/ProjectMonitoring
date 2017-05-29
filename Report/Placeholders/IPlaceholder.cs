using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Teleform.Office.Reporting.Placeholders
{
    public interface IPlaceholder
    {
        IEnumerable<PlaceholderData> GetPlaceholders();

        void FillPlaceholders(IDictionary<string, string> data);
    }
}
