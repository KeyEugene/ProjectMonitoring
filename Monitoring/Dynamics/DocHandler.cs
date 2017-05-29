using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;


namespace Teleform.ProjectMonitoring.Dynamics
{
    public class DocHandler : IHttpHandler
    {

        public HttpContext con;

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var id = context.Request.Params["id"];

            if (id != null)
            {
                var strg = string.Format("SELECT [body] FROM [_Application] WHERE [objID] = {0}", id);
                object body = null;

                using (var comm = new SqlConnection(ConfigurationManager.ConnectionStrings["stend"].ConnectionString))
                using (var cmd = new SqlCommand(strg, comm))
                {
                    comm.Open();
                    body = cmd.ExecuteScalar();
                    comm.Close();
                }
                if (body != null)
                {
                    //context.Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";  // "application/ms-word"; // "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; 
                    //context.Response.BinaryWrite((byte[])body);
                    context.Response.ContentType = "text/html";
                    context.Response.Write("<h1>Heelo</h1>");
                    context.Response.Flush();
                    return;
                } else
                {
                    //InfoMultiView.ActiveViewIndex = 0;
                }

            }
            context.Response.End();
        }




    }
}
