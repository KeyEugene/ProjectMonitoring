using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Web.UI.WebControls;
using System.IO;

namespace Teleform.ProjectMonitoring.Templates
{
    using Reporting;

    public abstract class FileBasedTemplateDesigner : GeneralTemplateDesigner
    {
        protected override Template GetTemplate()
        {
#if true
            var bytes = FileUpload.FileBytes;
            var tmp = Convert.ToBase64String(bytes); // tmp - временный хранилище, для того чтобы не изменялся массив байтов(переменная bytes ,
            //изменяется в методе RetrieveTemplate

            var template = RetrieveTemplate(TemplateNameBox.Text, bytes, TemplateID);
            template.FileName = Path.GetFileNameWithoutExtension(FileUpload.FileName);

            var t = Convert.FromBase64String(tmp);
            template.Content = t;
            
            return template;
#else
            //создаем элемент, содержащий всю информацию о шаблоне
            var template = new XElement("template");
            //проверяем заполнены ли пользователем поля имя шаблоны, имя файла шаблона
            var nameBox = this.FindControl("NameBox") as TextBox;
            if (string.IsNullOrEmpty(nameBox.Text)) return null;

            var fileNameBox = this.FindControl("FileNameBox") as TextBox;
            if (string.IsNullOrEmpty(fileNameBox.Text)) return null;
            //записываем имя шаблоны, имя файла шаблона
            template.Add(new XAttribute("name", nameBox.Text.Trim()), new XAttribute("fileName", fileNameBox.Text.Trim()));
            //получаем текст файла шаблона
            string body = string.Empty;
            string mimeType = string.Empty;
            GetFileContent(out body, out mimeType);
            //если тело файла или тип пустые, уведомляем пользователя
            if (string.IsNullOrEmpty(body) || string.IsNullOrEmpty(mimeType)) return null;
            //записываем файли его тип
            template.Add(new XAttribute("mimeType", mimeType));
            template.Add(new XElement("content", body));
#warning уведомить пользователя, о том, что тип пустые
            //извлекаем аттрибуты из файла
            var attributes = RetrieveTemplate(body);
            //если в файле нет ни одного аттрибута, уведомляем пользователя
            if (attributes.Elements("attribute").Count() == 0) return null;
#warning уведомить пользователя, о том, что в файле нет ни одного аттрибута
            //записываем аттрибуты шаблона
            template.Add(attributes);
#warning еще entityID
            //возвращаем итоговый элемент, содержащий всю информацию о шаблоне
            return template;
#endif
        }

        protected abstract Template RetrieveTemplate(string name, byte[] body,string TemplateID = null);

        protected Table CreateBasicControls()
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

            if (!string.IsNullOrEmpty(TemplateID))
            {
                var template = Storage.Select<Template>(TemplateID);
                TemplateNameBox.Text = template.Name;
            }

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