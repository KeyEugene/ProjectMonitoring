using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Teleform.ProjectMonitoring.HttpApplication;
using Teleform.Reporting;
using Teleform.Reporting.Sequring;

namespace Teleform.ProjectMonitoring.admin
{
    public partial class UserManagement : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {            
            UpdateMainTable();
            visibleUMdialogTableAndSaveButton("hidden");
        }

        protected void ButtonNew_Click(object sender, EventArgs e)
        {
            ResetItems();
            FillForCraeteNewUser();
            visibleUMdialogTableAndSaveButton("inherit");
            ObjIDTB.Text = string.Empty;
            textBoxLogin.ReadOnly = false;
        }
        protected void ButtonUpdate_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(ObjIDTB.Text))
                return;

            ResetItems();
            FillForUpdateUser();
            visibleUMdialogTableAndSaveButton("inherit");
            textBoxLogin.ReadOnly = true;

        }
        protected void ButtonDelete_Click(object sender, EventArgs e)
        {
            var objID = ObjIDTB.Text;
            if (String.IsNullOrEmpty(objID))
                return;


            
            // проверяем на то что бы мы не могли удалить себя
            if (Session["SystemUser.objID"] != null)
            {
                if (Session["SystemUser.objID"].ToString().Equals(objID))
                {
                    validText.Text = "Нельзя удалить самого себя.";
                    return;
                }
            }

            var query = string.Format("select login from _User where objID = {0}", objID);
            var login = Global.ExeccuteScalarString(query);


            query = string.Format("exec permission.KillUserProceses {0}", login);
            Global.GetDataTable(query);

            query = String.Concat("DELETE FROM [_User] WHERE [objID] = ", objID);
            QueryToDB(query);

            ph.Controls.Clear();
            UpdateMainTable();
        }

        private void FillForUpdateUser()
        {
            var query = "SELECT [_personID],[login],[pwd],[disable],[typeID] FROM [_User] WHERE [objID] = " + ObjIDTB.Text;
            var dt = QueryToDB(query);

            CreateRowPerson(dt.Rows[0]["_personID"].ToString());
            CreateRowType(dt.Rows[0]["typeID"].ToString());
            CreateRowLogin(dt.Rows[0]["login"].ToString());
            CreateRowPassword(dt.Rows[0]["pwd"].ToString());
            CreateRowDisable(dt.Rows[0]["disable"].ToString());
        }

        private void FillForCraeteNewUser()
        {
            CreateRowPerson();
            CreateRowType();
        }

        #region CreateTable New/Update

        private void CreateRowPassword(string pwd = null)
        {
            textBoxPwd.Attributes.Add("placeholder", "Введите новый пароль ?");
            textBoxPwd.Text = Encryption.Decrypt(pwd);
        }
        private void CreateRowDisable(string b = null)
        {
            checkBoxDisable.Checked = Convert.ToBoolean(b);
        }
        private void CreateRowLogin(string login)
        {
            textBoxLogin.Text = login;
        }

        private void CreateRowType(string id = null)
        {
            var query = "SELECT [objID], [name] FROM [_UserType]";
            var dt = QueryToDB(query);

            var item = new ListItem { Text = "не выбрано", Value = "" };
            item.Attributes.Add("id", "validdl");

            DDLtype.Items.Add(item);

            foreach (DataRow row in dt.Rows)
            {
                var objID = (row["objID"]).ToString();
                if (objID == id)
                    DDLtype.Items.Add(new ListItem { Text = (row["name"]).ToString(), Value = objID, Selected = true });
                else
                    DDLtype.Items.Add(new ListItem { Text = (row["name"]).ToString(), Value = objID });
            }
        }

        private void CreateRowPerson(string id = null)
        {
            var query = "SELECT [objID], [name] FROM [_Person]";
            var dt = QueryToDB(query);

            DDLperson.Items.Add(new ListItem { Text = "не выбрано", Value = "" });

            foreach (DataRow row in dt.Rows)
            {
                var objID = (row["objID"]).ToString();
                if (objID == id)
                    DDLperson.Items.Add(new ListItem { Text = (row["name"]).ToString(), Value = objID, Selected = true });
                else
                    DDLperson.Items.Add(new ListItem { Text = (row["name"]).ToString(), Value = objID });
            }
        }
        #endregion

        protected void Save_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(DDLtype.SelectedValue) || String.IsNullOrEmpty(textBoxPwd.Text) || String.IsNullOrEmpty(textBoxLogin.Text))
            {
                validText.Text = "Значение : Тип, Логин, Пароль, не  могут быть пустыми";
                return;
            }

            if (String.IsNullOrEmpty(ObjIDTB.Text))
                InsertUser();
            else
                UpdateUser();

            ph.Controls.Clear();
            UpdateMainTable();
        }

        private void UpdateUser()
        {            
            var pwd = Encryption.Encrypt(textBoxPwd.Text);
            var person = string.IsNullOrEmpty(DDLperson.SelectedValue) ? "" : String.Format("[_personID] = {0}, ", DDLperson.SelectedValue);

            var query = String.Format(
@"UPDATE [_User]
   SET {0}
      [login] = '{1}'
      ,[pwd] = '{2}'
      ,[disable] = '{3}'
      ,[typeID] = {4}
 WHERE [objID] = {5}",
                     person,
                     textBoxLogin.Text,
                     pwd,
                     checkBoxDisable.Checked,
                     DDLtype.SelectedValue,
                     ObjIDTB.Text);

            QueryToDB(query);
        }

        private void InsertUser()
        {
            var person1 = string.IsNullOrEmpty(DDLperson.SelectedValue) ? "" : "[_personID] ,";
            var person2 = string.IsNullOrEmpty(DDLperson.SelectedValue) ? "" : DDLperson.SelectedValue + ", ";
            var pwd = Encryption.Encrypt(textBoxPwd.Text);

            var query = String.Format(
@"INSERT INTO [_User] ({0} [login],[pwd],[disable],[typeID]) 
values( {5} '{1}', '{2}', '{3}', {4})",
                                      person1,
                                      textBoxLogin.Text,
                                      pwd,
                                      checkBoxDisable.Checked,
                                      DDLtype.SelectedValue,
                                      person2
                                      );
            QueryToDB(query);
        }


        #region CreateMainTable

        private void UpdateMainTable()
        {
            var query = @"SELECT [p].[name], [ut].[name] [type], [u].[login], [u].[disable], [u].[objID], [u].[pwd] FROM [_User] u
  LEFT JOIN [_Person] p ON [u].[_personID] = [p].[objID]
  JOIN [_UserType] ut ON [ut].[objID] = [u].[typeID] WHERE [u].[deleted] = 0";

            var table = FillTheTable(QueryToDB(query));
            ph.Controls.Add(table);
        }

        private Table FillTheTable(DataTable dt)
        {
            using (var table = new Table())
            {
                table.ID = "SourceTable";
                table.Attributes.Add("class", "usersTable");
                var th = CreateHeaderForTable();
                table.Rows.Add(th);

                foreach (DataRow row in dt.Rows)
                {

                    var objID = row["objID"].ToString();
                    var trow = new TableRow();
                    trow.ID = "row" + objID;
                    trow.Attributes.Add("onclick", "GetObjOnRow(this);");
                    //trow.Attributes.Add("onmouseover", "mouseOverTR(this);");

                    var name = new TableCell();
                    name.Attributes.Add("class", "userRow");
                    name.Attributes.Add("name", ID = "td" + objID);
                    name.Controls.Add(new Label { Text = row["name"].ToString() });

                    var type = new TableCell();
                    type.Attributes.Add("class", "userRow");
                    type.Attributes.Add("name", ID = "td" + objID);
                    type.Controls.Add(new Label { Text = row["type"].ToString() });

                    var login = new TableCell();
                    login.Attributes.Add("class", "userRow");
                    login.Attributes.Add("name", ID = "td" + objID);
                    login.Controls.Add(new Label { Text = row["login"].ToString() });

                    var disable = new TableCell();
                    disable.Attributes.Add("class", "userRow");
                    disable.Attributes.Add("name", ID = "td" + objID);
                    var dis = row["disable"].ToString().Equals("False") ? "Нет" : "Да";
                    disable.Controls.Add(new Label { Text = dis });

                    trow.Cells.Add(name);
                    trow.Cells.Add(type);
                    trow.Cells.Add(login);
                    trow.Cells.Add(disable);

                    table.Rows.Add(trow);
                }
                return table;
            }
        }

        private TableHeaderRow CreateHeaderForTable()
        {
            var thRow = new TableHeaderRow();

            thRow.Cells.Add(new TableHeaderCell { Text = "ФИО" });
            thRow.Cells.Add(new TableHeaderCell { Text = "Тип пользователя" });
            thRow.Cells.Add(new TableHeaderCell { Text = "Логин" });
            thRow.Cells.Add(new TableHeaderCell { Text = "Заблокирован" });

            return thRow;
        }
        #endregion

        private void visibleUMdialogTableAndSaveButton(string hiddenOrInherit)
        {
            UMdialogTable.Attributes.CssStyle.Add("visibility", hiddenOrInherit);
            Save.Attributes.CssStyle.Add("visibility", hiddenOrInherit); //hidden inherit
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

        private void ResetItems()
        {
            DDLperson.Items.Clear();
            DDLtype.Items.Clear();
            textBoxLogin.Text = String.Empty;
            textBoxPwd.Text = String.Empty;
            checkBoxDisable.Checked = false;
        }

    }
}