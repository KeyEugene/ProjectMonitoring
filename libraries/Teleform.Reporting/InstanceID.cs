using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting
{
    /// <summary>
    /// Описывает уникальный идентификатор объекта.
    /// </summary>
    public class InstanceID
    {
        /// <summary>
        /// Возвращает внутреннее значение идентификатора объекта. 
        /// </summary>
        public IConvertible Value { get; private set; }

        /// <summary>
        /// Возвращает идентификатор несуществующего в источнике данных объекта.
        /// </summary>
        public static readonly InstanceID Unknown = new InstanceID(-1);

        /// <summary>
        /// Возвращает хэш-код текущего объекта.
        /// </summary>
        /// <returns>Хэш-код объекта.</returns>
        public override int GetHashCode()
        { return base.GetHashCode(); }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public InstanceID(IConvertible value)
        {
            Value = value;
        }

        public static bool operator ==(InstanceID id, int value)
        {
            return Convert.ToInt32(id.Value) == value;
        }

        public static bool operator !=(InstanceID id, int value)
        {
            return Convert.ToInt32(id.Value) != value;
        }

        public static bool operator ==(InstanceID id, string value)
        {
            return id.Value.ToString() == value;
        }

        public static bool operator !=(InstanceID id, string value)
        {
            return id.Value.ToString() != value;
        }
    }
}
