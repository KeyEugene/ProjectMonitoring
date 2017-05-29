#define Alex

using System.Collections.Generic;
using Teleform.Reporting.MicrosoftOffice;
using Teleform.Reporting.Serialization;

namespace Teleform.ProjectMonitoring.Templates_Anton
{
    public class ExcelTemplateContainer : ITemplateContainer<ExcelTemplate>
    {
        private ITemplatePersister<ExcelTemplate> persister;

        private string name;
        private int id;
        private List<string> admissableExtensions;
        private ExcelTemplate template { get; set; }


        public string Name
        {
            get { return name; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        
        public IEnumerable<string> AdmissableExtensions
        {
            get { return admissableExtensions; }
        }

        public string Sheet
        {
            get { return template.Sheet; }
        }

        public ExcelTemplate Template
        {
            get
            {
                return template;
            }
        }

        public ExcelTemplateContainer(string name, byte[] body, string fileName = null)
        {
            this.name = name;
            this.admissableExtensions = new List<string> { ".xlsx" };
            this.persister = new ExcelTemplatePersister(name, body);
            this.template = persister.Create();
        }

        public int Insert()
        {
            var templateXml = this.template.Serialize();
            id = (int) persister.Insert(templateXml);

            return id;
        }

        public void Update(object id)
        {
            var templateXml = this.template.Serialize();
            persister.Update(templateXml, id);

            //(persister as ExcelTemplatePersister).Save(templateXml, id);
        }
    }
}