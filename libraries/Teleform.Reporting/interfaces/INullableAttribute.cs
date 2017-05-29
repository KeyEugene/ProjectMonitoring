using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting
{
    public interface INullableAttribute
    {
        object DefaultValue { get; set; }
        NullableBehaviour Behaviour { get; set; }
    }
}
