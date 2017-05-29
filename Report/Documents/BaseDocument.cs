using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Report.Documents
{
    public enum TemplateType
    {
        GenericTemplate,
        WordTemplate,
        ExcelTemplate
    };

    public abstract class BaseDocument
    {
        internal string Location { get; set; }
        internal string DocumentName { get; set; }
        internal string FullPath
        {
            get
            {
                return Path.Combine(Location, DocumentName);
            }
        }

        internal string TemplatePath { get; set; }

        public TemplateType OriginalTemplate { get; set; }
    }
}
