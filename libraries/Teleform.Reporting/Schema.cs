using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting
{
    [Serializable]
    public class Schema
    {
        public IEnumerable<Entity> Entities { get; set; }
        private IDictionary<string, Type> types { get; set; }

        public Schema(IEnumerable<Entity> entities, IEnumerable<Type> types)
        {
            this.Entities = entities;
            this.types = new Dictionary<string, Type>(types.ToDictionary(o => o.Name, o => o));
        }

        public Type GetType(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name", "message is here");

            if (!types.Keys.Contains(name))
                throw new InvalidOperationException("Не удалось найти тип с именем '{0}'.");
            else return types[name];
        }
         
    }
}
