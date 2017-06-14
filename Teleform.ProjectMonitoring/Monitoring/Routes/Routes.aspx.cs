#define alexj

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using routes;
using Teleform.ProjectMonitoring.HttpApplication;

namespace Teleform.ProjectMonitoring.Routes
{
    public partial class Routes : BasePage
    {
        private string ConString
        {
            get { return Global.ConnectionString; }
        }

        private bool hasWork = Global.Schema.Entities.Where(x => x.SystemName == "_Application").ElementAt(0).Attributes.Where(a => a.FPath.ToLower().Contains("work")).Count() != 0;

        private List<Application> AppList
        {
            get { return Session["_AppList"] as List<Application>; }
            set { Session["_AppList"] = value; }
        }

        private List<RoutePoint> PointList
        {
            get { return Session["_PointList"] as List<RoutePoint>; }
            set { Session["_PointList"] = value; }
        }

        private List<RouteEvent> EventList
        {
            get { return Session["_EventList"] as List<RouteEvent>; }
            set { Session["_EventList"] = value; }
        }

        private Table routeTable;

        private RouteEvent routeEvent
        {
            get { return ViewState["_RouteEvent"] as RouteEvent; }
            set { ViewState["_RouteEvent"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //var a = Global.Schema.Entities.Where(x => x.SystemName.Contains("Route") || x.SystemName.Contains("Application"));
            Frame.UserControl_DocTypeList_SelectedIndexChanged += DocTypeList_SelectedIndexChanged;
            Frame.UserControl_BuildRouteButton_Click += BuildRouteButton_Click;

            if (!IsPostBack)
            {
                var userTypeID = Session["SystemUser.typeID"];

                if (Session["SystemUser.typeID"] == null )
                    throw new NullReferenceException("Тип пользователя не известен, надо войти в систему");                
                else if (userTypeID.ToString() != "1" || userTypeID.ToString() == "5" || userTypeID.ToString() == "2")
                    throw new Exception(string.Concat(userTypeID," таким типам пользователей доступ на эту страницу запрещен"));

                WorkPlaces.ActiveViewIndex = -1;
                //var queryTypes = "SELECT [objID], [name] FROM [_ApplicationType]";
                //var da = new SqlDataAdapter(queryTypes, ConString);
                //var dt = new DataTable();
                //da.Fill(dt);

                //TypesList.Items.Add(new ListItem()
                //{
                //    Text = "не выбрано",
                //    Value = "-1"
                //});
                //TypesList.DataSource = dt;
                //TypesList.DataTextField = "name";
                //TypesList.DataValueField = "objID";
                //TypesList.AppendDataBoundItems = true;
                //TypesList.DataBind();
            }
        }


        protected void TypesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //switch (TypesList.SelectedValue)
            //{
            //    case "-1":
            //        WorkPlaces.ActiveViewIndex = -1;
            //        break;
            //    default:
            //        PointList = null;
            //        AppList = null;
            //        EventList = null;
            //        TablePlace.Controls.Clear();
            //        FillRoute();
            //        FillLists();
            //        TablePlace.Controls.Clear();
            //        BuildRouteEvents();
            //        WorkPlaces.ActiveViewIndex = 0;
            //        break;
            //}
        }

        protected override void CreateChildControls()
        {
            //if (TypesList.SelectedValue != "-1")
            //{
            //    TablePlace.Controls.Clear();
            //    BuildRouteEvents();
            //}
            //base.CreateChildControls();
        }

        public override void DataBind()
        {
            CreateChildControls();
            //base.DataBind();
        }

        ///<summary>
        ///заполнить InstanceList, ApplicationList - только ApplicationList
        ///</summary>
        private void FillLists()
        {
            //заполнить список документов
            string queryApplication;
            if (Frame.DocTypeList.SelectedDataKey["objID"] == null) throw new Exception("Не выбран тип документа");
            else
            {
                if (hasWork)
                    queryApplication = string.Format("SELECT [objID], [number] FROM [_Application] WHERE [TypeID] = {0}", Frame.DocTypeList.SelectedDataKey["objID"]);
                else
                    queryApplication = string.Format("SELECT [objID], [name] FROM [_Application] WHERE [TypeID] = {0}", Frame.DocTypeList.SelectedDataKey["objID"]);

                var da = new SqlDataAdapter(queryApplication, ConString);
                var dt = new DataTable();
                da.Fill(dt);
                ApplicationList.DataSource = dt;
                if (hasWork)
                    ApplicationList.DataTextField = "number";
                else
                    ApplicationList.DataTextField = "name";

                ApplicationList.DataValueField = "objID";
                ApplicationList.DataBind();

                foreach (ListItem item in ApplicationList.Items)
                {
                    if (AppList != null && AppList.Find(x => x.ID == item.Value) != null)
                        item.Enabled = false;
                }
            }
        }

        ///<summary>
        ///заполнить PointList, AppList, EventList - сейчас только PointList
        ///</summary>
        private void FillRoute()
        {
            //заполнить список точек
            if (Frame.DocTypeList.SelectedDataKey["objID"] == null) throw new Exception("Не выбран тип документа");
            else
            {
                var queryPoints = string.Format(@"SELECT a.[objID], a.[stateID], c.[name], a.[_divisionID], b.[name], a.[position] FROM [_RoutePoint] a, [{1}] b, [_ApplicationState] c
                                            WHERE a.[_divisionID] = b.[objID] AND c.[objID] = a.[stateID] AND a.[TypeID] = {0} AND (a.[isArchived] is NULL OR a.[isArchived] = 'false')", Frame.DocTypeList.SelectedDataKey["objID"],
                                                hasWork ? "_Division" : "Division");
                var da = new SqlDataAdapter(queryPoints, ConString);
                var dt = new DataTable();
                da.Fill(dt);
                PointList = new List<RoutePoint>();
                foreach (DataRow item in dt.Rows)
                {
                    PointList.Add(new RoutePoint()
                    {
                        ID = item.ItemArray[0].ToString(),
                        stateID = item.ItemArray[1].ToString(),
                        state = item.ItemArray[2].ToString(),
                        divisionID = item.ItemArray[3].ToString(),
                        divisionName = item.ItemArray[4].ToString(),
                        position = Convert.ToInt32(item.ItemArray[5]),
                        //   typeID = TypesList.SelectedValue
                    });
                }
                PointList = PointList.OrderBy(x => x.position).ToList();
            }
        }

        ///<summary>
        ///Построить маршрут
        ///</summary>
        private void BuildRouteEvents()
        {
            if (PointList.Count == 0 || AppList == null) //AppList.Count == 0)
                return;
            TablePlace.Controls.Clear();

            routeTable = new Table()
            {
                ID = "routeTable",
                CssClass = "tblRoute"
            };
            routeTable.Rows.Add(BuildHeaderRow());

            foreach (var item in AppList)
            {
                routeTable.Rows.Add(BuildApplicationRow(item));
            }

            TablePlace.Controls.Add(routeTable);
        }

        ///<summary>
        ///построить строку заголовка
        ///</summary>
        private TableRow BuildHeaderRow()
        {
            var tr = new TableRow();
            var td = new TableCell()
            {
                Text = @"Инстанции <br /> \ <br /> Документы",
                HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center,
                CssClass = "headerCell"
            };
            tr.Cells.Add(td);

            var list = PointList.OrderBy(x => x.position);
            for (int i = 0; i < PointList.Count; i++)
            {
                td = new TableCell()
                {
                    CssClass = "headerCell"
                };
                var l = new Literal()
                {
                    Text = string.Format("{0};<br />{1}", list.ElementAt(i).state, list.ElementAt(i).divisionName)
                };

                td.Controls.Add(l);
                tr.Cells.Add(td);
            }

            var endCell = new TableCell()
            {
                CssClass = "endCell"
            };
            tr.Cells.Add(endCell);

            return tr;
        }

        ///<summary>
        ///построить строку документа
        ///</summary>
        private TableRow BuildApplicationRow(Application app)
        {
            System.Web.UI.WebControls.Image imgDoc;
            var tr = new TableRow();
            var td = new TableCell()
            {
                CssClass = "headerCell"
            };
#if alexj1
            td.Attributes["onclick"] = "SelectThisRow(this)";


            var chb = new CheckBox()
                {
                    CssClass = "lblDoc",
                    ID = string.Format("{0}chb",app.ID),
                    Checked = false
                };

            td.Controls.Add(chb);
#endif

            var l = new Literal()
            {
                Text = app.number
            };


            td.Controls.Add(l);

            var list = EventList.Where(x => x.application.ID == app.ID).ToList();

            int currentEventPosition = -1;
            if (list.FindIndex(x => x.current) != -1)
                currentEventPosition = list.First(x => x.current).point.position;

            if (list.Where(x => x.current == true).Count() == 0 && list.Where(x => x.done == true).Count() != list.Count)
            {
                imgDoc = new System.Web.UI.WebControls.Image()
                {
                    ImageUrl = @"../images/ImageDoc1.png",
                    CssClass = "imgDoc"
                };

                td.Controls.Add(imgDoc);
            }
            tr.Cells.Add(td);


            foreach (var item in list)
            {
                td = new TableCell()
                {
                    ID = string.Format("{0}_{1}", app.ID, item.point.ID),
                    CssClass = "docCell",
                    HorizontalAlign = HorizontalAlign.Right
                };

                var colorLabel = new TextBox()
                {
                    CssClass = "lblDoc",
                    ID = string.Format("{0}_{1}_lbl", app.ID, item.point.ID),
                    Text = "white"
                };

                td.Controls.Add(colorLabel);

                td.Attributes["appid"] = app.ID;
                td.Attributes["pointid"] = item.point.ID.ToString();

                if (item.done)
                {
                    td.Attributes["bgColor"] = "limegreen";
                    colorLabel.Text = "limegreen";
                }
                if (item.current)
                {
                    td.Attributes["bgColor"] = "yellow";
                    colorLabel.Text = "yellow";
                    imgDoc = new System.Web.UI.WebControls.Image()
                    {
                        ImageUrl = @"../images/ImageDoc1.png",
                        CssClass = "imgDoc"
                    };
                    td.Controls.Add(imgDoc);
                }
                if (item.point.position > currentEventPosition && !string.IsNullOrEmpty(item.dateR) && list.Where(x => x.done).Count() != list.Count)
                {
                    td.Attributes["bgColor"] = "tomato";
                    colorLabel.Text = "tomato";
                }

                var eventButton = new Button()
                {
                    ID = app.ID + item.point.ID
                };
                eventButton.Attributes["appid"] = app.ID;
                eventButton.Attributes["pointid"] = item.point.ID.ToString();
                eventButton.Click += new EventHandler(eventButton_Click);
                td.Controls.Add(eventButton);

                tr.Cells.Add(td);
            }

            var endCell = new TableCell()
            {
                HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center,
                CssClass = "endCell"
            };

            endCell.Attributes["bgColor"] = "white";

            if (list.Where(x => x.done).Count() == list.Count)
            {
                imgDoc = new System.Web.UI.WebControls.Image()
                {
                    ImageUrl = @"../images/ImageDoc1.png",
                    CssClass = "imgDoc"
                };
                endCell.Controls.Add(imgDoc);
            }

            tr.Cells.Add(endCell);

            return tr;
        }

        ///<summary>
        ///добавить доки/инстанции - только доки
        ///</summary>
        protected void includeButton_Click(object sender, EventArgs e)
        {
            SaveStates();
            if (routeTable != null)
                routeTable.Rows.Clear();

            if (PointList.Count == 0)
                return;

            if (AppList == null)
                AppList = new List<Application>();

            if (EventList == null)
                EventList = new List<RouteEvent>();

            foreach (ListItem item in ApplicationList.Items)
            {
                if (item.Selected)
                {
                    var a = new Application();
                    a.InitApplication(item.Value);

                    foreach (var routePoint in PointList)
                    {
                        var routeEvent = new RouteEvent();
                        routeEvent.InitEvent(a, routePoint);
                        EventList.Add(routeEvent);
                    }

                    AppList.Add(a);
                }
            }

            ApplicationFilterBox.Text = string.Empty;

            FillLists();

            DataBind();
            //BuildRoute();
        }

        ///<summary>
        ///сохранить состояния после drag'n'drop
        ///</summary>
        private void SaveStates()
        {
            if (routeTable == null || routeTable.Rows.Count == 0)
                return;

            for (int i = 1; i < routeTable.Rows.Count; i++)
            {
                for (int j = 1; j < routeTable.Rows[i].Cells.Count - 1; j++)
                {
                    var td = routeTable.Rows[i].Cells[j];
                    var clr = td.Controls[0] as TextBox;
                    var curEvent = EventList.FindIndex(x => x.application.ID == td.Attributes["appid"] && x.point.ID == td.Attributes["pointid"]);
                    switch (clr.Text)
                    {
                        case "yellow":
                            EventList[curEvent].done = false;
                            EventList[curEvent].current = true;
                            EventList[curEvent].dateR = string.Empty;
                            break;
                        case "limegreen":
                            EventList[curEvent].done = true;
                            EventList[curEvent].current = false;
                            if (string.IsNullOrEmpty(EventList[curEvent].dateR))
                            {
                                EventList[curEvent].dateR = System.DateTime.Now.ToShortDateString();
                            }
                            break;
                        case "tomato":
                            EventList[curEvent].done = false;
                            EventList[curEvent].current = false;
                            if (string.IsNullOrEmpty(EventList[curEvent].dateR))
                            {
                                EventList[curEvent].dateR = System.DateTime.Now.ToShortDateString();
                            }
                            break;
                        default:
                            EventList[curEvent].done = false;
                            EventList[curEvent].current = false;
                            EventList[curEvent].dateR = string.Empty;
                            break;
                    }
                }
            }
            this.DataBind();
        }

        ///<summary>
        ///сохранить маршрут
        ///</summary>
        protected void saveButton_Click(object sender, EventArgs e)
        {

            SaveStates();

            foreach (var item in EventList)
            {
                item.SaveStateToDB();
            }

            //TypesList.SelectedValue = "-1";
            WorkPlaces.ActiveViewIndex = -1;

        }

        ///<summary>
        ///открыть изменение события
        ///</summary>
        protected void eventButton_Click(object sender, EventArgs e)
        {
            SaveStates();
            var eventButton = sender as Button;
            routeEvent = EventList.First(x => x.application.ID == eventButton.Attributes["appid"] && x.point.ID == eventButton.Attributes["pointid"]);
            AppNumberBox.Text = routeEvent.application.number;
            PointNameBox.Text = routeEvent.point.divisionName;
            StateBox.Text = routeEvent.point.state;

            DateBox.Text = FormatDate(routeEvent.date);
            //DateBox.Attributes["defaultChecked"] = "true";
            //DateBox.Attributes["value"] = FormatDate(routeEvent.date);
            //DateBox.Attributes["value"] = FormatDate(routeEvent.date);

            DateRBox.Text = FormatDate(routeEvent.dateR);
            //DateRBox.Attributes["defaultChecked"] = "true";
            //DateRBox.Attributes["value"] = FormatDate(routeEvent.dateR);
            //DateBox.Attributes["value"] = FormatDate(routeEvent.date);

            WorkPlaces.ActiveViewIndex = 1;
        }

        public string FormatDate(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            if (s[2] == '.')
                return string.Format("{0}-{1}-{2}", s.Substring(6), s.Substring(3, 2), s.Substring(0, 2));
            else
                return string.Format("{0}.{1}.{2}", s.Substring(8, 2), s.Substring(5, 2), s.Substring(0, 4));
        }

        ///<summary>
        ///сохранить изменения события
        ///</summary>
        protected void saveStateButton_Click(object sender, EventArgs e)
        {
            routeEvent.date = FormatDate(DateBox.Text);
            //routeEvent.date = FormatDate(DateBox.Attributes["value"]);

            routeEvent.dateR = FormatDate(DateRBox.Text);
            //routeEvent.dateR = FormatDate(DateRBox.Attributes["value"]);

            var index = EventList.FindIndex(x => x.application.ID == routeEvent.application.ID && x.point.position == routeEvent.point.position);
            EventList[index] = routeEvent;
            BuildRouteEvents();
            WorkPlaces.ActiveViewIndex = 0;
        }

        ///<summary>
        ///отмена изменений события
        ///</summary>
        protected void cancelStateButton_Click(object sender, EventArgs e)
        {
            WorkPlaces.ActiveViewIndex = 0;
        }

        ///<summary>
        ///отмена работы с маршрутом
        ///</summary>
        protected void cancelButton_Click(object sender, EventArgs e)
        {
            //TypesList.SelectedValue = "-1";
            WorkPlaces.ActiveViewIndex = -1;
        }
        protected void DocTypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PointList = null;
            AppList = null;
            EventList = null;
            TablePlace.Controls.Clear();
            FillRoute();
            FillLists();
            TablePlace.Controls.Clear();
            BuildRouteEvents();
            WorkPlaces.ActiveViewIndex = 0;
        }

        protected void BuildRouteButton_Click(object sender, EventArgs e)
        {

        }
    }
}