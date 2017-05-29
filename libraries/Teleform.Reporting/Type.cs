

using System;
using System.Linq;
using System.Collections.Generic;

namespace Teleform.Reporting
{
    /// <summary>
    /// Список с типами
    /// </summary>
    public enum TypeDescription
    {
        Unknown = 0,
        Number = 1,
        Date = 2,
        Time = 4,
        String = 8,
        DateTime = Date | Time,
        Email,
        Telephone,
        Logic = 256,
        Float = 512
    }
    [Serializable()]
    /// <summary>
    /// Представляет тип атрибута.
    /// </summary>
    public sealed class Type
    {
        private IEnumerable<Format> accessibleFormats;
        private IEnumerable<Operator> accessibleOperators;
        private IEnumerable<AggregateFunction> accessibleAggregateFunctions;
        private long minValue, maxValue;

        private static IDictionary<string, Type> types = new Dictionary<string, Type>();

        /// <summary>
        /// Возвращает набор доступных к настоящему времени типов.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="ArgumentNullException">Параметр name имеет значение null.</exception>
        /// <exception cref="InvalidOperationException">Тип с указанным именем не существует.</exception>
        public static Type GetType(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name", "message is here");

            if (!types.Keys.Contains(name))
                throw new InvalidOperationException("Не удалось найти тип с именем '{0}'.");
            else return types[name];
        }
        /// <summary>
        /// Возвращает формат по умолчания для бизнес типа
        /// </summary>
        public Format DefaultFormat { get; private set; }

        /// <summary>
        /// Возвращает имя текущего объекта.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Возвращает CLR - тип текущего объекта.
        /// </summary>
        public string RuntimeType { get; private set; }

        /// <summary>
        /// Возвращает значение, указывающее, что текущий тип является числовым.
        /// </summary>
        [Obsolete("Использовать свойство Description.", true)]
        public bool Numeric { get; private set; }

        public TypeDescription Description { get; private set; }

        public string Pattern { get; private set; }

        public string PatternDescription { get; private set; }

        public int? Length { get; private set; }

        public int? ViewLength { get; private set; }

        public long MaxValue
        {
            get
            {
                //if (!Numeric)
                //    throw new InvalidOperationException("Невозможно получить максимальное значение типа, поскольку он не является численным.");

                return maxValue;
            }
        }

        public long MinValue
        {
            get
            {
                //if (!Numeric)
                //    throw new InvalidOperationException("Невозможно получить минимальное значение типа, поскольку он не является численным.");

                return minValue;
            }
        }

        /// <summary>
        /// Инициализирует объект типа Teleform.Reporting.Type.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <param name="accessibleFormats">Коллекция поддерживаемых форматов.</param>
        /// <exception cref="ArgumentNullException">Значения параметра name или accessibleFormats суть null.</exception>
        /// <exception cref="ArgumentException">Параметр accessibleFormats представляет пустую коллекцию.</exception>
        public Type(string name, string runtimeType, IEnumerable<Format> accessibleFormats, IEnumerable<Operator> accessibleOperators, IEnumerable<AggregateFunction> accessibleAggregateFunctions,
            TypeDescription typeDescription, long minValue, long maxValue, string pattern, string patternDescription,
            Format defaultFormat, int? length, int? viewLength)
        {
            if (name == null)
                throw new ArgumentNullException("name", "message is here");

            if (runtimeType == null)
                throw new ArgumentNullException("clrType", string.Format("Для типа {0} не заполнено поле runTimeTypeID в таблице [Typing].[BusinessType]", name));

            if (accessibleFormats == null)
                throw new ArgumentNullException("accessibleFormats",
                    string.Format("Коллекция доступных форматов типа '{0}' не является допустимой.", name));

            if (accessibleFormats.Count() == 0)
                throw new ArgumentException(
                    string.Format("Коллекция доступных форматов типа '{0}' пуста.", name),
                    "accessibleFormats");

            Description = typeDescription;

            //Numeric = Description.HasFlag(TypeDescription.Number);

            this.minValue = minValue;
            this.maxValue = maxValue;

            Pattern = pattern;
            PatternDescription = patternDescription;
            Length = length;
            ViewLength = viewLength;

            Name = name;
            RuntimeType = runtimeType;

            if (defaultFormat == null)
                DefaultFormat = new Format(0, "General", "", "", null, "{0}", null);
            else DefaultFormat = defaultFormat;

            this.accessibleFormats = new List<Format>(accessibleFormats);

            if (accessibleOperators != null)
                this.accessibleOperators = accessibleOperators.OrderBy(o => o.Order).ToList();
            else
                this.accessibleOperators = Enumerable.Empty<Operator>();

            if (accessibleAggregateFunctions != null)
                this.accessibleAggregateFunctions = new List<AggregateFunction>(accessibleAggregateFunctions);
            else
                this.accessibleAggregateFunctions = Enumerable.Empty<AggregateFunction>();

            if (!types.Keys.Contains(name))
                types.Add(name, this);
        }

        /// <summary>
        /// Возвращает набор поддерживаемых данным типом форматов.
        /// </summary>
        /// <returns>Коллекция поддерживаемых форматов.</returns>
        public IEnumerable<Format> GetAdmissableFormats()
        { return accessibleFormats; }

        /// <summary>
        /// Возвращает набор поддерживаемых данным типом операторов.
        /// </summary>
        /// <returns>Коллекция поддерживаемых операторов.</returns>
        public IEnumerable<Operator> GetAdmissableOperators()
        { return accessibleOperators; }

        /// <summary>
        /// Возвращает набор поддерживаемых данным типом агрегационных функций.
        /// </summary>
        /// <returns>Коллекция поддерживаемых агрегационных функций.</returns>
        public IEnumerable<AggregateFunction> GetAdmissableAggregateFunctions()
        { 
            return accessibleAggregateFunctions;
        }
    }
}