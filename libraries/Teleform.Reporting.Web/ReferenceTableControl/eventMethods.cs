using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Teleform.Reporting.Web
{
    partial class ReferenceTableControl
    {

        public event EventHandler CloseButtonClick;

        public event EventHandler SelectedIndexChanged;


        public string EntityInstanceID
        {
            get
            {
                var o = Page.Session["ReferenceEntityInstanceID"];
                if (o != null)
                    return (string)o;
                else return "";
            }
            set { Page.Session["ReferenceEntityInstanceID"] = value; }
        }


        public Dictionary<string, string> TableTitleAttributes
        {
            get
            {
                var o = Page.Session["RelationTableTitleAttributes"];
                if (o != null)
                    return (Dictionary<string, string>)o;
                else return null;
            }
            set { Page.Session["RelationTableTitleAttributes"] = value; }
        }


        protected void ReferenceTableGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            var link = sender as LinkButton;

            ReferenceTableGrid.SelectedIndex = Convert.ToInt32(link.CommandArgument);

            var row = ReferenceTableGrid.SelectedRow;

            var titles = new StringBuilder();
            for (int i = 2; i < row.Cells.Count; i++)
                titles.Append(row.Cells[i].Text).Append(" ~ ");


          

            //найти констраин, по имени поля 
            var fPath = _field.Attribute.FPath;
            var indexOf = fPath.IndexOf("/");
            var constrName = fPath.Substring(0, indexOf);


            TableTitleAttributes = new Dictionary<string, string>();
            TableTitleAttributes.Add(constrName, titles.ToString());

            EntityInstanceID = ReferenceTableGrid.SelectedDataKey["objID"].ToString();

            if (SelectedIndexChanged != null)
                SelectedIndexChanged(this, EventArgs.Empty);


            RecreateChildControls();
        }

        private void ReferenceTableGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var linkCell = new TableCell();

            e.Row.Cells.AddAt(0, linkCell);
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

                linkButton.Click += new EventHandler(ReferenceTableGrid_SelectedIndexChanged);

                linkCell.Controls.Add(linkButton);

                e.Row.Attributes.Add("data-row-content", "true");

                e.Row.Attributes["onclick"] = "var td = $(this).children('td:first'); eval($(td).children('a:first').attr('href')); ";
            }
            else
                e.Row.Attributes.Add("data-row-content", "false");
        }

        void closeButton_Click(object sender, EventArgs e)
        {
            if (CloseButtonClick != null)
                CloseButtonClick(this, EventArgs.Empty);
        }
    }
}
