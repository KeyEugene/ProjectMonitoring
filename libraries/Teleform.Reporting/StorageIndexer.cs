using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting
{
    public static class StorageIndexer
    {
        private static Dictionary<string, Entity> BusinessContentIndexer = new Dictionary<string, Entity>();

        public static void GetParentTbl(Entity entity)
        {
            string entitySysName = entity.SystemName;

            Entity value;
            BusinessContentIndexer.TryGetValue(entitySysName, out value);
            if (value == null)           
                BusinessContentIndexer.Add(entitySysName, entity);

                var parTblIDList = getParTblIDList(entity);

                foreach (var parTblID in parTblIDList)
                {
                    Storage.Select<BusinessContent>(parTblID);

                    var parentEntity = Storage.Select<Entity>(parTblID);

                    BusinessContentIndexer.TryGetValue(parentEntity.SystemName, out value);
                    if (value == null)
                        BusinessContentIndexer.Add(parentEntity.SystemName, parentEntity);
                }           

        }

        private static IEnumerable<string> getParTblIDList(Entity entity)
        {
            var parTblIDList = entity.Lists.Select(list => list.ParentTblID);

            return parTblIDList;
        }

     


    }
}
