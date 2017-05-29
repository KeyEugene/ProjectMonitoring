#warning 1. Использовать единый интерфейс сохранени шаблона в БД.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace Teleform.ProjectMonitoring.Templates
{
    using Reporting;
    using Reporting.Serialization;
    using System.IO;

    public abstract class GeneralTemplateDesigner : CompositeControl
    {
        protected List<string> AdmissableExtensions = new List<string>();
        protected TextBox TemplateNameBox;
        protected FileUpload FileUpload;
        public string TemplateTypeCode;



        public int userID
        {
            get
            {
                return ViewState["DesignerUserID"] == null ? 0 : (int)ViewState["DesignerUserID"];
            }
            set
            {
                ViewState["DesignerUserID"] = value;
            }
        }

        public abstract bool IsFileBased { get; }

        public string TemplateID //{ get; set; }
        {
            get
            {
                return ViewState["TemplateID"] == null ? null : ViewState["TemplateID"].ToString();
            }
            set { ViewState["TemplateID"] = value; }
        }

        public void Save(bool saveAs = false)
        {
            if (IsFileBased && !FileUpload.HasFile)
                throw new Exception("Для создания шаблона необходимо указать файл.");

            if (string.IsNullOrWhiteSpace(TemplateNameBox.Text))
                throw new Exception("Для создания шаблона необходимо указать его имя.");

            if (IsFileBased && FileUpload.HasFile)
            {
                var extension = Path.GetExtension(FileUpload.PostedFile.FileName);

                if (!AdmissableExtensions.Contains(extension))
                    throw new Exception(string.Concat("Для данного вида шаблонов допустимы следующие расширения файлов: ", string.Join(", ", AdmissableExtensions)));
            }

            var template = GetTemplate();

            //var saveAs = ViewState["saveTemplateAs"] == null ? false : Convert.ToBoolean(ViewState["saveTemplateAs"]);
            if (saveAs)
            {
                template.ID = null;
                template.FileName = template.Name;
            }

            SaveToDatabase(template.Serialize());

            if (!string.IsNullOrEmpty(TemplateID))
            {

                //Storage.ClearInstanceCache(typeof(Template), TemplateID);
                Storage.CleareTypeByEntityID(typeof(Template), template.Entity.ID.ToString());
            }

        }

        protected abstract Template GetTemplate();

        private void SaveToDatabase(string xml)
        {

            if (string.IsNullOrWhiteSpace(xml))
                throw new ArgumentNullException("xml", "Получено недействительное xml-представление шаблона.");

            Storage.ExecuteNonQueryXML("EXEC [model].[R$TemplateSave] @xml", xml);

           

            //var query = string.Format("EXEC [model].[R$TemplateSave] @xml = '{0}'", xml);
            //Storage.GetDataTable(query);


        }
    }
}