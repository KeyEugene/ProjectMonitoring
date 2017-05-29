// Teleform.SqlServer.Formatting, Version=1.0.1322.1703, Culture=neutral, PublicKeyToken=a4543ffffe07aa8d

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlTypes;
using System.Globalization;

namespace Teleform.SqlServer.Formatting
{
    [Serializable()]
    public class FormatProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentException"></exception> 
        /// <remarks></remarks>
        /// <returns></returns>
        public static string FormatExecutor(string provider, string format, object value)
        {
            if (value == DBNull.Value || value == null)
                return string.Empty;

            if (format == null)
                throw new ArgumentNullException("format", "Полученно несуществующее значение");

            IFormatProvider iProvider = null;
            if (provider == null)
                iProvider = CultureInfo.CurrentCulture;
            else
                iProvider = (IFormatProvider)Activator.CreateInstance(Type.GetType(provider));

            if (format.StartsWith("{"))
            {
                if (value is SqlMoney)
                    return string.Format(iProvider, format, ((SqlMoney)value).Value).Trim();
                else if (value is SqlInt16)
                    return string.Format(iProvider, format, ((SqlInt16)value).Value).Trim();
                else if (value is SqlInt32)
                    return string.Format(iProvider, format, ((SqlInt32)value).Value).Trim();
                else if (value is SqlInt64)
                    return string.Format(iProvider, format, ((SqlInt64)value).Value).Trim();
                else if (value is SqlDouble)
                    return string.Format(iProvider, format, ((SqlDouble)value).Value).Trim();
                else if (value is SqlDecimal)
                    return string.Format(iProvider, format, ((SqlDecimal)value).Value).Trim();
                else if (value is SqlDateTime)
                    return string.Format(iProvider, format, ((SqlDateTime)value).Value).Trim();

                return string.Format(iProvider, format, value).Trim();
            }
            else
                return value.ToString();//throw new ArgumentException("Данный формат является недопустимым.");
        }

    }
}
