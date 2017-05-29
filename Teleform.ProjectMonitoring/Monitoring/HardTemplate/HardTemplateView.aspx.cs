#define Alexj
#define Viktor

using System;
using System.Linq;
using System.Web.UI.WebControls;
//using Teleform.ProjectMonitoring.admin.SeparationOfAccessRights;

namespace Teleform.ProjectMonitoring.HardTemplate
{
    using Teleform.Reporting;
    using System.Data.SqlClient;
    using Teleform.ProjectMonitoring.HttpApplication;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Collections;
    using Teleform.ProjectMonitoring.HardTemplate.Type_report.Children;
    using Teleform.ProjectMonitoring.HardTemplate.Type_report;
    using System.Collections.Generic;
    using TreeType = Teleform.Reporting.Reporting.Template.EnumTreeType;
    using Teleform.ProjectMonitoring.admin.SeparationOfAccessRights;


    public partial class HardTemplateView : BasePage
    {
                

        public Template template { get; private set; }

        //[Obsolete("Временно public")]
        public DynamicQueryForHeardTemplate dynamicQueryForGeneral;
        public Dynamic_Query_For_Heard_Template_Type_Children dynamicQueryForChildren;

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
                            ButtonConsctuctor.Enabled = true;
                        else
                            ButtonConsctuctor.Enabled = false;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(TemplateList.SelectedValue))
                        {
                            if (AuthorizationRules.TemplateResolution(ActionType.create, Session["SystemUser.objID"].ToString()))
                                ButtonConsctuctor.Enabled = true;
                            else
                                ButtonConsctuctor.Enabled = false;
                        }
                    }
                }

                EntityList.DataBind();
                //VerifyRenderingInServerForm(EntityList);
            }


            //var templatePermission = StorageUserObgects.Select<UserTemplatePermission>(userID, userID).Permission.Substring(1, 2).Contains('1');

            //if (!templatePermission && userType != 1 && userType != 0)
            //    ButtonConsctuctor.Enabled = false;


            base.OnInit(e);
        }

     


        #region Show contructor and show report

        protected void BtnConstructor_OnClick(object sender, EventArgs e)
        {
            var templateID = TemplateList.SelectedValue;

            treeDesigner.userID = Convert.ToInt32(Session["SystemUser.objID"]);

            if (templateID == "0" || string.IsNullOrEmpty(templateID))
                return;

            if (templateID == "-1")
            {
                if (!AuthorizationRules.TemplateResolution(ActionType.create, Session["SystemUser.objID"].ToString()))
                {
                    WarningMessageBox.Show();
                    return;
                }

                treeDesigner.TemplateID = null;
                treeDesigner.templateIsNew = true;
                treeDesigner.SortHashtable = null;
            }
            else
            {
                if (!AuthorizationRules.TemplateResolution(ActionType.read, Session["SystemUser.objID"].ToString(), templateID))
                {
                    WarningMessageBox.Show();
                    return;
                }

                treeDesigner.TemplateID = templateID;
                treeDesigner.SortHashtable = Session["SortTemplate" + templateID] == null
                    ? new Hashtable()
                    : (Hashtable)Session["SortTemplate" + templateID];
            }

            treeDesigner.EntityID = EntityList.SelectedItem.Value;

            treeDesigner.template = null;
            treeDesigner.selectedID = treeDesigner.buttonAttributID = null;
            treeDesigner.isDialogShow = false;

            treeDesigner.DataBind();

            MainView.ActiveViewIndex = 1;
        }

        protected void ShowTemplateView_OnClick(object sender, EventArgs e)
        {
            string query = string.Empty; var templateID = TemplateList.SelectedValue;

            if (String.IsNullOrEmpty(templateID) || templateID == "-1")
                return;

            template = Storage.Select<Template>(templateID);


            var userID = Convert.ToInt32(Session["SystemUser.objID"]);
            UserTemlatePermission.SetFieldsTaboo(userID, template);

            if (template.TreeTypeEnum == TreeType.Undefined || template.TreeTypeEnum == TreeType.General)
            {
                query = CreateObjectDynamicQuery(string.Format(@"declare @sql varchar(max)= '' 
                                    EXEC [report].[getBObjectData]  null, {0}, @cyr = 1, @select=@sql out
                                    select @sql", template.ID));

                dynamicQueryForGeneral = new DynamicQueryForHeardTemplate(query, template,
                            Session["SortTemplate" + template.ID] == null
                            ? new Hashtable()
                            : (Hashtable)Session["SortTemplate" + template.ID]);

                StartBuildTreeView();
            }
            else if (template.TreeTypeEnum == TreeType.Children) //template.Entity.IsHierarchic && 
            {
                string systemName = template.Entity.IsHierarchic ? template.Entity.SystemName : GetHierarchicEntityName(template);

                query = CreateObjectDynamicQuery(string.Format(@"declare @sql varchar(max)= '' 
                                    EXEC [report].[getBObjectData]  null, {0}, @cyr = 1, @condition ='{1}.parentID is null', @select=@sql out
                                    select @sql", template.ID, systemName));

                dynamicQueryForChildren = new Dynamic_Query_For_Heard_Template_Type_Children(query, template,
                            Session["SortTemplate" + template.ID] == null ? new Hashtable() : (Hashtable)Session["SortTemplate" + template.ID]);
                          
                StartBuildTreeView_Children();
            }
            else if (template.Entity.IsHierarchic && template.TreeTypeEnum == TreeType.Branch)
            {

            }

        }

        private string GetHierarchicEntityName(Template template)
        {
            var hierarchicList = GetHierarchicList();

            for (int i = 0; i < template.Fields.Count; i++)
                for (int j = 0; j < hierarchicList.Count; j++)
                    if (template.Fields[i].Attribute.FPath.Contains(hierarchicList.ElementAt(j).Key))
                        return hierarchicList.ElementAt(j).Value;

            return null;
        }

        private Dictionary<string, string> GetHierarchicList()
        {
            var list = new Dictionary<string, string>();
            var dt = Global.GetDataTable(@"select pc.[parent], pc.[const],  isNULL(b.isHierarchic,0) isHierarchicParent from [model].[vo_ParentChildEntity] pc 
                                                    join model.BTables b on b.name=pc.parent where b.isHierarchic != 0");

            for (int i = 0; i < dt.Rows.Count; i++)
                list.Add(dt.Rows[i]["const"].ToString(), dt.Rows[i]["parent"].ToString());

            return list;
        }

        public string CreateObjectDynamicQuery(string query)
        {
            using (var conn = new SqlConnection(Global.ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                var read = cmd.ExecuteReader();

                if (read.HasRows)
                    while (read.Read())
                        return read.GetString(0);
            }

            return "";
        }

        #region Alexj
        protected void toExcelButton_Click(object sender, EventArgs e)
        {
            var templateID = TemplateList.SelectedValue;
            if (string.IsNullOrEmpty(templateID))
                return;
            //throw new InvalidOperationException("Не выбран шаблон! Невозможно произвести экспорт в Excel.");

            Template template = Storage.Select<Template>(templateID);
            string fileName = template.FileName;


            using (var stream = new MemoryStream())
            {
                var builder = new HardReportExcelBuilder();


                //ToDo: Доделать
                if (template.TreeTypeEnum == Reporting.Reporting.Template.EnumTreeType.Undefined || template.TreeTypeEnum == Reporting.Reporting.Template.EnumTreeType.General)
                {
                    var query = string.Format(@"declare @sql varchar(max)= '' 
                                    EXEC [report].[getBObjectData]  null, {0}, @cyr = 1, @select=@sql out
                                    select @sql", template.ID);

                    query = CreateObjectDynamicQuery(query);
                    dynamicQueryForGeneral = new DynamicQueryForHeardTemplate(query, template,
                                Session["SortTemplate" + template.ID] == null ? new Hashtable() : (Hashtable)Session["SortTemplate" + template.ID]);

                    builder.dynamicQuery = dynamicQueryForGeneral;
                }
                else if (template.Entity.IsHierarchic && template.TreeTypeEnum == Reporting.Reporting.Template.EnumTreeType.Children)
                {
                    string systemName = "";
                    if (template.Entity.IsHierarchic)
                        systemName = template.Entity.SystemName;
                    else
                        systemName = GetHierarchicEntityName(template);

                    var query = string.Format(@"declare @sql varchar(max)= '' 
                                    EXEC [report].[getBObjectData]  null, {0}, @cyr = 1, @condition ='{1}.parentID is null', @select=@sql out
                                    select @sql", template.ID, systemName);

                    query = CreateObjectDynamicQuery(query);

                    dynamicQueryForChildren = new Dynamic_Query_For_Heard_Template_Type_Children(query, template,
                                Session["SortTemplate" + template.ID] == null ? new Hashtable() : (Hashtable)Session["SortTemplate" + template.ID]);

                    #region extract 'where jnt0.parentID is null'
                    // var whereParentID = query.Substring(query.LastIndexOf("where"), query.Count() - query.LastIndexOf("where"));
                    // query = query.Remove(query.LastIndexOf("where"), query.Count() - query.LastIndexOf("where"));
                    // dynamicQueryForChildren.whereParentID = whereParentID.Remove(whereParentID.LastIndexOf("is"), whereParentID.Count() - whereParentID.LastIndexOf("is"));
                    #endregion

                    builder.dynamicQueryChildren = dynamicQueryForChildren;
                }
                else if (template.Entity.IsHierarchic && template.TreeTypeEnum == Reporting.Reporting.Template.EnumTreeType.Branch)
                {

                }


                builder.template = template;

                builder.Create(stream, template);

                Response.Clear();
                Response.ContentType = "text/html";
                Response.AddHeader("content-disposition", string.Format("attachment;fileName={0}.xlsx", fileName));
                Response.ContentEncoding = Encoding.UTF8;
                Response.BinaryWrite(stream.ToArray());
                Response.Flush();
                Response.End();
            }

        }

        #endregion

        #endregion

        #region Work with DropDownList's

        private void FillTemplateList(string entityID)
        {

#if Alexj

           

            var query = string.Format(@"SELECT p.[objID], p.[name] FROM [Permission].[IUTemplatePermission]({0}) p, model.BTables t
                                        where t.[name] = p.[baseTable] and t.[object_ID] = {1} and p.[typeCode] like 'screenTree' and p.[read] = 'True'
                                        order by p.[name]", Session["SystemUser.objID"].ToString(), entityID);
#else
            var query = string.Format(@"SELECT [RT].[objID], [RT].[name] FROM [model].[R$Template] [RT] 
                                          right JOIN [model].[R$TemplateType] [RTT] ON [RTT].[objID] = [RT].[typeID]
                                          WHERE [RTT].[code] like 'screenTree' AND [RT].[entityID] = '{0}'", entityID);
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

            MainView.ActiveViewIndex = 0;
        }


        private void SelectIndexTemplateList(string name)
        {
            if (!string.IsNullOrEmpty(EntityList.SelectedValue))
                EntityList_OnSelectedIndex(null, EventArgs.Empty);

            for (int i = 0; i < TemplateList.Items.Count; i++)
            {
                if (TemplateList.Items[i].Text == name)
                {
                    TemplateList.Items[i].Selected = true;
                    return;
                }
            }
        }

        protected void TemplateList_SelectedIndexChanged(object sender, EventArgs e)
        {

            var updatePermission = AuthorizationRules.TemplateResolution(ActionType.update, Session["SystemUser.objID"].ToString(), TemplateList.SelectedValue);


            //var query = string.Format("SELECT * FROM Permission.UserPermissionForObject ({0}, 'R$Template', {1})",
            //                           Session["SystemUser.objID"].ToString(), TemplateList.SelectedValue);

            //var dt = Storage.GetDataTable(query);

            if (string.IsNullOrEmpty(TemplateList.SelectedValue))
            {
                var createPermission = AuthorizationRules.TemplateResolution(ActionType.create, Session["SystemUser.objID"].ToString());
                if (createPermission)
                {
                    ButtonConsctuctor.Enabled = true;
                    if (MainView.ActiveViewIndex == 1)
                    {
                        BtnConstructor_OnClick(null, EventArgs.Empty);
                    }
                }
                else
                {
                    ButtonConsctuctor.Enabled = false;
                    MainView.ActiveViewIndex = 0;
                }

                return;
            }

            if (updatePermission)
            {
                ButtonConsctuctor.Enabled = true;

                if (MainView.ActiveViewIndex == 1)
                {
                    BtnConstructor_OnClick(null, EventArgs.Empty);
                }
            }
            else
            {
                ButtonConsctuctor.Enabled = false;
                MainView.ActiveViewIndex = 0;
            }

            //if (MainView.ActiveViewIndex == 1)
            //    BtnConstructor_OnClick(null, EventArgs.Empty);

        }


        #endregion

        protected void Multiview_AciveViewChanged(object sender, EventArgs e)
        {
            if (MainView.ActiveViewIndex == 0)
            {
                ChangeVisibleElements(true);

                if (treeDesigner.template != null)
                {
                    SelectIndexTemplateList(treeDesigner.template.Name);
                    Session["SortTemplate" + treeDesigner.template.ID] = treeDesigner.SortHashtable;
                    treeDesigner.SortHashtable = null;
                }

            }
            else if (MainView.ActiveViewIndex == 1)
            {
                ChangeVisibleElements(false);
            }
        }

        private void ChangeVisibleElements(bool p)
        {
            MainEntityLiteral.Visible = EntityList.Visible = ButtonConsctuctor.Visible = ButtonShowTemplate.Visible = toExcelButton.Visible = p;
            //TemplateList.AutoPostBack = !p;
        }

    }
}