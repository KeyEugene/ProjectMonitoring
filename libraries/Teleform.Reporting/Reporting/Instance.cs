using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting
{
    public class Instance
    {
        public Entity Entity { get; private set; }
        public IEnumerable<Property> Properties { get; private set; }

        public Instance(Entity entity, IEnumerable<Property> properties)
        {
            this.Entity = entity;
            this.Properties = new List<Property>(properties);
        }
    }
}
