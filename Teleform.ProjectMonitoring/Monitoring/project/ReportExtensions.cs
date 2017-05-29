using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Text;

namespace Teleform.ProjectMonitoring
{
    /// <summary>
    /// Удаляет лишние HTML тэги и их аттрибуты в выходном потоке
    /// </summary>
    public class ExcelHtmlTextWriter : HtmlTextWriter
    {
        public ExcelHtmlTextWriter(TextWriter writer)
            : base(writer)
        {
        }

        protected override bool OnAttributeRender(string name, string value, HtmlTextWriterAttribute key)
        {
            return name == "border";
        }

        protected override bool OnTagRender(string name, HtmlTextWriterTag key)
        {
            return key != HtmlTextWriterTag.A && base.OnTagRender(name, key);
        }
    }

    public static class ReportExtensions
    {
        public static void CreateExcelReport(this Page page, System.Web.UI.WebControls.GridView grid)
        {
            DisplayExcelDocument(grid, page.Response);
        }

        public static void CreateExcelReport(this UserControl control, System.Web.UI.WebControls.GridView grid)
        {
            DisplayExcelDocument(grid,  control.Page.Response);
        }

        private static void DisplayExcelDocument(System.Web.UI.WebControls.GridView grid, HttpResponse response)
        {

               var filename = string.Format("Отчет_{0}.xls", DateTime.Now.ToString());

               response.Clear();
               response.ContentType = "text/html";
               response.AddHeader("content-disposition", "attachment;filename=" + filename);

               response.ContentEncoding = Encoding.UTF8;

               var s = new StringWriter();
               var writer = new ExcelHtmlTextWriter(s);

               var v = grid.AllowPaging;
               grid.AllowPaging = false;

               grid.RenderControl(writer);

               var hypertext = string.Format(
                   @"<html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" /></head><body>{0}</body></html>",
                   s.ToString());

               response.Write(hypertext);
               response.Flush();
               response.End();

               grid.AllowPaging = v;
        }
    }
}