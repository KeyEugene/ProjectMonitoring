using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using System.Data.SqlClient;
using System.Data;

namespace Teleform.Reporting
{
    public static class StorageUserObgects
    {
        private static Dictionary<System.Type, List<IUserDeterminedObject>> objects = new Dictionary<System.Type, List<IUserDeterminedObject>>();

        private static int _userID { get; set; }

        public static T Select<T>(int userID, object id) where T : IUserDeterminedObject
        {
            _userID = userID;

            var type = typeof(T);

            var tuple = new Tuple<System.Type, int, object>(type, _userID, id.ToString());

            lock (tuple)
            {
                if (!objects.ContainsKey(type) || !objects[type].Exists(o => o.ID.ToString() == id.ToString() && o.UserID == _userID))
                    CreateInstance(type, id);

                return (T)objects[type].Find(o => o.ID.ToString() == id.ToString() && o.UserID == _userID);
            }
        }

        private static IUserDeterminedObject CreateInstance(System.Type type, object id)
        {
            if (type == typeof(UserFilter))
            {
                using (var c = new SqlConnection(Storage.ConnectionString))
                {
                    var query = String.Format("SELECT hash, filterPredicate FROM Permission.UserFilter({0})", id);
                    var dt = Storage.GetDataTable(query);

                    var instance = new UserFilter(id, _userID, dt);

                    instance.Changed += new EventHandler(instance_Changed);

                    Save(instance);

                    return instance;
                }
            }
            else if (type == typeof(UserEntityPermission))
            {
                using (var c = new SqlConnection(Storage.ConnectionString))
                {
                    var query = string.Format(@"select iif(t.alias is NULL,'Шаблон',t.alias) typeAlias, b.object_ID as entityID, p.* from Permission.UserPermission({0},NULL)p 
                                                left join model.BTables b on b.name=p.entity
                                                left join model.AppTypes t on t.object_id=b.appTypeID
                                                where  p.objID is NULL
                                                order by p.entityAlias", id);

                    var dt = Storage.GetDataTable(query);

                    dt.Rows.Remove(dt.AsEnumerable().First(x => x["entity"].ToString() == "R$Template"));
                    dt.AcceptChanges();
                    var entityPermission = dt;

                    var instance = new UserEntityPermission(id, _userID, entityPermission);

                    instance.Changed += new EventHandler(instance_Changed);

                    Save(instance);

                    return instance;
                }
            }
            else
            {
                throw new InvalidOperationException(
                      string.Format("Текущий провайдер не обеспечивает выборку элементов типа {0}.", type.FullName));
            }
        }

        private static void Save(IUserDeterminedObject o)
        {
            var type = o.GetType();
            
            if (!objects.ContainsKey(type))
                objects.Add(type, new List<IUserDeterminedObject>());

            objects[type].Add(o);
        }

        public static void ClearAllCache()
        {
            objects.Clear();
        }
        
       

        private static void ClearInstanceCache(System.Type type, int userID, object id)
        {
            if (objects.ContainsKey(type))
            {
                var list = objects[type];

                foreach (var o in list)
                {
                    if (o.ID.ToString() == id.ToString() && o.UserID == _userID)
                    {
                        if (type == typeof(UserFilter))
                        {
                            var userFilter = o as UserFilter;
                            userFilter.AcceptChanges();
                            break;
                        }                    
                        else if (type == typeof(UserEntityPermission))
                        {
                            var entityPermission = o as UserEntityPermission;
                            entityPermission.AcceptChanges();
                            break;
                        }
                        else
                        {
                            throw new InvalidOperationException(string.Format("Текущий провайдер не обеспечивает выборку элементов типа {0}.", type.FullName));
                        }
                    }
                }
            }
        }


        private static void instance_Changed(object sender, EventArgs e)
        {
            if (sender is UserFilter)
            {
                var instance = sender as UserFilter;
                objects[typeof(UserFilter)].Remove(instance);
            }          
            else if (sender is UserEntityPermission)
            {
                var instance = sender as UserEntityPermission;
                objects[typeof(UserEntityPermission)].Remove(instance);
            }
            
        }
    }
}
