using Teleform.Reporting;

namespace Teleform.ProjectMonitoring.Templates_Anton
{
    interface ITemplatePersister<T> where T : Template
    {
        T Create();
        object Insert(string xml);
        void Update(string xml, object id);
    }
}
