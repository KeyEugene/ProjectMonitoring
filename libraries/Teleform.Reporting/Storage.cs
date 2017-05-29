#define Alex



using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Web;
using System.Data.SqlClient;
using System.Data;

using StringReader = System.IO.StringReader;
using XElement = System.Xml.Linq.XElement;
using Teleform.Reporting.Parsers;
using System.Configuration;



namespace Teleform.Reporting
{
    public static class Storage
    {
        private static string connectionString;

        public static string archivatorPath { get; set; }


        public static void LogOutUser()
        {
            var Session = System.Web.HttpContext.Current.Session;

            var SystemUser = Session["SystemUser"];

            string query = string.Format("UPDATE [Log].[ServerSession] SET [finish] = GETDATE() WHERE [sessionID] = '{0}' AND [finish]  is null ", SystemUser);
            GetDataTable(query);

            StorageUserObgects.ClearAllCache();

            Session.Abandon();
            Session.Clear();

            HttpContext.Current.Cache.Remove(Session.SessionID);
            
        }


        [Obsolete("Отключать когда везем к заказчику")]
        public static string ConnectionString
        {
            get
            {
                if (System.Web.HttpContext.Current != null)
                {
#warning // Отключать когда везем к заказчику
                   System.Web.HttpContext.Current.Session["ConnectionString"] = string.Format(ConfigurationManager.ConnectionStrings["Server"].ConnectionString, "sa", "345");
                   System.Web.HttpContext.Current.Session["SystemUser.objID"] = "0";


                    var constring = System.Web.HttpContext.Current.Session["ConnectionString"];

                    if (constring == null)
                    {
                        throw new NullReferenceException("Необходимо войти в систему");
                    }
                    else
                    {
                        return System.Web.HttpContext.Current.Session["ConnectionString"].ToString();
                    }


                  

                    
                }
                else
                    return connectionString;
            }
            set
            {
                if (System.Web.HttpContext.Current != null)
                    System.Web.HttpContext.Current.Session["ConnectionString"] = value;
                else
                    connectionString = value;

            }
        }


        //private static Dictionary<System.Type, List<IUniquelyDeterminedObject>> objects = new Dictionary<System.Type, List<IUniquelyDeterminedObject>>();
        public static Dictionary<System.Type, List<IUniquelyDeterminedObject>> objects = new Dictionary<System.Type, List<IUniquelyDeterminedObject>>();


        public static T Select<T>(object id) where T : IUniquelyDeterminedObject
        {
            var type = typeof(T);

            var tuple = new Tuple<System.Type, string>(type, id.ToString());

            lock (tuple)
            {
                if (!objects.ContainsKey(type) || !objects[type].Exists(o => o.ID.ToString() == id.ToString()))
                    CreateInstance(type, id);

                return (T)objects[typeof(T)].Find(o => o.ID.ToString() == id.ToString());
            }

        }

        public static Entity SelectEntityByName(string name)
        {
            var type = typeof(Entity);

            var entities = objects[type];

            foreach (var item in entities)
            {
                if (((Entity)item).SystemName == name)
                    return (Entity)item;
            }
            return null;
        }

        private static IUniquelyDeterminedObject CreateInstance(System.Type type, object id)
        {
            if (type == typeof(Template))
            {
                if (id.ToString().Contains("AttributesTemplate"))
                {
                    var instance = RunTimeTemplates.GetRuntimeTemplate(id.ToString());

                    addRemoveTypes(type, id);

                    objects[type].Add(instance);

                    instance.Changed += new EventHandler(instance_Changed);

                    return instance;
                }
                else
                {
                    using (var c = new SqlConnection(ConnectionString))
                    {
                        var query = String.Format("SELECT [Model].[XMLTemplate]({0})", id);
                        var reader = ExecuteScalarStringReader(query);
                        var e = XElement.Load(reader);
                        var parser = new TemplateParser();
                        var instance = parser.Parse(e);

                        addRemoveTypes(type, id);

                        objects[type].Add(instance);

                        instance.Changed += new EventHandler(instance_Changed);

                        return instance;
                    }
                }
            }
            else if (type == typeof(EntityFilter))
            {
                using (var c = new SqlConnection(ConnectionString))
                {
                    var query = String.Format("SELECT model.xmlEntityFilter({0})", id);
                    var reader = ExecuteScalarStringReader(query);

                    var e = XElement.Load(reader);
                    var parser = new EntityFilterParser();

                    var instance = parser.Parse(e);

                    addRemoveTypes(type, id);

                    objects[type].Add(instance);

                    instance.Changed += new EventHandler(instance_Changed);

                    return instance;
                }
            }
            else if (type == typeof(BusinessContent))
            {
                var entity = Storage.Select<Entity>(id);

                using (var c = new SqlConnection(ConnectionString))
                {
                    var query = String.Format("EXEC [report].[getBObjectData] @baseTable={0}, @flFormat = 0, @flheader = 0", id);

                    var dt = GetDataTable(query);

                    var instance = new BusinessContent(id, entity.Name, dt);

                    addRemoveTypes(type, id);

                    objects[type].Add(instance);

                    instance.Changed += new EventHandler(instance_Changed);

                    BusinessContentIsChanged = true;

                    return instance;
                }
            }
            else
            {
                throw new InvalidOperationException(
                       string.Format("Текущий провайдер не обеспечивает выборку элементов типа {0}.", type.FullName));
            }
        }


