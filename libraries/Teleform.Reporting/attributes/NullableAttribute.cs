using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting
{
#if f
    public class NullableAttribute : Attribute, INullableAttribute
    {
        public object DefaultValue { get; set; }
        public NullableBehaviour Behaviour { get; set; }

        public NullableAttribute(object id, string name, Type type)
            : base(id, name, null, type, Description.Base)
        {
        }
    }
#endif
}
