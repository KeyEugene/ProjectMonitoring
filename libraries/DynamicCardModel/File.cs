using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting.DynamicCard
{
    public class File
    {
        public string FileName { get; set; }

        public byte[] Content { get; set; }

        public string MimeType { get; set; }

        public bool IsModified { get; set; }

        public override string ToString()
        {
            return FileName ?? string.Empty;
        }
    }
}