        public static void ClearAllCache()
        {
            objects.Clear();

            BusinessContentIsChanged = true;
        }

        public static void ClearBusinessContents()
        {
            var type = (typeof(BusinessContent));

            var listUniqObj = new List<IUniquelyDeterminedObject>();

            objects.TryGetValue(type, out listUniqObj);

            if (listUniqObj != null)
                listUniqObj.Clear();

            BusinessContentIsChanged = true;

        }

        /// <summary>
        /// Чистит все объекты у заданого типа и заданого entityID
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entityID"></param>
        public static void CleareTypeByEntityID(System.Type type, string entityID)
        {
            if (objects.ContainsKey(type))
            {
                var list = objects[type];
                switch (type.Name)
                {
                    case "Template":
                        for (int i = 0; i < list.Count; i++)
                        {
                            if ((list.ElementAt(i) as Template).Entity.ID.ToString() == entityID)
                            {
                                list.RemoveAt(i);
                                i--;
                            }
                        }
                        break;
                    default:
                        throw new InvalidOperationException(
                        string.Format("Текущий провайдер не обеспечивает выборку элементов типа {0}.", type.FullName));
                }
            }
        }
        public static void ClearInstanceCache(System.Type type, object id)
        {
            var tuple = new Tuple<System.Type, string>(type, id.ToString());

            lock (tuple)
            {
                if (objects.ContainsKey(type))
                {
                    var list = objects[type];

                    foreach (var o in list)
                        if (o.ID.ToString() == id.ToString())
                        {
                            if (type == typeof(BusinessContent))
                            {
                                var businessContent = o as BusinessContent;
                                businessContent.AcceptChanges();
                                break;
                            }
                            else if (type == typeof(Template))
                            {
                                var template = o as Template;
                                template.AcceptChanges();
                                break;
                            }
                            else if (type == typeof(EntityFilter))
                            {
                                var entityFilter = o as EntityFilter;
                                entityFilter.AcceptChanges();
                                break;
                            }
                            else
                            {
                                throw new InvalidOperationException(
                                       string.Format("Текущий провайдер не обеспечивает выборку элементов типа {0}.", type.FullName));
                            }
                        }

                }
            }
        }

        public static bool BusinessContentIsChanged { get; set; }

        static void instance_Changed(object sender, EventArgs e)
        {
            if (sender is Template)
            {
                var template = sender as Template;
                objects[typeof(Template)].Remove(template);
            }
            else if (sender is EntityFilter)
            {
                var EntityFilter = sender as EntityFilter;
                objects[typeof(EntityFilter)].Remove(EntityFilter);
            }
            else if (sender is BusinessContent)
            {
                var businessContent = sender as BusinessContent;
                objects[typeof(BusinessContent)].Remove(businessContent);
                BusinessContentIsChanged = true;
            }
        }

        private static void addRemoveTypes(System.Type type, object id)
        {
            if (!objects.ContainsKey(type))
                objects.Add(type, new List<IUniquelyDeterminedObject>());
            else
                objects[type].RemoveAll(o => o.ID.ToString() == id.ToString());

        }


        public static void Save(IUniquelyDeterminedObject o)
        {
            var type = o.GetType();

            if (!objects.ContainsKey(type))
                objects.Add(type, new List<IUniquelyDeterminedObject>());
            objects[type].Add(o);
        }


        public static void ExecuteNonQueryXML(string query, string xml)
        {
            try
            {
                using (var c = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand(query, c))
                {
                    cmd.Parameters.Add("xml", DbType.String).Value = xml;
                    c.Open();

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Не удалось выполнить действие, {0}", ex.Message));
            }

            
        }

        public static DataTable GetDataTable(string query)
        {
            try
            {
                var da = new SqlDataAdapter(query, ConnectionString);
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {

                throw new Exception(string.Format("Не удалось выполнить действие, {0}", ex.Message));
            }
        }

        public static StringReader ExecuteScalarStringReader(string query)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                try
                {
                    var result = new StringReader(cmd.ExecuteScalar().ToString());
                    return result;
                }
                catch (NullReferenceException)
                {
                    throw new Exception(string.Format("Не удалось сохранить значения в таблицу"));
                }
            }
        }

        public static string ExecuteScalarString(string query)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                try
                {
                    var result = cmd.ExecuteScalar().ToString();
                    return result;

                }
                catch (NullReferenceException)
                {
                    throw new Exception(string.Format("Не удалось сохранить значения в таблицу"));
                }
            }
        }



    }
}
