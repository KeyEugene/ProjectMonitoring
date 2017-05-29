#define Alex

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using Teleform.ProjectMonitoring.HttpApplication;
using Teleform.ProjectMonitoring;

namespace Monitoring
{
    using CheckBoxBase = System.Web.UI.WebControls.CheckBox;

    partial class Administration
    {
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


        #region Setting The Types Of Objects
#if Alex
        protected void DataBindCheckBoxForTypesOfObjects(object sender, EventArgs e)
        {
            if (TableObjects.Rows.Count > 2)
                return;

            var query = @"SELECT [isLogicMain], b.alias tblAlias, b.object_id tblID FROM model.BTables b 
join model.AppTypes at on at.name='Base' and b.appTypeID=at.object_ID order by b.alias";

            var bTables = Global.GetDataTable(query);

            var ddl = new DropDownList();

            for (int i = 0; i < bTables.Rows.Count; i++)
            {
                var row = new TableRow(); var cellCheckBox = new TableCell(); var cellDDL = new TableCell();
                var checkBox = new System.Web.UI.WebControls.CheckBox();
                var hidden = new HiddenField { Value = bTables.Rows[i]["tblID"].ToString() };

                checkBox.Checked = Convert.ToBoolean(Convert.ToInt16(bTables.Rows[i]["isLogicMain"]));// bTables.Rows[i]["isLogicMain"].ToString() == "1";
                checkBox.Text = bTables.Rows[i]["tblAlias"].ToString();
                ddl = GetEntityTemplatesDDL(bTables.Rows[i]["tblID"]);

                cellCheckBox.Controls.Add(checkBox); cellCheckBox.Controls.Add(hidden);
                cellDDL.Controls.Add(ddl);

                row.Cells.Add(cellCheckBox); row.Cells.Add(cellDDL);
                TableObjects.Rows.Add(row);
            }
            Session["isTypesOfObjectsView"] = 1;
        }

        protected DropDownList GetEntityTemplatesDDL(object tblID)
        {
            var ddl = new DropDownList();

            using (var con = new SqlConnection(Kernel.ConnectionString))
            using (var ad = new SqlDataAdapter("SELECT t.[objID],t.[name], (case when b.templateID is null then 0 else 1 end) isTemplateDefault FROM [model].[R$Template] t left join model.BTables b on b.templateID = t.objID WHERE [entityID] = @entityID", con))
            {
                ad.SelectCommand.Parameters.Add("entityID", SqlDbType.Variant).Value = tblID;
                var dt = new DataTable();
                ad.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ddl.Items.Add(new ListItem
                    {
                        Text = dt.Rows[i]["name"].ToString(),
                        Value = dt.Rows[i]["objID"].ToString(),
                        Selected = Convert.ToBoolean(Convert.ToInt16(dt.Rows[i]["isTemplateDefault"])) // dt.Rows[i]["isTemplateDefault"].ToString() == "1"
                    });
                }
            }
            return ddl;
        }

        protected void SaveObjectsViewNew_Click(object sender, EventArgs e)
        {
            Session["EntityDropDownList"] = null;

            bool isShow; string entityID, templateID;
            StringBuilder querys = new StringBuilder();
            var items = TableObjects.Rows;

            for (int i = 1; i < items.Count; i++)
            {
                isShow = ((items[i].Cells[0].Controls[0] is CheckBoxBase) ? items[i].Cells[0].Controls[0] as CheckBoxBase : new CheckBoxBase()).Checked;
                entityID = ((items[i].Cells[0].Controls[1] is HiddenField) ? items[i].Cells[0].Controls[1] as HiddenField : new HiddenField()).Value;
                templateID = ((items[i].Cells[1].Controls[0] is DropDownList) ? items[i].Cells[1].Controls[0] as DropDownList : new DropDownList()).SelectedValue;

                //if (!string.IsNullOrEmpty(templateID))
                querys.AppendLine(
                    string.Concat(@" UPDATE [model].[BTables] SET [islogicMain]='", Convert.ToInt16(isShow), "' , [templateID] = ",
                    string.IsNullOrEmpty(templateID) ? "NULL" : "'" + templateID + "'", " WHERE [object_ID]= '", entityID, "' ")
                    );
            }
            Global.GetDataTable(querys.ToString());

            Synchronize(null, EventArgs.Empty);
        }


#endif

        #endregion

#if Alex
        protected void RowDeleted_OnClick(object sender, EventArgs e)
        {
            Synchronize(null, EventArgs.Empty);
        }

#endif
    }
}