#define REFACTORING
#define Alex
#define FilterListAddItem
#define XXX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Teleform.ProjectMonitoring
{
    using Reporting;
    using Reporting.Web;
    using System.Xml.Linq;
    using System.IO;
    using Teleform.ProjectMonitoring.HttpApplication;

    public partial class FilterDesigner : System.Web.UI.UserControl
    {        

        private bool save = true;
        private string attributeFilter;

        private bool isResetFiltersButton_Click;

        Entity entity
        {
            get { return Session["_FilterDesignerEntity"] as Entity; }
            set { Session["_FilterDesignerEntity"] = value; }
        }

        public Entity Entity
        {
            get
            {
                try
                {
                    entity = Storage.Select<Entity>(Request.QueryString["entity"]);
                    return entity;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Возвращает или задаёт идентификатор редактируемого фильтра типа.
        /// </summary>
        public object EntityFilterID
        {
            get
            {
                return ViewState["entityFilterID"] == null ? -1 : (int)ViewState["entityFilterID"];
            }
            set
            {
                ViewState["entityFilterID"] = value;

                entityFilter = null;

                AttributeList.DataBind();

                FieldList.Items.Clear();

                FieldList.DataBind();
            }
        }

        EntityFilter entityFilter
        {
            get { return Session["entityFilter"] as EntityFilter; }
            set { Session["entityFilter"] = value; }
        }

        public EntityFilter EntityFilter
        {
            get
            {
                try
                {
                    var o = this.GetSchema();

                    if (entityFilter == null)
                        entityFilter = Storage.Select<EntityFilter>(EntityFilterID);

                    return entityFilter;
                }
                catch
                {
                    return null;
                }
            }
        }

        protected void AttributeFilterBox_TextChanged(object sender, EventArgs e)
        {
            attributeFilter = AttributeFilterBox.Text;
            AttributeList.DataBind();
        }

        protected void AttributeSource_OnSelecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var entity = Entity;
            if (entity != null)
            {
                if (string.IsNullOrEmpty(attributeFilter))
                    e.Result = entity.Attributes.Where(a => !a.Name.ToLower().Contains("objid"));
                else
                    e.Result = entity.Attributes.Where(a => !a.Name.ToLower().Contains("objid") && a.Name.ToLower().Contains(attributeFilter.ToLower()));
            }
            else
                e.Cancel = true;
        }


        /// <summary>
        /// Происходит после окончания работы с конструктором.
        /// </summary>
        public event EventHandler DesigningFinished;

        protected void BackwardButton_Click(object sender, EventArgs e)
        {
            entity = null;
            if (DesigningFinished != null)
                DesigningFinished(this, EventArgs.Empty);
        }

        public void ErrorsChecking(string name, bool isFilterCheckedByDB)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Поле имя фильтра обязательно для заполнения.");

            if (isFilterCheckedByDB == false)
            {
                var message1 = string.Empty;
                message1 = string.Concat("Фильтр с таким именем уже есть ", message1);
                message1 = message1.Remove(message1.Length - 1);
                throw new Exception(message1);

            }

        }

        private void childControls_FilterApplied(object sender, EventArgs e)
        {
            if (sender is IFilterControl)
            {
                foreach (var field in EntityFilter.Fields)
                {
                    var i = field.Sequence;

                    var userPredicateBox = FieldList.Items[i].FindControl("UserPredicateBox") as TextBox;
                    //var techPredicateBox = FieldList.Items[i].FindControl("TechPredicateBox") as TextBox;
                    var filterControl = FieldList.Items[i].FindControl("CompositePredicateControl") as CompositePredicateControl;

                    userPredicateBox.Text = filterControl.UserPredicate;
                   // techPredicateBox.Text = filterControl.TechPredicate;

                }
            }

        }

        protected void ResetFiltersButton_Click(object sender, EventArgs e)
        {
            isResetFiltersButton_Click = true;

            if (EntityFilter != null)
            {
                foreach (EntityFilterField field in EntityFilter.Fields)
                {
                    var i = field.Sequence;
                    var filterControl = FieldList.Items[i].FindControl("CompositePredicateControl") as CompositePredicateControl;
                    filterControl.RejectFilter();

                    var userPredicateBox = FieldList.Items[i].FindControl("UserPredicateBox") as TextBox;
                    userPredicateBox.Text = null;
                }
            }
            FieldList.DataBind();
        }

        private void SaveToCache()
        {
            foreach (var field in EntityFilter.Fields)
            {
                var i = field.Sequence;

                var filterControl = FieldList.Items[i].FindControl("CompositePredicateControl") as CompositePredicateControl;

                field.PredicateInfo = filterControl.PredicateInfo;
                field.TechPredicate = filterControl.TechPredicate;
                field.UserPredicate = filterControl.UserPredicate;
            }
        }

        protected void FieldSource_OnSelecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            if (FieldList.Items.Count > 0 && save && EntityFilter != null)
                SaveToCache();

            if (EntityFilter != null)
                e.Result = EntityFilter.Fields;
            else e.Cancel = true;
        }

        protected void FieldList_ItemCreated(object sender, ListViewItemEventArgs e)
        {
            if (EntityFilter != null)
            {
                var field = EntityFilter.Fields[e.Item.DataItemIndex];

                var filterControl = e.Item.FindControl("CompositePredicateControl") as CompositePredicateControl;

                var fsec = field.Sequence;
                var csec = filterControl.ClientID;


                filterControl.Attribute = field.Attribute;

                filterControl.AttributeID = "#a";

                filterControl.PredicateInfo = field.PredicateInfo;

                filterControl.FilterApplied += childControls_FilterApplied;

                var userPredicateBox = e.Item.FindControl("UserPredicateBox") as TextBox;
                userPredicateBox.ReadOnly = true;
                if (isResetFiltersButton_Click == true)
                    userPredicateBox.Text = null;
                else userPredicateBox.Text = field.UserPredicate;
            }
        }

        public void UpdateEntityFilterButton_Click(object sender, EventArgs e)
        {
            SaveToCache();
            ErrorsChecking(entityFilter.Name.ToString(), true);

            var xml = GenerateXml(entityFilter);
            UpdateFilter(xml);
        }
        
        protected void IncludeAttributeButton_Click(object sender, EventArgs e)
        {
            if (AttributeList.SelectedItem != null)
            {
                var items = AttributeList.Items.OfType<ListItem>().Where(o => o.Selected).Select(o => o.Value);


                var fields = EntityFilter.Entity.Attributes.Where(o => items.Contains(o.ID.ToString())).Select(a => new EntityFilterField(a));

                var index = FieldList.SelectedIndex;

                if (index == -1)
                    EntityFilter.Fields.AddRange(fields);
                else
                    EntityFilter.Fields.InsertRange(index + 1, fields);

                save = false;

                FieldList.DataBind();
            }
        }

        protected void ExcludeAttributeButton_Click(object sender, EventArgs e)
        {
            var index = FieldList.SelectedIndex;

            if (index != -1)
            {
                EntityFilter.Fields.RemoveAt(index);

                if (index == EntityFilter.Fields.Count)
                    FieldList.SelectedIndex = -1;

                save = false;

                FieldList.DataBind();
            }
        }

        protected void UpButton_Click(object sender, EventArgs e)
        {
            if (FieldList.SelectedIndex > 0)
            {
                int i = FieldList.SelectedIndex, k = i - 1;
                EntityFilter.Fields.Permute(i, k);

                var item = FieldList.Items[i];
                FieldList.Items[i] = FieldList.Items[k];
                FieldList.Items[k] = item;

                FieldList.SelectedIndex = FieldList.SelectedIndex - 1;

            }
        }

        protected void DownButton_Click(object sender, EventArgs e)
        {
            int i = FieldList.SelectedIndex, k = i + 1;

            if (k < FieldList.Items.Count)
            {
                EntityFilter.Fields.Permute(i, k);

                var item = FieldList.Items[i];
                FieldList.Items[i] = FieldList.Items[k];
                FieldList.Items[k] = item;

                FieldList.SelectedIndex = FieldList.SelectedIndex + 1;
            }
        }

        #region Create entityFilter


        protected void CopyFilterButton_Click(object sender, EventArgs e)
        {
            SaveToCache();
            CopyFilterDialog.Caption = "Создание фильтра";
            CopyFilterDialog.Show();
        }

        public void ApplyCopyFilterButton_Click(object sender, EventArgs e)
        {
            SaveToCache();

            string name = (CopyFilterDialog.FindControl("InsertNameBox") as TextBox).Text;

            ErrorsChecking(name, true);

            entityFilter.Name = name;

            var xml = GenerateXml(entityFilter);
            CreateFilter(xml);
            var filterID = GetFilterID(name, entityFilter.Entity.ID.ToString());

            CopyFilterDialog.Close();

            var entityListAttributeView = this.Parent;
            var FilterList = entityListAttributeView.FindControl("FilterList") as DropDownList;

            FilterList.Items.Clear();
            FilterList.Items.Add(new ListItem("не выбрано", ""));
            FilterList.DataBind();
            FilterList.SelectedValue = filterID;

            this.DataBind();
            this.EntityFilterID = int.Parse(filterID);

            if (EntityFilter.Name != null)
            {
                (TemplateSavedMessageBox.FindControl("LabelShowNameTemplate") as Label).Text = string.Format("Фильтр «{0}» успешно сохранён.", EntityFilter.Name);
                TemplateSavedMessageBox.Show();
            }

            if (ButtonSaveClick != null)
            {
                ButtonSaveClick(this, e);
            }


        }

        public string GetFilterID(string name, string entityID)
        {
            string filterID;
            using (var c = new SqlConnection(Kernel.ConnectionString))
            using (var cmd = new SqlCommand("SELECT [objID] FROM [model].[R$EntityFilter] WHERE [name] = @name and [entityID] = @entityID", c))
            {
                //cmd.Parameters.Add(new SqlParameter
                //{
                //    Value = name, ParameterName = "name"
                //});

                cmd.Parameters.AddRange(
                    new SqlParameter[]
                    {
                        new SqlParameter{ParameterName = "name", DbType = DbType.String, Value= name},
                        new SqlParameter{ParameterName = "entityID", DbType = DbType.String, Value= entityID}
                    });

                c.Open();
                filterID = cmd.ExecuteScalar().ToString();
            }
            return filterID;
        }

        public event EventHandler ButtonSaveClick;

        protected void CopyFilterDialogClose(object sender, EventArgs e)
        {
            (CopyFilterDialog.FindControl("InsertNameBox") as TextBox).Text = string.Empty;
        }


        

        public void UpdateFilter(XElement xml)
        {


            Storage.ExecuteNonQueryXML("EXEC [model].[EntityFilterUpdate] @xml", xml.ToString());


            //using (var c = new SqlConnection(Global.ConnectionString))
            //using (var cmd = new SqlCommand("EXEC [model].[EntityFilterUpdate] @xml", c))
            //{
            //    cmd.Parameters.Add("xml", DbType.String).Value = xml.ToString();// GenerateXml(EntityFilter).ToString();
            //    c.Open();

            //    cmd.ExecuteNonQuery();
            //}
            
            Storage.ClearInstanceCache(typeof(EntityFilter), EntityFilter.ID);
            if (EntityFilter.Name != null)
            {
                (TemplateSavedMessageBox.FindControl("LabelShowNameTemplate") as Label).Text = string.Format("Фильтр «{0}» успешно сохранён.", EntityFilter.Name);
                TemplateSavedMessageBox.Show();
            }
        }

        public void CreateFilter(XElement xml)
        {
            using (var c = new SqlConnection(Global.ConnectionString))
            using (var cmd = new SqlCommand("EXEC [model].[EntityFilterCreate] @xml", c))
            {
                cmd.Parameters.Add("xml", DbType.String).Value = xml.ToString();
                c.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public XElement GenerateXml(string id, string name, string userID, string entityID)
        {
            var xml = new XElement("entityFilter",
                new XAttribute("id", id == "" ? string.Empty : id),
                new XAttribute("name", name),
                new XAttribute("userID", userID),
                new XAttribute("entityID", entityID)
                );

            return xml;
        }

        public XElement GenerateXml(EntityFilter entityFilter)
        {
              var userID = Session["SystemUser.objID"].ToString();

            var xml = new XElement("entityFilter",
                new XAttribute("id", entityFilter.ID == null ? string.Empty : entityFilter.ID),
                new XAttribute("name", entityFilter.Name),
                new XAttribute("userID", userID),
                new XAttribute("entityID", entityFilter.Entity.ID)
                );

            foreach (var item in EntityFilter.Fields)
            {
                var field = new XElement("field",
                    new XAttribute("attributeID", item.Attribute.ID),
                    new XAttribute("predicateInfo", item.PredicateInfo == null ? string.Empty : item.PredicateInfo),
                    new XAttribute("techPredicate", item.TechPredicate == null ? string.Empty : item.TechPredicate),
                    new XAttribute("userPredicate", item.UserPredicate == null ? string.Empty : item.UserPredicate),
                    new XAttribute("sequence", item.Sequence)
                    );
                xml.Add(field);
            }
            return xml;
        }


        #endregion


    }
}