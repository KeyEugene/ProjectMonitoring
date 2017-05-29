using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace Teleform.ProjectMonitoring.Templates
{
    using Reporting;
    using Teleform.ProjectMonitoring.HttpApplication;

    public class WordTemplateDesigner : FileBasedTemplateDesigner
    {
        public WordTemplateDesigner()
        {
            AdmissableExtensions.Add(".docx");
        }

        public override bool IsFileBased
        {
            get { return true; }
        }

        protected override void CreateChildControls()
        {
            var table = CreateBasicControls();
            this.Controls.Add(table);
        }

        protected override Template RetrieveTemplate(string name, byte[] body, string TemplateID = null )
        {
            var fields = new List<TemplateField>();
            Entity entity = null;

            using (var t = new Teleform.Office.Reporting.OpenXMLWordTemplate(body))
            {
                var placeholders = t.GetPlaceholders().ToList();
                if (placeholders.Count == 0)
                    throw new Exception("Указанный файл не содержит шаблон.");

                var creatorID = new Teleform.Reporting.UniqueIDCreator();
                object entityID, attributeID, formatID;

                creatorID.Split(placeholders.First().Tag, out entityID, out attributeID, out formatID);
                entity = Global.Schema.Entities.FirstOrDefault(o => o.ID.ToString() == entityID.ToString());

                Attribute attribute = null;
                foreach (var p in placeholders)
                {
                    creatorID.Split(p.Tag, out entityID, out attributeID, out formatID);
                    attribute = entity.Attributes.FirstOrDefault(o => o.ID.ToString() == attributeID.ToString());

                    if (attribute == null)
                        throw new InvalidOperationException(
                            string.Format("Сущность '{0}' не имеет атрибут с идентификатором {1}.",
                                entity.Name, attributeID));
                    
#warning порядок всегда равен 0
                    var field = new TemplateField(attribute);

                    field.Format = attribute.Type.GetAdmissableFormats().FirstOrDefault(o => o.ID.ToString() == formatID.ToString());
                    if (field.Format == null)
                        throw new InvalidOperationException(
                            string.Format("Тип '{0}' атрибута {1} не имеет формат с идентификатором {2}.",
                                attribute.Type.Name, attribute.Name, formatID));
                    fields.Add(field);
                }
            }
#warning при содании требуется задавать id, а также fileName и name, задаются в GeneralTemplateDesigner
            var template = new Template(name, entity, this.TemplateTypeCode, body, fields, TemplateID);

            return template;
        }
    }
}