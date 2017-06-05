using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Teleform.ProjectMonitoring.NavigationFrame.admin
{
    public partial class NavigationFrame_1 : System.Web.UI.UserControl
    {
        public event EventHandler UserControl_EventAddButton_Click;
        public event EventHandler UserControl_EventEditButton_Click;
        public event EventHandler UserControl_EventDeleteButton_Click;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void EventAddButton_Click(object sender, EventArgs e)
        {
            if (UserControl_EventAddButton_Click != null)
            {
                UserControl_EventAddButton_Click(sender, e);
            }
        }
        public void EventEditButton_Click(object sender, EventArgs e)
        {
            if (UserControl_EventEditButton_Click != null)
            {
                UserControl_EventEditButton_Click(sender, e);
            }
        }
        public void EventDeleteButton_Click(object sender, EventArgs e)
        {
            if (UserControl_EventDeleteButton_Click != null)
            {
                UserControl_EventDeleteButton_Click(sender, e);
            }
        }
    }
}