using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace Teleform.ProjectMonitoring
{
    /// <summary>
    /// Данный обработчик нужен для отправки отчетов в Response страницы(см страницу PreparedReports)
    /// Необходим так как кнопка, которая осуществляет загрузку, находится в UpdatePanel
    /// </summary>
    public class LoadReportHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var request = context.Request.ToString();
            var param = request.Split('#');
            param[0] = param[0].Replace('%', ' ');
            param[0] = param[0].Replace('@', ':');

            string link = string.Empty;
            string name = string.Empty;
            using (var c = new SqlConnection(Kernel.ConnectionString))
            using (var ad = new SqlDataAdapter("SELECT [link], [name] FROM [R_Report] WHERE [created] = @created AND [userID] = userID", c))
            {
                ad.SelectCommand.Parameters.Add("created", SqlDbType.DateTime).Value = param[0];
                ad.SelectCommand.Parameters.Add("userID", SqlDbType.Int).Value = param[1];

                var table = new DataTable();
                ad.Fill(table);

                link = table.Rows[0]["link"].ToString();
                name = table.Rows[0]["name"].ToString();
            }

            if (string.IsNullOrEmpty(link)) return;

            link = context.Server.MapPath(link);

           
            var response = context.Response;
            response.Clear();
            response.AddHeader("Content-Type", "application/octet-stream");
            response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}{1}", name, Path.GetExtension(link)));
            response.WriteFile(link);
            response.End();
        }

        public bool IsReusable { get { return true; } }
    }
}