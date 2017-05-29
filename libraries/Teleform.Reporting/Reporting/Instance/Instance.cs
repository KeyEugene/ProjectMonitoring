using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting
{
    public partial class Instance
    {
        private string fullName;

        public Entity Entity { get; private set; }

        public IEnumerable<Teleform.Reporting.Property> TelefReprotProperties { get; private set; }

        public IEnumerable<Instance.Property> OwnProperties { get; private set; }

        public Instance(Entity entity, IEnumerable<Teleform.Reporting.Property> properties)
        {
            this.Entity = entity;
            this.TelefReprotProperties = new List<Teleform.Reporting.Property>(properties);
        }

        public Instance(Entity entity, IEnumerable<Property> properties)
        {
            if (entity == null)
                throw new ArgumentNullException("entity", Message.Get("Common.NullArgument", "entity"));

            if (properties == null)
                throw new ArgumentNullException("properties", Message.Get("Common.NullArgument", "properties"));

            this.Entity = entity;

            OwnProperties = new List<Property>(properties);
        }

        /// <summary>
        /// Возвращает полное имя объекта.
        /// </summary>
        public string FullName
        {
            get
            {
                if (fullName == null)
                {
                    var namingProperties = OwnProperties.Where(o => (o.Attribute.Description & Description.Naming) != 0);

                    var s = new StringBuilder();

                    foreach (var property in namingProperties)
                    {
                        string part;

                        if (property.Value is DateTime)
                            part = ((DateTime) property.Value).ToShortDateString();
                        else part = property.Value.ToString();

                        s.AppendFormat("{0} ", part);
                    }

                    s.Length--;

                    fullName = s.ToString();
                }

                return fullName;
            }
        }

        
    }
}
