#define Dasha
#define alexj

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting
{



    /// <summary>
    /// Представляет описание атрибута.
    /// </summary>
    public enum Description
    {
        /// <summary>
        /// Именующий атрибут.
        /// </summary>
        Naming = 1,

        /// <summary>
        /// Атрибут является закрытым для системы подготовки отчетов.
        /// </summary>
        Private = 2,

        /// <summary>
        /// Базовый атрибут. Возможно устарело
        /// </summary>
        //Base = 4
    }
   

    public enum AppType
    {
        objid,
        title,
        parentid,
        info
    }



    [Serializable()]
    public sealed class Attribute : UniquelyDeterminedObject
    {
        /// <summary>
        /// Возвращает набор поддерживаемых данным типом операторов.
        /// </summary>
        /// <returns>Коллекция поддерживаемых операторов.</returns>
        public IEnumerable<Operator> GetAccessibleOperators()
        {
            var list = Type.GetAdmissableOperators();

            var nullable = IsNullable;
            var isbase = IsBase;

            if (!IsNullable)
                return list;
            else
                return list.Concat(Operator.NullOperators);
        }



#if Dasha
        /// <summary>
        /// Возвращает атрибут по имени поля в списковом атрибуте, если имя колонки пустое, возвращаем списковый атрибут
        /// </summary>
        public Attribute GetAttributeByColumnName(string columnName)
        {
            if (string.IsNullOrEmpty(this.EntityID))
                throw new ArgumentNullException(string.Format("Аттрибут {0} не является списковым.", this.Name));

            if (string.IsNullOrEmpty(columnName))
                return this;

            var entity = Storage.Select<Entity>(this.EntityID);

            var attr = entity.Attributes.First(o => o.FPath == columnName);

            return entity.Attributes.First(o => o.FPath == columnName);
        }
#endif


        /// <summary>
        /// Возвращате системный тип атрибута
        /// </summary>
        public string SType { get; private set; }

        public bool IsComputed { get; private set; }

        public bool IsIdentity { get; private set; }

        //public bool IsIdentified { get; private set; }

        public AppType AppType { get; private set; }


        /// <summary>
        /// Возвращает физическое имя атрибута. Состоит из названия ссылок (полный путь)
        /// </summary>
        public string FPath { get; set; }

        /// <summary>
        /// возвращает название колонки
        /// </summary>
        public string Col { get; set; }


        public string AttrbuteSystemName { get; set; }

        /// <summary>
        /// Возвращает или задаёт значение, указывающее может ли текущий
        /// атрибут принимать null-значение.
        /// </summary>
        public bool  IsNullable { get; private set; }

        /// <summary>
        /// Возвращает описание атрибута.
        /// </summary>
        public Description Description { get; private set; }

        /// <summary>
        /// проверяет списковый ли это атрибут
        /// </summary>
        public bool IsListAttribute { get; private set; }

        /// <summary>
        /// возвращает идентификатор сущности спискового атрибута
        /// </summary>
        public string EntityID { get; private set; }

        /// <summary>
        /// возвращает сущность атрибута
        /// </summary>
        public Entity Entity { get; private set; }

        public Type Type { get; set; }

        /// <summary>
        /// если true то атрибут является титульным
        /// </summary>
        public bool Naming { get; private set; }

        //Description description,
        public Attribute(object id, string lPath, string col, string fPath, Type type, string sType,
            bool isBase,  Description description, AppType appType, bool isListAttribute,
            string entityID, bool isIdentity, bool isNullable, bool naming = false, bool isComputed = false)
            : base(id, lPath)
        {
            IsNullable = isNullable;

            IsComputed = isComputed;

            IsIdentity = isIdentity;        

            this.Type = type;

            SType = sType;

            Naming = naming;

            IsBase = isBase;

            //AttrbuteSystemName = attrbuteSystemName ?? name;

            FPath = fPath;

            Col = col;

            Description = description;

            AppType = appType;

            IsListAttribute = isListAttribute;

            EntityID = entityID;

            //if (!string.IsNullOrEmpty(EntityID))
            //    entity = Storage.Select<Entity>(EntityID);
        }



        /// <summary>
        /// Возвращает значение, указывающее, что текущий атрибут является базовым.
        /// </summary>
        [Obsolete("Возможно, следует заменить перечислением. Пока имеет всегда значение true.")]
        public bool IsBase { get; private set; }

        public override int GetHashCode()
        // { return AttrbuteSystemName.GetHashCode(); }
        { return FPath.GetHashCode(); }

        public override bool Equals(object o)
        {
            return o is Attribute && o.GetHashCode() == GetHashCode();
        }


    }
}
