#define Alex
#define alexj

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using Teleform.ProjectMonitoring.HttpApplication;
using System.Configuration;
using Teleform.Reporting;
using Teleform.Reporting.Sequring;
//using System.Data.SqlClient;

namespace Teleform.ProjectMonitoring
{
    public partial class LoginPage : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {           
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            #region Авторизация через проект  Репетитор

            if (!IsHaveAcce())
                return;

            var query = string.Concat("SELECT [repetitorPersonID] FROM [permission].[Acce$$] where [sID] = '", Session.SessionID, "'",
            @" DELETE [permission].[Acce$$] WHERE [sID] = '", Session.SessionID, "'");//Сразу удаляем  для того, что бы пойтом выйти из системы.

            var da = new SqlDataAdapter(query, Global.StartServerConnectionString);
            var dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0 || dt.Rows[0]["repetitorPersonID"] == DBNull.Value)
                return;

            da = new SqlDataAdapter("SELECT [login], [pwd] FROM [_User] WHERE [_personID] = " + dt.Rows[0]["repetitorPersonID"].ToString(),
                Global.StartServerConnectionString);
            dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                ErrorMessageBox.InnerText = "Данного пользователя не существует в этом проекте.";
                return;
            }
            var login = dt.Rows[0]["login"].ToString();
            var password = dt.Rows[0]["pwd"].ToString();

            var connString = string.Format(ConfigurationManager.ConnectionStrings["Server"].ConnectionString, login, password);
            Storage.ConnectionString = connString;

            if (TryAuthenticate(login, password))
                Session_Start(login);

            #endregion
        }


        protected void TryLoginButton_Click(object o, EventArgs e)
        {            
            var login = LoginBox.Text;
            var password = Encryption.Encrypt(PasswordBox.Text);


            var connString = string.Format(ConfigurationManager.ConnectionStrings["Server"].ConnectionString, login, password);
            Storage.ConnectionString = connString;

            if (TryAuthenticate(login, password))
                Session_Start(login);

        }

        private void Session_Start(string login)
        {           

            string query = string.Format("INSERT INTO [Log].[ServerSession] ([sessionID], [username], [start]) VALUES ('{0}','{1}',GETDATE())",
                Session.SessionID,
                login);

            using (var conn = new SqlConnection(Global.ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            Response.Redirect("~/Person_Page/Home.aspx");
        }

        private bool TryAuthenticate(string login, string password)
        {

            using (var c = new SqlConnection(Global.ConnectionString))
            {

                #region Проверка на подключение к базеданных

                try
                {
                    c.Open();
                }
                catch (Exception)
                {
                    GetErrorMessage();
                    return false;
                }

                #endregion

                #region Аутентификация пользователя
                var command = new SqlCommand
                {
                    CommandText = "EXEC [Permission].[Authenticate] @login, @password",
                    Connection = c
                };

                command.Parameters.AddRange(new[] {
                    new SqlParameter { ParameterName = "login", Value = login },
                    new SqlParameter { ParameterName = "password", Value = password }
                });

                var adapter = new SqlDataAdapter(command);
                var t = new DataTable();

                try
                {
                    adapter.Fill(t);
                }
                catch (Exception ex)
                {
                    ErrorMessageBox.InnerText = ex.Message;
                    return false;
                }


                if (t.Rows.Count == 0)
                {
                    GetErrorMessage();
                    return false;
                }
                #endregion

                var j = t.Rows[0]["_personID"].ToString();
                var objID = t.Rows[0]["objID"].ToString();


                Session["SystemUser"] = true;
                Session["SystemUser.ID"] = string.IsNullOrEmpty(j) ? "" : j;
                Session["SystemUser.objID"] = objID;
                Session["SystemUser.Name"] = t.Rows[0]["FullName"];
                Session["SystemUser.typeID"] = t.Rows[0]["typeID"];

                if (t.Columns["TypeName"] != null)
                    Session["SystemUser.typeName"] = t.Rows[0]["TypeName"];


            }
            return true;
        }

        private void GetErrorMessage()
        {
            ErrorMessageBox.InnerText = "Предоставлена неверная пара логин-пароль.";
        }

        /// <summary>
        /// Проверяем существует ли таблица [permission].[Acce$$]
        /// </summary>
        /// <returns>true - существует, false- не существует</returns>
        private bool IsHaveAcce()
        {
            var query = string.Concat("SELECT * FROM information_schema.tables WHERE table_name = 'Acce$$' ");

            var da = new SqlDataAdapter(query, Global.StartServerConnectionString);
            var dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0)
                return false;

            return true;
        }
    }
}