using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;


namespace Teleform.ProjectMonitoring.Templates
{
    using Teleform.Reporting;

    [Serializable()]
    public class TreeCell : CompositeControl
    {
        public Label labelFieldAlias { get; set; }
        public Button dialogButton { get; private set; }
        public TemplateField Field;
        public TextBox textBoxSort;
        public Color color { get; private set; }
        public System.Web.UI.WebControls.CheckBox isCheck;
        private string sort;

        public TreeCell(TemplateField field, Color c, string tb)
        {
            this.EnableViewState = false;

            Field = field;
            color = c;
            sort = tb;

            labelFieldAlias = new Label
            {
                Text = field.Name,
                ID = string.Concat("_link", field.Attribute.ID.ToString()),
                EnableViewState = false
            };

            labelFieldAlias.Attributes.Add("name", string.Concat("_link", field.Attribute.ID.ToString()));
            labelFieldAlias.Attributes.Add("onclick", "SelectedFieldHardTemplate(this);");
            labelFieldAlias.Attributes.Add("class", "HardTemplateAlex");

            isCheck = new System.Web.UI.WebControls.CheckBox { ID = labelFieldAlias.ID + "hardCheck" };
            isCheck.Attributes.Add("hidden", "false");

            dialogButton = new Button
            {
                BackColor = System.Drawing.Color.Gray,
                BorderStyle = System.Web.UI.WebControls.BorderStyle.Outset,
                Text = "+",
                ID = string.Concat("_dialog", field.Attribute.ID.ToString()),
                EnableViewState = false
            };
        }

        protected override void CreateChildControls()
        {
            Controls.Clear();

            var tb = new Table
            {
                BorderColor = System.Drawing.Color.CornflowerBlue,
                BackColor = color,
            };
            tb.Attributes.Add("padding", "5px");
            tb.Attributes.Add("class", "treeCellTable");

            var row = new TableRow();
            var cell = new TableCell();

            cell.Controls.Add(isCheck);
            cell.Controls.Add(labelFieldAlias);
            cell.Controls.Add(new System.Web.UI.LiteralControl("&nbsp;&nbsp;"));
            cell.Controls.Add(dialogButton);

            AddToCellImagesAndTextBox(ref cell);

            row.Controls.Add(cell);
            tb.Controls.Add(row);

            Controls.Add(tb);
        }

        private void AddToCellImagesAndTextBox(ref TableCell cell)
        {
            var fieldID = Field.ID;
            textBoxSort = new TextBox { ID = "tbSort" + Field.Attribute.ID, Text = sort };
            textBoxSort.Attributes.Add("class", "tbSort");
            cell.Controls.Add(textBoxSort);

            var imgUp = new System.Web.UI.WebControls.Image { ID = "arrowUp" + fieldID, ImageUrl = "~/images/arrow_up.png", ToolTip = "Сортировка от Я-А, 9-0." };
            imgUp.Attributes.Add("class", "imageUpDown");
            imgUp.Attributes.Add("style", sort.Equals("DESC") ? "opacity: 1;" : "opacity: .3;");
            imgUp.Attributes.Add("onclick", "clickUp(this, '" + textBoxSort.ClientID + "', '" + fieldID + "');");
            cell.Controls.Add(imgUp);

            var imgDown = new System.Web.UI.WebControls.Image { ID = "arrowDown" + fieldID, ImageUrl = "~/images/arrow_down.png", ToolTip = "Сортировка от А-Я, 0-9." };
            imgDown.Attributes.Add("class", "imageUpDown");
            imgDown.Attributes.Add("style", sort.Equals("ASC") ? "opacity: 1;" : "opacity: .3;");
            imgDown.Attributes.Add("onclick", "clickDown(this, '" + textBoxSort.ClientID + "', '" + fieldID + "');");
            cell.Controls.Add(imgDown);


        }
    }
}