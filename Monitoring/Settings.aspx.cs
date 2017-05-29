using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using Teleform.ProjectMonitoring.HttpApplication;
using Teleform.Reporting;
using Teleform.Reporting.Sequring;

namespace Teleform.ProjectMonitoring
{
    using CheckBoxBase = System.Web.UI.WebControls.CheckBox;
    using System.Text;
    public partial class Settings : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["checkBoxMainNavigation"] = checkBoxMainNavigation.Checked;
            Session["checkBoxObjectsNavigation"] = checkBoxObjectsNavigation.Checked; 

            var userID = Convert.ToInt32(Session["SystemUser.objID"]);
            var ReadUserPermission = StorageUserObgects.Select<UserEntityPermission>(userID, userID).getReadPermittedEntities().AsEnumerable().Select(x => x["entityID"].ToString()).ToList<string>();
            var Ent = Global.GetDataTable(@"SELECT [isLogicMain], b.alias tblAlias, b.object_id tblID 
                    FROM model.BTables b join model.AppTypes at on at.name='Base' and b.appTypeID=at.object_ID");
            listView.DataSource = Ent.AsEnumerable().Where(x => ReadUserPermission.Contains(x["tblID"].ToString())).CopyToDataTable();
            listView.DataBind();

            //var userID = Convert.ToInt32(Session["SystemUser.objID"]);

            //var PermittedEntities = StorageUserObgects.Select<UserEntityPermission>(userID, userID).getReadPermittedEntities().AsEnumerable().Select(x => x["entityID"].ToString()).ToList<string>();

            //var query = "SELECT [isLogicMain], b.alias tblAlias, b.object_id tblID FROM model.BTables b join model.AppTypes at on at.name='Base' and b.appTypeID=at.object_ID";

            //var dt = Global.GetDataTable(query);

            //var dataSource = dt.AsEnumerable().Where(x => PermittedEntities.Contains(x["tblID"].ToString())).CopyToDataTable();

            //listView.DataSource = dataSource;
            //listView.DataBind();

            

        }

        
        #region Переключатели между View
        protected void MainEntityButton_Click(object sender, EventArgs e)
        {
            SettingView.SetActiveView(MainEntityView);
        }

        protected void ChangePasswordButton_Click(object sender, EventArgs e)
        {
            SettingView.SetActiveView(ChangePasswordView);
        }

        protected void OnOffNavigationButton_Click(object sender, EventArgs e)
        {
            SettingView.SetActiveView(NavigationSettingsView);
        }
        #endregion

        #region MainEntityView

        protected DataTable GetEntityTemplates(object tblID)
        {
            using (var con = new SqlConnection(Kernel.ConnectionString))
            using (var ad = new SqlDataAdapter(
@"SELECT t.[objID],t.[name], (case when b.templateID is null then 0 else 1 end) [isTemplateDefault] FROM [model].[R$Template] t 
left join model.BTables b on b.templateID = t.objID WHERE [entityID] = @entityID", con))
            {
                ad.SelectCommand.Parameters.Add("entityID", SqlDbType.Variant).Value = tblID;
                var dt = new DataTable();
                ad.Fill(dt);
                return dt;
            }
        }
        #endregion

        #region ChangePasswordView
        protected void ChangeUserPasswordButton_Click(object sender, EventArgs e)
        {
            if (!PasswordBox.Text.Equals(DublicatePasswordBox.Text))
                throw new Exception("Пароли не совпадают");

            string objID = string.Empty;

            if (Session["SystemUser.objID"] == null)
                return;
            else
                objID = Session["SystemUser.objID"].ToString();

            var query = string.Concat("UPDATE  [_User] SET [pwd] = '", Encryption.Encrypt(PasswordBox.Text), "' WHERE [objID] = ", objID);

            Global.GetDataTable(query);
            PasswordChangeMessage.Show();
        }
        #endregion

        #region CheckBox Navigation



        #endregion

    }
}