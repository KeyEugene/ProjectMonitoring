using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Teleform.Reporting
{
   public abstract class GroupReportBuilder : IGroupReportBuilder
    {
       protected IGroupReportBuilder ReportBuilder { get; private set; }

       public GroupReportBuilder(IGroupReportBuilder reporterBuilder)
       {
           this.ReportBuilder = reporterBuilder;
       }

       public abstract void Create(Stream output, GroupReport report);
    }
}