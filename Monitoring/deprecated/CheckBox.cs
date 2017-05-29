using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Drawing.Design;
using System.Web.UI;

namespace Teleform.ProjectMonitoring
{
    [Obsolete("")]
    public class CheckBox : System.Web.UI.WebControls.CheckBox
    {
        [Bindable(true, BindingDirection.OneWay)]
        [DefaultValue("")]
        public virtual string Tag
        {
            get { return ViewState["Tag"].ToString(); }
            set { ViewState["Tag"] = value; }
        }
    }
}