

using ArgumentNullException = System.ArgumentNullException;
using ArgumentException = System.ArgumentException;
using System.Collections.Generic;
using System.Linq;
using System;
using Teleform.Reporting.constraint;
//using Teleform.Reporting.EntityFilters;

namespace Teleform.Reporting
{
    /// <summary>
    /// Описывает сущность в схеме.
    /// </summary>
    [Serializable()]
    public sealed class Entity : UniquelyDeterminedObject
    {
        private readonly List<Attribute> attributes;
        /// <summary>
        /// Возвращает набор атрибут, связанных с текущей сущностью.
        /// </summary>
        public IEnumerable<Attribute> Attributes { get; private set; }
        /// <summary>
        /// Возвращает значение, указывающее является ли данная сущность основной.
        /// </summary>
        [System.Obsolete("Временное поле.")]
        public bool IsMain { get; private set; }

        public int AncestorID { get; private set; }

        public string SystemName { get; set; }

        /// <summary>
        /// Возвращает значение, указывающее на то, что текущий тип сущности является перечнем.
        /// </summary>
        public bool IsEnumeration { get; private set; }

        public bool IsHierarchic { get; private set; }
        /// <summary>
        /// Возвращает констрейнты данной сущности
        /// </summary>
        public IEnumerable<Constraint> Constraints { get; private set; }

        public IEnumerable<ListConstraint> Lists { get; private set; }

        /// <summary>
        /// Инициализирует объекти типа Teleform.Reporting.Entity.
        /// </summary>
        /// <param name="id">Уникальный идентификатор сущности.</param>
        /// <param name="systemName">Имя сущности.</param>
        /// <param name="isMain">Значение, указывающее является ли данная сущность основной.</param>
        /// <param name="attributes">Список, связанных с текущей сущностью атрибутов.</param>
        /// <exception cref="ArgumentNullException">Одно из значений параметров id, name, attributes равно null.</exception>
        /// <exception cref="ArgumentException">Параметр name имеет недопустимое значение для объектов данного типа.</exception>
        /// <exception cref="ArgumentException">Набор атрибутов сущности пуст.</exception>
        public Entity(object id, string alias, string systemName, bool isMain, int ancestorID, IEnumerable<Attribute> attributes, IEnumerable<Constraint> constraints,
           IEnumerable<ListConstraint> lists, bool isHierarchic = false, bool isEnumeration = false)
            : base(id, alias)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes",
                    Message.Get("Common.NullArgument", this, "attributes"));

            IsEnumeration = isEnumeration;

            IsHierarchic = isHierarchic;

            if (attributes.Count() == 0)
                throw new ArgumentException(Message.Get("Common.EmptyCollection", this, "attributes"),
                    "attributes");

            IsMain = isMain;

            AncestorID = ancestorID;

            SystemName = systemName;

            this.Attributes = attributes = new List<Attribute>(attributes);
            this.Constraints = constraints = new List<Constraint>(constraints);
            this.Lists = lists = new List<ListConstraint>(lists);

            
            Storage.Save(this);
        }

        public override string ToString()
        {
            return Name;
        }

        public void RemoveAttribute(Attribute attribute)
        {
            attributes.Remove(attribute);
        }

        public override bool Equals(object o)
        {
            return o is Entity && (o as Entity).ID.ToString() == ID.ToString();
        }

        public override int GetHashCode()
        {
            return System.Convert.ToInt32(ID);
        }
    }
}
