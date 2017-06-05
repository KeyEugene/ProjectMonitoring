using Phoenix.Web.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Teleform.ProjectMonitoring.HttpApplication;

namespace Teleform.ProjectMonitoring.admin
{
    public partial class EventManagement : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Frame.UserControl_EventAddButton_Click += EventAddButton_Click;
            Frame.UserControl_EventEditButton_Click += EventEditButton_Click;
            Frame.UserControl_EventDeleteButton_Click += EventDeleteButton_Click;
        }
        #region EventPart


        protected void EventAddButton_Click(object sender, EventArgs e)
        {
            ((FormView)EventDialog.FindControl("EventForm")).ChangeMode(FormViewMode.Insert);
            EventDialog.Caption = "Создание события";
            EventDialog.Show(EditorMode.Insert);
        }

        protected void EventEditButton_Click(object sender, EventArgs e)
        {
            if (EventGridView.Rows.Count == 0)
                return;

            if (EventGridView.SelectedIndex != -1)
            {
                ((FormView)EventDialog.FindControl("EventForm")).ChangeMode(FormViewMode.Edit);
                EventDialog.Caption = "Редактирование события";
                EventDialog.Show(EditorMode.Edit);
            }
        }

        protected void EventDeleteButton_Click(object sender, EventArgs e)
        {
            if (EventGridView.Rows.Count == 0)
                return;

            if (EventGridView.SelectedIndex != -1)
                DeleteWarningDialog.Show();
        }

        protected void EventSaveHandler(object sender, EventArgs e)
        {
            var formView = EventDialog.FindControl("EventForm") as FormView;
            if (formView.CurrentMode == FormViewMode.Edit)
                EventForm.UpdateItem(false);
            else if (formView.CurrentMode == FormViewMode.Insert)
                EventForm.InsertItem(false);
            EventDialog.Close();
        }

        protected void iEventTableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var f = (FormView)EventDialog.FindControl("EventForm");
            var ddl = (DropDownList)f.FindControl("iEventColumnList");
            for (int i = 1; i < ddl.Items.Count; i++)
                ddl.Items.RemoveAt(i);
        }

        protected void DeleteWarningDialog_Close(object sender, EventArgs e)
        {
            if ((e as MessageBoxEventArgs).Result.ToString() == "Yes")
            {
                using (var conn = new SqlConnection(Global.ConnectionString))
                using (var cmd = new SqlCommand())
                {
                    conn.Open();
                    cmd.Connection = conn;
                    cmd.CommandText = "EXEC [model].[EventDelete] @eventID";
                    cmd.Parameters.Add("eventID", System.Data.SqlDbType.BigInt).Value = EventGridView.SelectedDataKey["objID"];
                    cmd.ExecuteNonQuery();
                }

            }
        }

        #endregion EventPart
    }
}