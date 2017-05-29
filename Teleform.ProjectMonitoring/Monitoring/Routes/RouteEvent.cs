using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Teleform.ProjectMonitoring.HttpApplication;

namespace routes
{
    [Serializable()]
    public class RouteEvent
    {
        private string ConString
        {
            get { return Global.ConnectionString; }
        }

        public Application application;

        public RoutePoint point;

        public bool current;

        public bool done;

        public string date;

        public string dateR;

        public string ID;

        public void InitEvent(Application app, RoutePoint rPoint)
        {
            application = app;
            point = rPoint;

            var query = string.Format(@"SELECT [isCurrent], [isDone], [date], [dateR], [objID] FROM [_RouteApplication] WHERE [_routePointID] = {0} AND [_applicationID] = {1}",
                point.ID,
                application.ID);

            var da = new SqlDataAdapter(query, ConString);
            var dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 1)
            {
                current = dt.Rows[0].ItemArray[0] == DBNull.Value ? false : Convert.ToBoolean(dt.Rows[0].ItemArray[0]);
                done = dt.Rows[0].ItemArray[1] == DBNull.Value ? false : Convert.ToBoolean(dt.Rows[0].ItemArray[1]);
                date =  dt.Rows[0].ItemArray[2] == DBNull.Value ? string.Empty : dt.Rows[0].ItemArray[2].ToString().Split(' ')[0];
                dateR = dt.Rows[0].ItemArray[3] == DBNull.Value ? string.Empty : dt.Rows[0].ItemArray[3].ToString().Split(' ')[0];
                ID = dt.Rows[0].ItemArray[4].ToString();
            }
            else
            {
                current = false;
                done = false;
                date = string.Empty;
                dateR = string.Empty;
                ID = string.Empty;
            }
        }

        public void SaveStateToDB()
        {
            string saveQuery = "";
            string newDate = string.Empty;
            string newDateR = string.Empty;
            if (!string.IsNullOrEmpty(date))
            {
                //newDate = string.Format("'{0}'", date.Replace('-', '.'));
                newDate = string.Concat("'", date.Substring(6, 4), ".", date.Substring(3, 2), ".", date.Substring(0, 2), "'");
            }
            if (!string.IsNullOrEmpty(dateR))
            {
                newDateR = string.Concat("'", dateR.Substring(6, 4), ".", dateR.Substring(3, 2), ".", dateR.Substring(0, 2), "'");
            }

            #region
            //if(!string.IsNullOrEmpty(date))
            //    date = date.Substring(0, 10);
            //if (!string.IsNullOrEmpty(dateR))
            //    dateR = dateR.Substring(0, 10);
            //date = date.Replace(".", "/");
            //dateR = dateR.Replace(".", "/");
            #endregion

            var sb = new StringBuilder();
            if (string.IsNullOrEmpty(this.ID))
            {
                #region не то
                //                saveQuery = string.Format(@"INSERT INTO [_RouteApplication] ([_routePointID], [_applicationID], [typeID], [_workID], [current], [date], [dateR], [_divisionID],
                //                                            [done], [stateID]) VALUES({0}, {1}, {2}, {3}, '{4}', {5}, {6}, {7}, '{8}', {9})",
                //                                            point.ID, application.ID, application.typeID, application.workID, current, string.IsNullOrEmpty(newDate) ? "NULL" : newDate,
                //                                            string.IsNullOrEmpty(newDateR) ? "NULL" : newDateR, point.divisionID, done, point.stateID);
                #endregion

                var hasWork = Global.Schema.Entities.Where(x => x.SystemName == "_Application").ElementAt(0).Attributes.Where(a => a.FPath.ToLower().Contains("work")).Count() != 0;

                if (hasWork)
                    saveQuery = string.Format(@"INSERT INTO [_RouteApplication] ([_routePointID], [_applicationID], [typeID], [_workID], [isCurrent], [date], [dateR], [_divisionID],
                                            [isDone], [stateID]) VALUES({0}, {1}, {2}, {3}, '{4}', {5}, {6}, {7}, '{8}', {9})",
                                                point.ID, application.ID, application.typeID, application.workID, current, string.IsNullOrEmpty(newDate) ? "NULL" : newDate,
                                                string.IsNullOrEmpty(newDateR) ? "NULL" : newDateR, point.divisionID, done, point.stateID);
                else
                    saveQuery = string.Format(@"SET DATEFORMAT ymd;
                                            INSERT INTO [_RouteApplication] ([_routePointID], [_applicationID], [typeID], [isCurrent], [date], [dateR], [_divisionID],
                                            [isDone], [stateID]) VALUES({0}, {1}, {2}, '{3}', {4}, {5}, {6}, '{7}', {8})",
                                            point.ID, application.ID, application.typeID, current, string.IsNullOrEmpty(newDate) ? "NULL" : newDate,
                                            string.IsNullOrEmpty(newDateR) ? "NULL" : newDateR, point.divisionID, done, point.stateID);
            }
            else
            {
                saveQuery = string.Format(@"SET DATEFORMAT ymd;
                                            UPDATE [_RouteApplication] SET [isCurrent] = '{0}',
                                            [date] = {1}, [dateR] = {2}, [isDone] = '{3}' WHERE [objID]={4}",
                                            current, string.IsNullOrEmpty(newDate) ? "NULL" : newDate,
                                            string.IsNullOrEmpty(newDateR) ? "NULL" : newDateR, done, ID);
            }

            using (SqlConnection con = new SqlConnection(ConString))
            {
                using (SqlCommand saveCom = new SqlCommand(saveQuery))
                {
                    saveCom.Connection = con;
                    con.Open();
                    saveCom.ExecuteNonQuery();
                    con.Close();
                }
            }

        }
    }
}