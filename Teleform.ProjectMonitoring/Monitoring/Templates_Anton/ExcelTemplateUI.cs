using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Teleform.ProjectMonitoring.Templates_Anton
{
    public class ExcelTemplateUI : WebControl, INamingContainer
    {
        private TextBox TemplateNameBox;
        private FileUpload FileUpload;

        private TextBox FileNameBox;
        private TextBox SheetBox;

        protected override void CreateChildControls()
        {
            var table = CreateBasicControls();

            TableRow row;
            TableCell cell;

            row = new TableRow();
            cell = new TableCell();
            var label = new Label() { Text = "Лист" };
            cell.Controls.Add(label);
            row.Cells.Add(cell);

            cell = new TableCell();
            SheetBox = new TextBox();
            cell.Controls.Add(SheetBox);
            row.Cells.Add(cell);
            table.Rows.Add(row);

            this.Controls.Add(table);
        }

        private Table CreateBasicControls()
        {
            var table = new Table();
            TableRow row;
            TableCell cell;
            Label label;

            row = new TableRow();
            cell = new TableCell();
            label = new Label { Text = "Имя шаблона" };
            cell.Controls.Add(label);
            row.Cells.Add(cell);

            cell = new TableCell();
            TemplateNameBox = new TextBox { ID = "NameBox" };
            cell.Controls.Add(TemplateNameBox);
            row.Cells.Add(cell);
            table.Rows.Add(row);

            row = new TableRow();
            cell = new TableCell();
            label = new Label { Text = "Имя документа" };
            cell.Controls.Add(label);
            row.Cells.Add(cell);

            cell = new TableCell();
            FileNameBox = new TextBox() { ID = "FileNameBox" };
            cell.Controls.Add(FileNameBox);
            row.Cells.Add(cell);
            table.Rows.Add(row);

            row = new TableRow();
            cell = new TableCell();
            label = new Label { Text = "Загрузить файл" };
            cell.Controls.Add(label);
            row.Cells.Add(cell);

            cell = new TableCell();
            FileUpload = new FileUpload() { ID = "FileUpload" };
            cell.Controls.Add(FileUpload);
            row.Cells.Add(cell);
            table.Rows.Add(row);

            return table;
        }
    }
}