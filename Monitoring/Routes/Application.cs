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
    public class Application
    {
        private string ConString
        {
            get { return Global.ConnectionString; }
        }

        public bool hasWork;

        public string ID;

        public string workID;

        public string typeID;

        public string number;

        public void InitApplication(string appID)
        {
            ID = appID;
            string query;

            hasWork = Global.Schema.Entities.Where(x => x.SystemName == "_Application").ElementAt(0).Attributes.Where(a => a.FPath.ToLower().Contains("work")).Count() != 0;

            if (!hasWork)
                query = string.Format("SELECT [TypeID], [name] FROM [_Application] WHERE [objID] = {0}", ID);
            else
                query = string.Format("SELECT [TypeID], [number], [_workID] FROM [_Application] WHERE [objID] = {0}", ID);

            var dt = new DataTable();
            var da = new SqlDataAdapter(query, ConString);
            da.Fill(dt);

            if (hasWork)
                workID = dt.Rows[0].ItemArray[2].ToString();

            typeID = dt.Rows[0].ItemArray[0].ToString();
            number = dt.Rows[0].ItemArray[1].ToString();
        }
    }

}