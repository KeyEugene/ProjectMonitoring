
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Monitoring
{
    using Teleform.ProjectMonitoring;
    using Teleform.ProjectMonitoring.HttpApplication;
    using Teleform.Reporting;
    using Teleform.ProjectMonitoring.Dynamics;

    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        public string WhoCameIn
        {
            get
            {
                string name = "";
                if (Session["SystemUser.Name"] == null)
                {
                    name = (Session["SystemUser.objID"] ?? "").ToString();
                }
                else
                {
                    name = (Session["SystemUser.Name"] ?? "").ToString();
                }

                return string.Format("АРМ {0} : {1} ", (Session["SystemUser.typeName"] ?? "").ToString(), name);
            }
        }

        protected void LogoutButton_Click(object sender, EventArgs e)
        {
            var SystemUser = Session["SystemUser"];

            string query = string.Format("UPDATE [Log].[ServerSession] SET [finish] = GETDATE() WHERE [sessionID] = '{0}' AND [finish]  is null ", SystemUser);
            Global.GetDataTable(query);


            StorageUserObgects.ClearAllCache();

            Session.Abandon();
            Session.Clear();

            HttpContext.Current.Cache.Remove(Session.SessionID);

            Response.Redirect("~/Login.aspx");

        }

        protected void GoToEvents(object sender, EventArgs e)
        {
            RecentEventsDialog_Closed(sender, e);
            RecentEventsDialog.Visible = false;
            Response.Redirect("~/Events.aspx");
        }

        protected void RecentEventsDialog_Closed(object sender, EventArgs e)
        {
            Session["RecentEventsShowed"] = true;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            PersonInfo.Text = WhoCameIn;
        }

        public void button_Click(object sender, EventArgs e)
        {
            var sessionKey = string.Concat("entity=", Request["entity"]);


            Session[sessionKey] = null;

            var b = sender as LinkButton;

            Page.ClientScript.RegisterStartupScript(GetType(), "blablabla",
            Page.ClientScript.GetPostBackEventReference(
                        new PostBackOptions(this, null, b.CommandArgument, false, true, false, true, false, null)),
                    true);
        }

        protected void RedirectPathItemHandler(object sender, EventArgs e)
        {
            if (sender is IButtonControl)
            {
                var button = sender as IButtonControl;

                Response.Redirect(button.CommandArgument);
            }
        }

    }
}
