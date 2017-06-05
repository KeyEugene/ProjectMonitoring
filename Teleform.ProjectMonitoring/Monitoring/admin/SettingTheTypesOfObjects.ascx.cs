using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Teleform.ProjectMonitoring.HttpApplication;

namespace Teleform.ProjectMonitoring.admin
{
    using System.Data;
    using System.Data.SqlClient;
    using CheckBoxBase = System.Web.UI.WebControls.CheckBox;

    public partial class SettingTheTypesOfObjects : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Frame.UserControl_SaveObjectsViewNew_Click += SaveObjectsViewNew_Click;
        }


        public void DataBindCheckBoxForTypesOfObjects(object sender, EventArgs e)
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
            ddl.CssClass = "form-control";

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

        protected void Synchronize(object sender, EventArgs e)
        {
            Teleform.ProjectMonitoring.HttpApplication.Global.UpdateSchema();
        }
    }
}