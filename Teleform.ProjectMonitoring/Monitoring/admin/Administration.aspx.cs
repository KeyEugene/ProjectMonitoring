#define Alex

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Phoenix.Web.UI.Dialogs;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Monitoring
{
    using Teleform.ProjectMonitoring;
    using System.IO;
    using System.ComponentModel;
    using System.Text.RegularExpressions;
    using Teleform.ProjectMonitoring.HttpApplication;

    public partial class Administration : BasePage
    {

        protected void Synchronize(object sender, EventArgs e)
        {
            Teleform.ProjectMonitoring.HttpApplication.Global.UpdateSchema();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Frame.UserControl_AdminManagementButton_Click += AdminManagementButton_Click;
            Frame.UserControl_EnumerationManagement_Click += EnumerationManagement_Click;
            
            if (!IsPostBack)
            {
                //_IsImportWork = false;
                AdministrationOptionsMulti.ActiveViewIndex = 0;
                Frame.EventManagementButton.CssClass = "button_active";

                var userTypeID = Session["SystemUser.typeID"].ToString();

                //Если пользователь случайно забрел на страницу администрирования - выкинуть его на страницу 404 и вернуть в систему
                if (string.IsNullOrEmpty(userTypeID) || userTypeID != "1")
                {
                    Server.Transfer("~/ErrorPage2.aspx");
                }
            }

            //Что бы из-за ViewState не терялась таблица TableObjects
            if (Session["isTypesOfObjectsView"] != null)
                SettingTheTypesOfObjects.DataBindCheckBoxForTypesOfObjects(null, EventArgs.Empty);
        }


        [Obsolete("Не используется")]
        public bool getCheckedList(string colName)
        {
            List<DataRow> list;
            using (SqlConnection conn = new SqlConnection(Global.ConnectionString))
            {
                var cmd = new SqlCommand("select [read] from Permission.UserTypePermission(1,null)", conn);

                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);
                list = dt.AsEnumerable().ToList();
            }

            foreach (var item in list)
            {
                var check1 = item[0];
            }
            return true;

        }
        [Obsolete("Не используется")]
        private bool getChecked(List<DataRow> list)
        {
            return true;
        }


        protected void AdminManagementButton_Click(object sender, EventArgs e)
        {
            Frame.EventManagementButton.CssClass = "";
            Frame.ImportManagementBunnon.CssClass = "";
            Frame.SettingTheTypesOfObjects.CssClass = "";
            Frame.AliasManagementButton.CssClass = "";
            Frame.EnumerationManagement.CssClass = "";
            Frame.UserManagementButton.CssClass = "";
            Frame.SeparationOfAccessRights.CssClass = "";
            Frame.AuditButton.CssClass = "";

            if (sender is LinkButton)
            {
                LinkButton button = sender as LinkButton;
                AdministrationOptionsMulti.ActiveViewIndex = Convert.ToInt32(button.CommandArgument);
                button.CssClass = "button_active";
            }

            if (AdministrationOptionsMulti.ActiveViewIndex != 2) //Не делаем Bind для таблицу TableObjects, когда мы находимся на других View
                Session["isTypesOfObjectsView"] = null;
        }

        protected void EnumerationManagement_Click(object sender, EventArgs e)
        {
            var entity = Global.Schema.Entities.FirstOrDefault(o => o.IsEnumeration);
            string entityString = "";

            if (entity != null)
                entityString = entity.ID.ToString();

            Response.Redirect(string.Format
                (
                    "~/EntityListAttributeView.aspx?entity={0}&checker=isClassifier",
                    entityString
                ));

        }
    }
}