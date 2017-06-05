#define Paging

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Teleform.ProjectMonitoring.HttpApplication;
using Teleform.Reporting;
using System.Data.Linq;

namespace Teleform.ProjectMonitoring.admin
{
    using System.Web.UI.WebControls;
    using System.Text;
    using System.Collections.Specialized;
    public partial class Audit : System.Web.UI.UserControl
    {
        protected override void OnLoad(EventArgs e)
        {
            Frame.UserControl_EntityListAudit_Load += EntityListAudit_Load;
            Frame.UserControl_UserListAudit_Load += UserListAudit_Load;
            Frame.UserControl_ViewButton_Click += ViewButton_Click;

            base.OnLoad(e);
        }

        protected void EntityListAudit_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Frame.EntityListAudit.DataSource = this.GetSchema().Entities.Where(o => o.IsEnumeration == false).ToList();
                Frame.EntityListAudit.DataTextField = "Name";
                Frame.EntityListAudit.DataValueField = "ID";
                Frame.EntityListAudit.DataBind();
            }
        }

        protected void UserListAudit_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                string query = string.Empty;

                Frame.UserListAudit.Items.Clear();
                Frame.UserListAudit.Items.Add(new ListItem { Text = "Не выбрано", Value = "" });

                query = string.Concat("SELECT [ObjID], [login],[typeID] FROM [_User]");

                var dt = QueryToDB(query);

                foreach (DataRow item in dt.Rows)
                {
                    Frame.UserListAudit.Items.Add(new ListItem { Value = item[0].ToString(), Text = item[1].ToString() });
                }
            }
        }

        protected void ViewAudit_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.ClientIDMode = ClientIDMode.Static;
                var rowID = e.Row.Cells[0].Text;
                e.Row.ID = rowID;

                e.Row.Attributes.Add("class", "AlternativeRow");

                var entityID = Frame.EntityListAudit.SelectedValue;

                e.Row.Attributes.Add("onclick", "switchSelectedRow(" + rowID + "," + entityID + "," + "true" + " )");
            }
        }

        private DataTable QueryToDB(string query)
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
#if Paging
        #region Paging
        public int PageIndex { get; set; }

        private void TrackPageIndex()
        {
            var controlID = Page.Request["__EVENTTARGET"];

            if (controlID != null)
            {
                var controlArguments = Page.Request["__EVENTARGUMENT"];

                if (controlArguments != null)
                {
                    var agrumentIsFind = controlArguments.IndexOf("Page");

                    if (agrumentIsFind != -1)
                    {
                        if (!string.IsNullOrWhiteSpace(controlID) && controlID.StartsWith(UniqueID))
                        {
                            var pageIndex = controlArguments.Substring(agrumentIsFind + 5);
                            PageIndex = int.Parse(pageIndex) - 1;
                        }
                    }
                }
            }

        }

        protected void ViewAudit_OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            TrackPageIndex();
            ViewAudit.PageIndex = PageIndex;
            ViewAudit.DataSource = Session["ViewAuditDataSource"];
            ViewAudit.DataBind();
        }
        #endregion
#endif

        protected void ViewButton_Click(object sender, EventArgs e)
        {
            var entitySysName = Storage.Select<Entity>(Frame.EntityListAudit.SelectedValue).SystemName;
            if (entitySysName == "__Empty" || string.IsNullOrEmpty(entitySysName))
                entitySysName = string.Empty;
            int userID;// = -1;

            if (string.IsNullOrEmpty(Frame.UserListAudit.SelectedValue))
                userID = -1;
            else
                Int32.TryParse(Frame.UserListAudit.SelectedValue, out userID);

            var dateFrom = Frame.DateFrom.Text.Replace("T", " ");

            var dateTo = Frame.DateTo.Text.Replace("T", " ");
            var query = string.Format("set dateformat ymd;EXEC report.getAudit @entity ='{0}', @userID={1}, @from='{2}', @to='{3}'", entitySysName, userID, dateFrom, dateTo);
            var dt = QueryToDB(query);

            Session["ViewAuditDataSource"] = dt;

            ViewAudit.DataSource = dt;
            ViewAudit.DataBind();
        }

    }

}