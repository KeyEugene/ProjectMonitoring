using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Teleform.DocumentModel
{
    partial class Route
    {
        partial class Point
        {
            public partial class Description
            {
                public IEnumerable<Property> Properties { get; private set; }

                public Description(IEnumerable<Property> properties)
                {
                    Properties = properties ?? Enumerable.Empty<Property>();
                }
            }
        }
    }
}
