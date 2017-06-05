using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Teleform.ProjectMonitoring.NavigationFrame.admin
{
    public partial class NavigationFrame_6 : System.Web.UI.UserControl
    {
            public event EventHandler UserControl_LinkBtnTemplate_Click;
            public event EventHandler UserControl_CloseTemplate_Click;
            public event EventHandler UserControl_UserTypeList_IndexChanged;
            public event EventHandler UserControl_UserList_IndexChanged;
            public event EventHandler UserControl_EntityList_IndexChanged;
            public event EventHandler UserControl_SaveButton_OnClick;
            public event EventHandler UserControl_SaveTemplate_Click;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SaveTemplate_Click(object sender, EventArgs e)
        {
            if (UserControl_SaveTemplate_Click != null)
            {
                UserControl_SaveTemplate_Click(sender, e);
            }
        }
        

        protected void CloseTemplate_Click(object sender, EventArgs e)
        {
            if (UserControl_CloseTemplate_Click != null)
            {
                UserControl_CloseTemplate_Click(sender, e);
            }
        }

        protected void UserTypeList_IndexChanged(object sender, EventArgs e)
        {
            if (UserControl_UserTypeList_IndexChanged != null)
            {
                UserControl_UserTypeList_IndexChanged(sender, e);
            }
        }
        protected void UserList_IndexChanged(object sender, EventArgs e)
        {
            if (UserControl_UserList_IndexChanged != null)
            {
                UserControl_UserList_IndexChanged(sender, e);
            }
        }
        protected void EntityList_IndexChanged(object sender, EventArgs e)
        {
            if (UserControl_EntityList_IndexChanged != null)
            {
                UserControl_EntityList_IndexChanged(sender, e);
            }
        }
        protected void SaveButton_OnClick(object sender, EventArgs e)
        {
            if (UserControl_SaveButton_OnClick != null)
            {
                UserControl_SaveButton_OnClick(sender, e);
            }
        }
        protected void LinkBtnTemplate_Click(object sender, EventArgs e)
        {
            if (UserControl_LinkBtnTemplate_Click != null)
            {
                UserControl_LinkBtnTemplate_Click(sender, e);
            }
        }
    }
}