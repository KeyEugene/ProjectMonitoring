using ArgumentNullException = System.ArgumentNullException;
using ArgumentExcepiton = System.ArgumentException;
using System;

namespace Teleform.Reporting
{
    /// <summary>
    /// Описывает однозначно определяемый именованный объект.
    /// </summary>
    [Serializable()]
    public class UniquelyDeterminedObject : IUniquelyDeterminedObject
    {
        /// <summary>
        /// Возвращает имя объекта.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Возвращает уникальный идентификатор объекта.
        /// </summary>
        public object ID { get; private set; }

        /// <summary>
        /// Возвращает значение, указывающее является ли данное имя корректным для объектов данного типа.
        /// </summary>
        /// <param name="name">Проверяемое на корректность имя.</param>
        protected virtual bool IsNameCorrect(string name)
        { return true; }

        /// <summary>
        /// Инициализирует объект типа Teleform.Reporting.UniquelyDeterminedObject.
        /// </summary>
        /// <param name="id">Уникальный идентификатор объекта.</param>
        /// <param name="name">Имя объекта.</param>
        /// <exception cref="ArgumentNullException">Одно из значений параметров id или name.</exception>
        /// <exception cref="ArgumentException">Параметр name имеет недопустимое значение для объектов данного типа.</exception>
        public UniquelyDeterminedObject(object id, string name)
        {
            if (id == null)
                throw new ArgumentNullException("id", Message.Get("Common.NullIdentifier"));

            if (name == null)
                throw new ArgumentNullException("name", Message.Get("Common.NullName"));

            if (!IsNameCorrect(name))
                throw new ArgumentExcepiton(Message.Get("Common.InvalidName", this), "name");

            ID = id;
            Name = name;
        }

        /// <summary>
        /// Инициализирует объект типа Teleform.Reporting.UniquelyDeterminedObject через другой экземпляр объекта этого типа.
        /// </summary>
        /// <param name="another">Объект, на основе которого происходит инициализация.</param>
        public UniquelyDeterminedObject(UniquelyDeterminedObject another)
        {
            ID = another.ID;
            Name = another.Name;
        }
    }
}
