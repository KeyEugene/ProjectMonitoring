using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Teleform.ProjectMonitoring.NavigationFrame.admin
{
    public partial class NavigationFrame_3 : System.Web.UI.UserControl
    {
        public event EventHandler UserControl_SaveObjectsViewNew_Click;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SaveObjectsViewNew_Click(object sender, EventArgs e)
        {
            if (UserControl_SaveObjectsViewNew_Click != null)
            {
                UserControl_SaveObjectsViewNew_Click(sender, e);
            }
        }
    }
}