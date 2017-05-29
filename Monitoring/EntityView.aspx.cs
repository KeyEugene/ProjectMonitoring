using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Text;

using Teleform.ProjectMonitoring;
using System.IO;

namespace Teleform.ProjectMonitoring
{
    using Phoenix.Web.UI.Dialogs;

    public partial class EntityView : BasePage
    {

        private string SelectedEntityID
        {
            get { return Session["SelectedEntityID"] == null ? null : (string)Session["SelectedEntityID"]; }
            set { Session["SelectedEntityID"] = value; }
        }

        private int SelectedEntityIndex
        {
            get { return Session["SelectedEntityIndex"] == null ? -1 : (int)Session["SelectedEntityIndex"]; }
            set { Session["SelectedEntityIndex"] = value; }
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["ReportCondition"] = "null";
                EntityList.DataTextField = "Name";
                EntityList.DataValueField = "ID";
                EntityList.DataSource = this.GetSchema().Entities.OrderBy(o => o.Name);
                EntityList.DataBind();

                if (SelectedEntityIndex != -1)
                {
                    EntityList.SelectedIndex = SelectedEntityIndex;
                }
                else
                {
                    EntityList_SelectedIndexChanged(EntityList, EventArgs.Empty);
                }
            }
        }

        protected void EntityList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedEntityID = (sender as DropDownList).SelectedValue;
            SelectedEntityIndex = (sender as DropDownList).SelectedIndex;
            Response.Redirect(string.Format("~/EntityView.aspx?entity={0}", SelectedEntityID));
        }
    }
}