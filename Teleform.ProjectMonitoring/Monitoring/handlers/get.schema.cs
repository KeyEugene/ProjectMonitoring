using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Xml;
using System.Text;

using System.Data;
using System.Data.SqlClient;

using Teleform.Reporting;
using Teleform.ProjectMonitoring.HttpApplication;

namespace Teleform.ProjectMonitoring
{
    public partial class LocalSchemaHandler : System.Web.UI.Page, IHttpHandler
    {

        public string ErrorMessage;

        private static IHttpHandler handler;


        static LocalSchemaHandler()
        {
            handler = new DbSchemaHandler();
        }


        public override void ProcessRequest(HttpContext context)
        {
            var url = context.Request.Url;

            var login = context.Request.QueryString["login"];
            var password = context.Request.QueryString["password"];

            //var secureSring = context.Request.QueryString["secureSring"];

            var connString = string.Format(ConfigurationManager.ConnectionStrings["Server"].ConnectionString, login, password);
            Global.ConnectionString = connString;

            if (TryAuthenticate(login, password))
                handler.ProcessRequest(context);

        }


        private bool TryAuthenticate(string login, string password)
        {
            using (var conn = new SqlConnection(Storage.ConnectionString))
            {
                var command = new SqlCommand
                {
                    CommandText = "EXEC [Permission].[Authenticate] @login, @password",
                    Connection = conn
                };

                command.Parameters.AddRange(new[] {
                    new SqlParameter { ParameterName = "login", Value = login },
                    new SqlParameter { ParameterName = "password", Value = password }
                });

                var adapter = new SqlDataAdapter(command);
                var dt = new DataTable();

                try
                {
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    return false;
                }

                //var personID = dt.Rows[0]["_personID"].ToString();
                //var objID = dt.Rows[0]["objID"].ToString();
                //Session["SystemUser"] = true;
                //Session["SystemUser.ID"] = string.IsNullOrEmpty(personID) ? "" : personID;
                //Session["SystemUser.objID"] = objID;
                //Session["SystemUser.Name"] = dt.Rows[0]["FullName"];
                //Session["SystemUser.typeID"] = dt.Rows[0]["typeID"];

                //if (dt.Columns["TypeName"] != null)
                //    Session["SystemUser.typeName"] = dt.Rows[0]["TypeName"];


                //string query = string.Format("INSERT INTO [Log].[ServerSession] ([sessionID], [username], [start]) VALUES ('{0}','{1}',GETDATE())", Session.SessionID, login);
                //Storage.GetDataTable(query);              

                return true;
            }
        }
    }
}