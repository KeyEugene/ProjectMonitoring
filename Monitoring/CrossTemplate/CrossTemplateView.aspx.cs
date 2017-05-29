#define alexj

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Teleform.ProjectMonitoring.HttpApplication;
using Teleform.Reporting;
using Teleform.Reporting.MicrosoftOffice;
using Teleform.ProjectMonitoring.admin.SeparationOfAccessRights;
using Teleform.Reporting.Web;

namespace Teleform.ProjectMonitoring.CrossTemplate
{
    public partial class CrossTemplateView : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            var userType = Convert.ToInt32(Session["SystemUser.typeID"]);
            var userID = Convert.ToInt32(Session["SystemUser.objID"]);


            var entityID = Request.QueryString["entity"];
            var templateID = Request.QueryString["templateID"];

            if (!IsPostBack)
            {
                var permittedEntities = StorageUserObgects.Select<UserEntityPermission>(userID, userID).getReadPermittedEntities().AsEnumerable();

                var list = this.GetSchema().Entities.Where(o => !o.IsEnumeration && permittedEntities.Select(x => x["entity"].ToString()).Contains(o.SystemName)).OrderBy(o => o.Name).ToList();

                EntityList.Items.Add(new ListItem { Text = "Не выбрано", Value = "" });
                for (int i = 0; i < list.Count; i++)
                    EntityList.Items.Add(new ListItem { Text = list[i].Name, Value = list[i].ID.ToString() });

                if (entityID != null)
                {
                    EntityList.SelectedValue = entityID;

                    FillTemplateList(EntityList.SelectedValue);

                    if (templateID != null)
                    {
                        TemplateList.SelectedValue = templateID;
                        if (AuthorizationRules.TemplateResolution(ActionType.update, Session["SystemUser.objID"].ToString(), templateID))
                            ConstructorButton.Enabled = true;
                        else
                            ConstructorButton.Enabled = false;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(TemplateList.SelectedValue))
                        {
                            if (AuthorizationRules.TemplateResolution(ActionType.create, Session["SystemUser.objID"].ToString()))
                                ConstructorButton.Enabled = true;
                            else
                                ConstructorButton.Enabled = false;
                        }
                    }
                }                
                
