using ArgumentNullException = System.ArgumentNullException;
using IFormatProvider = System.IFormatProvider;
using Activator = System.Activator;
using CultureInfo = System.Globalization.CultureInfo;
using System;
using System.Reflection;

namespace Teleform.Reporting
{
    [Serializable()]
    /// <summary>
    /// Описывает формат некоторго типа схемы.
    /// </summary>
    public class Format : UniquelyDeterminedObject
    {
        IFormatProvider provider;
        string providerType;

        /// <summary>
        /// Возвращает пример применения текущего формата.
        /// </summary>
        public string Example { get; private set; }

        /// <summary>
        /// Возвращает текст описания текущего формата.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Возвращает провайдер, осуществляющий форматирование.
        /// </summary>
        public IFormatProvider Provider
        {
            get
            {
                if (provider == null)
                {
                    if (!string.IsNullOrEmpty(providerType))
                    {
                        if (!string.IsNullOrEmpty(Assembly))
                        {
                            var assembly = System.Reflection.Assembly.Load(Assembly);

                            var type = assembly.GetType(providerType);
                            provider = Activator.CreateInstance(type) as IFormatProvider;
                        }
                        else
                            provider = Activator.CreateInstance(System.Type.GetType(providerType)) as IFormatProvider;
                    }
                    else provider = CultureInfo.CurrentCulture;
                }

                return provider;
            }
        }

        public string FormatString { get; private set; }

        private string Assembly { get; set; }

        /// <summary>
        /// Инициализирует объект типа Teleform.Reporting.Format.
        /// </summary>
        /// <param name="id">Уникальный идентификатор объекта.</param>
        /// <param name="name">Имя объекта.</param>
        /// <param name="example">Пример применения формата.</param>
        /// <param name="description">Текст описания формата.</param>
        /// <exception cref="ArgumentNullException">Одно из значений параметров id, name, description, example.</exception>
        /// <exception cref="ArgumentException">Параметр name имеет недопустимое значение для объектов данного типа.</exception>
        public Format(object id, string name, string description, string example, string providerType, string formatString, string assembly)
            : base(id, name)
        {
#if false
            if (example == null)
                throw new ArgumentNullException("example", Message.Get("Common.NullArgument", this, "example"));
#endif
            if (description == null)
                throw new ArgumentNullException("description", Message.Get("Common.NullArgument", this, "description"));

            if(!string.IsNullOrEmpty(example))
                Example = example;

            this.providerType = providerType;

            this.FormatString = formatString;

            Description = description;
            Assembly = assembly;
        }
    }
}
