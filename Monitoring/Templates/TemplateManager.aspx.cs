#warning Оптимизация загрузки шаблона.
#define alexj

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Configuration;
using System.Data;
using System.Data.SqlClient;

using Teleform.ProjectMonitoring.HttpApplication;
using Phoenix.Web.UI.Dialogs;
using Monitoring;
using System.IO;

namespace Teleform.ProjectMonitoring.Templates
{
    using System.Text;
    using System.Web.UI.WebControls;
    using Teleform.Reporting;
    using Teleform.ProjectMonitoring.admin.SeparationOfAccessRights;

    public partial class TemplateManager : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Frame.UserControl_EntityList_SelectedIndexChanged += EntityList_SelectedIndexChanged;
            Frame.UserControl_TemplateList_SelectedIndexChanged += TemplateList_SelectedIndexChanged;
            Frame.UserControl_CreateButton_Click += CreateButton_Click;
            Frame.UserControl_EditButton_Click += EditButton_Click;
            Frame.UserControl_DownloadButton_Click += DownloadButton_Click;
            Frame.UserControl_ShowPreview_Click += ShowPreview_Click;
            Frame.UserControl_DeteleButton_Click += DeteleButton_Click;

            if (!IsPostBack)
            {

                var userTypeID = Session["SystemUser.typeID"];

                if (Session["SystemUser.typeID"] == null)
                    throw new NullReferenceException("Тип пользователя не известен, надо войти в систему");
                else if (userTypeID.ToString() != "1")
                {
                    var userTypeName = Session["SystemUser.typeName"];

                    if (userTypeName != null)
                        throw new Exception(string.Concat(userTypeName.ToString(), " таким типам пользователей доступ на эту страницу запрещен"));
                    else
                        throw new NotImplementedException();
                }

                TemplateList_SelectedIndexChanged(null, null);
            }
            // var placeHolder = TemplateDesignerDialog.FindControl("PlaceHolder") as PlaceHolder;
            if (ViewState["templateCode"] != null)
            {
                string templateID, entityID;
                templateID = entityID = string.Empty;

                if (ViewState["templateID"] != null)
                    templateID = ViewState["templateID"].ToString();

                if (ViewState["entityID"] != null)
                    entityID = ViewState["entityID"].ToString();

                var templateDesigner = new TemplateFactory(ViewState["templateCode"].ToString(), templateID, entityID).InstantiateIn();
                PlaceHolder.Controls.Add(templateDesigner);
            }
        }

        protected void DownloadButton_Click(object sender, EventArgs e)
        {
            if (Frame.TemplateList.SelectedDataKey["code"].ToString() == "InputExcelBased")
            {
                using (var stream = new MemoryStream())
                {
                    var excelTableBased = new TableBasedTemplateToExcel();
                    //  excelTableBased.template = Storage.Select<Teleform.Reporting.Template>(Frame.TemplateList.SelectedDataKey["objID"]);
                    excelTableBased.Entitys = this.GetSchema().Entities.ToList();
                    var template = Storage.Select<Teleform.Reporting.Template>(Frame.TemplateList.SelectedDataKey["objID"]);
                    var report = GroupReport.Make(template, new DataTable());
                    excelTableBased.Create(stream, report);

                    Response.Clear();
                    Response.ContentType = "text/html";
                    Response.AddHeader("content-disposition", string.Format("attachment;fileName={0}.xlsx", template.Name));
                    Response.ContentEncoding = Encoding.UTF8;
                    Response.BinaryWrite(stream.ToArray());
                    Response.Flush();
                    Response.End();
                }
            }
            else
            {

                var adapter = new SqlDataAdapter(
                    string.Concat(
                        @"SELECT
                        [A].[fileName],
                        [A].[body],
                        [B].[mime],
                        [B].[extension]
                    FROM [model].[R$Template] [A] JOIN [MimeType] [B] ON [A].[mimeTypeID] = [B].[objID]
                    WHERE [A].[objID] = ", Frame.TemplateList.SelectedValue),
                    Global.ConnectionString);

                var table = new DataTable();

                adapter.Fill(table);

                if (table.Rows.Count > 0)
                {
                    var data = table.Rows[0];

                    Response.Clear();
                    Response.ContentType = data["mime"].ToString();
                    Response.AddHeader("content-disposition",
                        string.Concat("attachment;fileName=", data["fileName"], data["extension"]));

                    Response.BinaryWrite((byte[])data["body"]);
                    Response.End();
                }
            }
        }

        protected void DeteleButton_Click(object sender, MessageBoxEventArgs e)
        {
            if (e.Result == MessageBoxResult.Yes)
            {
                if (!AuthorizationRules.TemplateResolution(ActionType.delete,
                    Session["SystemUser.objID"].ToString(),
                    Frame.TemplateList.SelectedDataKey["objID"].ToString()))
                {
                    Frame.WarningMessageBox.Show();
                    return;
                }

                Frame.TemplateListSource.Delete();
                Frame.TemplateList.ClearSelection();
                EnableItems(false);
            }
        }

        private void EnableItems(bool enabled)
        {
            SetCss(Frame.EditButton, enabled);
            SetCss(Frame.DeleteButton, enabled);

            if (!enabled)
            {
                SetCss(Frame.DownloadButton, false);
                SetCss(Frame.PreviewButton, false);
            }
            else
            {
                var b = Frame.TemplateList.HasSelection();

                if (b)
                {
                    var hasBody = Convert.ToBoolean(Frame.TemplateList.SelectedDataKey["body"]);

                    SetCss(Frame.DownloadButton, hasBody);
                    SetCss(Frame.PreviewButton, hasBody);

                    //Разрешаем скачивать тип TableBased шаблонов
                    if (Frame.TemplateList.SelectedDataKey["code"].ToString() == "InputExcelBased")
                        SetCss(Frame.DownloadButton, true);
                }
                else
                {
                    SetCss(Frame.DownloadButton, false);
                    SetCss(Frame.PreviewButton, false);
                }
            }
        }

        /// <summary>
        /// Устанавливаем или удалем cssClass disabled
        /// </summary>
        /// <param name="button"> кнопка для которой устанавливаем\удаляем CssClass</param>
        /// <param name="enabled">установить или удалить</param>
        private void SetCss(LinkButton button, bool enabled)
        {
            string currentCss = button.CssClass;
            if (enabled)
            {
                if (currentCss.Contains("disabled"))
                {
                    button.CssClass = currentCss.Replace("disabled", "");
                }
            }
            else
            {
                if (!currentCss.Contains("disabled"))
                {
                    button.CssClass += " disabled";
                }
            }
        }


        protected void EntityList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // it is likely not necessary
            Frame.TemplateList.ClearSelection();
            TemplateList_SelectedIndexChanged(null, null);
        }

        protected void TemplateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableItems(Frame.TemplateList.HasSelection());
        }

