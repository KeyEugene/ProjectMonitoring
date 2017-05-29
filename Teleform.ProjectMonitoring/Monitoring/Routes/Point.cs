using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Teleform.ProjectMonitoring.HttpApplication;

namespace routes
{
    [Serializable()]
    public class Point
    {
        private string ConString
        {
            get { return Global.ConnectionString; }
        }

        public string ID;

        public string typeID;

        public string state;

        public string stateID;

        public string divisionID;

        public string name;

        public int position;

        public void InitPoint(string pointID)
        {
            var query = string.Format("SELECT a.[TypeID], a.[stateID], c.[name], a.[_divisionID], b.[name], a.[position] FROM [_RoutePoint] a, [_Division] b, [_ApplicationState] c WHERE a.[_divisionID] = b.[objID] AND c.[objID] = a.[stateID] AND a.[objID] = {0}", pointID);
            var da = new SqlDataAdapter(query, ConString);
            var dt = new DataTable();
            da.Fill(dt);

            ID = pointID;
            typeID = dt.Rows[0].ItemArray[0].ToString();
            stateID = dt.Rows[0].ItemArray[1].ToString();
            state = dt.Rows[0].ItemArray[2].ToString();
            divisionID = dt.Rows[0].ItemArray[3].ToString();
            name = dt.Rows[0].ItemArray[4].ToString();
            position = Convert.ToInt32(dt.Rows[0].ItemArray[5]);
        }
    }
}