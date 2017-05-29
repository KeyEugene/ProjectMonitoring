using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting
{
    [Serializable()]
    public class EntityFilterItem
    {

        public EntityFilterItem(string hash, string predicate)
        {
            Hash = hash;
            Predicate = predicate;
        }
        public string Predicate { get; set; }
        public string Hash { get; set; } 
    }
}
