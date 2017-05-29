using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Writer = System.Web.UI.HtmlTextWriter;
namespace Teleform.Reporting.Web
{
    partial class AddRemoveInstanceControl
    {
        protected override void Render(Writer writer)
        {
            AddRemoveInstanceButton.RenderControl(writer);
        }

    }
}
