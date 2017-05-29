#define isIndexed
#define parentTblID

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Teleform.ProjectMonitoring.HttpApplication;

namespace Teleform.ProjectMonitoring
{
    public partial class NavigationPanel : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["entity"] == null)
                return;

            var dt = new DataTable();

            var conn = new SqlConnection(Global.ConnectionString);

#if true
            var cmd = new SqlCommand("SELECT * FROM [model].[ListAttribute](@entity) where isIndexed = 1", conn); 
#endif

            //var cmd = new SqlCommand("select * from model.ForeignKeysColumns where parentTbl = @entity and isIndexed=1", conn);
            cmd.Parameters.Add(new SqlParameter { ParameterName = "entity", DbType = System.Data.DbType.Int64, Value = Request["entity"] }); 

            var da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            FormTable(dt);
        }

        private void FormTable(DataTable dt)
        {
            if (dt.Rows.Count == 0)
                return;

            var table = new Table();
            table.ID = "myMenu";
            table.CellSpacing = 5;

            var row = new TableRow();

            foreach (DataRow r in dt.Rows)
            {
                var id = Convert.ToInt64(r["objID"]);
#if true
                var path = Convert.ToString(r["alias"]); 
#endif
                //var path = Convert.ToString(r["nameCT"]); 
                var tblID = Convert.ToInt64(r["parentTblID"]);

                var linkButton = new LinkButton() {CommandArgument = id.ToString(), Text = path, PostBackUrl = string.Format
                    ("~/ListAttributeView.aspx?entity={0}&constraint={1}&id={2}",  tblID, id, Request["id"])};
               // var cell = new TableCell()  { Text = path };
                var cell = new TableCell();
                cell.Controls.Add(linkButton);
                
                row.Cells.Add(cell);
            }
           
            table.Rows.Add(row);

            Navigation.Controls.Add(table);
        }
    }
}