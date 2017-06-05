using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Teleform.ProjectMonitoring.NavigationFrame.admin
{
    public partial class NavigationFrame_4 : System.Web.UI.UserControl
    {
        public event EventHandler UserControl_Synchronize;
        public event EventHandler UserControl_ButtonAdd_Attribute;
        public event EventHandler UserControl_ButtonAdd_Close;
        public event EventHandler UserControl_AddAttributeShow_Click;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Synchronize(object sender, EventArgs e)
        {
            if (UserControl_Synchronize != null)
            {
                UserControl_Synchronize(sender, e);
            }
        }

        protected void ButtonAdd_Attribute(object sender, EventArgs e)
        {
            if (UserControl_ButtonAdd_Attribute != null)
            {
                UserControl_ButtonAdd_Attribute(sender, e);
            }
        }

        protected void ButtonAdd_Close(object sender, EventArgs e)
        {
            if (UserControl_ButtonAdd_Close != null)
            {
                UserControl_ButtonAdd_Close(sender, e);
            }
        }

        protected void AddAttributeShow_Click(object sender, EventArgs e)
        {
            if (UserControl_AddAttributeShow_Click != null)
            {
                UserControl_AddAttributeShow_Click(sender, e);
            }
        }
    }
}