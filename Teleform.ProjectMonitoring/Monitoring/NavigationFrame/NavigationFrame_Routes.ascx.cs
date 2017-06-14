using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Teleform.ProjectMonitoring.NavigationFrame
{
    public partial class NavigationFrame_Routes : System.Web.UI.UserControl
    {
        public event EventHandler UserControl_DocTypeList_SelectedIndexChanged;
        public event EventHandler UserControl_BuildRouteButton_Click;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void DocTypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UserControl_DocTypeList_SelectedIndexChanged != null)
            {
                UserControl_DocTypeList_SelectedIndexChanged(sender, e);
            }
        }

        protected void BuildRouteButton_Click(object sender, EventArgs e)
        {
            if (UserControl_BuildRouteButton_Click != null)
            {
                UserControl_BuildRouteButton_Click(sender, e);
            }
        }
    }
}