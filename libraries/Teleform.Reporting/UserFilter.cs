

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Teleform.Reporting
{
    public class UserFilter : IUserDeterminedObject, IUniquelyDeterminedObject
    {
        public UserFilter(object id, int userID, DataTable table)
        {
            ID = id;
            Table = table;
            UserID = userID;
        }

        public object ID { get; set; }

        public int UserID { get; set; }

        public DataTable Table { get; private set; }
        
        public event EventHandler Changed;

        public string GetUserFilterExpression()
        {            
            if (Table.Rows.Count > 0)
            {
                var SecurityFilterList = new List<string>();
                for (int i = 0; i < Table.Rows.Count; i++)
                {
                    var predicate = Table.Rows[i]["filterPredicate"].ToString();
                    var hash = Table.Rows[i]["hash"].ToString();

                    if (!string.IsNullOrEmpty(predicate) && !string.IsNullOrEmpty(hash))
                    {                       
                        var techPredicate = predicate.Replace("#a", hash);
                        SecurityFilterList.Add(techPredicate);
                    }

                   
                }
                return string.Join(" AND ", SecurityFilterList);
            }
            else
                return "";
        }

        public void AcceptChanges()
        {
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }      

       
    }
}
