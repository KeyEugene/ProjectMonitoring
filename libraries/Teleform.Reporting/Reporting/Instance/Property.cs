using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting
{
    partial class Instance
    {
        public class Property : IProperty
        {
            public object Value { get; set; }

            public Attribute Attribute { get; private set; }

            public Property(Attribute attribute)
            {
                if (attribute == null)
                    throw new ArgumentNullException("attribute", Message.Get("Common.NullArgument", "attribute"));

                Attribute = attribute;
            }

            public Property(Attribute attribute, object value)
                : this(attribute)
            {
                Value = value;
            }
        }
    }
}
