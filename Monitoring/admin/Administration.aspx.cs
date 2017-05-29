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
        private bool _Import
        {
            get
            {
                return ViewState["ImportFlag"] == null ? false : (bool)ViewState["ImportFlag"];
            }
            set
            {
                ViewState["ImportFlag"] = value;
            }
        }

        protected void Synchronize(object sender, EventArgs e)
        {
            Teleform.ProjectMonitoring.HttpApplication.Global.UpdateSchema();
        }

        private bool _IsImportWork
        {
            get
            {
                return ViewState["IsImportWork"] == null ? false : (bool)ViewState["IsImportWork"];
            }
            set
            {
                ViewState["IsImportWork"] = value;
            }
        }

        private string _Path = string.Empty;

        SqlConnection importConnection;
        SqlCommand importCommand;

        protected void Page_Load(object sender, EventArgs e)
        {
            var entity = Global.Schema.Entities.FirstOrDefault(o => o.IsEnumeration);
            string entityString = "";

            if (entity != null)
                entityString = entity.ID.ToString();

            EnumerationManagement.PostBackUrl =
                string.Format
                (
                    "~/EntityListAttributeView.aspx?entity={0}&checker=isClassifier",
                    entityString
                );

            if (!IsPostBack)
            {
                _IsImportWork = false;
                AdministrationOptionsMulti.ActiveViewIndex = 0;
                EventManagementButton.CssClass = "optionButtonActive";

                var userTypeID = Session["SystemUser.typeID"].ToString();

                //Если пользователь случайно забрел на страницу администрирования - выкинуть его на страницу 404 и вернуть в систему
                if (string.IsNullOrEmpty(userTypeID) || userTypeID != "1")
                {
                    Server.Transfer("~/ErrorPage2.aspx");
                }
            }

            //Что бы из-за ViewState не терялась таблица TableObjects
            if (Session["isTypesOfObjectsView"] != null)
                DataBindCheckBoxForTypesOfObjects(null, EventArgs.Empty);
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


        IAsyncResult BeginAsync(object sender, EventArgs e, AsyncCallback callback, object state)
        {
            if (_IsImportWork)
            {
                ImportBusyMessageBox.Show();
                return new ImportCompletedSyncResult(new Exception(), callback, state);
            }

            importConnection = new SqlConnection(Global.ConnectionString);
            importCommand = new SqlCommand("EXEC [Import].[ImportData] @personID, @fileName, @add", importConnection);
            importCommand.CommandTimeout = 1200;
            importCommand.Parameters.AddRange(
                new SqlParameter[] {
                    new SqlParameter
                    {
                        ParameterName = "personID",
                        DbType = DbType.Int32,
                        Value = this.GetSystemUser()
                    },
                    new SqlParameter 
                    {
                        ParameterName = "fileName",
                        DbType = DbType.String,
                        Value = _Path
                    },
                    new SqlParameter
                    {
                        ParameterName = "add",
                        DbType = DbType.String,
                        Value = int.Parse(ImportModeList.SelectedValue)
                    }
                });

            try
            {
                importConnection.Open();
            }
            catch (Exception err)
            {
                return new ImportCompletedSyncResult(err, callback, state);
            }


            _IsImportWork = true;
            return importCommand.BeginExecuteNonQuery(callback, state);
        }


        void EndAsync(IAsyncResult ar)
        {
            if (ar is ImportCompletedSyncResult)
                return;

            try
            {
                _Import = false;
                _IsImportWork = false;
                importCommand.EndExecuteNonQuery(ar);
            }
            catch
            {
                if (importConnection != null) importConnection.Close();
                UnSuccessfulImportMessageBox.Show();
            }
        }


        protected void AdminManagementButton_Click(object sender, EventArgs e)
        {
            foreach (var item in OptionsDiv.Controls.OfType<Button>())
                item.CssClass = "optionButton";
            AdministrationOptionsMulti.ActiveViewIndex = Convert.ToInt32((sender as Button).CommandArgument);
            (sender as Button).CssClass = "optionButtonActive";

            if (AdministrationOptionsMulti.ActiveViewIndex != 2) //Не делаем Bind для таблицу TableObjects, когда мы находимся на других View
                Session["isTypesOfObjectsView"] = null;
        }
        #region EntityPart
        protected void EntityAddButton_Click(object sender, EventArgs e)
        {
            //((FormView)EventDialog.FindControl("AliasForm")).ChangeMode(FormViewMode.Insert);
            //EventDialog.Caption = "Создание Атрибута";
            //EventDialog.Show(EditorMode.Insert);
        }
        #endregion EntityPart


        #region EventPart


        protected void EventAddButton_Click(object sender, EventArgs e)
        {
            ((FormView)EventDialog.FindControl("EventForm")).ChangeMode(FormViewMode.Insert);
            EventDialog.Caption = "Создание события";
            EventDialog.Show(EditorMode.Insert);
        }

        protected void EventEditButton_Click(object sender, EventArgs e)
        {
            if (EventGridView.Rows.Count == 0)
                return;

            if (EventGridView.SelectedIndex != -1)
            {
                ((FormView)EventDialog.FindControl("EventForm")).ChangeMode(FormViewMode.Edit);
                EventDialog.Caption = "Редактирование события";
                EventDialog.Show(EditorMode.Edit);
            }
        }

        protected void EventDeleteButton_Click(object sender, EventArgs e)
        {
            if (EventGridView.Rows.Count == 0)
                return;

            if (EventGridView.SelectedIndex != -1)
                DeleteWarningDialog.Show();
        }

        protected void EventSaveHandler(object sender, EventArgs e)
        {
            var formView = EventDialog.FindControl("EventForm") as FormView;
            if (formView.CurrentMode == FormViewMode.Edit)
                EventForm.UpdateItem(false);
            else if (formView.CurrentMode == FormViewMode.Insert)
                EventForm.InsertItem(false);
            EventDialog.Close();
        }

        protected void iEventTableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var f = (FormView)EventDialog.FindControl("EventForm");
            var ddl = (DropDownList)f.FindControl("iEventColumnList");
            for (int i = 1; i < ddl.Items.Count; i++)
                ddl.Items.RemoveAt(i);
        }
        #endregion EventPart


        protected void DeleteWarningDialog_Close(object sender, EventArgs e)
        {
            if ((e as MessageBoxEventArgs).Result.ToString() == "Yes")
            {
                using (var conn = new SqlConnection(Global.ConnectionString))
                using (var cmd = new SqlCommand())
                {
                    conn.Open();
                    cmd.Connection = conn;
                    if (AdministrationOptionsMulti.GetActiveView() == EventView)
                    {
                        cmd.CommandText = "EXEC [model].[EventDelete] @eventID";
                        cmd.Parameters.Add("eventID", System.Data.SqlDbType.BigInt).Value = EventGridView.SelectedDataKey["objID"];
                    }
                    cmd.ExecuteNonQuery();
                }

            }
        }

        #region ImportData

        protected void ImportButton_Click(object sender, EventArgs e)
        {
            if (!ImportUpload.HasFile)
            {
                NoFileMessageBox.Show();
                return;
            }

            var savePath = Server.MapPath(@"~\Import\");
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            var newPath = _Path = Path.Combine(savePath, Path.GetFileName(ImportUpload.FileName));
            ImportUpload.SaveAs(newPath);

            _Import = true;
            AddOnPreRenderCompleteAsync(new BeginEventHandler(BeginAsync), new EndEventHandler(EndAsync));

#if truef
            try
            {
                using (var conn = new SqlConnection(Global.ConnectionString))
                using (var cmd = new SqlCommand("EXEC [Import].[ImportData] @personID, @fileName, @add", conn))
                {
                    cmd.CommandTimeout = 600;
                    cmd.Parameters.Add(
                        new SqlParameter
                        {
                            ParameterName = "personID",
                            DbType = System.Data.DbType.Int32,
                            Value = this.GetSystemUser()
                        });

                    cmd.Parameters.Add(
                        new SqlParameter
                        {
                            ParameterName = "fileName",
                            DbType = System.Data.DbType.String,
                            Value = newPath
                        });

                    cmd.Parameters.Add(
                        new SqlParameter
                        {
                            ParameterName = "add",
                            DbType = System.Data.DbType.String,
                            Value = int.Parse(ImportModeList.SelectedValue)
                        });

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    SuccessfulImportMessageBox.Show();
                }
            }
            catch
            {
                UnSuccessfulImportMessageBox.Show();
                return;
            }
            
#endif

        }



        protected void HistoryTimer_Tick(object sender, EventArgs e)
        {
            // ImportGridView.DataBind();
            // ShowStatus();
        }

        private void ShowStatus()
        {
            /*var statusCell = ImportGridView.Rows[0].Cells[3] as TableCell;
            if (statusCell.Text == "3")
            {
                var image = new Image();
                image.ImageUrl = Server.MapPath(@"~/images/loader.gif");
                statusCell.Controls.Add(image);
            }
            else
            {
                for (int i = 0; i < statusCell.Controls.Count; i++ )
                {
                    if (statusCell.Controls[i] is Image)
                        statusCell.Controls.Remove(statusCell.Controls[i]);
                }
            }*/
        }
        #endregion ImportData

    }
}