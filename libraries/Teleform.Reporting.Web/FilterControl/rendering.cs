#define Alex

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.UI;
using Writer = System.Web.UI.HtmlTextWriter;
using TableItemStyle = System.Web.UI.WebControls.TableItemStyle;
using System.Web.UI.WebControls;
using System.IO;

namespace Teleform.Reporting.Web
{
    partial class FilterControl
    {
        protected override void Render(Writer writer)
        {
            writer.AddAttribute("class", "FilterBoxDiv");
            writer.RenderBeginTag("div");

            if (!string.IsNullOrWhiteSpace(ID))
                writer.AddAttribute("id", UniqueID);

            foreach (string key in Style.Keys)
                writer.AddStyleAttribute(key, Style[key]);

            if (!string.IsNullOrEmpty(CssClass))
                writer.AddAttribute("class", CssClass);

            writer.RenderBeginTag("details");

            if (ActiveStyle != null && Active)
                writer.AddAttribute("class", ActiveStyle.CssClass);

            writer.RenderBeginTag("summary");
            writer.RenderEndTag();

            writer.RenderBeginTag("div");
            LiveFilterBox.Attributes.Add("onkeyup", string.Format("keyup_handlerFilterControl2(this,'{0}')", CheckBoxList.ClientID));
            LiveFilterBox.Attributes.Add("placeholder", " поиск ");
            LiveFilterBox.RenderControl(writer);

            writer.AddAttribute("class", "FilterboxTable");
            CheckBoxList.RenderControl(writer);

            writer.RenderEndTag();
            writer.AddAttribute("class", "ApplyPanel");
            writer.RenderBeginTag("div");

            ApplyButton.RenderControl(writer);
            CancelButton.RenderControl(writer);

            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
        }
    }
}