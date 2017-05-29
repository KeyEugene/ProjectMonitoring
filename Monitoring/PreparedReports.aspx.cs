using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Collections;

namespace Teleform.ProjectMonitoring
{
    using Phoenix.Web.UI.Dialogs;
    using System.Xml.Linq;
    using System.Text;
    using System.Data.SqlClient;
    using System.Data;

    public partial class PreparedReports : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void ReportLinkButton_Click(object sender, EventArgs e)
        {
            var linkButton = sender as LinkButton;

            var filePath = linkButton.CommandArgument;

            var extension = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(extension)) return;

            var row = linkButton.Parent.Parent as GridViewRow;
            var rowIndex = row.RowIndex;

            var fileName = PreparedReportsView.DataKeys[rowIndex]["reportName"].ToString();

            filePath = Server.MapPath(string.Format("{0}", filePath));

            var response = Page.Response;
            response.Clear();
            response.AddHeader("Content-Type", "application/octet-stream");
            response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}{1}", fileName, extension));
            response.WriteFile(filePath);
            response.End();
        }

        protected void DeleteButon_Click(object sender, EventArgs e)
        {
            if (PreparedReportsView.SelectedDataKey != null)
                DeleteMessageBox.Show();
        }

        protected void DeleteMessageBox_Close(object sender, EventArgs e)
        {
            if ((e as MessageBoxEventArgs).Result.ToString() == "Yes")
            {
                PreparedReportsDataSource.Delete();
                var link = PreparedReportsView.SelectedDataKey["link"].ToString();
                link = Server.MapPath(link);
                File.Delete(link);
                PreparedReportsView.DataBind();
            }
        }

        protected string GetLoadUrl(object created, object userID)
        {
            return string.Format("{0}#{1}.loadReport", created.ToString().Replace(':','@'), userID.ToString());
        }
#region фильтрация
#if f
        protected void FilterBox_TextChanged(object sender, EventArgs e)
        {
            PreparedReportsDataSource.DataBind();
        }

        protected void List_SelectedIndexChanged(object sender, EventArgs e)
        {
            PreparedReportsDataSource.DataBind();
        }

        private string FormCondition()
        {
            string condition = string.Empty;
            
            if (!string.IsNullOrEmpty(PersonBox.Text.Trim()))
                condition += string.Format("[P].[name] LIKE '%{0}%' AND ", PersonBox.Text.Trim());

            if (!string.IsNullOrEmpty(ReportNameBox.Text.Trim()))
                condition += string.Format("[RR].[name] LIKE '%{0}%' AND ", ReportNameBox.Text.Trim());

            if (EntityList.SelectedIndex != 0)
                condition += string.Format("[RT].[entityID] = {0} AND ", EntityList.SelectedValue.Trim());

            if (TypeList.SelectedIndex != 0)
                condition += string.Format("CASE WHEN [RR].[fileTypeID] IS NULL THEN [MTS].[objID] ELSE [MTF].[objID] END  = {0} AND ", TypeList.SelectedValue.Trim());

            if (!string.IsNullOrEmpty(condition))
            {
                condition = condition.Remove(condition.Length - 4, 4);
                condition = condition.Insert(0, "WHERE ");
            }
            else
                condition = " ";


            return condition;
        }

        protected void PreparedReportsDataSource_DataBinding(object sender, EventArgs e)
        {
#if f
         PreparedReportsDataSource.SelectParameters.Clear();
            PreparedReportsDataSource.SelectCommand = string.Format("EXEC [Report].[GetPreparedReports] @condition = '{0}'", FormCondition());        
#else

            PreparedReportsDataSource.SelectParameters.Clear();
            PreparedReportsDataSource.SelectCommand = "EXEC [Report].[GetPreparedReports] @condition";
            PreparedReportsDataSource.SelectParameters.Add(new Parameter("condition",DbType.String, FormCondition()));
#endif
        }
#endif
#endregion
    }

}