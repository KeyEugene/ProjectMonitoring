using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Teleform.ProjectMonitoring.NavigationFrame.admin
{
    public partial class NavigationFrame_2 : System.Web.UI.UserControl
    {
        public event EventHandler UserControl_ImportButton_Click;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void ImportButton_Click(object sender, EventArgs e)
        {
            if (UserControl_ImportButton_Click != null)
            {
                UserControl_ImportButton_Click(sender, e);
            }
        }
    }
}