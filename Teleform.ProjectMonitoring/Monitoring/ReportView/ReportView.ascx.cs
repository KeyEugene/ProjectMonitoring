#define Viktor

#define DEPRECATED_CODE
#define Dasha
#define Alex
#define AlexNewConstructor
#define noGetFilterExpression
#define alexj


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml;

using Teleform.Reporting.MicrosoftOffice;
using Teleform.Reporting;
using Teleform.IO.Compression;
using Phoenix.Web.UI.Dialogs;
using System.Threading;
using Teleform.ProjectMonitoring.HttpApplication;
using Teleform.Reporting.Web;
using Dialogs = Phoenix.Web.UI.Dialogs;
using Teleform.Reporting.DynamicCard;

using System.Web.Script.Serialization;
using System.Net;
using Teleform.ProjectMonitoring.admin.SeparationOfAccessRights;

using Teleform.ProjectMonitoring.Templates;

using EnumTypeCode = Teleform.Reporting.Reporting.Template.EnumTypeCode;

namespace Teleform.ProjectMonitoring
{
    public partial class ReportView : System.Web.UI.UserControl, INamingContainer
    {
        StringBuilder idList = new StringBuilder();

        public string EntityID { get; set; }

        public string ObjectID
        {
            get { return ViewState["ObjectID"] == null ? null : (string)ViewState["ObjectID"]; }
            set { ViewState["ObjectID"] = value; }
        }
        private string SomeEntity
        {
            get { return ViewState["SomeEntity"] as string; }
            set { ViewState["SomeEntity"] = value; }
        }
        private int SelectedRowIndex
        {
            get { return ViewState["SelectedRowIndex"] == null ? -1 : (int)ViewState["SelectedRowIndex"]; }
            set { ViewState["SelectedRowIndex"] = value; }
        }

        protected string Markup
        {
            get { return Markup; }
            set { Markup = value; }
        }

        private string WebEntityName
        {
            get
            {
                var wen = ViewState["WebEntityName"] as string;
                return wen;
            }
            set
            {
                ViewState["WebEntityName"] = value;
            }
        }

        #region вспомогательные методы для загрузки и подготовки отчетов






        private string GetTypeName(int templateID)
        {
            using (var c = new SqlConnection(Kernel.ConnectionString))
            using (var cmd = new SqlCommand("SELECT [RTT].[internal] [typeName] FROM [model].[R$Template] [RT]" +
                                                "JOIN [model].[R$TemplateType] [RTT]  ON [RT].[typeID] = [RTT].[objID]" +
                                                    "WHERE [RT].[objID] = @templateID", c))
            {
                c.Open();
                cmd.Parameters.Add("templateID", SqlDbType.Int).Value = templateID;
                return cmd.ExecuteScalar().ToString();
            }
        }

        private Teleform.Reporting.GroupReport GetExcelReport(int templateID, out string file)
        {
            Teleform.Reporting.Template template = Storage.Select<Template>(templateID);
            string constraint = Request["constraint"], instance = Request["id"];
            /*, @constrID={1}, @instanceID={2}*/
            using (var c = new SqlConnection(Kernel.ConnectionString))
            {

                var command = new SqlCommand("EXEC [report].[getBObjectData] @templateID = @tid, @flFormat = 0, @flHeader = 0", c);

                command.Parameters.AddRange(new[]
                {
                    new SqlParameter { ParameterName = "tid", Value = template.ID, DbType = DbType.Int32 }

                });

                var adapter = new SqlDataAdapter(command);
                var table = new DataTable();
                adapter.Fill(table);
                file = template.FileName;

                return Teleform.Reporting.GroupReport.Make(template, table, Session["SystemUser.objID"].ToString());
            }

        }

        private string GetFileName(int templateID)
        {
            var textBox = GroupReportForm2.FindControl("ArchiveNameBox") as TextBox;
            if (!string.IsNullOrEmpty(textBox.Text)) return textBox.Text + ".xlsx";

            using (var c = new SqlConnection(Kernel.ConnectionString))
            using (var cmd = new SqlCommand("SELECT [fileName] FROM [model].[R$Template] where [objID] =  @templateID", c))
            {
                c.Open();
                cmd.Parameters.Add("templateID", SqlDbType.Int).Value = templateID;
                return cmd.ExecuteScalar().ToString() + ".xlsx";
            }
        }

        private Teleform.Reporting.GroupReport GetGroupReport()
        {

            var objIDList = new StringBuilder();
            var filterTable = ReportViewControl.DataView.ToTable();


            foreach (DataRow row in filterTable.Rows)
                objIDList.Append(row["objID"] + ",");

            if (objIDList.Length == 0)
                return null;

            objIDList.Length--;

            string instanceList = objIDList.ToString();

            var ddl = GroupReportForm2.FindControl("ReportsTemplatesList") as DropDownList;

            Teleform.Reporting.Template template = Storage.Select<Template>(ddl.SelectedValue);
            string constraint = Request["constraint"], instance = Request["id"];
            /*, @constrID={1}, @instanceID={2}*/
            using (var c = new SqlConnection(Kernel.ConnectionString))
            {
                var command = new SqlCommand("EXEC [report].[getListAttributeData] @templateID = @tid, @flFormat = 0, @flHeader = 0, @instances = @list", c); //, @_flTitle = 1", c);

                command.Parameters.AddRange(new[]
                {
                    new SqlParameter { ParameterName = "tid", Value = template.ID, DbType = DbType.Int32 },
                    new SqlParameter { ParameterName = "list", Value = instanceList }
                });

                var adapter = new SqlDataAdapter(command);
                var table = new DataTable();

                adapter.Fill(table);


                return Teleform.Reporting.GroupReport.Make(template, table);
            }
        }
        #endregion
        #region загрузить отчет
        protected void DownloadButton_Click(object sender, EventArgs e)
        {

            if (ReportsTemplatesList.Items.Count == 0) return;
            var templateID = int.Parse(ReportsTemplatesList.SelectedValue);

            var typeName = GetTypeName(templateID);

            if (string.IsNullOrEmpty(typeName)) return;

            if (typeName == "Excel")
                DownLoadExcel(templateID);
            else
                DownLoadZip();

            GroupReportForm2.Close();
        }

