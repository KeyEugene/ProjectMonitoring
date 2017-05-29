using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.UI;
using Writer = System.Web.UI.HtmlTextWriter;
using TableItemStyle = System.Web.UI.WebControls.TableItemStyle;
using System.Web.UI.WebControls;

namespace Teleform.Reporting.Web
{
    partial class TableViewControl
    {
        protected override void Render(Writer writer)
        {
            //var envControl = CreateEnvelopeControl();
            //envControl.RenderControl(writer);
#if true
            if (table != null && DataRow.Count() > 0)
                base.Render(writer);
#else
            if (table != null)
                base.Render(writer);
#endif
        }
    }
}