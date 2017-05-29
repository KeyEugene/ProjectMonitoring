
#define UserFilterWWW
#define alexj

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


namespace Teleform.Reporting
{
    public class BusinessContent : IUniquelyDeterminedObject
    {

        public BusinessContent(object id, string name, DataTable table)
        {
            ID = id;
            Name = name;
            this.table = table;
        }


        public object ID { get; private set; }

        public string Name { get; private set; }


        private DataTable table;

        public DataTable GetTable(int userID)
        {
#if alexj
            var entity = Storage.Select<Entity>(ID);

            var query = string.Format("SELECT * FROM Permission.UserPermission({0}, '{1}') WHERE [objID] IS NOT NULL", userID, entity.SystemName);

            var InstanceNoPermission = Storage.GetDataTable(query).AsEnumerable().Where(x => !Convert.ToBoolean(x["read"])).Select(o => o["objID"].ToString()).ToList<string>();

            
            DataRow[] rows = this.table.AsEnumerable().Where(x => !InstanceNoPermission.Contains(x["objID"].ToString())).ToArray();
#else

            var userFilter = StorageUserObgects.Select<UserFilter>(userID, ID);
            var expression = userFilter.GetUserFilterExpression();

            DataRow[] rows = this.table.Select(expression);
#endif

            if (rows != null && rows.Count() > 0)
            {
                var filteredTable = rows.CopyToDataTable();
                return filteredTable;
            }
            else
            {
                var emtyTable = new DataTable();
                return emtyTable;
            }
        }


        public event EventHandler Changed;

        public void AcceptChanges()
        {
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }

    }
}
