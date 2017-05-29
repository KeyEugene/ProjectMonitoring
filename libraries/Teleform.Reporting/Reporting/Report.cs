using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting 
{
    public class BuilderAttribute : System.Attribute
    {
        public System.Type BuilderType { get; private set; }

        public BuilderAttribute(System.Type builderType)
        {
            BuilderType = builderType;
        }
    }

    public class ParametersAttribute : System.Attribute
    {
        public object[] Parameters { get; private set; }
        public ParametersAttribute(params object[] parameters)
        {
            if (parameters != null)
                for (int i = 0; i < parameters.Count(); i++)
                {
                    if (parameters[i] is System.Type)
                        parameters[i] = Activator.CreateInstance(parameters[i] as System.Type);
                }
            Parameters = parameters;
        }
    }

    public class ContentTypeAttribute : System.Attribute
    {
        public string ContentType { get; private set; }

        public string Extension { get; private set; }

        public ContentTypeAttribute(string contentType, string extension)
        {
            ContentType = contentType;
            Extension = extension;
        }
    }

    public abstract class Report 
    {
        public enum Type {
            Word,
            ZipWord,
            Excel,
            ZipExcel,
            Csv
        }

        public Template Template { get; private set; }
        public Type ReportType { get; set; }

        public Report(Template template) 
        {
            this.Template = template;
        }
    }
}