        private void DownLoadExcel(int templateID)
        {
            var output = new MemoryStream();

            string file = string.Empty;
            var report = GetExcelReport(templateID, out file);

            if (report == null) return;

            using (var stream = new MemoryStream())
            {
                var builder = new ReportViewExcelBuilder();
                builder.Create(stream, report);

                Response.Clear();
                Response.ContentType = "text/html";
                Response.AddHeader("content-disposition", string.Format("attachment;fileName={0}.xlsx", file));
                Response.ContentEncoding = Encoding.UTF8;
                Response.BinaryWrite(stream.ToArray());
                Response.Flush();
                Response.End();
            }
        }

        private void DownLoadZip()
        {
            var nameBox = GroupReportForm2.FindControl("ArchiveNameBox") as TextBox;

            var report = GetGroupReport();

            var builder = new ArchiveReportBuilder(new WordReportBuilder(),
                         new SevenZipArchivator(), MapPath("~/app_data/zip/"));

            string file;

            if (string.IsNullOrEmpty(nameBox.Text))
                file = string.Format("отчёт_{0}", DateTime.Now.ToShortTimeString().Replace(":", "."));
            else file = nameBox.Text;

            Response.Clear();
            Response.ContentType = "application/x-zip-compressed";
            Response.AddHeader("content-disposition", string.Format("attachment;fileName={0}.zip", file));

            builder.Create(Response.OutputStream, report);

            Response.End();
        }
        #endregion
        #region подготовить отчеты
        protected void PrepareReport_Click(object sender, EventArgs e)
        {
            if (ReportsTemplatesList.Items.Count == 0) return;

            var nameBox = GroupReportForm2.FindControl("ArchiveNameBox") as TextBox;

            var templateID = int.Parse(ReportsTemplatesList.SelectedValue);

            var typeName = GetTypeName(templateID);

            if (string.IsNullOrEmpty(typeName)) return;

            using (var c = new SqlConnection(Kernel.ConnectionString))
            using (var cmd = new SqlCommand("EXEC [model].[R$ReportInsert] @templateID, @created, @userID, @link, @name", c))
            {
                c.Open();

                cmd.Parameters.Add("templateID", SqlDbType.Int).Value = templateID;
                cmd.Parameters.Add("created", SqlDbType.DateTime).Value = DateTime.Now.ToString();
                cmd.Parameters.Add("userID", SqlDbType.Int).Value = this.Page.GetSystemUser();
                cmd.Parameters.Add("link", SqlDbType.VarChar).Value = PutReportInMemory(templateID, typeName);
                cmd.Parameters.Add("name", SqlDbType.VarChar).Value = nameBox.Text;

                cmd.ExecuteNonQuery();
            }

            GroupReportForm2.Close();
        }

