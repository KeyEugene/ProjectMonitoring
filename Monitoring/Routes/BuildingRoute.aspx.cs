using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using Teleform.ProjectMonitoring.HttpApplication;
using Teleform.Reporting;

using routes;

namespace Teleform.ProjectMonitoring.Routes
{
    public partial class BuildingRoute : BasePage
    {
        private bool hasWork = Global.Schema.Entities.Where(x => x.SystemName == "_Application").ElementAt(0).Attributes.Where(a => a.FPath.ToLower().Contains("work")).Count() != 0;

        private string ConString 
        {
            get { return Global.ConnectionString; }
        }

        private List<RoutePoint> PointList
        {
            get {  return Session["_PointList"] as List<RoutePoint>; }
            set { Session["_PointList"] = value; }
        }

        Table tableRoute;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var listItem = new ListItem("не выбрано", "-1");
                TypesList.Items.Add(listItem);
                TypesList.AppendDataBoundItems = true;

                var query = "SELECT [objID], [name] FROM [_ApplicationType]";
                var dt = new DataTable();
                var da = new SqlDataAdapter(query, ConString);
                da.Fill(dt);
                TypesList.DataSource = dt;
                TypesList.DataTextField = "name";
                TypesList.DataValueField = "objID";
                TypesList.DataBind();
                MView.ActiveViewIndex = -1;
            }
        }

        protected override void CreateChildControls()
        {
            if (TypesList.SelectedValue != "-1")
            {
                WorkPlace.Controls.Clear();
                BuildRoutePoints();
            }
            //base.CreateChildControls();
        }

        public override void DataBind()
        {
            CreateChildControls();
            //base.DataBind();
        }

        protected void TypesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (TypesList.SelectedValue)
            {
                case "-1":
                    MView.ActiveViewIndex = -1;
                    break;
                default:
                    PointList = null;
                    WorkPlace.Controls.Clear();
                    FillPoints();
                    FillList();
                    BuildRoutePoints();
                    MView.ActiveViewIndex = 0;
                    break;
            }
        }

        private void FillPoints()
        {
            PointList = new List<RoutePoint>();
            var query = string.Format(@"SELECT p.[objID], p.[stateID], p.[_divisionID], d.[name], p.[position] FROM [_RoutePoint] p, [{1}] d 
                                        WHERE d.[objID] = p.[_divisionID] AND p.[typeID] = {0} AND (p.[isArchived] is NULL OR p.[isArchived] = 'false')", TypesList.SelectedValue,
                                        hasWork ? "_Division" : "Division");
            var dt = new DataTable();
            var da = new SqlDataAdapter(query, ConString);
            da.Fill(dt);
            if (dt.Rows.Count == 0)
                return;

            foreach (DataRow item in dt.Rows)
            {
                var point = new RoutePoint();
                point.ID = item.ItemArray[0].ToString();
                point.stateID = item.ItemArray[1].ToString();
                point.divisionID = item.ItemArray[2].ToString();
                point.divisionName = item.ItemArray[3].ToString();
                point.position = Convert.ToInt32(item.ItemArray[4]);
                point.typeID = TypesList.SelectedValue;
                PointList.Add(point);
            }
        }

        private void BuildRoutePoints()
        {
            if (PointList == null || PointList.Count == 0)
                return;
            if (tableRoute != null)
                tableRoute.Rows.Clear();

            PointList = PointList.OrderBy(x => x.position).ToList();
            tableRoute = new Table()
            {
                ID = "tableRoute",
                CssClass = "routeTable"
            };

            var row = BuildHeaderRow();
            tableRoute.Rows.Add(row);

            foreach (var item in PointList)
            {
                row = BuildPointRow(item);
                tableRoute.Rows.Add(row);
            }

            WorkPlace.Controls.Add(tableRoute);
        }

        private TableRow BuildHeaderRow()
        {
            var row = new TableRow();
            var cell = new TableCell()
            {
                ForeColor = Color.White,
                CssClass = "HeaderCell",
                Text = "№"
            };
            row.Cells.Add(cell);
            cell = new TableCell()
            {
                ForeColor = Color.White,
                CssClass = "HeaderCell",
                Text = "Инстанция"
            };
            row.Cells.Add(cell);
            cell = new TableCell()
            {
                ForeColor = Color.White,
                CssClass = "HeaderCell",
                Text = "Состояние"
            };
            row.Cells.Add(cell);

            return row;
        }

        private TableRow BuildPointRow(RoutePoint point)
        {
            var IsSelected = GetSelectedItem() - 1 == point.position ? true : false;
            var row = new TableRow();
            var cell = new TableCell()
            {
                CssClass = "PointCell"
            };
            if (IsSelected)
                cell.ForeColor = Color.Red;
            var cb = new CheckBox()
            {
                CssClass = "cb",
                ID = string.Format("cb_{0}", point.position)
            };
            cell.Controls.Add(cb);
            var l = new Literal()
            {
                Text = point.position.ToString()
            };

            cell.Controls.Add(l);

            row.Cells.Add(cell);

            cell = new TableCell()
            {
                CssClass = "PointCell",
                Text = point.divisionName
            };
            cell.Attributes["onclick"] = "selectThisRow(this)";
            if (IsSelected)
                cell.ForeColor = Color.Red;

            row.Cells.Add(cell);

            var query = "SELECT [objID], [name] FROM [_ApplicationState]";
            var dt = new DataTable();
            var da = new SqlDataAdapter(query, ConString);
            da.Fill(dt);
            var StatesList = new DropDownList()
            {
                ID = string.Format("stateList{0}", point.position)
            };
            StatesList.Items.Add(new ListItem("не выбрано", "-1"));
            StatesList.DataSource = dt;
            StatesList.AppendDataBoundItems = true;
            StatesList.DataTextField = "name";
            StatesList.DataValueField = "objID";
            StatesList.DataBind();
            StatesList.SelectedValue = point.stateID;

            cell = new TableCell();
            cell.Controls.Add(StatesList);
            row.Cells.Add(cell);

            return row;
        }

        private void FillList()
        {
            var query = string.Format("SELECT [objID], [name] FROM [{0}]", hasWork ? "_Division" : "Division");
            var da = new SqlDataAdapter(query, ConString);
            var dt = new DataTable();
            da.Fill(dt);
            InstanceList.DataSource = dt;
            InstanceList.DataTextField = "name";
            InstanceList.DataValueField = "objID";
            InstanceList.DataBind();
        }

        protected void SavePoints()
        {
            for (int i = 1; i < tableRoute.Rows.Count; i++)
            {
                PointList[i - 1].stateID = (tableRoute.Rows[i].Cells[2].Controls[0] as DropDownList).SelectedValue;
            }
        }

        protected int GetSelectedItem()
        {
            SavePoints();

            for (int i = 1; i < tableRoute.Rows.Count; i++)
            {
                if ((tableRoute.Rows[i].Cells[0].Controls[0] as CheckBox).Checked)
                    return i - 1;
            }

            return -1;
        }

        protected void includeButton_Click(object sender, EventArgs e)
        {
            int pos;
            List<RoutePoint> list = new List<RoutePoint>();
            int index = -1;

            if(PointList.Count > 0)
                index = GetSelectedItem();

            if (index < 0)
                pos = PointList.Count + 1;
            else
                pos = PointList[index].position + 1;

            foreach (ListItem item in InstanceList.Items)
            {
                if (item.Selected)
                {
                    var point = new RoutePoint()
                    {
                        ID = "-1",
                        divisionID = item.Value,
                        divisionName = item.Text,
                        stateID = "-1",
                        typeID = TypesList.SelectedValue,
                        position = pos
                    };
                    list.Add(point);
                    pos++;
                }
            }

            if (index >= 0)
            {
                for (int i = index + 1; i < PointList.Count; i++)
                {
                    PointList[i].position = PointList[i].position + list.Count;
                }
            }

            PointList = PointList.Concat(list).ToList();
            DataBind();
            //FillList();
        }

        protected void excludeButton_Click(object sender, EventArgs e)
        {
            if (PointList.Count == 0)
                throw new ArgumentNullException("Маршрут не содержит инстанций");

            var index = GetSelectedItem();

            if (index < 0)
                throw new ArgumentNullException("Необходимо выбрать удаляемый элемент");

            PointList.RemoveAt(index);
            for (int i = index; i < PointList.Count; i++)
            {
                PointList[i].position--;
            }

            PointList = PointList.OrderBy(x => x.position).ToList();
            DataBind();
            //FillList();
        }

        protected void saveButton_Click(object sender, EventArgs e)
        {
            if (PointList.Count == 0)
                throw new ArgumentNullException("Маршрут не содержит инстанций");

            SavePoints();
            
            foreach (var item in PointList)
            {
                if (string.IsNullOrEmpty(item.stateID) || item.stateID == "-1")
                    throw new ArgumentNullException("Не для всех точек указано состояние");
            }

            var con = new SqlConnection(ConString);
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            
            var query = string.Format("UPDATE [_RoutePoint] SET [isArchived] = 'true' WHERE [typeID] = {0}", TypesList.SelectedValue);
            cmd.CommandText = query;
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            foreach (var item in PointList)
            {
                item.savePointToDB();
            }
            Storage.ClearBusinessContents();
            TypesList.SelectedValue = "-1";
            MView.ActiveViewIndex = -1;
        }

        protected void cancelButton_Click(object sender, EventArgs e)
        {
            TypesList.SelectedValue = "-1";
            MView.ActiveViewIndex = -1;
        }

        protected void upButton_Click(object sender, EventArgs e)
        {
            if (PointList.Count == 0)
                throw new ArgumentNullException("Маршрут не содержит инстанций");

            var index = GetSelectedItem();

            if (index < 0)
                throw new ArgumentNullException("Необходимо выбрать элемент");

            PointList[index].position--;
            PointList[index - 1].position++;
            PointList = PointList.OrderBy(x => x.position).ToList();
            DataBind();
        }

        protected void downButton_Click(object sender, EventArgs e)
        {
            if (PointList.Count == 0)
                throw new ArgumentNullException("Маршрут не содержит инстанций");

            var index = GetSelectedItem();

            if (index < 0)
                throw new ArgumentNullException("Необходимо выбрать элемент");

            PointList[index].position++;
            PointList[index + 1].position--;
            PointList = PointList.OrderBy(x => x.position).ToList();
            DataBind();
        }
    }
}