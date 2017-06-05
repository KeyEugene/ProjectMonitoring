using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Teleform.ProjectMonitoring.NavigationFrame.admin
{
    public partial class NavigationFrame_7 : System.Web.UI.UserControl
    {
        public event EventHandler UserControl_EntityListAudit_Load;
        public event EventHandler UserControl_UserListAudit_Load;
        public event EventHandler UserControl_ViewButton_Click;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void EntityListAudit_Load(object sender, EventArgs e)
        {
            if (UserControl_EntityListAudit_Load != null)
            {
                UserControl_EntityListAudit_Load(sender, e);
            }
        }

        protected void UserListAudit_Load(object sender, EventArgs e)
        {
            if (UserControl_UserListAudit_Load != null)
            {
                UserControl_UserListAudit_Load(sender, e);
            }
        }

        protected void ViewButton_Click(object sender, EventArgs e)
        {
            if (UserControl_ViewButton_Click != null)
            {
                UserControl_ViewButton_Click(sender, e);
            }
        }
    }
}