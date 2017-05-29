using System;

namespace Teleform.Model.Types
{
    /// <summary>
    /// Представляет год.
    /// </summary>
    public class Year : IComparable<Year>, IComparable, IFormattable
    {
        /// <summary>
        /// Инициализирует объект типа Teleform.Model.Types.Year.
        /// </summary>
        /// <param name="value">Порядковый номер года.</param>
        public Year(int value)
        {
            if (value < 1900)
                throw new ArgumentOutOfRangeException("value", "");

            Value = value;
        }

        public int Value { get; set; }

        public int CompareTo(Year year)
        {
            return Value.CompareTo(year.Value);
        }

        public int CompareTo(object o)
        {
            if (o is Year)
            {
                var right = o as Year;

                return Value.CompareTo(right.Value);
            }
            else throw new ArgumentException("Аргумент должен иметь тип Teleform.Model.Types.Year.", "o");
        }

        public override bool Equals(object o)
        {
            if (o is Year)
            {
                var right = o as Year;

                return right == this;
            }
            else throw new ArgumentException("Аргумент должен иметь тип Teleform.Model.Types.Year.", "o");
        }

        public override int GetHashCode()
        { return base.GetHashCode(); }

        public static bool operator ==(Year left, Year right)
        {
            return left.Value == right.Value;
        }

        public static bool operator !=(Year left, Year right)
        {
            return left.Value != right.Value;
        }

        public static implicit operator Year(int value)
        {
            return new Year(value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public string ToString(string format)
        {
            return ToString(format, System.Globalization.CultureInfo.CurrentCulture);
        }

        public static Year operator +(Year left, Year right)
        {
            return new Year(2000);
        }
        
        [Obsolete("Параметр formatProvider не используется.")]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            var value = Value.ToString();

            if (string.IsNullOrEmpty(format))
                format = "I";

            switch (format)
            {
                case "I":
                    return value;
                case "Г":
                    return value + " г.";
                case "ВП":
                case "ИП":
                    return value + " год";
                case "ДП":
                    return value + " году";
                case "РП":
                    return value + " года";
                default:
                    throw new FormatException(string.Format("Форматная строка '{0}' не поддерживается.", format));
            }
        }
    }
}
