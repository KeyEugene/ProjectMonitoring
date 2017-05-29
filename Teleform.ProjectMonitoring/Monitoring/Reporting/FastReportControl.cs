
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

using Teleform.Reporting;
using Teleform.Reporting.Parsers;
using Teleform.Reporting.Providers;
using Teleform.ProjectMonitoring.HttpApplication;


namespace Teleform.ProjectMonitoring
{
    [ToolboxData("<{0}:FastReportControl runat=\"server\"></{0}:FastReportControl>")]
    public class FastReportControl : WebControl, INamingContainer
    {
        private string entityID
        {

            get
            {
                var o = ViewState["fastReportControlEntityID"];

                return o == null ? "" : (string)o;
            }
            set { ViewState["fastReportControlEntityID"] = value; }
        }

        public string BaseTable { get; set; }
        private string selectedID;

        private DropDownList templatesList;
        private Button formReportButton;

#if true || dasha
        private Button prepareReportButton;
#endif

        private Phoenix.Web.UI.Dialogs.Form NameDialog;
        private string SelectedID
        {
            get
            {
                string key;

                if (Page.Request["entity"] == null)
                    key = "objID";
                else key = "id";

                return this.Page.Request[key] == null ? "" : this.Page.Request[key].ToString();
            }

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            selectedID = SelectedID;
        }

        protected override void CreateChildControls()
        {
            
            if (string.IsNullOrEmpty(entityID))
                entityID = this.Page.Request.QueryString["entity"].ToString();

            var query = string.Format("SELECT [name], [objID] FROM [model].[R$Template] WHERE [mimeTypeID] = 664 AND entityID = {0}", entityID);
            
            var dt = Storage.GetDataTable(query);
           

            templatesList = new DropDownList();
            templatesList.DataTextField = "name";
            templatesList.DataValueField = "objID";
            templatesList.AppendDataBoundItems = true;
            templatesList.Items.Add(new ListItem { Text = "Сформировать отчет по", Value = "-1" });
            templatesList.DataSource = dt;
            templatesList.AutoPostBack = true;
            templatesList.DataBind();
            templatesList.SelectedIndexChanged += new EventHandler(templatesList_SelectedIndexChanged);

            this.Controls.Add(templatesList);
            
            CreateDialog();
        }

        void formReportButton_Click(object sender, EventArgs e)
        {
            if (templatesList.SelectedValue == "-1" || selectedID == "") return;

            var report = GetSingleReport();
            if (report == null) return;

            var reportBuilder = new WordReportBuilder();

            var response = Page.Response;

            response.Clear();
            response.AddHeader("Content-Type", "application/octet-stream");
            response.AddHeader("Content-Disposition", "attachment;filename=" + templatesList.SelectedItem.Text + ".docx");

            reportBuilder.Create(response.OutputStream, report);
            response.End();
        }

        private void templatesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (templatesList.SelectedValue == "-1" || selectedID == "") return;

            var report = GetSingleReport();
            if (report == null) return;

            var reportBuilder = new WordReportBuilder();

            var response = Page.Response;

            response.Clear();
            response.AddHeader("Content-Type", "application/octet-stream");
            response.AddHeader("Content-Disposition", "attachment;filename=" + templatesList.SelectedItem.Text + ".docx");

            reportBuilder.Create(response.OutputStream, report);
            response.End();
        }


        void CreateDialog()
        {
            NameDialog = new Phoenix.Web.UI.Dialogs.Form();
            NameDialog.Caption = "Подготовить отчет";

            NameDialog.ContentTemplate = new DialogContentTemplate();

            var cancelButton = new Phoenix.Web.UI.Dialogs.ButtonItem();
            var acceptButton = new Phoenix.Web.UI.Dialogs.ButtonItem();

            NameDialog.Buttons = new List<Phoenix.Web.UI.Dialogs.ButtonItem>();

            NameDialog.Buttons.Add(acceptButton);
            NameDialog.Buttons.Add(cancelButton);

            cancelButton.Text = "Отмена";
            cancelButton.ControlID = "CancelButton";

            NameDialog.CancelButtonID = "CancelButton";


            acceptButton.Text = "Подготовить";
            acceptButton.ControlID = "AcceptButton";

            NameDialog.AcceptButtonID = "AcceptButton";

            NameDialog.Accepted += new EventHandler(nameDialog_Accepted);

            this.Controls.Add(NameDialog);
        }

        void prepareReportButton_Click(object sender, EventArgs e)
        {
            if (templatesList.SelectedValue == "-1" || selectedID == "") return;

            NameDialog.Show();
        }

        void nameDialog_Accepted(object sender, EventArgs e)
        {
            var textBox = NameDialog.FindControl("NameBox") as TextBox;
            var templateID = int.Parse(templatesList.SelectedValue);
            using (var c = new SqlConnection(Kernel.ConnectionString))
            using (var cmd = new SqlCommand("EXEC [model].[R$ReportInsert] @templateID, @created, @userID, @link, @name", c))
            {
                c.Open();

                cmd.Parameters.Add("templateID", SqlDbType.Int).Value = templateID;
                cmd.Parameters.Add("created", SqlDbType.DateTime).Value = DateTime.Now.ToString();
                cmd.Parameters.Add("userID", SqlDbType.Int).Value = this.Page.GetSystemUser();
                cmd.Parameters.Add("link", SqlDbType.VarChar).Value = PrepareReport(templateID);
                cmd.Parameters.Add("name", SqlDbType.VarChar).Value = textBox.Text;

                cmd.ExecuteNonQuery();
            }
        }

        private string PrepareReport(int templateID)
        {
            var extension = GetReportExtension(templateID);
            var guid = Guid.NewGuid();
            var link = string.Format(@"~\temp_data\reports\{0}{1}", guid, extension);

            var reportPath = HttpContext.Current.Server.MapPath(link);

            FileStream fileStream;
            try
            {
                var dir = Path.GetDirectoryName(reportPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                fileStream = File.Create(reportPath);
            }
            catch
            {
                throw new Exception(string.Format("Не удалось создать файл {0} с расширением {1}, полный путь {2}", guid, extension, reportPath));
            }

            var reportBuilder = new WordReportBuilder();

            var report = GetSingleReport();
            if (report != null)
                reportBuilder.Create(fileStream, report);

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

        private Teleform.Reporting.GroupReport GetSingleReport()
        {
            var schema = this.Page.GetSchema();

            if (schema == null) return null;

            var xml = string.Empty;

            var query = string.Format("EXEC [Report].[xmlDataCreating] {0}, @instances = {1}, @flagFormat = 0", templatesList.SelectedValue, selectedID);

            xml = Storage.ExecuteScalarString(query);

            if (string.IsNullOrEmpty(xml)) return null;

            var reportProvider = new ReportProvider<Teleform.Reporting.GroupReport>(new ReportParser<Teleform.Reporting.GroupReport>(schema), xml);

            return reportProvider.GetInstance();
        }


        

    }

    public class DialogContentTemplate : ITemplate
    {
        public void InstantiateIn(Control container)
        {
            var nameLabel = new Label();
            nameLabel.Text = "Имя файла";

            var nameBox = new TextBox();
            nameBox.ID = "NameBox";
            nameBox.ClientIDMode = ClientIDMode.Static;


            container.Controls.Add(nameLabel);
            container.Controls.Add(nameBox);
        }
    }
}
