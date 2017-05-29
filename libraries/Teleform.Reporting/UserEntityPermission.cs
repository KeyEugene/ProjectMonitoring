using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Teleform.Reporting
{
    /// <summary>
    /// Содержит инфо о разрешениях пользователя для всех entity
    /// </summary>
    public class UserEntityPermission : IUserDeterminedObject, IUniquelyDeterminedObject
    {
        public object ID { get; set; }

        public int UserID { get; set; }

        public DataTable Permission { get; set; }

        public event EventHandler Changed;

        public UserEntityPermission(object id, int userID, DataTable permission)
        {
            this.ID = id;
            this.UserID = userID;
            this.Permission = permission;
        }

        public DataTable getReadDeniedEntities()
        {
            var deny = Permission.AsEnumerable().Where(x => !Convert.ToBoolean(x["read"]));
            
            return deny.Count() == 0 ? new DataTable() : deny.CopyToDataTable();
        }


        public DataTable getReadPermittedEntities()
        {
            var permiss = Permission.AsEnumerable().Where(x => Convert.ToBoolean(x["read"]));
            return permiss.Count() == 0 ? new DataTable() : permiss.CopyToDataTable();
        }

        public DataTable getCreatePermittedEntities()
        {
            var permiss = Permission.AsEnumerable().Where(x => Convert.ToBoolean(x["create"]));
            return permiss.Count() == 0 ? new DataTable() : permiss.CopyToDataTable();
        }

        public DataTable getUpdatePermittedEntities()
        {
            var permiss = Permission.AsEnumerable().Where(x => Convert.ToBoolean(x["update"]));
            return permiss.Count() == 0 ? new DataTable() : permiss.CopyToDataTable();
        }

        public DataTable getDeletePermittedEntities()
        {
            var permiss = Permission.AsEnumerable().Where(x => Convert.ToBoolean(x["delete"]));
            return permiss.Count() == 0 ? new DataTable() : permiss.CopyToDataTable();
        }

        public void AcceptChanges()
        {
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }      
    }
}
