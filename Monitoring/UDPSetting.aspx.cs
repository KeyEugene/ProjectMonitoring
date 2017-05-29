using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Phoenix.Web.UI.Dialogs;

namespace Monitoring
{
    public partial class UDPSetting : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void DeleteUDPButton_Click(object sender, EventArgs e)
        {
            if (UDPView.SelectedDataKey != null)
            {
                UDPDataSource.DeleteParameters.Add("udp", UDPView.SelectedDataKey["udp"].ToString());
                UDPDataSource.Delete();
            }
        }

        protected void EditUDPButton_Click(object sender, EventArgs e)
        {
            if (UDPView.SelectedDataKey != null)
            {
                (UDPEditorForm.FindControl("NameBox") as TextBox).ReadOnly = true;
                UDPEditorForm.Show(EditorMode.Edit);
            }
        }

        protected void InsertUDPButton_Click(object sender, EventArgs e)
        {
            var txt = (TextBox)UDPEditorForm.FindControl("NameBox");
            if (txt.ReadOnly == true)
                txt.ReadOnly = false;
            UDPEditorForm.Show(EditorMode.Insert);
        }

        protected void DateColumnsList_DataBinding(object o, EventArgs e)
        {
            if (o is ListControl)
            {
                var list = o as ListControl;

                for (int i = list.Items.Count - 1; i > 0; i--)
                    list.Items.RemoveAt(i);
            }
        }
    }
}