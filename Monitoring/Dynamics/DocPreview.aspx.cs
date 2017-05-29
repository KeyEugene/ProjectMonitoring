
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Data;
using Monitoring;
using System.IO;
using Teleform.ProjectMonitoring.Dynamics;
using Teleform.Reporting;
using Teleform.Reporting.DynamicCard;

namespace Teleform.ProjectMonitoring
{
    public partial class DocPreview : BasePage, IHttpHandler
    {
        private const string
            ExcelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            WordMimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";


        public override void ProcessRequest(HttpContext context)
        {
            var id = context.Request["id"];
            var entityID = Convert.ToInt32(context.Request["entity"]);

            if (id != null)
            {
                var entitySystemName = Storage.Select<Entity>(entityID).SystemName;

                var query = string.Format
                (
                    "SELECT [A].[objID], [A].[body], [A].[modified], [MT].[mime] FROM [{0}] [A] LEFT JOIN [MimeType] [MT] ON [A].[mimeTypeID] = [MT].[objID] WHERE [A].[objID] = {1} ",
                    entitySystemName,
                    id
                );

                var table = Storage.GetDataTable(query);

                if (table.Rows.Count > 0)
                {
                    var row = table.Rows[0];

                    var mimeType = row["mime"] == DBNull.Value ? null : row["mime"].ToString();
                    var documentID = Convert.ToInt32(row["objID"]);
                    var documentBody = row["body"] == DBNull.Value ? null : (byte[])row["body"];
                    var modified = row["modified"] == DBNull.Value ? new DateTime(2011, 9, 22) : (DateTime)row["modified"];


                    if (documentBody != null && mimeType != null)
                    {                        
                        var cacheDirectory = Server.MapPath("~/Dynamics/temp_data/doc-cache");

                        if (!Directory.Exists(cacheDirectory))
                            Directory.CreateDirectory(cacheDirectory);

                        context.Response.Clear();

                        if (HtmlConverter.CanConvert(mimeType))
                        {
                            HtmlConverter.Convert(mimeType, documentBody, cacheDirectory, documentID.ToString(), modified);

                            var documentUrl = string.Format("temp_data/doc-cache/{0}/index.html", documentID);

                            context.Response.ContentType = "text/html";
                            context.Response.Write(string.Format
                            (
                                "<html><body><iframe src='{0}' width=\"100%\" height=\"100%\" style=\"border: none\"></iframe></body></html>",
                                documentUrl
                            ));
                        }
                        else
                        {
                            context.Response.ContentType = mimeType;
                            context.Response.BinaryWrite(documentBody);
                        }
                    }
                    else
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "text/html";
                        context.Response.Write("<html><body><h1>Данный тип документа не отображается.</h1></body></html>");
                    }
                }
                else
                {
                    context.Response.Clear();
                    context.Response.ContentType = "text/html";
                    context.Response.Write("<html><body><h1>Запрашиваемый документ не существует.</h1></body></html>");
                }

                context.Response.Flush();
                context.Response.End();
            }
        }
    }
}

