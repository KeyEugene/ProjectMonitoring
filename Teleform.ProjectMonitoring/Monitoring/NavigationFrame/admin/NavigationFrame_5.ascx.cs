using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Teleform.ProjectMonitoring.NavigationFrame.admin
{
    public partial class NavigationFrame_5 : System.Web.UI.UserControl
    {
        public event EventHandler UserControl_ButtonNew_Click;
        public event EventHandler UserControl_ButtonUpdate_Click;
        public event EventHandler UserControl_ButtonDelete_Click;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonNew_Click(object sender, EventArgs e)
        {
            if (UserControl_ButtonNew_Click != null)
            {
                UserControl_ButtonNew_Click(sender, e);
            }
        }
        protected void ButtonUpdate_Click(object sender, EventArgs e)
        {
            if (UserControl_ButtonUpdate_Click != null)
            {
                UserControl_ButtonUpdate_Click(sender, e);
            }
        }
        protected void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (UserControl_ButtonDelete_Click != null)
            {
                UserControl_ButtonDelete_Click(sender, e);
            }
        }
    }
}