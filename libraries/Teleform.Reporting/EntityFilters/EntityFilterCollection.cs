using System;
using System.Collections.Generic;

namespace Teleform.Reporting.EntityFilters
{
    [Serializable()]
    public class EntityFilterCollection
    {
        public readonly List<EntityFilterField> filters;

        public EntityFilterCollection(IEnumerable<EntityFilterField> Filters)
        {
            filters = new List<EntityFilterField>();

            if (Filters != null)
            {
                foreach (var item in Filters)
                {
                    filters.Add(item);
                }
            }
        }

        public EntityFilterField this[int index]
        {
            get { return filters[index]; }
            set
            {
                filters[index] = value;
            }

        }
    }
}
