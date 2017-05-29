using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.SqlClient;
using System.Data;
using Teleform.Reporting;

namespace Teleform.ProjectMonitoring.Templates
{

    public class TemplateFactory
    {
        private string TemplateID;
        private string TemplateTypeCode;
        private string EntityID;

        public TemplateFactory(string templateTypeCode, string templateID, string entityID)
        {
            this.TemplateTypeCode = templateTypeCode;

            this.TemplateID = templateID;

            this.EntityID = entityID;
        }

        public GeneralTemplateDesigner InstantiateIn()
        {
            //зная код шаблона, который нужно создать, в таблице базы данных R$TemplateType узнаем имя класса, который наследуется от WebСontrol
            if (string.IsNullOrEmpty(this.TemplateTypeCode))
                throw new ArgumentException("Не задан код типа для работы с шаблоном.");

            //string executorName = string.Empty;


            var query = string.Format("SELECT [executor] FROM [model].[R$TemplateType] WHERE [code] = '{0}'", this.TemplateTypeCode);
            var executorName = Storage.ExecuteScalarString(query);

            
            //using (var c = new SqlConnection(Kernel.ConnectionString))
            //using (var cmd = new SqlCommand("SELECT [executor] FROM [model].[R$TemplateType] WHERE [code] = @templateCode", c))
            //{
            //    c.Open();
            //    cmd.Parameters.Add("templateCode", SqlDbType.VarChar).Value = this.TemplateTypeCode;
            //    executorName = cmd.ExecuteScalar().ToString();
            //}




            if (string.IsNullOrEmpty(executorName))
                throw new ArgumentException("У данного типа шаблона нет обработчика.", "executorName");

            var typeName = System.Type.GetType(executorName);

            //создаем экземпляр этого класса
            var templateControl = (GeneralTemplateDesigner)Activator.CreateInstance(typeName);
            
            templateControl.ID = "TemplateControl";
            templateControl.TemplateTypeCode = TemplateTypeCode;

            if (!string.IsNullOrEmpty(this.TemplateID))
                templateControl.TemplateID = this.TemplateID;


            if (!string.IsNullOrEmpty(this.EntityID))
            {
                if ((templateControl is TableTemplateDesigner))
                    (templateControl as TableTemplateDesigner).EntityID = EntityID;
                else if (templateControl is TreeBasedTemplateDesigner)
                    (templateControl as TreeBasedTemplateDesigner).EntityID = EntityID;
                else if (templateControl is CrossReportTemplateDesigner)
                    (templateControl as CrossReportTemplateDesigner).EntityID = EntityID;
                   
            }
            return templateControl;
        }
    }
}

