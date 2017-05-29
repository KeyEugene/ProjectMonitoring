using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting
{
    [Serializable()]
   public class User :IUniquelyDeterminedObject
    {
        public object ID { get; private set; }

        public string Name { get; set; }

        public IEnumerable<EntityFilter> EntityFilters { get; set; }

        public User(object id, string name, IEnumerable<EntityFilter> entityFilters)
        {
            ID = id;
            Name = name;
            EntityFilters = entityFilters;
        }
    }
}
