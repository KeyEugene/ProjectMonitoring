using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Teleform.Reporting;



namespace Teleform.ProjectMonitoring
{
    public partial class ErrorPage2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var error = Server.GetLastError();
            if (error != null)
            {
                if (error.InnerException != null)
                {
                    MessageLabel.Text = error.InnerException.Message;
                    ErrorTextBox.Text = string.Concat(error.InnerException.Message, "\n", error.InnerException.StackTrace);

                }
            }
        }


        protected void LogoutButton_Click(object sender, EventArgs e)
        {

            var SystemUser = Session["SystemUser"];

            if (SystemUser != null)
            {
                string query = string.Format("UPDATE [Log].[ServerSession] SET [finish] = GETDATE() WHERE [sessionID] = '{0}' AND [finish]  is null ", SystemUser);
                Storage.GetDataTable(query); 
            }
                        
            
            StorageUserObgects.ClearAllCache();

            Session.Abandon();
            Session.Clear();

            HttpContext.Current.Cache.Remove(Session.SessionID);

            Response.Redirect("~/Login.aspx");

        }

        protected void MainPage_OnClick(object sender, EventArgs e)
        {

            Response.Redirect(string.Concat(Path(), "environment.aspx"));
        }

        protected void Monitoring_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(string.Concat(Path(), "EntityListAttributeView.aspx"));
        }

        private string Path()
        {
            foreach (var item in Request.Url.Segments)
                if (item.StartsWith("monitoring")) return string.Concat(Request.Url.GetLeftPart(UriPartial.Authority), "/", item);

            return null;

        }
    }
}