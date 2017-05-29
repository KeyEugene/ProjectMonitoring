using Phoenix.Web.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Teleform.ProjectMonitoring.HttpApplication;

namespace Teleform.ProjectMonitoring.NavigationFrame
{
    public partial class NavigationFrame_Template : System.Web.UI.UserControl
    {
        public event EventHandler UserControl_EntityList_SelectedIndexChanged;
        public event EventHandler UserControl_TemplateList_SelectedIndexChanged;
        public event EventHandler UserControl_CreateButton_Click;
        public event EventHandler UserControl_EditButton_Click;
        public event EventHandler UserControl_DownloadButton_Click;
        public event EventHandler UserControl_ShowPreview_Click;
        public event EventHandler<MessageBoxEventArgs> UserControl_DeteleButton_Click;

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void GetEntities(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = Global.Schema.Entities;
        }

        protected void EntityList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UserControl_EntityList_SelectedIndexChanged != null)
            {
                UserControl_EntityList_SelectedIndexChanged(sender, e);
            }
        }

        protected void TemplateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UserControl_TemplateList_SelectedIndexChanged != null)
            {
                UserControl_TemplateList_SelectedIndexChanged(sender, e);
            }
        }

        #region Buttons event

        protected void CreateButton_Click(object sender, EventArgs e)
        {
            if (UserControl_CreateButton_Click != null)
            {
                UserControl_CreateButton_Click(sender, e);
            }

        }

        protected void EditButton_Click(object sender, EventArgs e)
        {
            if (UserControl_EditButton_Click != null)
            {
                UserControl_EditButton_Click(sender, e);
            }

        }

        protected void DownloadButton_Click(object sender, EventArgs e)
        {
            if (UserControl_DownloadButton_Click != null)
            {
                UserControl_DownloadButton_Click(sender, e);
            }
        }

        protected void ShowPreview_Click(object sender, EventArgs e)
        {
            if (UserControl_ShowPreview_Click != null)
            {
                UserControl_ShowPreview_Click(sender, e);
            }

        }

        protected void DeteleButton_Click(object sender, MessageBoxEventArgs e)
        {
            if(UserControl_DeteleButton_Click != null)
            {
                UserControl_DeteleButton_Click(sender, e);
            }
        }

        #endregion
    }
}