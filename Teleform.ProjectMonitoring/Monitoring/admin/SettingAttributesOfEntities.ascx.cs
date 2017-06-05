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
    public partial class SettingAttributesOfEntities : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //UserControl_ButtonAdd_Attribute += ButtonAdd_Attribute;
            //UserControl_ButtonAdd_Close += ButtonAdd_Close;
            //UserControl_Synchronize += Synchronize;
        }


        protected void ButtonAdd_Attribute(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameColumn.Text))
                throw new InvalidOperationException("Поле «Псевдоним» не может содержать пустую строку.");

            if (string.IsNullOrWhiteSpace(NameAttribute.Text))
                throw new InvalidOperationException("Поле «Код» не может содержать пустую строку.");

            List<char> num = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            if (num.Contains(NameAttribute.Text[0]))
                throw new InvalidOperationException("«Код» не может начинатсья с цифры.");

            var query = string.Format(@"EXEC [model].[UserAttributeAdd] '{0}', '{1}', '{2}', '{3}'",
                EntityList.SelectedValue, NameAttribute.Text.Trim(), ListType.SelectedValue, NameColumn.Text.Trim());

            using (var conn = new SqlConnection(Global.ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(String.Format("Не удалось добавить атрибут в таблицу.\n{0}.", ex.Message));
                }
            }

            NameColumn.Text = NameAttribute.Text = null;
            AddAttriabute.Close();
            AliasGridView.DataBind();
        }

        protected void ButtonAdd_Close(object sender, EventArgs e)
        {
            NameColumn.Text = NameAttribute.Text = null;
            AddAttriabute.Close();
        }

        protected void RowDeleted_OnClick(object sender, EventArgs e)
        {
            Synchronize(null, EventArgs.Empty);
        }
        protected void Synchronize(object sender, EventArgs e)
        {
            Teleform.ProjectMonitoring.HttpApplication.Global.UpdateSchema();
        }
    }
}