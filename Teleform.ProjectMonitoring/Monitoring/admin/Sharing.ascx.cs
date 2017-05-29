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
    public partial class Sharing : System.Web.UI.UserControl
    {
        protected override void OnInit(EventArgs e)
        {
            if (!IsPostBack)
            {
                DataSourceUserTypeDDL();
                DataSourceUserDDL();
                DataSourcePermissionDDL();
            }
        }

        #region Fill DropDownList's

        private void DataSourcePermissionDDL()
        {
            var list = this.GetSchema().Entities.OrderBy(o => o.Name).ToList(); //.Where(o => o.IsMain)
            EntityList.Items.Add(new ListItem { Text = "Не выбрано", Value = "" });
            for (int i = 0; i < list.Count; i++)
                EntityList.Items.Add(new ListItem { Text = list[i].Name, Value = list[i].ID.ToString() });
            EntityList.AutoPostBack = true;
            EntityList.DataBind();

        }
        private void DataSourceUserTypeDDL()
        {
            var dt = QueryToDB("SELECT [ObjID], [name] FROM [_UserType]");

            if (dt.Rows.Count == 0) return;

            UserTypeList.Items.Add(new ListItem { Text = "Не выбрано", Value = "" });
            foreach (DataRow item in dt.Rows)
            {
                UserTypeList.Items.Add(new ListItem { Value = item[0].ToString(), Text = item[1].ToString() });

            }
        }
        private void DataSourceUserDDL(string id = null)
        {
            string query = string.Empty;

            UserList.Items.Add(new ListItem { Text = "Не выбрано", Value = "" });

            if (id == null)
                return;
            else
                query = string.Concat("SELECT [ObjID], [login],[typeID] FROM [_User]  WHERE [deleted] = 0 AND typeID =   ", id);

            var dt = QueryToDB(query);

            foreach (DataRow item in dt.Rows)
            {
                UserList.Items.Add(new ListItem { Value = item[0].ToString(), Text = item[1].ToString() });
            }
        }

        #endregion

        #region Save

        protected void SaveButton_OnClick(object sender, EventArgs e)
        {
            if (EntityList.SelectedValue != "")
            {
                //if (PredicateBuilder1.template.Fields.Count == 0)
                //    return;
                if (UserList.SelectedValue != "")
                    SaveEntityFilter(UserList.SelectedValue, "[Z_EUFilter]", "[Z_EUFilterAttribute]", "[UserID]", "[Z_IUPermission]");
                else if (UserTypeList.SelectedValue != "")
                    SaveEntityFilter(UserTypeList.SelectedValue, "[Z_EUTFilter]", "[Z_EUTFilterAttribute]", "[userTypeID]", "[Z_IUTPermission]");
            }
            else if (UserList.SelectedValue != "")
            {
                SavePermission(UserList.SelectedValue, "[Z_EUPermission]", "[userID]");
            }
            else if (UserTypeList.SelectedValue != "")
            {
                SavePermission(UserTypeList.SelectedValue, "[Z_EUTPermission]", "[userTypeID]");
            }

            StorageUserObgects.ClearAllCache();

            //Storage.ClearAllCache();

            Storage.ClearBusinessContents();

            

        }



        public static string GetTransactionString(string delete, string insert)
        {
            return string.Format(@"BEGIN TRANSACTION [Tran1]
            BEGIN TRY
            {0}

            {1}

            COMMIT TRANSACTION [Tran1]

            END TRY
            BEGIN CATCH
              ROLLBACK TRANSACTION [Tran1]
            END CATCH  ", delete, insert);
        }


        private void SaveEntityFilter(string id, string nameTableFilter,
            string nameTableFilterAttr, string nameCol, string nameTableIPermission)
        {
            var delete = new StringBuilder();
            var insert = new StringBuilder();

            #region Delete and insert [Z_EUFilter], [Z_EUFilterAttribute], [Z_EUTFilter], [Z_EUTFilterAttribute]
            var d = " DELETE {0} WHERE {1} = {2} AND [entity] = '{3}' ";

            delete.Append(string.Format(d, nameTableFilterAttr, nameCol, id, PredicateBuilder1.template.Entity.SystemName));
            delete.Append(string.Format(d, nameTableFilter, nameCol, id, PredicateBuilder1.template.Entity.SystemName));

            insert.Append(string.Format("INSERT INTO {0} ({1}, [entity], [read], [create], [update], [delete], [comment]) VALUES ({2}, '{3}', '{4}', '{5}', '{6}', '{7}','{8}')",
                    nameTableFilter,
                    nameCol,
                    id,
                    PredicateBuilder1.template.Entity.SystemName,
                    PredicateBuilder1.read.Checked,
                    PredicateBuilder1.create.Checked,
                    PredicateBuilder1.update.Checked,
                    PredicateBuilder1.delete.Checked,
                    PredicateBuilder1.comment.Text));

            foreach (Teleform.Reporting.TemplateField field in PredicateBuilder1.template.Fields)
            {
                insert.Append(string.Format(" INSERT INTO {0} ({1}, [entity], [attr], [filterPredicate], [predicateInfo]) VALUES ({2}, '{3}', '{4}', '{5}', '{6}') ",
                    nameTableFilterAttr,
                    nameCol,
                    id,
                    PredicateBuilder1.template.Entity.SystemName,
                    field.Attribute.FPath,
                    field.Predicate == null ? string.Empty : field.Predicate.Replace("'", "''"),
                    field.PredicateInfo.Replace("'", "''")));
            }

            var fullQuery = GetTransactionString(delete.ToString(), insert.ToString());
            QueryToDB(fullQuery);

            #endregion

            #region
            insert.Clear();
            delete.Clear();

            var dataView = PredicateBuilder1.GetDataView();

            if (dataView.Count == 0) //|| dataView[[0]["ojbID"] == null)
                return;

            var objIDs = new StringBuilder();
            objIDs.Append(string.Concat("( ", dataView[0]["objID"].ToString()));

            for (int i = 1; i < dataView.Count; i++)
            {
                objIDs.Append(string.Concat(",", dataView[i]["objID"].ToString()));
            }
            objIDs.Append(")");

            insert.Append(string.Format(
                "INSERT INTO {0} ([instanceID],[entity],{1}, [read],[create],[update],[delete]) SELECT [objID],'{2}',{3},'{4}','{5}','{6}','{7}' FROM [{2}] WHERE [objID] IN {8}",
                nameTableIPermission,
                nameCol,
                PredicateBuilder1.template.Entity.SystemName,
                id,
                PredicateBuilder1.read.Checked,
                PredicateBuilder1.create.Checked,
                PredicateBuilder1.update.Checked,
                PredicateBuilder1.delete.Checked,
                objIDs.ToString()
                    ));
            delete.Append(string.Format(d, nameTableIPermission, nameCol, id, PredicateBuilder1.template.Entity.SystemName));

            fullQuery = GetTransactionString(delete.ToString(), insert.ToString());
            QueryToDB(fullQuery);

            #endregion

        }

        private void SavePermission(string id, string nameTable, string nameCol)
        {
            var bigQueryInsert = new StringBuilder();

            foreach (GridViewRow row in GVPermission.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    var literal = (row.Cells[0].Controls[1] as Literal);

                    foreach (var cell in row.Cells)
                    {
                        var c = cell as TableCell;
                        var cText = c.Text;
                    }


                    CheckBox cBox1 = (row.Cells[3].Controls[1] as CheckBox);
                    CheckBox cBox2 = (row.Cells[4].Controls[1] as CheckBox);
                    CheckBox cBox3 = (row.Cells[5].Controls[1] as CheckBox);
                    CheckBox cBox4 = (row.Cells[6].Controls[1] as CheckBox);

                    bigQueryInsert.Append(
                        string.Format(" INSERT INTO {0} ({1}, [entity], [read], [create], [update], [delete]) VALUES ({2}, '{3}', '{4}', '{5}', '{6}', '{7}') ",
                        nameTable,
                        nameCol,
                        id,
                        literal.Text,
                        cBox1.Checked,
                        cBox2.Checked,
                        cBox3.Checked,
                        cBox4.Checked
                        ));
                }
            }
            string delete = string.Format("DELETE {0} WHERE {1} = {2} ", nameTable, nameCol, id);

            var s = GetTransactionString(delete, bigQueryInsert.ToString());
            QueryToDB(s);
        }
        protected void SaveTemplate_Click(object sender, EventArgs e)
        {
            if (UserTypeList.SelectedValue != "" && UserList.SelectedValue != "" && EntityList.SelectedValue == "")
            {
                SaveTemplatePermission(UserList.SelectedValue, "[Z_IUPermission]", "[userID]");
                return;
            }
            else if (UserTypeList.SelectedValue != "" && UserList.SelectedValue == "" && EntityList.SelectedValue == "")
            {
                SaveTemplatePermission(UserTypeList.SelectedValue, "[Z_IUTPermission]", "[userTypeID]");
                return;
            }
        }
        private void SaveTemplatePermission(string id, string nameTable, string nameCol)
        {
            var bigQueryInsert = new StringBuilder();

            foreach (GridViewRow row in GVTemplate.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    var objID = (row.Cells[0].Controls[1] as Literal).Text;
                    CheckBox cBox1 = (row.Cells[4].Controls[1] as CheckBox);
                    CheckBox cBox2 = (row.Cells[5].Controls[1] as CheckBox);
                    CheckBox cBox3 = (row.Cells[6].Controls[1] as CheckBox);
                    CheckBox cBox4 = (row.Cells[7].Controls[1] as CheckBox);

                    bigQueryInsert.Append(
                        string.Format(" INSERT INTO {0} ({1}, [entity], [read], [create], [update], [delete],  [instanceID]) VALUES ({2}, 'R$Template', '{3}', '{4}', '{5}', '{6}', {7}) ",
                        nameTable,
                        nameCol,
                        id,
                        cBox1.Checked,
                        cBox2.Checked,
                        cBox3.Checked,
                        cBox4.Checked,
                        objID
                        ));
                }
            }
            string delete = string.Format("DELETE {0} WHERE {1} = {2} and [entity] = 'R$Template' ", nameTable, nameCol, id);

            var s = GetTransactionString(delete, bigQueryInsert.ToString());
            QueryToDB(s);
        }

        #endregion

        #region Event's and IndexCnanged -> DropDownList's

        protected void UserTypeList_IndexChanged(object sender, EventArgs e)
        {
            VisibleButtonSaveTemplate(false);

            if (UserTypeList.SelectedValue == "")
            {
                UserList.Items.Clear();
                UserList.Items.Add(new ListItem { Text = "Не выбрано", Value = "" });
                imageSecond.Visible = false;

                if (UserTypeList.SelectedValue == "" && UserList.SelectedValue == "")
                {
                    EntityList.SelectedIndex = 0;
                    MView.ActiveViewIndex = 0;
                    imageThird.Visible = imageFirst.Visible = false;
                }
                GVPermission.DataSource = null;
                GVPermission.DataBind();
                return;
            }
            if (EntityList.SelectedValue != "")
                return;

            UserList.Items.Clear();
            DataSourceUserDDL(UserTypeList.SelectedValue);

            var query = string.Format(@"select iif(t.alias is NULL,'Шаблон',t.alias) typeAlias, p.* from Permission.UserTypePermission({0},NULL)p 
                                        left join model.BTables b on b.name=p.entity 
                                        left join model.AppTypes t on t.object_id=b.appTypeID 
                                        where  p.objID is NULL
                                        order by p.entityAlias", UserTypeList.SelectedValue);

            FillGridView(query);

            MView.ActiveViewIndex = 0;
            imageFirst.Visible = true;
        }

        protected void UserList_IndexChanged(object sender, EventArgs e)
        {
            VisibleButtonSaveTemplate(false);

            if (UserList.SelectedValue == "")
            {
                imageSecond.Visible = false;
                if (UserList.SelectedValue == "" && EntityList.SelectedIndex == 0)
                {
                    imageSecond.Visible = imageThird.Visible = false;
                    UserTypeList_IndexChanged(null, EventArgs.Empty);
                }
                return;
            }
            else if (UserList.SelectedValue != "" && EntityList.SelectedValue != "")
            {
                imageSecond.Visible = true;
                return;
            }

            MView.ActiveViewIndex = 0;

            //FillGridView(string.Format("SELECT [entity], [entityAlias], [read], [create], [update],[delete] FROM [Permission].[UserPermission] ({0}, NULL)  WHERE [objID] IS NULL AND [entity] is not null order by [entityAlias]", UserList.SelectedValue));
            var query = string.Format(@"select iif(t.alias is NULL,'Шаблон',t.alias) typeAlias, p.* from Permission.UserPermission({0},NULL)p 
                                        left join model.BTables b on b.name=p.entity
                                        left join model.AppTypes t on t.object_id=b.appTypeID
                                        where  p.objID is NULL
                                        order by p.entityAlias", UserList.SelectedValue);
            FillGridView(query);

            imageSecond.Visible = true;
        }

        protected void EntityList_IndexChanged(object sender, EventArgs e)
        {
            VisibleButtonSaveTemplate(false);
            if (UserTypeList.SelectedValue == "" && UserList.SelectedValue == "")
            {
                EntityList.SelectedIndex = 0;
                imageSecond.Visible = imageThird.Visible = imageFirst.Visible = false;
                return;
            }

            if (EntityList.SelectedValue == "" && UserList.SelectedValue == "")
            {
                UserTypeList_IndexChanged(null, EventArgs.Empty);
                imageSecond.Visible = imageThird.Visible = false;
                return;
            }

            if (EntityList.SelectedValue == "")
            {
                UserList_IndexChanged(null, EventArgs.Empty);
                imageThird.Visible = false;
                return;
            }

            MView.ActiveViewIndex = 1;

            PredicateBuilder1.create.Checked = PredicateBuilder1.delete.Checked = PredicateBuilder1.read.Checked = PredicateBuilder1.update.Checked = false;
            PredicateBuilder1.comment.Text = "";

            PredicateBuilder1.EntityID = EntityList.SelectedValue;
            PredicateBuilder1.template = GetTemplate();
            PredicateBuilder1.DataBind();
            imageThird.Visible = true;
        }

        protected void LinkBtnTemplate_Click(object sender, EventArgs e)
        {
            if (UserTypeList.SelectedValue == "" && UserList.SelectedValue == "")
                return;

            VisibleButtonSaveTemplate(true);

            if (UserTypeList.SelectedValue != "" || UserList.SelectedValue != "")
                VisibleImage_LinkButton(true);

            if (EntityList.SelectedValue != "")
            {
                EntityList.SelectedIndex = 0;
                imageThird.Visible = false;
            }

            if (UserTypeList.SelectedValue != "" && UserList.SelectedValue != "")
            {
                GVTemplate.DataSource = QueryToDB(string.Format(@"SELECT * FROM [Permission].[IUTemplatePermission]({0}) order by name", UserList.SelectedValue));
                GVTemplate.DataBind();
                MView.ActiveViewIndex = 2;
                return;
            }
            else if (UserTypeList.SelectedValue != "" && UserList.SelectedValue == "")
            {
                GVTemplate.DataSource = QueryToDB(string.Format(@"SELECT * FROM [Permission].[IUTTemplatePermission]({0}) order by name", UserTypeList.SelectedValue));
                GVTemplate.DataBind();
                MView.ActiveViewIndex = 2;
                return;
            }

        }

        protected void CloseTemplate_Click(object sender, EventArgs e)
        {
            if (UserList.SelectedValue != "")
                UserList_IndexChanged(null, EventArgs.Empty);
            else if (UserTypeList.SelectedValue != "")
                UserTypeList_IndexChanged(null, EventArgs.Empty);

            VisibleImage_LinkButton(false);
        }

        private Template GetTemplate()
        {
            var entity = Storage.Select<Entity>(EntityList.SelectedValue);
            var template = new Template(string.Empty, entity, "TableBased", new byte[0]);
            string Filter = "";
            string FilterAttribute = "";

            if (UserList.SelectedValue != "")
            {
                Filter = GetQueryStringFromTableFilter(
                    UserList.SelectedValue, template.Entity.SystemName, "[UserID]", "[Z_EUFilter]");
                FilterAttribute = GetQueryStringFromTableFilterAttribute(
                    UserList.SelectedValue, template.Entity.SystemName, "[UserID]", "[Z_EUFilterAttribute]");
            }
            else if (UserTypeList.SelectedValue != "")
            {
                Filter = GetQueryStringFromTableFilter(
                    UserTypeList.SelectedValue, template.Entity.SystemName, "[userTypeID]", "[Z_EUTFilter]");
                FilterAttribute = GetQueryStringFromTableFilterAttribute(
                    UserTypeList.SelectedValue, template.Entity.SystemName, "[userTypeID]", "[Z_EUTFilterAttribute]");
            }

            var table = QueryToDB(Filter);

            if (table.Rows.Count == 1)
            {
                var row = table.Rows[0];
                PredicateBuilder1.read.Checked = row["read"] == null ? false : Convert.ToBoolean(row["read"]);
                PredicateBuilder1.create.Checked = row["create"] == null ? false : Convert.ToBoolean(row["create"]);
                PredicateBuilder1.update.Checked = row["update"] == null ? false : Convert.ToBoolean(row["update"]);
                PredicateBuilder1.delete.Checked = row["delete"] == null ? false : Convert.ToBoolean(row["delete"]);
                PredicateBuilder1.comment.Text = row["comment"] == null ? null : row["comment"].ToString();
            }

            table = QueryToDB(FilterAttribute);

            if (table.Rows.Count != 0)
                foreach (DataRow row in table.Rows)
                {
                    var attr = row["attr"] == null ? null : row["attr"].ToString(); // throw new Exception("Неверные данные")
                    //var attribute = template.Entity.Attributes.FirstOrDefault(x => x.ID.Equals(attr));
                    var attribute = template.Entity.Attributes.FirstOrDefault(x => x.FPath.Equals(attr));
                    var field = new Teleform.Reporting.TemplateField(attribute);
                    // var filterExpression = row["filterExpression"] == null ? null : row["filterExpression"].ToString();
                    var filterPredicate = row["filterPredicate"] == null ? null : row["filterPredicate"].ToString();
                    var predicateInfo = row["predicateInfo"] == null ? null : row["predicateInfo"].ToString();
                    field.Predicate = filterPredicate;
                    field.PredicateInfo = predicateInfo;

                    template.Fields.Add(field);
                }
            else
                return null;

            return template;
        }

        private string GetQueryStringFromTableFilterAttribute(string id, string entity, string user, string nameCol)
        {
            return string.Format(
@"SELECT [attr] ,[filterExpression] ,[filterPredicate] ,[predicateInfo] FROM {0} WHERE {1} = {2} AND [entity] = '{3}'",
nameCol,
user,
id,
entity
);
        }

        private string GetQueryStringFromTableFilter(string id, string entity, string user, string nameCol)
        {
            return string.Format(
@"SELECT [read] ,[create] ,[update] ,[delete] ,[comment] FROM {2} WHERE {0} = {1} AND [entity] = '{3}'",
user,
id,
nameCol,
entity
);
        }

        #endregion

        #region Delete object/object's
        protected void BtnResetOneObject_Click(object sender, EventArgs e)
        {

            if (UserTypeList.SelectedValue == "" && UserList.SelectedValue == "")
            {
                //
                var query = @"DELETE FROM [Z_IUTPermission] WHERE [entity] != 'R$Template'
                              DELETE FROM [Z_EUTFilter] DELETE FROM [Z_EUTFilterAttribute]";
                QueryToDB(query);
                return;
            }
            else if (EntityList.SelectedValue == "" && UserList.SelectedValue == "")
            {
                var query = String.Format(@"DELETE FROM [Z_EUTFilterAttribute] WHERE [userTypeID] = {0}
                                            DELETE FROM [Z_EUTFilter] WHERE [userTypeID] = {0}  
                                            DELETE FROM [Z_IUTPermission] WHERE [userTypeID] = {0} AND [entity] != 'R$Template' ", UserTypeList.SelectedValue);
                QueryToDB(query);
                return;
            }
            else if (UserList.SelectedValue != "" && EntityList.SelectedValue == "")
            {
                var query = String.Format(@"DELETE FROM [Z_EUFilterAttribute] WHERE [userID] = {0} 
                                            DELETE FROM [Z_EUFilter] WHERE [userID] = {0}  
                                            DELETE FROM [Z_IUPermission] WHERE [userID] = {0} AND [entity] != 'R$Template' ",
                UserList.SelectedValue);
                QueryToDB(query);
                return;
            }
            else if (UserList.SelectedValue != "" && EntityList.SelectedValue != "")
            {
                var query = String.Format(@"DELETE FROM [Z_EUFilterAttribute] WHERE [userID] = {0} AND [entity] = '{1}' 
                                            DELETE FROM [Z_EUFilter] WHERE [userID] = {0} AND [entity] = '{1}'  
                                            DELETE FROM [Z_IUPermission] WHERE [userID] = {0} AND [entity] = '{1}'",
                                    UserList.SelectedValue,
                                    PredicateBuilder1.template.Entity.SystemName);
                QueryToDB(query);
                EntityList_IndexChanged(null, EventArgs.Empty);
                return;
            }
            else if (UserList.SelectedValue == "" && EntityList.SelectedValue != "")
            {
                var query = String.Format(@"DELETE FROM [Z_EUTFilterAttribute] WHERE [userTypeID] = {0} AND [entity] = '{1}' 
                                            DELETE FROM [Z_EUTFilter] WHERE [userTypeID] = {0} AND [entity] = '{1}'  
                                            DELETE FROM [Z_IUTPermission] WHERE [userTypeID] = {0} AND [entity] = '{1}'",
                                    UserTypeList.SelectedValue,
                                    PredicateBuilder1.template.Entity.SystemName);
                QueryToDB(query);
                EntityList_IndexChanged(null, EventArgs.Empty);
                return;
            }
        }

        protected void BtnResetALotOfObjects_Click(object sender, EventArgs e)
        {
            if (UserTypeList.SelectedValue == "" && UserList.SelectedValue == "")
            {
                var query = "DELETE FROM [Z_EUTPermission]";
                QueryToDB(query);
                return;
            }
            else if (EntityList.SelectedValue == "" && UserList.SelectedValue == "")
            {
                var query = String.Format("DELETE FROM [Z_EUTPermission] WHERE [userTypeID] = {0} ", UserTypeList.SelectedValue);
                QueryToDB(query);
                UserTypeList_IndexChanged(null, EventArgs.Empty);
                return;
            }
            else if (UserList.SelectedValue != "" && EntityList.SelectedValue == "")
            {
                var query = String.Format("DELETE FROM [Z_EUPermission] WHERE [userID] = {0}", UserList.SelectedValue);
                QueryToDB(query);
                DataSourceUserDDL(UserList.SelectedValue);
                UserList_IndexChanged(null, EventArgs.Empty);
                return;

            }
            else if (UserList.SelectedValue != "" && EntityList.SelectedValue != "")
            {
                var query = String.Format("DELETE FROM [Z_EUPermission] WHERE [userID] = {0} AND [entity] = '{1}'",
                    UserList.SelectedValue,
                    PredicateBuilder1.template.Entity.SystemName);
                DataSourcePermissionDDL();
                QueryToDB(query);
                EntityList_IndexChanged(null, EventArgs.Empty);
                return;
            }
            else if (UserList.SelectedValue == "" && EntityList.SelectedValue != "")
            {
                var query = String.Format("DELETE FROM [Z_EUTPermission] WHERE [userTypeID] = {0} AND [entity] = '{1}'",
                    UserTypeList.SelectedValue,
                    PredicateBuilder1.template.Entity.SystemName);
                QueryToDB(query);
                EntityList_IndexChanged(null, EventArgs.Empty);
                return;
            }
        }

        protected void BtnResetTemplates_Click(object sender, EventArgs e)
        {
            if (UserTypeList.SelectedValue == "" && UserList.SelectedValue == "")
            {
                var query = @"DELETE FROM [Z_IUPermission] WHERE [entity] = 'R$Template'
                              DELETE FROM [Z_IUTPermission] WHERE [entity] = 'R$Template'";
                QueryToDB(query);
                return;
                //
                //TODO: ADD SOMETHING:
                //SOME EVENT                    READY
                //OR RETURN
                //
            }
            else if (UserTypeList.SelectedValue != "" && UserList.SelectedValue == "")
            {
                var query = string.Format(@"DELETE FROM [Z_IUTPermission] WHERE [entity] = 'R$Template' AND userTypeID = {0}", UserTypeList.SelectedValue);
                QueryToDB(query);
                
                if (SaveButton.Visible)
                    UserTypeList_IndexChanged(null, EventArgs.Empty);
                else
                    LinkBtnTemplate_Click(null, EventArgs.Empty);

                return;
                //
                //TODO: ADD SOMETHING:
                //SOME EVENT                    READY
                //OR RETURN
                //
            }
            else if (UserList.SelectedValue != "")
            {
                var query = string.Format(@"DELETE FROM [Z_IUPermission] WHERE [entity] = 'R$Template' AND userID = {0}", UserList.SelectedValue);
                QueryToDB(query);

                if (SaveButton.Visible)
                    UserList_IndexChanged(null, EventArgs.Empty);
                else
                    LinkBtnTemplate_Click(null, EventArgs.Empty);

                return;
                //
                //TODO: ADD SOMETHING:
                //SOME EVENT                    READY
                //OR RETURN
                //
            }
        }

        #endregion


        private void FillGridView(string query)
        {
            GVPermission.DataSource = QueryToDB(query);
            GVPermission.DataBind();
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


        #region Visible elements
        private void VisibleButtonSaveTemplate(bool p)
        {
            SaveTemplate.Visible = p;
            SaveButton.Visible = !p;

            VisibleImage_LinkButton(p);
        }

        private void VisibleImage_LinkButton(bool visible)
        {
            Image ImageCloseTemplate;

            if (CloseTemplate_LinkButton.Controls[0] is Image)
            {
                ImageCloseTemplate = CloseTemplate_LinkButton.Controls[0] as Image;
                ImageCloseTemplate.Visible = visible;
            }
        }
        #endregion

    }

}