using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Teleform.ProjectMonitoring.admin.SeparationOfAccessRights;
using Teleform.Reporting;

namespace Teleform.ProjectMonitoring
{
    public partial class ReportView
    {
        /// <summary>
        /// Свойство для того чтобы запоминать изменял ли пользователь название шаблона(это нужно чтобы каждый раз,
        /// после завершения работы с конструктором, не запускать метод FillDropDownList())
        /// </summary>
        public string oldTemplateName
        {
            get { return ViewState["oldName"] == null ? null : ViewState["oldName"].ToString(); }
            set { ViewState["oldName"] = value; }
        }

        protected void CloseButtonClick_Click(object sender, EventArgs e)
        {
            VisibleSomeElements(true);
            ReportMultiView.SetActiveView(TemplateView);

            if (TemplateDesigner.template != null)
                if (TemplateDesigner.template.Name != oldTemplateName)
                    FillDropDownList();

            TemplateDesigner.template = null; // не засоряем ViewState
            oldTemplateName = null;

            if (TemplateDesigner.TemplateID != null)
                GetInstanceList();
        }

        protected void TemplateConstructorButton_Load(object sender, EventArgs e)
        {
            var userID = Session["SystemUser.objID"].ToString();
            var templateID = Frame.TemplateList.SelectedValue.ToString();

            if (templateID.Contains("AttributesTemplate"))
                Frame.TemplateConstructorButton.Enabled = false;
            else
            {
#if true

                if (!string.IsNullOrEmpty(templateID))
                {
                    var isUpdate = AuthorizationRules.TemplateResolution(ActionType.update, userID, templateID);

                    if (isUpdate)
                        Frame.TemplateConstructorButton.Enabled = true;
                    else
                        Frame.TemplateConstructorButton.Enabled = false;
                }

#else
                var query = string.Format("SELECT [update] FROM [Permission].[IUTemplatePermission]({0}) where objID = {1}", Session["SystemUser.objID"].ToString(), Frame.TemplateList.SelectedValue);
                
                var dt = Storage.GetDataTable(query);

                if (dt.Rows.Count > 0)
                {
                    bool result = false;
                    Boolean.TryParse(dt.Rows[0][0].ToString(), out result);

                    if (!result)
                        Frame.TemplateConstructorButton.Enabled = false;
                    else
                        Frame.TemplateConstructorButton.Enabled = true;
                }
                else
                    Frame.TemplateConstructorButton.Enabled = false;
#endif
            }
        }

        protected void TemplateConstructorButton_Click(object sender, EventArgs e)
        {
            if (Frame.TemplateList.SelectedIndex == 0 || Frame.TemplateList.SelectedIndex == 1)
            {
                if (!AuthorizationRules.TemplateResolution(ActionType.create, Session["SystemUser.objID"].ToString()))
                {
                    WarningMessageBoxAuthorization.Show();
                    return;
                }
            }
            else
            {
                if (!AuthorizationRules.TemplateResolution(ActionType.read,
                    Session["SystemUser.objID"].ToString(),
                    Frame.TemplateList.SelectedValue))
                {
                    WarningMessageBoxAuthorization.Show();
                    return;
                }
            }

            if (TemplateDesigner.template != null)
                oldTemplateName = TemplateDesigner.template.Name;

            TemplateDesigner.IsNotShowThis = false;
            TemplateDesigner.TemplateID = Frame.TemplateList.SelectedValue;
            TemplateDesigner.EntityID = Request["entity"];
            TemplateDesigner.userID = Convert.ToInt32(Session["SystemUser.objID"]);
            TemplateDesigner.DataBind();

            ReportMultiView.SetActiveView(TemplateDesignerView);

            VisibleSomeElements(false);
        }

        private void VisibleSomeElements(bool p)
        {
            var reportView = this.Parent;

            var entityList = reportView.FindControl("EntityList") as DropDownList;
            var entitiListLabel = reportView.FindControl("EntitiListLabel") as Label;

            if (entityList != null && entitiListLabel != null)
            {
                entityList.Visible = entitiListLabel.Visible = p;
            }

            Frame.GoToFilterDesignerButton.Visible = Frame.FilterList.Visible = p;
        }

    }
}