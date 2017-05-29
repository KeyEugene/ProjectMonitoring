using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlTypes;

namespace Teleform.SqlServer.Formatting
{
    [Serializable()]
    public class MoneyFormat : IFormatProvider, ICustomFormatter
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
            if (format == "W")
                return MoneyInWords.GetMoneyInWords(arg);
            else if (format == "T")
            {
                if (arg is DBNull)
                    return string.Empty;

                string value;

                if (arg is Int32)
                {
                    var i = (Int32)arg;

                    value = (i / 1000).ToString();
                }
                else if (arg is Int64)
                {
                    var i = (Int64)arg;

                    value = (i / 1000).ToString();
                }
                else if (arg is Decimal)
                {
                    var i = (Decimal)arg;

                    value = (i / 1000).ToString("F2");
                }
                else if (arg is SqlMoney)
                {
                    var i = (SqlMoney)arg;

                    value = (i / 1000).ToString();
                }
                else throw new InvalidOperationException(string.Format
                (
                    "Нельзя отформатировать по тысячам тип '{0}'.",
                    arg.GetType()
                ));

                return value; // string.Concat(value, " тыс. р.");
            }
            else
                return null;
        }
    }
}