                EntityList.DataBind();
                //VerifyRenderingInServerForm(EntityList);                
            }

            //var templatePermission = StorageUserObgects.Select<UserTemplatePermission>(userID, userID).Permission.Substring(1, 2).Contains('1');

            //if (!templatePermission && userType != 1 && userType != 0)
            //    ConstructorButton.Enabled = false;

            base.OnInit(e);
        }

      
        #region Button Event's
        protected void ReportButton_Click(object sender, EventArgs e)
        {
            if (TemplateList.SelectedIndex == 0)
                return;

            var dt = Global.GetDataTable("EXEC [report].[getCubeByTemplate] " + TemplateList.SelectedValue); //SELECT * FROM [model].[R$Template]"); 

            multiView.ActiveViewIndex = 2;
            gv.DataSource = dt;
            gv.DataBind();
        }

        protected void ConstructorButton_Click(object sender, EventArgs e)
        {
            //Не открываем конструктор если не выбран Entity
            if (string.IsNullOrEmpty(EntityList.SelectedValue))
                return;

            var userID = Convert.ToInt32(Session["SystemUser.objID"]);

        

            CrossTemplate.userID = userID;

            if (TemplateList.SelectedIndex == 0)
            {
                if (!AuthorizationRules.TemplateResolution(ActionType.create, Session["SystemUser.objID"].ToString()))
                {
                    WarningMessageBox.Show();
                    return;
                }
            }
            else
            {
                if (!AuthorizationRules.TemplateResolution(
                    ActionType.read,
                    Session["SystemUser.objID"].ToString(),
                    TemplateList.SelectedValue))
                {
                    WarningMessageBox.Show();
                    return;
                }
            }

            multiView.ActiveViewIndex = 1;
            CrossTemplate.template = null;

            if (TemplateList.SelectedValue != "-1")
                CrossTemplate.TemplateID = TemplateList.SelectedValue;

            CrossTemplate.EntityID = EntityList.SelectedValue;
            CrossTemplate.DataBind();

        }

        protected void toExcelButton_Click(object sender, EventArgs e)
        {
            if (TemplateList.SelectedIndex == 0 || TemplateList.SelectedIndex == -1)
                return;

            Teleform.Reporting.Template template = Storage.Select<Template>(TemplateList.SelectedValue);

            var dt = Global.GetDataTable("EXEC [report].[getCubeByTemplate] " + TemplateList.SelectedValue);

            string file = template.Name;

            using (var stream = new MemoryStream())
            {
                //var builder = new CrossReportExcelBuilder();
                //var builder = new ExcelReportBuilder();
                var builder = new CrossTemplateExcelBuilder();
                List<string> headerList = new List<string>();
                var groupReport = Make(template, dt, ref headerList);
                builder.HeaderList = headerList;
                builder.Create(stream, groupReport);
                Response.Clear();
                Response.ContentType = "text/html";
                Response.AddHeader("content-disposition", string.Format("attachment;fileName={0}.xlsx", file));
                Response.ContentEncoding = Encoding.UTF8;
                Response.BinaryWrite(stream.ToArray());
                Response.Flush();
                Response.End();
            }
        }
        #endregion

        //Переопределяем, потому что не подходит как со стандартными таблицами
        public GroupReport Make(Template template, DataTable table, ref List<string> headerList)
        {
            if (table.Rows.Count == 0) throw new Exception();

            int rowsCount = table.Rows.Count, colCount = table.Columns.Count;
            var instances = new Instance[rowsCount];
            DataRow row;
            var attributes = new List<Teleform.Reporting.Attribute>(colCount);

            for (int i = 0; i < colCount; i++)
            {
                var column = table.Columns[i];
                headerList.Add(column.Caption);
                var attribute = template.Fields.FirstOrDefault(x => x.Name == column.Caption);

                if (attribute == null)
                    attribute = template.Fields.First(x => x.CrossTableRoleID == 3);

                attributes.Add(attribute.Attribute);
            }

            for (int i = 0; i < instances.Length; i++)
            {
                row = table.Rows[i];

                var properties = new List<Instance.Property>();

                for (int j = 0; j < colCount; j++)
                    if (attributes[j] != null)
                    {
                        properties.Add(new Instance.Property(attributes[j], row[j]));
                    }

                instances[i] = new Instance(template.Entity, properties);
            }

            return new Teleform.Reporting.GroupReport(template, instances);
        }


        protected void multiView_ActiveViewChanged(object sender, EventArgs e)
        {
            if (multiView.ActiveViewIndex == 0) //Main View , Images
            {
                VisibleElements(true);
                if (EntityList.SelectedValue != "")
                    EntityList_OnSelectedIndex(this, EventArgs.Empty);
            }
            else if (multiView.ActiveViewIndex == 1) // View Constructor
            {
                VisibleElements(false);
            }
            else if (multiView.ActiveViewIndex == 2) // View Report
            {
                VisibleElements(true);
            }
        }

        private void VisibleElements(bool p)
        {
            ReportButton.Visible = toExcelButton.Visible = ConstructorButton.Visible = EntityList.Visible = MainEntityLiteral.Visible = p;
            //TemplateList.AutoPostBack = !p;
        }


        #region Work with DropDownList's
        private void FillTemplateList(string entityID)
        {

#if alexj
            var query = string.Format(@"SELECT p.[objID], p.[name] FROM [Permission].[IUTemplatePermission]({0}) p, model.BTables t
                                        where t.[name] = p.[baseTable] and t.[object_ID] = {1} and p.[typeCode] like 'crossReport' and p.[read] = 'True'
                                        order by p.[name]", Session["SystemUser.objID"].ToString(), entityID);
#else
            var query = string.Format(@"SELECT [RT].[objID], [RT].[name] FROM [model].[R$Template] [RT] 
                                          right JOIN [model].[R$TemplateType] [RTT] ON [RTT].[objID] = [RT].[typeID]
                                          WHERE [RTT].[code] like 'crossReport' AND [RT].[entityID] = '{0}'", entityID);
#endif



            var dt = Storage.GetDataTable(query);

            TemplateList.Items.Clear();

            TemplateList.Items.Add(new ListItem { Text = "Создать новый", Value = "-1" });

            foreach (DataRow item in dt.Rows)
                TemplateList.Items.Add(new ListItem { Value = item[0].ToString(), Text = item[1].ToString() });

            TemplateList.DataBind();
        }



        protected void EntityList_OnSelectedIndex(object sender, EventArgs e)
        {
            if (EntityList.SelectedIndex == 0)
            {
                TemplateList.Items.Add(new ListItem { Text = "Не выбрано", Value = "-1" });
                TemplateList.DataBind();
            }
            else
                FillTemplateList(EntityList.SelectedValue);
            multiView.ActiveViewIndex = 0;

        }

        protected void TemplateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var query = string.Format("SELECT * FROM Permission.UserPermissionForObject ({0}, 'R$Template', {1})",
            //                           Session["SystemUser.objID"].ToString(), TemplateList.SelectedValue);

            //var dt = Storage.GetDataTable(query);

            if (string.IsNullOrEmpty(TemplateList.SelectedValue))
            {
                var createPermission = AuthorizationRules.TemplateResolution(ActionType.create, Session["SystemUser.objID"].ToString());
                if (createPermission)
                {
                    ConstructorButton.Enabled = true;
                    if (multiView.ActiveViewIndex == 1)
                    {
                        ConstructorButton_Click(null, EventArgs.Empty);
                    }
                }
                else
                {
                    ConstructorButton.Enabled = false;
                    multiView.ActiveViewIndex = 0;
                }

                return;
            }

            var updatePermission = AuthorizationRules.TemplateResolution(ActionType.update, Session["SystemUser.objID"].ToString(), TemplateList.SelectedValue);

            if (updatePermission)
            {
                ConstructorButton.Enabled = true;

                if (multiView.ActiveViewIndex == 1)
                {
                    ConstructorButton_Click(null, EventArgs.Empty);
                }
            }
            else
            {
                ConstructorButton.Enabled = false;
                multiView.ActiveViewIndex = 0;
            }


            //ConstructorButton_Click(null, EventArgs.Empty);
        }

        #endregion
    }
}