#if true || dasha
        protected void CreateButton_Click(object sender, EventArgs e)
        {
            if (!AuthorizationRules.TemplateResolution(ActionType.create, Session["SystemUser.objID"].ToString())) //, Session["SystemUser.objID"].ToString() 
            {
                Frame.WarningMessageBox.Show();
                return;
            }

            TemplateDesignerDialog.Caption = "Создание шаблона";
            TemplateDesignerDialog.Show();

            var radioList = TemplateDesignerDialog.FindControl("RadioList") as RadioButtonList;
            radioList.Visible = true;

            CleareDataFromViewState();
            //this.DataBind();
        }

        protected void TemplateTypeButton_Click(object sender, EventArgs e)
        {
            var radioList = TemplateDesignerDialog.FindControl("RadioList") as RadioButtonList;
            var placeHolder = TemplateDesignerDialog.FindControl("PlaceHolder") as PlaceHolder;
            var templateDesigner = new TemplateFactory(radioList.SelectedValue, null, Frame.EntityList.SelectedValue).InstantiateIn();

            templateDesigner.userID = Convert.ToInt32(Session["SystemUser.objID"]);

            ViewState["templateCode"] = radioList.SelectedValue;
            ViewState["entityID"] = Frame.EntityList.SelectedValue;
            ViewState["templateID"] = null;
            placeHolder.Controls.Clear();
            placeHolder.Controls.Add(templateDesigner);
        }


        [Obsolete("вернуть когда везем к заказчику")]
        protected void EditButton_Click(object sender, EventArgs e)
        {
            // Для отладки, вернуть когда сдавать заказчику , тут по умолчанию admin (id = 0)
            string UserID = Session["SystemUser.objID"] == null ? "0" : Session["SystemUser.objID"].ToString();
            if (!AuthorizationRules.TemplateResolution(ActionType.read, UserID, Frame.TemplateList.SelectedDataKey["objID"].ToString())) //, Session["SystemUser.objID"].ToString() 
            {
                Frame.WarningMessageBox.Show();
                return;
            }

            TemplateDesignerDialog.Caption = "Редактирование шаблона";
            TemplateDesignerDialog.Show();

            var templateID = Frame.TemplateList.SelectedDataKey["objID"].ToString();
            var templateCode = Frame.TemplateList.SelectedDataKey["code"].ToString();

            var placeHolder = TemplateDesignerDialog.FindControl("PlaceHolder") as PlaceHolder;
            var fileBasedTemplateDesigner = new TemplateFactory(templateCode, templateID, null).InstantiateIn();

            var userID = Convert.ToInt32(Session["SystemUser.objID"]);
            fileBasedTemplateDesigner.userID = userID;

            var template = Storage.Select<Template>(templateID);
            UserTemlatePermission.SetFieldsTaboo(userID, template);


            ViewState["templateCode"] = templateCode;
            ViewState["entityID"] = null;
            ViewState["templateID"] = templateID;

            placeHolder.Controls.Clear();
            placeHolder.Controls.Add(fileBasedTemplateDesigner);

            var radioList = TemplateDesignerDialog.FindControl("RadioList") as RadioButtonList;
            radioList.Visible = false;

        }




        #region Save Template

        private void PreSaveTemplate()
        {
            var tddType = TemplateDesignerDialog.FindControl("TemplateControl") as GeneralTemplateDesigner;

            AnyVerification(tddType);

            if (tddType != null)
                if (ViewState["saveTemplateAs"] == null || Convert.ToBoolean(ViewState["saveTemplateAs"]) == false)
                    tddType.Save();
                else tddType.Save(true);
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            ViewState["saveTemplateAs"] = false;

            PreSaveTemplate();

            TemplateDesignerDialog.Close();
            Frame.TemplateList.DataBind();
            Frame.TemplateSavedMessageBox.Show();

            //TableTemplateDesigner table;
            //if (tddType is TableTemplateDesigner)
            //    table = tddType as TableTemplateDesigner;

            CleareDataFromViewState();
        }

        protected void SaveAsButton_Click(object sender, EventArgs e)
        {
#if alexj
            ViewState["saveTemplateAs"] = true;

            PreSaveTemplate();

            TemplateDesignerDialog.Close();
            Frame.TemplateList.DataBind();
            Frame.TemplateSavedMessageBox.Show();

            CleareDataFromViewState();

#else
            PreSaveTemplate();

            Frame.TemplateList.DataBind();

#endif
#if alexj1
            TemplateDesignerDialog.Close();
            TemplateSavedMessageBox.Show();
#endif
        }

        #endregion

        /// <summary>
        /// Валидация при сохранении разных типов шаблонов
        /// </summary>
        /// <param name="tddType">общий объект</param>
        private void AnyVerification(GeneralTemplateDesigner tddType)
        {
            #region  Костыль-проверка на наличие только агрегации в уровне

            if (tddType.TemplateTypeCode.ToLower() == ("screenTree").ToLower())
            {
                var tdd = tddType as TreeBasedTemplateDesigner;
                TreeBasedTemplateDesigner.CheckFieldsWithAggrIntoLevel(tdd.Template);
            }
            #endregion

            #region Еще костыль-проверка для CrossTableRole на кол-во типов полей
            if (tddType.TemplateTypeCode.ToLower() == ("crossReport").ToLower())
            {
                var tdd = tddType as CrossReportTemplateDesigner;
                tdd.CheckerCrossTableRole();
            }
            #endregion

            #region  Еще еще костыль-проверка на наличие обязательных полей в Template
            if (tddType.TemplateTypeCode.ToLower() == ("inputexcelbased").ToLower())
            {
                var tdd = tddType as InputExcelTemplateDesigner;
                tdd.CheckTemplateForRequiredFields();
            }
            #endregion
        }

        protected void TemplateDialog_Closed(object sender, EventArgs e)
        {
            DialogCleaner(TemplateDesignerDialog);
            CleareDataFromViewState();
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

        private void CleareDataFromViewState()
        {
            ViewState["templateCode"] = null;
            // ViewState["entityID"] = null;
            ViewState["templateID"] = null;
#if alexj
            ViewState["saveTemplateAs"] = null;
#endif
        }
#endif

        #region Show preview and delete folder's
        protected void ShowPreview_Click(object sender, EventArgs e)
        {
            var directoryPath = Server.MapPath("~/Templates/temp_data/cache");

            string src = string.Empty;
            var templateID = Frame.TemplateList.SelectedDataKey["objID"].ToString();
            var templateCode = Frame.TemplateList.SelectedDataKey["code"].ToString();

            if (templateID != "0")
            {
                if (templateCode != "TableBased" && templateCode != "screentree")
                {
                    var path = string.Concat(directoryPath, "\\", templateID);

                    if (Directory.Exists(path))
                        DeleteDirectoryFiles(path);

                    var preview = new Preview(Convert.ToInt32(templateID), directoryPath, templateCode.ToLower());

                    src = preview.GetPreviewTemplate();

                    if (preview != null)
                        PreviewFrame.Attributes["src"] = src;
                }

            }
        }

        public void DeleteDirectoryFiles(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string dir in dirs)
                DeleteDirectoryFiles(dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            while (Directory.GetFiles(target_dir).Count() > 0)
            { Directory.Delete(target_dir); }

        }
        #endregion

    }

    public static class ControlExtensions
    {
        public static void ClearSelection(this ListView control)
        {
            control.SelectedIndex = -1;
        }

        public static bool HasSelection(this ListView control)
        {
            return control.SelectedIndex != -1;
        }
    }
}