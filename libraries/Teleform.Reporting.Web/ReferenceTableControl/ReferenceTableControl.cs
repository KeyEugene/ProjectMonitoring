using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Writer = System.Web.UI.HtmlTextWriter;

namespace Teleform.Reporting.Web
{
    //[Serializable]
    public partial class ReferenceTableControl : CompositeControl
    {

        Button closeButton;

        HtmlGenericControl ContainerDiv;

        GridView ReferenceTableGrid;

        Entity _entity { get; set; }

        TemplateField _field { get; set; }

        DataRowView _rowInPage { get; set; }

        public DataTable DataSource { get; set; }

            
        public ReferenceTableControl(Entity entity, TemplateField field, DataRowView rowInPage)
        {
            _entity = entity;
            _field = field;
            _rowInPage = rowInPage;
        }

        protected override void CreateChildControls()
        {
            ContainerDiv = new HtmlGenericControl("div") { ID = "ListAttributeContainer" };
            ContainerDiv.Attributes.Add("class", "ListAttributeContainer");
            Controls.Add(ContainerDiv);

            var toolBarDiv = new HtmlGenericControl("div");
            toolBarDiv.Attributes.Add("class", "ListAttributeContent");
            ContainerDiv.Controls.Add(toolBarDiv);

            var headerDiv = new HtmlGenericControl("div");
            headerDiv.Attributes.Add("class", "headDiv");
            toolBarDiv.Controls.Add(headerDiv);

            var searchPanelDiv = CreateSearchPanel();
            toolBarDiv.Controls.Add(searchPanelDiv);

            closeButton = new Button { Text = "Закрыть" };
            closeButton.Click += new EventHandler(closeButton_Click);
            headerDiv.Controls.Add(closeButton);

            var bodyDiv = new HtmlGenericControl("div");
            bodyDiv.Attributes.Add("class", "bodyDiv");
            bodyDiv.Controls.Add(CreateReferenceTableGrid());
            toolBarDiv.Controls.Add(bodyDiv);

            ContainerDiv.Controls.Add(toolBarDiv);            
        }        

        private GridView CreateReferenceTableGrid()
        {
            ReferenceTableGrid = new GridView
            {
                ID = "ListView",
                //ID = "ReferenceGridView",
                DataKeyNames = new[] { "objID" },
                AutoGenerateColumns = true,
                UseAccessibleHeader = true
            };
            ReferenceTableGrid.SelectedIndexChanged += new EventHandler(ReferenceTableGrid_SelectedIndexChanged);
            ReferenceTableGrid.RowDataBound += new GridViewRowEventHandler(ReferenceTableGrid_RowDataBound);
            ReferenceTableGrid.DataSource = DataSource;
            ReferenceTableGrid.DataBind();

            return ReferenceTableGrid;
        }
        
        string GetReferenceTableName(string fPath)
        {
            var indexOf = fPath.IndexOf("/");

            var constrName = fPath.Substring(0, indexOf);

            var referenceTableName = _entity.Constraints.First(c => c.ConstraintName == constrName).RefTblName;
               
            return referenceTableName;
        }
        private HtmlGenericControl CreateSearchPanel()
        {
            var searchDiv = new HtmlGenericControl("div");
            searchDiv.Attributes.Add("class", "ListAttributeSearch");

            var searchTextBox = new HtmlGenericControl("input");
            searchTextBox.Attributes.Add("type", "text");
            searchTextBox.Attributes.Add("placeholder", "Поиск:");
            searchTextBox.Attributes.Add("autocomplete", "off");
            searchTextBox.Attributes.Add("oninput", "ListAttributeSearchTextChanged(this)");
            searchTextBox.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            searchTextBox.ID = "ListAttributeSearchText";

            var searchClearButton = new HtmlGenericControl("input");
            searchClearButton.Attributes.Add("type", "button");
            searchClearButton.Attributes.Add("class", "resetButton");
            searchClearButton.Attributes.Add("value", "\u2613");
            searchClearButton.Attributes.Add("onclick",
                "ShowSelection(null); $('#ListAttributeSearchText').val(''); $('#ListAttributeSearchText').focus();");
            searchClearButton.Attributes.Add("title", "Очистить строку поиска");

            searchDiv.Controls.Add(searchTextBox);
            searchDiv.Controls.Add(searchClearButton);

            return searchDiv;
        }
        protected override void Render(Writer writer)
        {
            ContainerDiv.RenderControl(writer);
        }


    }
}
