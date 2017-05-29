
#define alexj

using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Teleform.Reporting;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Reflection;
using Teleform.ProjectMonitoring.admin.SeparationOfAccessRights;

namespace Teleform.ProjectMonitoring.Templates
{
    public partial class Designer : CompositeControl
    {

        public string EntityID { get; set; }

        /// <summary>
        /// Список атрибутов
        /// </summary>
        public ListBox AttributeListBox { get; private set; }

        /// <summary>
        /// Список атрибутов которые нужно исключить из AttributeListBox
        /// </summary>
        public List<string> FilterAttributeIDList { get; set; }

        /// <summary>
        /// Контрол для фильтрации списка атрибутов в AttributeListBox
        /// </summary>
        private TextBox AttributeFilterBox;

        /// <summary>
        /// ID пользователя, вызвавшего дизайнер
        /// </summary>
        public int userID;
        

        public Style AttributeBoxStyle
        {
            get
            {
                return ViewState["AttributeBoxStyle"] == null ? null : (Style)ViewState["AttributeBoxStyle"];
            }
            set
            {
                ViewState["AttributeBoxStyle"] = value;
            }
        }

        public bool isFromAdministration
        {
            get
            {
                var o = ViewState["isFromAdmin"];
                if (o != null)
                    return (bool)ViewState["isFromAdmin"];
                else return false;
            }
            set { ViewState["isFromAdmin"] = value; }
        }

        public bool isInputExcelBased
        {
            get
            {
                var o = ViewState["InputExcelBased"];
                if (o != null)
                    return (bool)ViewState["InputExcelBased"];
                else return false;
            }
            set { ViewState["InputExcelBased"] = value; }
        }

        private Entity entity
        {
            get { return ViewState["_Entity"] as Entity; }
            set
            {
                ViewState["_Entity"] = value;
            }
        }

        private Entity Entity
        {
            get
            {
                try
                {
                    if (entity == null)
                        if (!string.IsNullOrEmpty(EntityID))
                            entity = Storage.Select<Entity>(EntityID);

                    return entity;
                }
                catch
                {
                    return null;
                }
            }
        }

        protected override void CreateChildControls()
        {
#if true
            var label = new Label { ID = "FilterLabel", Text = "Фильтр:" };
            this.Controls.Add(label);

            AttributeListBox = new ListBox()
            {
                SelectionMode = ListSelectionMode.Multiple,
                Width = 450,
                Height = Unit.Pixel(450),
                ID = "AttributeListBox",
                EnableViewState = true
            };

#if alexj
            AttributeListBox.Attributes.Add("style", "overflow-x:auto;");
#endif


            if (isFromAdministration == true)
                AttributeListBox.Height = Unit.Point(200);

            AttributeFilterBox = new TextBox { ID = "AttributeFilterBox", AutoPostBack = false, EnableViewState = true, ViewStateMode = System.Web.UI.ViewStateMode.Enabled };
            //AttributeFilterBox.TextChanged += new System.EventHandler(AttributeFilterBox_TextChanged);
            AttributeFilterBox.Attributes["onkeyup"] = string.Format("keyup_handlerFilterControl(this, '{0}')", AttributeListBox.ClientID);

            if (isFromAdministration == true)
                AttributeListBox.DataSource = GetAttributesForAdministration();
            else if (isInputExcelBased)
                AttributeListBox.DataSource = GetAttributesForInputExcelBased();
            else
                AttributeListBox.DataSource = GetAttributes();


            AttributeListBox.DataTextField = "name";
            AttributeListBox.DataValueField = "id";
            AttributeListBox.DataBind();

#if alexj
            //Делаем что бы выплывали подсказки при наведении на атрибут в AttributeListBox
            foreach (ListItem item in AttributeListBox.Items)
                item.Attributes["title"] = item.Text;

#endif

#if alexj
            //Делаем что бы показывало только разрешенные Entity

#endif

            this.Controls.Add(AttributeFilterBox);

            var br = new HtmlGenericControl("br");
            this.Controls.Add(br);

            if (AttributeBoxStyle != null)
                AttributeListBox.ApplyStyle(AttributeBoxStyle);


            this.Controls.Add(AttributeListBox);

#else
            
            
            this.Controls.Clear();
            var c = ScriptManager.GetCurrent(this.Page);
            var col = c.Controls;
            
            var filterPanel = new UpdatePanel { ID = "FilterPanel", UpdateMode = UpdatePanelUpdateMode.Conditional, ChildrenAsTriggers = false };
            

            var label = new Label { ID = "FilterLabel", Text = "Фильтр:" };
            filterPanel.ContentTemplateContainer.Controls.Add(label);

            AttributeFilterBox = new TextBox { ID = "AttributeFilterBox", AutoPostBack = true, EnableViewState = true, ViewStateMode = System.Web.UI.ViewStateMode.Enabled };
            AttributeFilterBox.TextChanged += new System.EventHandler(AttributeFilterBox_TextChanged);
            AttributeFilterBox.Attributes["onkeyup"] = "keyup_handler2(this)";
            filterPanel.ContentTemplateContainer.Controls.Add(AttributeFilterBox);
            this.Controls.Add(filterPanel);


            var br = new HtmlGenericControl("br");
            this.Controls.Add(br);

            var attributePanel = new UpdatePanel { ID = "AttributePanel" };
            AttributeBox = new ListBox() { SelectionMode = ListSelectionMode.Multiple, Width = 450, ID = "AttributeListBox", Height = Unit.Point(450), EnableViewState = true };
            AttributeBox.DataSource = GetAttributes();
            AttributeBox.DataTextField = "name";
            AttributeBox.DataValueField = "id";
            AttributeBox.DataBind();
            attributePanel.ContentTemplateContainer.Controls.Add(AttributeBox);

            this.Controls.Add(attributePanel);
#endif
        }