        private string PutReportInMemory(int templateID, string typeName)
        {
            var extension = typeName == "Word" ? ".zip" : GetReportExtension(templateID);
            var guid = Guid.NewGuid();
            //var link = string.Format(@"~\App_data\reports\{0}{1}", guid, extension);
            var link = string.Format(@"~\temp_data\reports\{0}{1}", guid, extension);

            var reportPath = HttpContext.Current.Server.MapPath(link);

            FileStream fileStream;
            try
            {
                var dir = Path.GetDirectoryName(reportPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                fileStream = System.IO.File.Create(reportPath);
            }
            catch
            {
                throw new Exception(string.Format("Не удалось создать файл {0} с расширением {1}, полный путь {2}", guid, extension, reportPath));
            }

            if (typeName == "Excel")
            {
                string file = string.Empty;
                var report = GetExcelReport(templateID, out file);

                //var builder = new ExcelReportBuilder();
                var builder = new ReportViewExcelBuilder();
                builder.Create(fileStream, report);
            }
            else if (typeName == "Word")
            {
                var report = GetGroupReport();
                var builder = new ArchiveReportBuilder(new WordReportBuilder(),
                        new SevenZipArchivator(), MapPath("~/App_Data/zip"));

                builder.Create(fileStream, report);
            }

            fileStream.Flush();
            fileStream.Close();

            return link;
        }

        private string GetReportExtension(int templateID)
        {
            using (var c = new SqlConnection(Kernel.ConnectionString))
            using (var cmd = new SqlCommand("SELECT [MT].[extension] FROM [model].[R$Template] [RT]" +
                                               "JOIN [MimeType] [MT] ON [MT].[objID] = [RT].[mimeTypeID]" +
                                                   "WHERE [RT].[objID] = @templateID", c))
            {
                c.Open();
                cmd.Parameters.Add("templateID", SqlDbType.Int).Value = templateID;

                return cmd.ExecuteScalar().ToString();
            }
        }
        #endregion



        protected void Page_Init(object sender, EventArgs e)
        {
            if (Request["t"] != null)
            {
                var input = Request["t"];
                var b = Convert.FromBase64String(input);
                var t = Encoding.UTF8.GetString(b);

                (Page as BasePage).CurrentPageTitle = t;

            }

        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (ViewState["templateCode"] != null)
            {
                string templateID, entityID;
                templateID = entityID = string.Empty;

                if (ViewState["templateID"] != null)
                    templateID = ViewState["templateID"].ToString();

                if (ViewState["entityID"] != null)
                    entityID = ViewState["entityID"].ToString();

                var templateDesigner = new TemplateFactory(ViewState["templateCode"].ToString(), templateID, entityID).InstantiateIn();
                PlaceHolder1.Controls.Add(templateDesigner);


            }
            
            var requestEntity = Request["entity"];
            var someEntity = SomeEntity;

            if (SomeEntity != Request["entity"])
                SomeEntity = Request["entity"];

            if (!Page.IsPostBack)
            {

                FillDropDownList();
                if (ReportMultiView.GetActiveView() == TemplateView)
                {
                    TemplateDesigner.IsNotShowThis = true;

                    GetInstanceList();
                }
            }
            else
            {

                Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "CallJS", "afterpostback();", true);

                var actView = ReportMultiView.GetActiveView();

                if (ReportMultiView.GetActiveView() == TemplateView)
                {
                    TemplateDesigner.IsNotShowThis = true;
                    GetInstanceList();
                }
            }

#if true
            var path = string.Format("~/Dynamics/XDynamicCard.aspx?entity={0}", this.Request["entity"]);                
            InsertInstance.PostBackUrl = path;
#else
            var path = string.Format("~/Dynamics/XDynamicCard.aspx?entity={0}{1}{2}&regime=insert{3}",
                this.Request["entity"],
                (this.Request["constraint"] == null ? "" : "&constraint=" + this.Request["constraint"]),
                (this.Request["id"] == null ? "" : "&id=" + this.Request["id"]),
                (this.WebEntityName == null ? "" : "&webentity=" + this.WebEntityName));
            InsertInstance.PostBackUrl = path;

            var wen = this.WebEntityName;
#endif
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        private string MakeUniqueKey(string item)
        {
            var entityID = Request.QueryString["entity"];
            var constraintID = Request.QueryString["constraint"];
            var instanceID = Request.QueryString["id"];

            var sessionKey = string.Concat(entityID, "&", item, "&", constraintID, "&", instanceID);
            return sessionKey;
        }


        protected void FilterList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FilterList.SelectedValue))
            {
                var filterID = FilterList.SelectedValue.ToString();

                FilterDesigner.EntityFilterID = filterID;

                Session[MakeUniqueKey("FilterList")] = filterID;

            }
            else if (FilterList.SelectedValue == "")
            {
                Session[MakeUniqueKey("FilterList")] = "";

                ReportViewControl.EntityFilterID = null;
                goToReportView();
            }
        }

#if true

        protected void TemplateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TemplateList.SelectedValue))
            {
                var userID = Session["SystemUser.objID"].ToString();
                var templateID = TemplateList.SelectedValue.ToString();

                var tContr = TemplateDesigner.Controls.Count;
                var entID = TemplateDesigner.EntityID;
                var tempID = TemplateDesigner.TemplateID;

                TemplateDesigner.TemplateID = templateID;
                TemplateDesigner.template = null;
                TemplateDesigner.DataBind();
                if (TemplateDesigner.template != null)
                    oldTemplateName = TemplateDesigner.template.Name;

                Session[MakeUniqueKey("TemplateList")] = templateID;
            }
            else
            {
                var entityID = Request.QueryString["entity"];
                var templateID = string.Format("{0}_{1}", "titleAttributesTemplate", entityID);
                Session[MakeUniqueKey("TemplateList")] = templateID;
            }
            GetInstanceList();
        }

#else
        protected void TemplateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TemplateList.SelectedValue))
            {

                var userID = Session["SystemUser.objID"].ToString();
                var templateID = TemplateList.SelectedValue.ToString();


                if (templateID.Contains("AttributesTemplate"))
                {
                    if (!AuthorizationRulesTemplate.Resolution(ActionType.create, Session["SystemUser.objID"].ToString()))
                    {
                        WarningMessageBoxAuthorization.Show();
                        return;
                    }
                }
                else
                {
                    if (!AuthorizationRulesTemplate.Resolution(ActionType.read, Session["SystemUser.objID"].ToString(), TemplateList.SelectedValue))
                    {
                        WarningMessageBoxAuthorization.Show();
                        return;
                    }
                }

               

                var tContr = TemplateDesigner.Controls.Count;
                var entID = TemplateDesigner.EntityID;
                var tempID = TemplateDesigner.TemplateID;

                TemplateDesigner.TemplateID = templateID;
                TemplateDesigner.template = null;
                TemplateDesigner.DataBind();
                if (TemplateDesigner.template != null)
                    oldTemplateName = TemplateDesigner.template.Name;

                Session[MakeUniqueKey("TemplateList")] = templateID;
            }
            else
            {
                if (!AuthorizationRulesTemplate.Resolution(ActionType.create, Session["SystemUser.objID"].ToString()))
                {
                    WarningMessageBoxAuthorization.Show();
                    return;
                }

                var entityID = Request.QueryString["entity"];


                var templateID = string.Format("{0}_{1}", "titleAttributesTemplate", entityID);
                Session[MakeUniqueKey("TemplateList")] = templateID;
            }
            GetInstanceList();
        }
