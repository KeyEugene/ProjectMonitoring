#define entityR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Teleform.Reporting.EntityFilters;

namespace Teleform.Reporting
{
    [Serializable()]
    public class EntityFilter : IUniquelyDeterminedObject
    {

        public event EventHandler Changed;

        public void AcceptChanges()
        {
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }

        public object ID { get; private set; }
        public string Name { get; set; }
        public string UserID { get; set; }
        public Entity Entity { get; private set; }
        public EntityFilterFieldCollection Fields { get; private set; }

        public EntityFilter(object id, string name, string userID, Entity entity, IEnumerable<EntityFilterField> fields)
        {
            ID = id;
            Name = name;
            UserID = userID;
            Entity = entity;
            Fields = new EntityFilterFieldCollection(fields);

            // Storage.Save(this);
        }

        public string GetFilterExpression()
        {
            List<string> filterExpression = new List<string>();
            foreach (var item in Fields)
            {
                if (!string.IsNullOrEmpty(item.TechPredicate))
                    filterExpression.Add(item.TechPredicate.Replace("#a", item.AttributeID));
            }

            return string.Join(" AND ", filterExpression);
        }
    }
}
