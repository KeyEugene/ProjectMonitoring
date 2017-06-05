using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Teleform.ProjectMonitoring.NavigationFrame.admin
{
    public partial class NavigationFrame_Administration : System.Web.UI.UserControl
    {
        public event EventHandler UserControl_AdminManagementButton_Click;
        public event EventHandler UserControl_EnumerationManagement_Click;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void AdminManagementButton_Click(object sender, EventArgs e)
        {
            if(UserControl_AdminManagementButton_Click != null)
            {
                UserControl_AdminManagementButton_Click(sender, e);
            }
        }

        protected void EnumerationManagement_Click(object sender, EventArgs e)
        {
            if(UserControl_EnumerationManagement_Click != null )
            {
                UserControl_EnumerationManagement_Click(sender, e);
            }
        }
    }
}