        private void ScriptRegister()
        {
            var scriptName = "keyUpHandler";

            if (!this.Page.ClientScript.IsClientScriptBlockRegistered(scriptName))
            {
                var sb = new StringBuilder();
                sb.Append("\n<script type=\"text/javascript\">\n");

                sb.Append("var __control, __t;\n");

                sb.Append("$(document).ready(function () {\n");
                sb.Append("PredicateDesignerExecutor();\n");
                sb.Append("});\n");

                sb.Append("function update2() { __control.onchange() }\n");

                sb.Append("function keyup_handler2(o) {\n");
                sb.Append("if (__t != undefined) {\n");
                sb.Append("clearTimeout(__t);\n");
                sb.Append(" __t = undefined}\n");
                sb.Append("__control = o;\n");
                sb.Append("__t = setTimeout(update2, 150)}\n");

                sb.Append("</script>");

                this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), scriptName, sb.ToString());
            }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            AttributeFilterBox.Attributes["onkeyup"] = string.Format("keyup_handlerFilterControl(this, '{0}')", AttributeListBox.ClientID);
            base.Render(writer);
        }



        private IEnumerable<Teleform.Reporting.Attribute> GetAttributesForAdministration()
        {
            IEnumerable<Teleform.Reporting.Attribute> filterList;
            if (FilterAttributeIDList != null)
                filterList = Entity.Attributes.Where(o => !FilterAttributeIDList.Contains(o.ID) && !o.IsListAttribute); //&& o.FPath.IndexOf('/') == -1
            else
                filterList = Entity.Attributes; //.Where(x => x.FPath.IndexOf('/') == -1);

            if (string.IsNullOrEmpty(AttributeFilterBox.Text))
                return filterList.Where(o => !o.Name.ToLower().Contains("objid") && !o.Name.ToLower().Contains("parentid"));
            else
                return filterList.Where(o => !o.Name.ToLower().Contains("objid") && !o.Name.ToLower().Contains("parentid") && o.Name.ToLower().Contains(AttributeFilterBox.Text.ToLower()));
        }


#if true // Alex
        private object GetAttributesForInputExcelBased()
        {
            IEnumerable<Teleform.Reporting.Attribute> filterList = new List<Teleform.Reporting.Attribute>();

            if (FilterAttributeIDList != null)
                //делает выборку, если  добавленные атрибуты нужно убрать из списка в дизайнере
                filterList = Entity.Attributes.Where(o => o.AppType == AppType.parentid && !FilterAttributeIDList.Contains(o.ID) && !o.IsListAttribute && !o.FPath.Contains("/") && o.Type.Name != "Table").AsEnumerable();
            else
                filterList = Entity.Attributes.Where(x => !x.FPath.Contains("/") && x.Type.Name != "Table").AsEnumerable();

            var constraint = entity.Constraints.ToArray();

            List<Teleform.Reporting.Attribute> list = new List<Teleform.Reporting.Attribute>();
            for (int i = 0; i < constraint.Count(); i++)
            {
                var attribute = entity.Attributes.FirstOrDefault(x => x.FPath == (constraint[i].ConstraintName + "/objID")); // && !FilterAttributeIDList.Contains(x.ID));
                if (attribute != null)
                    list.Add(attribute);
            }

            //Добавляем ссылку на себя (Table_Table) к списку полей
            if (entity.IsHierarchic)
                list.Add(entity.Attributes.FirstOrDefault(x => x.FPath == (constraint.FirstOrDefault(o => o.RefTblName == entity.SystemName).ConstraintName + "/name")));

            filterList = filterList.Concat(list.AsEnumerable());

            return filterList.Where(o => o.Name.ToLower() != "objid");
        }
#endif
        private IEnumerable<Teleform.Reporting.Attribute> GetAttributes()
        {
            IEnumerable<Teleform.Reporting.Attribute> FilteredAttrsList;

            if (FilterAttributeIDList != null)
                FilteredAttrsList = Entity.Attributes.Where(o => !FilterAttributeIDList.Contains(o.ID));
            else
                FilteredAttrsList = Entity.Attributes;

            if (userID != null)
            {
                var PermittedEntitieNamesList = StorageUserObgects.Select<UserEntityPermission>(userID, userID).getReadPermittedEntities().AsEnumerable().Select(x => x["entityID"].ToString()).ToList<string>();

                foreach (var item in PermittedEntitieNamesList)
                {
                    if (item == "202535855")
                    {
                        
                    }
                }


                FilteredAttrsList = FilteredAttrsList.Where(x => PermittedEntitieNamesList.Contains(x.EntityID.ToString()));

                foreach (var item in FilteredAttrsList)
                {
                    if (item.EntityID == "202535855")
                    {

                    }

                }

            }

            if (string.IsNullOrEmpty(AttributeFilterBox.Text))
                return FilteredAttrsList.Where(o => !o.Name.ToLower().Contains("objid") && !o.Name.ToLower().Contains("parentid"));
            else
                return FilteredAttrsList.Where(o => !o.Name.ToLower().Contains("objid") && !o.Name.ToLower().Contains("parentid") && o.Name.ToLower().Contains(AttributeFilterBox.Text.ToLower()));
        }

        void AttributeFilterBox_TextChanged(object sender, EventArgs e)
        {
            // AttributeBox.DataBind();
        }
    }


}
