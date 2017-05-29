using System;
using System.Collections.Generic;
using System.Text;

namespace Teleform.SqlServer.Formatting
{
   [Serializable()] //Если что редактировал Алексей К.
    public class BooleanFormat : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (format == "G" && arg != null && arg is bool)
            {
                var b = (bool)arg;

                return b ? "да" : "нет";
            }

            return null;
        }
    }
}
