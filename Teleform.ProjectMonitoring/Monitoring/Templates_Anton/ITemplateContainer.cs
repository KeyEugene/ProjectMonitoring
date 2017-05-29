using System.Collections.Generic;
using Teleform.Reporting;

namespace Teleform.ProjectMonitoring.Templates_Anton
{
    interface ITemplateContainer<T> where T : Template
    {
        string Name { get; }

        IEnumerable<string> AdmissableExtensions { get; }

        T Template { get; }
    }
}