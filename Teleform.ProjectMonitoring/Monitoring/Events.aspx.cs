using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Teleform.ProjectMonitoring;
using Teleform.Reporting.MicrosoftOffice;
using Teleform.Reporting;

namespace Monitoring
{
    using Teleform.ProjectMonitoring;
    using System.Data;
    using System.Data.SqlClient;
    using System.Configuration;
    using System.Text;
    using System.IO;
    using Teleform.ProjectMonitoring.HttpApplication;

    public partial class Events : BasePage
    {

        private string selectedTemplateID { get; set; }

        private long SelectedEntity
        {
            get
            {
                return ViewState["SelectedEntity"] == null ? -1 : (long)ViewState["SelectedEntity"];
            }
            set
            {
                ViewState["SelectedEntity"] = value;
            }
        }

        private string SelectedTableName
        {
            get
            {
                return ViewState["TableName"] == null ? string.Empty : (string)ViewState["TableName"];
            }
            set
            {
                ViewState["TableName"] = value;
            }
        }

        private string SelectedTableColumnName
        {
            get
            {
                return ViewState["ColumnName"] == null ? string.Empty : (string)ViewState["ColumnName"];
            }
            set
            {
                ViewState["ColumnName"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var dataKeys = EventListGridView.DataKeys;

                if (Request["t"] != null && Request["c"] != null)
                {
                    for (int i = 0; i < dataKeys.Count; i++)
                    {
                        var key = dataKeys[i];

                        if (key["tableID"].ToString() == Request["t"] && key["columnID"].ToString() == Request["c"])
                        {
                            EventListGridView.SelectedIndex = i;
                            EventListGridView_SelectedIndexChanged(EventListGridView, EventArgs.Empty);
                            break;
                        }
                    }
                }
            }

            var userID = Convert.ToInt32(Session["SystemUser.objID"]);
            
            #region заполнение gridview разрешенными на просмотр entity
            var allowedEntities = StorageUserObgects.Select<UserEntityPermission>(userID, userID).getReadPermittedEntities();
            var query = "EXEC [Report].[GetCountedEventList] 0";

            var EventListDataSource = Storage.GetDataTable(query).AsEnumerable().Where(x => allowedEntities.AsEnumerable().Select(o => o["entity"].ToString()).Contains(x["tableName"]));
            if (EventListDataSource.Count() == 0)
                return;

            EventListGridView.DataSource = EventListDataSource.CopyToDataTable();
            EventListGridView.DataBind();
            #endregion

            
        }

        
         protected override void OnPreRender(EventArgs e)
        {
            var userID = Convert.ToInt32(Session["SystemUser.objID"]);

            
            #region заполнение templatelist dropdownlist'а разрешенными на просмотр template'ами
            //            var TemplateListDataQuery = string.Format(@"SELECT p.[objID], p.[name] FROM [Permission].[IUTemplatePermission]({0}) p, model.BTables t
            //                                        where t.[name] = p.[baseTable] and t.[object_ID] = {1} and p.[typeCode] like 'TableBased' and p.[read] = 'True'
            //                                        order by p.[name]", userID, (long)EventListGridView.SelectedDataKey["tableID"]);
            var TemplateListDataQuery = string.Format(@"SELECT p.[objID], p.[name] FROM [Permission].[IUTemplatePermission]({0}) p, model.BTables t
                                        where t.[name] = p.[baseTable] and t.[object_ID] = {1} and p.[typeCode] like 'TableBased' and p.[read] = 'True'
                                        order by p.[name]", userID, SelectedEntity);
            TemplateList.DataSource = Storage.GetDataTable(TemplateListDataQuery);
            TemplateList.DataBind();

            if (!string.IsNullOrEmpty(selectedTemplateID) && TemplateList.Items.FindByValue(selectedTemplateID) != null)
                TemplateList.SelectedValue = selectedTemplateID;

            #endregion



            base.OnPreRender(e);
        }


        protected void Old_CreateExcelReportButton_Click(object sender, EventArgs e)
        {
            EventObjectsGridView.DataBind();
            this.CreateExcelReport(EventObjectsGridView);
        }

       

        protected void EventObjectsGridView__RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {                
                e.Row.ClientIDMode = ClientIDMode.Static;
                var rowID = e.Row.Cells[0].Text;            
                e.Row.ID = rowID;

                e.Row.Attributes.Add("class", "AlternativeRow");

                var entityID = EventListGridView.DataKeys[0].Value;
                e.Row.Attributes.Add("onclick", "switchSelectedRow(" + rowID + "," + entityID + "," + "false" + " )");
            }
        }

      

        protected void EventListGridView_SelectedIndexChanged(object sender, EventArgs e)
        {

            SelectedTableName = (string)EventListGridView.SelectedDataKey["tableName"];
            SelectedTableColumnName = (string)EventListGridView.SelectedDataKey["columnName"];

            var currentEntity = (long)EventListGridView.SelectedDataKey["tableID"];
            if (SelectedEntity == currentEntity && TemplateList.Items.Count > 0) TemplateList.SelectedIndex = 0;

            SelectedEntity = currentEntity;
            var k = EventListGridView.SortExpression;
        }

        protected void EventListGridView_DataBound(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EventListGridView.SelectedIndex = 0;

                if (EventListGridView.SelectedDataKey != null)
                    SelectedEntity = (long)EventListGridView.SelectedDataKey["tableID"];
                TemplateList.DataBind();
            }

            selectedTemplateID = TemplateList.SelectedValue;

        }
      
