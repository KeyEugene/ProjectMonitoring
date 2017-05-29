using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Teleform.Reporting
{
    public interface IReportBuilder<T> where T : Report
    {
        void Create(Stream output, T report);
    }
}
