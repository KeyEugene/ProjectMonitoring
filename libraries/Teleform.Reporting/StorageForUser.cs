using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using System.Data.SqlClient;
using System.Data;

namespace Teleform.Reporting
{
    public static class StorageForUser
    {
        private static Dictionary<int, List<IUniquelyDeterminedObject>> objects = new Dictionary<int, List<IUniquelyDeterminedObject>>();

        private static int _userID { get; set; }

        public static T Select<T>(int userID, object id) where T : IUniquelyDeterminedObject
        {
            _userID = userID;

            var type = typeof(T);

            var tuple = new Tuple<int, object>(_userID, id.ToString());

            lock (tuple)
            {
                if (!objects.ContainsKey(_userID) || !objects[_userID].Exists(o => o.ID.ToString() == id.ToString()))
                    CreateInstance(type, id);

                return (T)objects[_userID].Find(o => o.ID.ToString() == id.ToString());
            }
        }

        private static IUniquelyDeterminedObject CreateInstance(System.Type type, object id)
        {
            if (type == typeof(UserFilter))
            {
                using (var c = new SqlConnection(Storage.ConnectionString))
                {
                    var query = String.Format("SELECT hash, filterPredicate FROM Permission.UserFilter({0})", id);
                    var dt = Storage.GetDataTable(query);

                    var instance = new UserFilter(id, _userID, dt);

                    //instance.Changed += new EventHandler(instance_Changed);

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

        private static void Save(IUniquelyDeterminedObject o)
        {
            if (!objects.ContainsKey(_userID))
                objects.Add(_userID, new List<IUniquelyDeterminedObject>());

            objects[_userID].Add(o);
        }

        public static void ClearAllUsersData()
        {
            objects.Clear();

            //if (objects.ContainsKey(_userID))
            //    objects.Remove(_userID);
        }
        

        #region возможно методы в регионе уже не понадобяться

#if trueWWW
        private static void DeleteInstance(System.Type type, int userID, object id)
        {
            if (objects.ContainsKey(userID))
            {
                var list = objects[userID];

                foreach (var o in list)
                {
                    if (o.ID.ToString() == id.ToString())
                    {
                        if (type == typeof(UserFilter))
                        {
                            var userFilter = o as UserFilter;
                            userFilter.AcceptChanges();
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
                var userFilter = sender as UserFilter;
                objects[userFilter.UserID].Remove(userFilter);
            }
            
        }
#endif
        #endregion




    }
}
