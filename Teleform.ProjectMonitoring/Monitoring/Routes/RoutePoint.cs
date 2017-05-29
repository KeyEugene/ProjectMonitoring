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
    public class RoutePoint
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

        public string divisionName;

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
            divisionName = dt.Rows[0].ItemArray[4].ToString();
            position = Convert.ToInt32(dt.Rows[0].ItemArray[5]);
        }

        public void savePointToDB()
        {
            var saveQuery = string.Empty;
            saveQuery = string.Format("INSERT INTO [_RoutePoint] ([stateID], [typeID], [_divisionID], [position], [isArchived]) VALUES ({0}, {1}, {2}, {3}, 'false')",
                                       stateID, typeID, divisionID, position);

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