        protected void CreateExcelReportButton_Click(object sender, EventArgs e)
        {
#if truef
			ReportView rv = new ReportView();
			if (TemplateList.Items.Count == 0) return;
			rv.CreateExcelReport(TemplateList.SelectedValue);

			

#else
            //Что бы ничего не выскакивало если нет событий, то есть нет для них шаблонов
            if (string.IsNullOrEmpty(TemplateList.SelectedValue))
                return;

            EventObjectsGridView.AllowPaging = false;
            EventObjectsGridView.DataBind(); 

            var templateID = TemplateList.SelectedValue;            
            var output = new MemoryStream();
            var report = new DataSet();
            var listID = new StringBuilder();
            string fileName;

            for (int j = 0; j < EventObjectsGridView.Rows.Count; j++)
            {
                listID.Append(EventObjectsGridView.DataKeys[j]["objID"].ToString()).Append(",");
            }
            if (listID.Length == 0)
            {
                throw new Exception("Для генерации отчёта нет строк");
            }
            var objID = listID.ToString().Substring(0, listID.Length - 1);

            using (var connection = new SqlConnection(Global.ConnectionString))
            {
                var queryString = "EXEC [report].[getReportData] @templateID, @objID, @cyr=1, @flFormat=1";
                var command = new SqlCommand(queryString, connection);
                command.Parameters.AddRange(
                     new SqlParameter[]
                    {
                        new SqlParameter { ParameterName = "templateID", DbType = DbType.Int32, Value = templateID},
                        new SqlParameter { ParameterName = "objID", DbType = DbType.String, Value = objID},
                    });

                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(report);

                queryString = "SELECT [fileName] FROM [model].[R$Template] where [objID] =  @templateID";
                command = new SqlCommand(queryString, connection);
                command.Parameters.AddRange(
                     new SqlParameter[]
                    {                        
                        new SqlParameter { ParameterName = "templateID", DbType = DbType.String, Value = templateID},   
                    });
                connection.Open();
                try
                {
                    fileName = command.ExecuteScalar().ToString() + ".xlsx";
                }
                catch (NullReferenceException)
                {
                    throw new Exception("Для генерации отчёта необходимо выбрать шаблон.");
                }
                connection.Close();

                //var ExcelReportOpenXML = new ExcelReportBuilder();

                var ExcelReportOpenXML = new ReportViewExcelBuilder();
                ExcelReportOpenXML.CreateExcel(output, report);
            }

            //Загрузить файл с отчетом на комп пользователя
            Response.Clear();
            Response.ContentType = "text/html";
            Response.AddHeader("content-disposition", "attachment;fileName =" + fileName);
            Response.ContentEncoding = Encoding.UTF8;
            Response.BinaryWrite(output.ToArray());
            Response.Flush();
            Response.End();

            EventObjectsGridView.AllowPaging = true;
#endif
        }

       
    }
}
