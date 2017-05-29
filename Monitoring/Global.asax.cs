#define Alex

#define UserFilter

using System;
using System.Web;
using System.Linq;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Configuration;

using SqlConnection = System.Data.SqlClient.SqlConnection;
using SqlCommand = System.Data.SqlClient.SqlCommand;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace Teleform.ProjectMonitoring.HttpApplication
{
    using Teleform.Reporting;
    using Teleform.Reporting.Parsers;
    using Teleform.Reporting.Web;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Data;

    public class Global : System.Web.HttpApplication
    {
        private static object l = new object();
        private static readonly string SchemaXMLKey = "Application.Schema.XML", SchemaKey = "Application.Schema";

        //public static string MapPath { get; set; }

        public static string ConnectionString
        {
            get
            {
                return Storage.ConnectionString;
            }
            set
            {
                Storage.ConnectionString = value;
            }
        }


        public static string StartServerConnectionString
        {
            get
            {
                System.Web.HttpContext.Current.Session["StartServerConnectionString"] = string.Format(ConfigurationManager.ConnectionStrings["StartServer"].ConnectionString);

                var b = System.Web.HttpContext.Current.Session["StartServerConnectionString"];
                return b.ToString();
            }
        }

        /// <summary>
        /// Возвращает основную схему, на которую опирается приложение.
        /// </summary>
        public static Schema Schema
        {
            get
            {
                var context = HttpContext.Current;

                if (context == null)
                    throw new InvalidOperationException("Для предоставления схемы необходим http-контекст.");


                var contextApp = context.Application[SchemaKey];

                if (contextApp == null)
                    lock (l)
                        contextApp = context.Application[SchemaKey] = (new SchemaParser()).Parse(SchemaXML);

                var schema = (Schema)contextApp;
                return (Schema)contextApp;

            }
        }

        /// <summary>
        /// Возвращает xml-представление схемы, на которое опирается данное приложение,
        /// необходимое для внешних приложений.
        /// </summary>
        public static string SchemaXML
        {
            get
            {
                var context = HttpContext.Current;

                if (context == null)
                    throw new InvalidOperationException("Для предоставления схемы необходим http-контекст.");


                var contextApp = context.Application[SchemaXMLKey];

                if (contextApp == null)
                    contextApp = context.Application[SchemaXMLKey] = GetSchemaXML();

                return contextApp.ToString();

            }
        }


        

        void Application_Start(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine(Environment.GetEnvironmentVariable("PATH"));

            Phoenix.Web.Phoenix.ShowStackTrace = false;

            Storage.archivatorPath = Server.MapPath("7za.exe");
        }

        public void Application_Error(object sender, EventArgs e)
        {
            try
            {
                var error = Server.GetLastError();

                if (error.GetType() == typeof(HttpException) || error.GetType() == typeof(HttpUnhandledException))
                {
                    var session = Application[Session.SessionID];

                    if (session != null)
                    {
                        var query = string.Format("SELECT [objID] FROM [Log].[ServerSession] WHERE [sessionID] = '{0}'", Session.SessionID);

                        var adapter = new SqlDataAdapter(query, new SqlConnection(session.ToString()));
                        var table = new DataTable();

                        adapter.Fill(table);

                        var id = table.Rows[0]["objID"];

                        query = string.Format(@"INSERT INTO [Log].[Error] ([serverSessionID], [time], [message], [innerException], [stackTrace]) VALUES ({0}, GETDATE(), '{1}', '{2}', '{3}')",
                                                            id,
                                                            error.Message,
                                                            error.InnerException,
                                                            error.StackTrace);

                        GetDataTable(query);
                    }
                    else
                    {
                        var innerException = error.InnerException.InnerException.Message.Replace("'", "*");

                        var query = string.Format("EXEC [log].[InsertError] '{0}', '{1}', '{2}', '{3}', '{4}'",
                        Session.SessionID,
                        Server.MachineName,
                        error.Message,
                        innerException,
                        error.StackTrace);

                        GetDataTable(query);
                    }
                    if (error.Message.Contains("NoCatch") || error.Message.Contains("maxUrlLength"))
                        return;


                   Server.Transfer(@"~/ErrorPage2.aspx");
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                
                Server.Transfer(@"~/ErrorPage2.aspx");
            }
        }

        void Session_Start(object sender, EventArgs e)
        {
            var sessionID = Session.SessionID;

            Session["contexts"] = new Dictionary<string, SessionContent>();
            Session["FormatMoneyFlag"] = 0;
          
        }

        void Session_End(object sender, EventArgs e)
        {
            Session.Clear();
        }
        /// <summary>
        /// Возвращает из БД тип DataTable
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string query)
        {
            try
            {
                var da = new SqlDataAdapter(query, Global.ConnectionString);
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (SqlException ex)
            {
                throw new Exception("Не удалось сохранить значения в таблицу", ex.InnerException);
            }
        }

       

        public static string ExeccuteScalarString(string query)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                try
                {
                    var result = cmd.ExecuteScalar().ToString();
                    return result;

                }
                catch (NullReferenceException)
                {
                    throw new Exception(string.Format("Не удалось сохранить значения в таблицу"));
                }
            }
        }


        #region Hepler's
        ///// <summary>
        ///// Запрос к БД ExecuteScalar, возврат типа strign
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //public static string ExeccuteScalar(string query)
        //{
        //    using (var conn = new SqlConnection(Global.ConnectionString))
        //    using (var cmd = new SqlCommand(query, conn))
        //    {
        //        conn.Open();
        //        try
        //        {
        //            var result = cmd.ExecuteScalar().ToString();
        //            return result;
        //        }
        //        catch (NullReferenceException)
        //        {
        //            throw new Exception(string.Format("Не удалось сохранить значения в таблицу"));
        //        }
        //    }
        //}
        /// <summary>
        /// Возвращает из БД тип DataSet
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string query)
        {
            try
            {
                var da = new SqlDataAdapter(query, Global.ConnectionString);
                var ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (SqlException ex)
            {
                throw new Exception("Не удалось сохранить значения в таблицу", ex.InnerException);
            }
        }



        /// <summary>
        /// Извлекает из базы данных xml-представление схемы.
        /// </summary>
        private static string GetSchemaXML()
        {
            var context = HttpContext.Current;

            if (context == null)
                throw new InvalidOperationException("Для создания схемы необходим http-контекст.");

            using (var c = new SqlConnection(Global.ConnectionString))
            {
                var command = new SqlCommand
                {
                    CommandText = "SELECT [Model].GetSchema()",
                    Connection = c
                };

                c.Open();

                return command.ExecuteScalar().ToString();
            }
        }

        public static void UpdateSchema()
        {
            var context = HttpContext.Current;

            if (context == null)
                throw new InvalidOperationException();

            var application = context.Application;

            application.Lock();

            Storage.ClearAllCache();

            application.Remove(SchemaKey);
            application.Remove(SchemaXMLKey);

            application.UnLock();
        }
        #endregion
    }
}
