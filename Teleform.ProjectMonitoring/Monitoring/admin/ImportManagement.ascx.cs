using Monitoring;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Teleform.ProjectMonitoring.HttpApplication;

namespace Teleform.ProjectMonitoring.admin
{
    public partial class ImportManagement : System.Web.UI.UserControl
    {
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

        SqlConnection importConnection;
        SqlCommand importCommand;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _IsImportWork = false;
            }

            }
        #region ImportData

        protected void ImportButton_Click(object sender, EventArgs e)
        {
            if (!Frame.ImportUpload.HasFile)
            {
                NoFileMessageBox.Show();
                return;
            }

            var savePath = Server.MapPath(@"~\Import\");
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            var newPath = _Path = Path.Combine(savePath, Path.GetFileName(Frame.ImportUpload.FileName));
            Frame.ImportUpload.SaveAs(newPath);

            _Import = true;
            this.Page.AddOnPreRenderCompleteAsync(new BeginEventHandler(BeginAsync), new EndEventHandler(EndAsync));

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
                        Value = Session["SystemUser.ID"] ?? 0
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
                        Value = int.Parse(Frame.ImportModeList.SelectedValue)
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

        #endregion ImportData
    }
}