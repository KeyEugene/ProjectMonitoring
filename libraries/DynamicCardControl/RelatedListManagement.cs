


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Teleform.Reporting.DynamicCard
{
    partial class DynamicCardControl
    {
        GridView RelatedListView;

        bool DisplayRelatedList
        {
            get
            {
                return ViewState["DisplayRelatedList"] == null ? false : (bool)ViewState["DisplayRelatedList"];
            }
            set
            {
                ViewState["DisplayRelatedList"] = value;
            }
        }


        private void CloseRelatedListHandler(object sender, EventArgs e)
        {
            DisplayRelatedList = false;

            RecreateChildControls();
        }


        private void RelatedListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            var link = sender as LinkButton;

            RelatedListView.SelectedIndex = Convert.ToInt32(link.CommandArgument);

            var row = RelatedListView.SelectedRow;
            
            var titles = new StringBuilder();

            for (int i = 2; i < row.Cells.Count; i++)
                titles.Append(row.Cells[i].Text).Append(" ~ ");

      

            var constrName = OpenedRelation.SystemName;
            OpenedRelation.Card.DependencyRelations.RelationTableTitleAttributes[constrName] = titles.ToString();

       
            OpenedRelation.Value = titles.ToString();

            DataBaseReader.FillRelation(OpenedRelation, Convert.ToInt32(RelatedListView.SelectedDataKey["objID"]));

            DisplayRelatedList = false;
            
            OpenedRelationSystemName = null;

            RecreateChildControls();
        }

        private void RelatedListView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var linkCell = new TableCell();

            e.Row.Cells.AddAt(0,linkCell);
            e.Row.Cells[0].Style["display"] = "none";
            e.Row.Cells[1].Style["display"] = "none";

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var linkID = "SelectRow_" + e.Row.RowIndex.ToString();

                var linkButton = new LinkButton 
                { 
                    ID = linkID,
                    Text = "Выбор",
                    CommandArgument = e.Row.RowIndex.ToString()
                };

                linkCell.Controls.Add(linkButton);

               linkButton.Click += new EventHandler(RelatedListView_SelectedIndexChanged);

                e.Row.Attributes.Add("data-row-content", "true");

                e.Row.Attributes["onclick"] = "var td = $(this).children('td:first'); eval($(td).children('a:first').attr('href')); ";
            }
            else
                e.Row.Attributes.Add("data-row-content", "false");

        }


        private Control CreateRelatedList()
        {

            var ContainerDiv = new HtmlGenericControl("div")
            {
                ID = "ListAttributeContainer"
            };

            ContainerDiv.Visible = DisplayRelatedList;


#warning Style for RelatedList.
            ContainerDiv.Attributes.Add("class", "ListAttributeContainer");

            var toolBarDiv = new HtmlGenericControl("div");
            toolBarDiv.Attributes.Add("class", "ListAttributeContent");

            RelatedListView = new GridView
            {
                ID = "ListView",
                DataKeyNames = new[] { "objID" },
                AutoGenerateColumns = true,
                UseAccessibleHeader = true
            };

            RelatedListView.RowDataBound += RelatedListView_RowDataBound;
            RelatedListView.SelectedIndexChanged += RelatedListView_SelectedIndexChanged;



            if (DisplayRelatedList)
            {                
                var sourceTable = DataBaseReader.GetRelations(OpenedRelation);
                RelatedListView.DataSource = sourceTable;

                RelatedListView.DataBind();

            }

            var headerDiv = new HtmlGenericControl("div");

            headerDiv.Attributes.Add("class", "headDiv");

            var closeButton = new Button { Text = "Закрыть" };
            closeButton.Click += CloseRelatedListHandler;
                   
            headerDiv.Controls.Add(closeButton);
            toolBarDiv.Controls.Add(headerDiv);
            
            var searchPanelDiv = CreateSearchPanel();
            toolBarDiv.Controls.Add(searchPanelDiv);

            var bodyDiv = new HtmlGenericControl("div");
            bodyDiv.Attributes.Add("class", "bodyDiv");

            bodyDiv.Controls.Add(RelatedListView);
            toolBarDiv.Controls.Add(bodyDiv);

            ContainerDiv.Controls.Add(toolBarDiv);


            Controls.Add(ContainerDiv);

            if(DisplayRelatedList)
                ContainerDiv.FindControl("ListAttributeSearchText").Focus();

            return ContainerDiv;
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
    }
}
