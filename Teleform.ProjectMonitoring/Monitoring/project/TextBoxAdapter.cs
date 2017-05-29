using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Adapters;
using System.Text;
using System.IO;

namespace Teleform.ProjectMonitoring
{
    public class TextBoxAdapter : ControlAdapter
    {
        public TextBoxAdapter()
        {
            //(this.Control as WebControl).Attributes[
        }

        protected override void Render(HtmlTextWriter writer)
        {
            var s = new StringBuilder();
            var w = new HtmlTextWriter(new StringWriter(s));

            base.Render(w);

            var html = s.ToString();

            if (html.Contains("type=\"date\"") || html.Contains("type=\"number\""))
                html = s.Replace(@"type=""text""", "").ToString();

            writer.Write(html);

//            base.Render(writer);
        }

        protected override void EndRender(HtmlTextWriter writer)
        {
            base.EndRender(writer);
        }
    }
}