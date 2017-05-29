using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting.DynamicCard
{
    public static class SomeExtensions
    {    

        public static bool IsEmpty(this object value)
        {
            return value == null || value is DBNull || string.IsNullOrWhiteSpace(value.ToString());
        }
    }
}
