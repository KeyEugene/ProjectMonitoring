using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Teleform.ProjectMonitoring
{
    public class ListWithAdditionalField : DropDownList
    {
        [DefaultValue("")]
        [Themeable(false)]
        [Bindable(true, BindingDirection.TwoWay)]
        public string AdditionalField
        {
            get { return ViewState["AdditionalField"] == null ? string.Empty : ViewState["AdditionalField"].ToString(); }
            set { ViewState["AdditionalField"] = value; }
        }
    }
}