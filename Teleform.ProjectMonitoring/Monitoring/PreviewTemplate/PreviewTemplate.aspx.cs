using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using Teleform.Office.TemplatePreview;
using System.IO;
using System.Reflection;
using Teleform.ProjectMonitoring.HttpApplication;

namespace Monitoring.Reporting
{
    public partial class PreviewTemplate : Teleform.ProjectMonitoring.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ExampleButton.DataBind();
        }

        public void ShowPreviewTemplateButton_Click(object sender, EventArgs e)
        {
            string name, fileName;
            byte[] body;
            DownLoadFromBase(out name, out fileName, out body);
            var ind = Server.MapPath("~").LastIndexOf("\\");
            var path = string.Format(@"{0}\update\{1}{2}", Server.MapPath("~").Substring(0, ind), name, Path.GetExtension(fileName));

            try
            {
                File.WriteAllBytes(path, body);
                using (WordInterop wi = new WordInterop())
                   wi.SaveWithHtmlExtension(path);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при создании файла:\r\n" + ex.Message);
            }
            try
            {
               // FileStream read = File.OpenRead(Path.ChangeExtension(path, "htm"));
                File.Delete(path);
            }
            catch { }
        }

        private void DownLoadFromBase(out string name, out string fileName, out byte [] body)
        {
            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = @"SELECT [RT].[name],[RT].[fileName],[RT].[body]
                                        FROM [model].[R$Template] [RT]
                                        JOIN [MimeType] [M] ON [RT].[typeID]=[M].[objID]
                                        WHERE [RT].[objID]=@objID";
                cmd.Parameters.AddWithValue("@objID", 58);
                cmd.Connection = con;
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    try
                    {
                        sdr.Read();
                        body = (byte[])sdr["body"];
                        name = sdr["name"].ToString();
                        fileName = sdr["fileName"].ToString();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Ошибка скачивания файла:\r\n" + ex.Message);
                    }
                    con.Close();
                }
            }
        }
    }
}