#endif

        /// <summary>
        /// Получает данные из БД, кладет их в контрол и в оперативную память на Web сервере
        /// </summary>
        private void GetInstanceList()
        {
            var entityID = Request.QueryString["entity"];

            if (string.IsNullOrEmpty(entityID))
            {
                ReportViewControl.Visible = false;
                ReportViewControl.Enabled = false;
                ReportViewControl.DataSource = null;
                return;
            }

            var templateID = Session[MakeUniqueKey("TemplateList")];

            if (templateID == null)
                templateID = string.Format("titleAttributesTemplate_{0}", entityID);

            ColResizableTempIDBox.Text = templateID.ToString();


            var entityFilterID = string.Empty;
            if (!string.IsNullOrEmpty(FilterList.SelectedValue))
                entityFilterID = FilterList.SelectedValue;
            else
                entityFilterID = null;

            Draw_ReportViewControl(templateID.ToString(), entityFilterID);
        }

        private void Draw_ReportViewControl(string templateID, string entityFilterID)
        {
            var entityID = Request.QueryString["entity"].ToString();

            var businessContent = Storage.Select<BusinessContent>(entityID);

            var entity = Storage.Select<Entity>(entityID);


            var naviFilter = GetNavigatFilterExpression();

            var userID = Convert.ToInt32(HttpContext.Current.Session["SystemUser.objID"]);

            if (businessContent.GetTable(userID).Rows.Count < 1 || naviFilter.Equals("no instances"))
            {
                //пустая таблица для навигацонного контрола, если нет instances для определенного объекта
                Page.Session["TableFromPage"] = null;
                return;
            }

            var template = Storage.Select<Template>(templateID);

            UserTemlatePermission.SetFieldsTaboo(userID, template);


            var refTbl = businessContent.GetTable(userID);

            if (entity.IsHierarchic && Request.QueryString["parentID"] != null)
            {
                //ReportViewControl.IsExistsParentID = true;
                refTbl = GetHierarchicObjects(entity, refTbl);
            }
            else if (Request.QueryString["level"] != null)
            {
                //ReportViewControl.IsExistsLevel = true;
            }

            ListAggregation.ComputeAggrigate(template, refTbl);


            if (!IsEditModeCheckBox.Checked)
                Page.Session[string.Concat("DataViewFor", entity.SystemName)] = null;

            //Временно, отключить навигацию по объектам (надо разбираться)  
             Page.Session["checkBoxObjectsNavigation"] = true;

            initialiseTableContorl(templateID, refTbl, entityFilterID);
        }

        private void initialiseTableContorl(object templateID, DataTable refTbl, string entityFilterID)
        {
            if (!string.IsNullOrEmpty(SelectedRowIDBox.Text))
                ReportViewControl.SelectedRowIndex = int.Parse(SelectedRowIDBox.Text);
            ReportViewControl.EntityFilterID = entityFilterID;
            if (ReportViewControl.IsEditMode == true)
                ReportViewControl.SetSelfColumnsValue(SaveObjectsJeysonBox.Text);
            ReportViewControl.SetTemplateFieldsSize(ResizableTableControlBox.Text);
            ResizableTableControlBox.Text = null;

            //ColResizableBox.Text = null;

           

            ReportViewControl.DataSource = refTbl;
            ReportViewControl.NavigatFilterExpression = GetNavigatFilterExpression();
            ReportViewControl.TemplateID = templateID;
            ReportViewControl.AllowPaging = true;
            ReportViewControl.DataBind();
            ReportViewControl.Visible = true;
            ReportViewControl.Enabled = true;
        }



        protected void DeleteMessage_Closed(object sender, Phoenix.Web.UI.Dialogs.MessageBoxEventArgs e)
        {
            if (e.Result == Phoenix.Web.UI.Dialogs.MessageBoxResult.Yes)
            {               

                int instanceID;
                var isParse = Int32.TryParse(SelectedRowIDBox.Text, out instanceID);
                if (isParse)
                {
                    var entityID = Request.QueryString["entity"];

                    try
                    {
                        ReportViewControl.DeleteInstance(entityID, instanceID);
                    }
                    catch (Exception ex)
                    {
                        ErrorMessageBox.Show();
                        (ErrorMessageBox.FindControl("ErrorLabel") as Label).Text = ex.Message;
                    }

                   
                    GetInstanceList();
                }
                else
                {
                    throw new Exception("Объект не уадален, надо выбрать объект");
                }
            }
        }

        protected void DeleteInstance_Click(object sender, EventArgs e)
        {
            if (ReportViewControl.SelectedRowIndex != -1)
            {
                DeleteMessage.Show();
            }
        }

        protected void SaveObjects_OnClick(object sender, EventArgs e)
        {
            try
            {
                ReportViewControl.SaveInstances();
            }
            catch (Exception ex)
            {
                            
                ErrorMessageBox.Show();
                (ErrorMessageBox.FindControl("ErrorLabel") as Label).Text = ex.Message;
            }

            

            SaveObjectsJeysonBox.Text = null;

            GetInstanceList();
        }


        protected void InsertInstance_Click(object sender, EventArgs e)
        {
            if (Session["isPermission"] != null)
            {
                ErrorMessageBox.Show();
                (ErrorMessageBox.FindControl("ErrorLabel") as Label).Text = "Недостаточно прав для совершения данного действия.";
                Session["isPermission"] = null;
            }
        }


        private DataTable GetHierarchicObjects(Entity entity, DataTable refTbl)
        {
            var parentIDsHash = entity.Attributes.First(o => o.FPath == "parentID").ID.ToString();
            //отобразить root объекты
            var fromRequesParentID = Request.QueryString["parentID"];
            if (fromRequesParentID == "-1")
            {
                string expression = string.Format("[{0}] IS NULL", parentIDsHash);

                DataRow[] rows = refTbl.Select(expression);

                if (rows != null && rows.Count() > 0)
                    refTbl = rows.CopyToDataTable();
            }
            //если что то есть в parentID
            else if (Request.QueryString["parentID"] != null)
            {
                int number, parentID = 0;
                bool result = int.TryParse(Request.QueryString["parentID"].ToString(), out number);
                if (result)
                    parentID = number;

                string expression = string.Format("[{0}] = {1}", parentIDsHash, parentID);

                DataRow[] rows = refTbl.Select(expression);

                if (rows != null && rows.Count() > 0)
                    refTbl = rows.CopyToDataTable();
            }

            return refTbl;
        }


        protected void PageCountList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var value = PageCountList.SelectedValue;

            if (value == "all")
            {
                ReportViewControl.AllowPaging = false;
            }
            else
            {
                ReportViewControl.AllowPaging = true;
                ReportViewControl.PageSize = int.Parse(value);
            }
            ReportViewControl.Reset(true);
            ReportViewControl.DataBind();
        }


        protected string GetTitleTemplateID(string entityID)
        {
            var entityName = this.GetSchema().Entities.SingleOrDefault(x => x.ID.ToString().Equals(entityID)).Name;


            var isClassifier = Request.QueryString["checker"];

            var query = string.Empty;

            if (!string.IsNullOrEmpty(isClassifier))
                query = string.Format("SELECT objID FROM [model].[R$Template] WHERE entityID = {0}", entityID);
            else
                query = string.Format("SELECT templateID FROM [model].[BTables] WHERE object_ID = {0}", entityID);

            var templateID = Storage.ExecuteScalarString(query);

            return templateID;
        }






        //protected void DynamicCard_CardClosed(object sender, EventArgs e)
        //{
        //    InsertInstanceDialog.Close();
        //}

        protected void ReportView_RowCreated(object sender, GridViewRowEventArgs e)
        {
            throw new NotImplementedException();

            if (e.Row.RowType != DataControlRowType.Pager)
            {
                e.Row.Cells[0].Visible = false;
                e.Row.Cells[1].Visible = false;
            }
        }

        protected void ReportGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            foreach (TableCell c in e.Row.Cells)
            {
                if (c.Text == "False" || c.Text == "True")
                    c.Text = (c.Text == "False" ? "Нет" : "Да");
            }
        }

        protected void AgregateSource_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            if (e.Command.Parameters["@cID"].Value == null)
                e.Command.Parameters["@cID"].Value = DBNull.Value;

            if (e.Command.Parameters["@id"].Value == null)
                e.Command.Parameters["@id"].Value = DBNull.Value;
        }


        protected void ReportViewControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Session["isPermission"] != null)
            {
                var r = Session["isPermission"];
                ErrorMessageBox.Show();
                (ErrorMessageBox.FindControl("ErrorLabel") as Label).Text = "Недостаточно прав для совершения данного действия.";
                Session["isPermission"] = null;
            }
        }

        protected void ReportViewControl_DataReady(object sender, EventArgs e)
        {
            RecordsNumberLabel.Text = string.Concat("Число объектов: ", ReportViewControl.DataView.Count.ToString());

        }





        protected void DynamicCard_Accepted(object sender, EventArgs e)
        {
            Cache[WebEntityName] = new object();
            ReportViewControl.DataBind();
        }

        protected void CreateExcelReportButton_Click(object sender, EventArgs e)
        {

            if (TemplateList.Items.Count == 0)
                return;
            else
            {

                int userID;
                bool result = int.TryParse(Session["SystemUser.objID"].ToString(), out userID);
                if (true)
                    CreateExcelReport(TemplateList.SelectedValue, userID);
                else
                    CreateExcelReport(TemplateList.SelectedValue, 0);
            }

        }

        protected void FilterDesigner_DesigningFinished(object sender, EventArgs e)
        {
            goToReportView();
        }

        protected void goToReportView()
        {

            //TemplateDesigner.Dispose();

            ReportViewControl.DataBind();
            ReportMultiView.SetActiveView(TemplateView);
            var EntityListAttributeView = this.Parent;
            var entityList = EntityListAttributeView.FindControl("EntityList") as DropDownList;
            var entitiListLabel = EntityListAttributeView.FindControl("EntitiListLabel") as Label;
            if (entityList != null && entitiListLabel != null)
            {
                entityList.Visible = true;
                entitiListLabel.Visible = true;
            }

            FilterDeleteButton.Visible = false;
            FilterReNameButton.Visible = false;

            FilterList.Visible = true;
            GoToFilterDesignerButton.Visible = true;

            TemplateList.Visible = true;
            TemplateConstructorButton.Visible = true;


        }

        protected void GoToFilterDesignerButton_Click(object sender, EventArgs e)
        {
            if (FilterList.SelectedValue == "")
            {
                FilterDialog.Caption = "Создание фильтра";
                FilterDialog.Show();
            }
            else
            {
                goToFilterDesigner();
            }
        }

        public void ApplyNewFilterButton_Click(object sender, EventArgs e)
        {
            var id = string.Empty;

            var name = (FilterDialog.FindControl("InsertNameFilter") as TextBox).Text;

            if (string.IsNullOrEmpty(name))
                FilterDesigner.ErrorsChecking(name, true);


            var userID = Session["SystemUser.objID"].ToString();
            var entityID = Request.QueryString["entity"];

            var xml = FilterDesigner.GenerateXml(id, name, userID, entityID);
            FilterDesigner.CreateFilter(xml);
            var filterID = FilterDesigner.GetFilterID(name, entityID);

            FilterList.Items.Clear();
            FilterList.Items.Add(new ListItem("Не выбрано", ""));
            FilterList.DataBind();
            FilterList.SelectedValue = filterID;

            FilterDialog.Close();
            goToFilterDesigner();
        }


        public void FilterReNameButton_Click(object sender, EventArgs e)
        {
            ReNameFilterDialog.Caption = "Переименование фильтра";
            ReNameFilterDialog.Show();
        }

        public void ApplyReNameFilterButton_Click(object sender, EventArgs e)
        {
            var filterID = FilterList.SelectedValue;
            var entityFilter = Storage.Select<EntityFilter>(filterID);

            var name = (this.ReNameFilterDialog.FindControl("RenameNameBox") as TextBox).Text;
            entityFilter.Name = name;

            var i = 0;
            foreach (var item in FilterList.Items)
            {
                if (name == FilterList.Items[i].Text || string.IsNullOrEmpty(name))
                {
                    FilterDesigner.ErrorsChecking(name, false);
                }

                i++;
            }

            var xml = FilterDesigner.GenerateXml(entityFilter);
            FilterDesigner.UpdateFilter(xml);
            FilterList.SelectedItem.Text = RenameNameBox.Text;

            ReNameFilterDialog.Close();
            goToFilterDesigner();
        }

        protected void ReNameFilterDialogCloseHandler(object sender, EventArgs e)
        {
            (ReNameFilterDialog.FindControl("RenameNameBox") as TextBox).Text = string.Empty;
        }

        private void goToFilterDesigner()
        {
            FilterDesigner.EntityFilterID = int.Parse(FilterList.SelectedValue);
            ReportMultiView.SetActiveView(FilterDesignerView);

            var reportView = this.Parent;

            var entityList = reportView.FindControl("EntityList") as DropDownList;
            var entitiListLabel = reportView.FindControl("EntitiListLabel") as Label;
            if (entityList != null && entitiListLabel != null)
            {
                entityList.Visible = false;
                entitiListLabel.Visible = false;
            }
            FilterReNameButton.Visible = true;
            FilterDeleteButton.Visible = true;

            TemplateConstructorButton.Visible = false;
            TemplateList.Visible = false;

        }


        protected void IsEditModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = sender as System.Web.UI.WebControls.CheckBox;
            ReportViewControl.IsEditMode = checkBox.Checked;

            if (checkBox.Checked == false)
                ReportViewControl.SessionContent.EntityInstances.Clear();

            SaveObjectsJeysonBox.Text = null;

            ReportViewControl.DataBind();
        }


        protected void ResetAllFilters_OnClick(object sender, EventArgs e)
        {
            ReportViewControl.RejectAllFiltration();
        }

        protected void ResetAllSortings_OnClick(object sender, EventArgs e)
        {
            ReportViewControl.RejectAllSortings();
        }

        protected void FilterDeleteButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FilterList.SelectedValue))
                DeleteWarningDialog.Show();
        }
        protected void DeleteWarningDialog_Close(object sender, EventArgs e)
        {
            if ((e as MessageBoxEventArgs).Result.ToString() == "Yes")
            {
                var FilterList = this.FindControl("FilterList") as DropDownList;

                var index = FilterList.SelectedIndex;
                var entityFilterID = FilterList.SelectedValue;

                using (var c = new SqlConnection(Kernel.ConnectionString))
                using (var cmd = new SqlCommand("DELETE FROM [model].[R$EntityFilter] WHERE objID = @entityFilterID; DELETE FROM [model].[R$EntityFilterAttribute] WHERE [entityFilterID] = @entityFilterID", c))
                {
                    cmd.Parameters.AddRange(
                        new SqlParameter[] { new SqlParameter { ParameterName = "entityFilterID", DbType = DbType.String, Value = entityFilterID } });
                    c.Open();
                    cmd.ExecuteNonQuery();
                }
                Storage.ClearInstanceCache(typeof(EntityFilter), entityFilterID);


                FilterList.Items.Clear();
                FilterList.Items.Add(new ListItem("Не выбрано", ""));
                FilterList.DataBind();

                if (!string.IsNullOrEmpty(FilterList.SelectedValue))
                    ReportViewControl.EntityFilterID = FilterList.SelectedValue;
                else
                    ReportViewControl.EntityFilterID = null;

                goToReportView();
            }
        }





        #region Work with edit and ctreate templates


        protected void ReportsTemplatesList_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var templateID = ReportsTemplatesList.SelectedValue;

            var template = Storage.Select<Template>(templateID);

            var postBackUrl = getPostBackUrl(template);

            EditTemplateButton.PostBackUrl = postBackUrl;
        }



        protected void ToGroupReportButton_Click(object sender, EventArgs e)
        {
            GroupReportForm2.Show();
        }



        protected List<object> GetPermittedTemplates(string lexem, string typeCode)
        {       
            var ReportsTemplateslist = new List<object>();

            entityID = Request.QueryString["entity"];
            var userID = Session["SystemUser.objID"].ToString();

            var query = string.Format(@"select p.[objID] [templateID], p.[name] [templateName], p.[type] [typeName]
                                      from [Permission].[IUTemplatePermission]({0}) p, model.BTables t
                                      where t.[name] = p.[baseTable] and t.[object_ID] = '{1}'  and p.[read] = 1 and p.[typeCode] {2} '{3}'
                                      order by p.[name]", userID, entityID, lexem, typeCode);

            var dt = Storage.GetDataTable(query);

            var deniedEntity = StorageUserObgects.Select<UserEntityPermission>(6, 6).getReadDeniedEntities().AsEnumerable().Select(x => x["entity"].ToString()).ToList<string>();

            foreach (DataRow row in dt.Rows)
            {
                var templateID = row["templateID"];
                var typeName = row["typeName"];
                var templateName = row["templateName"];

                var entityName = string.Empty;
                var template = Storage.Select<Template>(templateID);
                foreach (var field in template.Fields)
                    entityName = deniedEntity.FirstOrDefault(e => e == field.NativeEntityName);

                if (entityName == null)
                    ReportsTemplateslist.Add(new { Value = templateID, Text = string.Format("({0}){1}", typeName, templateName) });

            }
            return ReportsTemplateslist;
        }

        protected void TemplateList_Load(object sender, EventArgs e)
        {
            entityID = Request.QueryString["entity"];
            var userID = Session["SystemUser.objID"].ToString();

        }


        private void FillDropDownList()
        {
            entityID = Request.QueryString["entity"];
            if (entityID != null)
            {
                var userID = Session["SystemUser.objID"].ToString();

                TemplateList.Items.Clear();

                var requireTemplateID = string.Format("requireAttributesTemplate_{0}", entityID);                
                var titleTemplateID = string.Format("titleAttributesTemplate_{0}", entityID);
                                
                var requireTemplate = Storage.Select<Template>(requireTemplateID);
                var titleTemplate = Storage.Select<Template>(titleTemplateID);

                var runtimeTeplates = new List<Template>();
                runtimeTeplates.Add(requireTemplate);
                runtimeTeplates.Add(titleTemplate);
                

                //TemplateList.Items.Add(new ListItem("Титульный шаблон", titleTemplateID));
                //TemplateList.Items.Add(new ListItem("Создание и редактирование объектов", requireTemplateID));
                

                var templateListSource = UserTemlatePermission.GetPermittedTemplates(entityID, userID, "=", "TableBased", runtimeTeplates);

                TemplateList.DataSource = templateListSource;              
                TemplateList.DataBind();

                var templateSessionKey = MakeUniqueKey("TemplateList");
                var templateID = Session[templateSessionKey];
                if (templateID != null)
                    TemplateList.SelectedValue = templateID.ToString();
                else
                    TemplateList.SelectedValue = string.Format("titleAttributesTemplate_{0}", entityID);
            }


            FilterList.Items.Clear();
            FilterList.Items.Add(new ListItem("Создать новый", ""));
            FilterList.DataBind();

            var filterSessionKey = MakeUniqueKey("FilterList");
            var filterID = Session[filterSessionKey];
            if (filterID != null)
                FilterList.SelectedValue = filterID.ToString();
            else
                FilterList.SelectedValue = "";
        }



        protected void ReportsTemplatesList_OnLoad(object sender, EventArgs e)
        {
            entityID = Request.QueryString["entity"];
            var userID = Session["SystemUser.objID"].ToString();
            var templateID = ReportsTemplatesList.SelectedValue;
            Template template;

            if (string.IsNullOrEmpty(templateID))
            {
        
                var templateList = UserTemlatePermission.GetPermittedTemplates(entityID, userID, " != ", "InputExcelBased");      
                if (templateList.Count == 0)
                {
                    EditTemplateButton.Visible = false;
                    ReportsTemplatesList.Visible = false;
                    return;
                }
                else
                {
                    EditTemplateButton.Visible = true;
                    ReportsTemplatesList.Visible = true;
                }


                ReportsTemplatesList.Items.Clear();
                ReportsTemplatesList.DataSource = templateList;
                ReportsTemplatesList.DataBind();

                templateID = ReportsTemplatesList.SelectedValue;

                template = Storage.Select<Template>(templateID);

                var postBackUrl = getPostBackUrl(template);

                EditTemplateButton.PostBackUrl = postBackUrl;
            }

            template = Storage.Select<Template>(templateID);

            #region Для файловый шаблонов, показать загрузчик файла
            if (template.TypeCode == EnumTypeCode.WordBased || template.TypeCode == EnumTypeCode.ExcelBased)
            {
                DownloadButton.Visible = true;

                if (template.TypeCode == EnumTypeCode.WordBased)
                {
                    ArchiveNameLabel.Visible = true;
                    ArchiveNameBox.Visible = true;
                }
                else
                {
                    ArchiveNameLabel.Visible = false;
                    ArchiveNameBox.Visible = false;
                }
            }
            else
            {
                DownloadButton.Visible = false;
                ArchiveNameLabel.Visible = false;
                ArchiveNameBox.Visible = false;
            }

            #endregion

#if trueWWW
  
            if (AuthorizationRulesTemplate.Resolution(ActionType.create, userID))
            {
                CreateTemplateButton.Visible = true;
            }
            else
                CreateTemplateButton.Visible = false;
         

            if (AuthorizationRulesTemplate.Resolution(ActionType.update, userID, templateID))
            {
                EditTemplateButton.Visible = true;
                ReportsTemplatesList.Visible = true;
            }
            else
                EditTemplateButton.Visible = false;


   
            if (template.TypeCode == EnumTypeCode.screenTree || template.TypeCode == EnumTypeCode.crossReport)
            {
                EditTemplateButton.Visible = true;
            }
        
#endif



        }


        protected void CencelButton_Click(object sender, EventArgs e)
        {
            FillDropDownList();
        }
        
        protected void EditTemplateButton_Click(object sender, EventArgs e)
        {
            var userID = Session["SystemUser.objID"].ToString();
            var templateID = ReportsTemplatesList.SelectedValue;
            var template = Storage.Select<Template>(templateID);           

            if (template.TypeCode == EnumTypeCode.crossReport || template.TypeCode == EnumTypeCode.screenTree)
            {
                InitializeTemplateFactory(template);
            }
            else
            {
                if (AuthorizationRules.TemplateResolution(ActionType.update, userID, templateID))
                    InitializeTemplateFactory(template);
                else
                {
                    var warningLabel = WarningMessageBox.FindControl("WarningLabel") as Label;
                    warningLabel.Text = "Нет прав для редактирования шаблона";
                    WarningMessageBox.Show();

                    return;
                }
            }

            UserTemlatePermission.SetFieldsTaboo(Convert.ToInt32(userID), template);

        }
        protected void CreateTemplateButton_Click(object sender, EventArgs e)
        {
            if (!AuthorizationRules.TemplateResolution(ActionType.create, Session["SystemUser.objID"].ToString())) //, Session["SystemUser.objID"].ToString() 
            {
                var warningLabel = WarningMessageBox.FindControl("WarningLabel") as Label;
                warningLabel.Text = "Нет прав для редактирования шаблона";
                WarningMessageBox.Show();
                return;
            }

            TemplateDesignerDialog.Caption = "Создание шаблона";
            TemplateDesignerDialog.Show();

            var radioList = TemplateDesignerDialog.FindControl("RadioList") as RadioButtonList;
            radioList.Visible = true;

            CleareDataFromViewState();
        }

        protected void TemplateTypeRadioButton_Click(object sender, EventArgs e)
        {
            var entityID = Request.QueryString["entity"];

            var radioList = TemplateDesignerDialog.FindControl("RadioList") as RadioButtonList;
            radioList.Visible = false;

            var placeHolder = TemplateDesignerDialog.FindControl("PlaceHolder1") as PlaceHolder;
            ViewState["templateCode"] = radioList.SelectedValue;
            ViewState["entityID"] = entityID;
            ViewState["templateID"] = null;
            placeHolder.Controls.Clear();
            var templateDesigner = new TemplateFactory(radioList.SelectedValue, null, entityID).InstantiateIn();
            templateDesigner.userID = Convert.ToInt32(Session["SystemUser.objID"]);
            placeHolder.Controls.Add(templateDesigner);

        }

        private void InitializeTemplateFactory(Template template)
        {
            var radioList = TemplateDesignerDialog.FindControl("RadioList") as RadioButtonList;
            radioList.Visible = false;

            TemplateDesignerDialog.Caption = "Редактирование шаблона";
            TemplateDesignerDialog.Show();

            var placeHolder = TemplateDesignerDialog.FindControl("PlaceHolder1") as PlaceHolder;
            var fileBasedTemplateDesigner = new TemplateFactory(template.TypeCodeString, template.ID.ToString(), null).InstantiateIn();

            fileBasedTemplateDesigner.userID = Convert.ToInt32(Session["SystemUser.objID"]);

            ViewState["templateCode"] = template.TypeCodeString;
            ViewState["entityID"] = null;
            ViewState["templateID"] = template.ID;

            placeHolder.Controls.Clear();
            placeHolder.Controls.Add(fileBasedTemplateDesigner);
        }



        private string getPostBackUrl(Template template)
        {
            if (template.TypeCode == EnumTypeCode.screenTree)
            {
                return string.Format("~/HardTemplate/HardTemplateView.aspx?entity={0}&templateID={1}", template.Entity.ID, template.ID);
            }
            else if (template.TypeCode == EnumTypeCode.crossReport)
            {
                return string.Format("~/CrossTemplate/CrossTemplateView.aspx?entity={0}&templateID={1}", template.Entity.ID, template.ID);
            }
            else return string.Empty;
        }

        private void PreSaveTemplate()
        {
            var tddType = TemplateDesignerDialog.FindControl("TemplateControl") as GeneralTemplateDesigner;

            if (tddType != null)
                if (ViewState["saveTemplateAs"] == null || Convert.ToBoolean(ViewState["saveTemplateAs"]) == false)
                    tddType.Save();
                else tddType.Save(true);


            entityID = Request.QueryString["entity"];
            var userID = Session["SystemUser.objID"].ToString();

            ReportsTemplatesList.Items.Clear();

            ReportsTemplatesList.DataSource = UserTemlatePermission.GetPermittedTemplates(entityID, userID, " != ", "InputExcelBased");
            ReportsTemplatesList.DataBind();


        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            ViewState["saveTemplateAs"] = false;

            PreSaveTemplate();

            TemplateDesignerDialog.Close();

            TemplateSavedMessageBox.Show();

            CleareDataFromViewState();

        }

        protected void TemplateDialog_Closed(object sender, EventArgs e)
        {
            DialogCleaner(TemplateDesignerDialog);
            CleareDataFromViewState();
        }

        private void CleareDataFromViewState()
        {
            ViewState["templateCode"] = null;
            ViewState["templateID"] = null;
        }

        private void DialogCleaner(Control control)
        {
            foreach (Control c in control.Controls)
            {
                if (c is TextBox)
                    (c as TextBox).Text = string.Empty;
                else if (c is ListControl)
                    (c as ListControl).ClearSelection();
                else if (c is CheckBox)
                    (c as CheckBox).Checked = false;
                else DialogCleaner(c);
            }
        }

        #endregion


    }

}