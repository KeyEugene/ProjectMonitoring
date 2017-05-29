#define Alex

using ArgumentNullException = System.ArgumentNullException;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Teleform.Reporting.constraint;
//using Teleform.ProjectMonitoring.HttpApplication;

namespace Teleform.Reporting
{

    /// <summary>
    /// Представляет поле шаблона.
    /// </summary>
    [Serializable()]
    public sealed class TemplateField : IField, ICloneable, IComparable<TemplateField>
    {

        public string NativeEntityName { get; set; }

        public bool IsForbidden { get; set; }

        public string TemplateID { get; set; }

        /// <summary>
        /// Возвращает или задаёт видимое поле в шаблоне.
        /// </summary>
        public bool IsVisible { get; set; }


        private string name;

        private IEnumerable<Teleform.Reporting.Attribute> attr;

        public object ID { get; set; }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    name = value;
                else name = Attribute.Name;
            }
        }

        public string HashName { get; set; }

        /// <summary>
        /// Возвращает или задаёт формат вывода данных в текущее поле.
        /// </summary>
        public Format Format { get; set; }


        /// <summary>
        /// Возвращает атрибут, который представляет текущее поле шаблона.
        /// </summary>
        public Attribute Attribute { get; private set; }


        /// <summary>
        /// Возвращает или задаёт предустановленный предикат для фильтрации.
        /// </summary>
        public string Predicate { get; set; }

        /// <summary>
        /// Возвращает или задаёт внутреннюю информацию о предикате.
        /// </summary>
        public string PredicateInfo { get; set; }

        /// <summary>
        /// Возвращает или задаёт порядковый номер текущего поля шаблона.
        /// </summary>
        public int Order { get; set; }

        public string Aggregation { get; set; }

        public AggregateFunction AggregationFunction { get; set; }

       


      

        /// <summary>
        /// проверяем списковый ли это аттрибут
        /// </summary>
        public bool isListAttribute { get; set; }

        /// <summary>
        /// Возвращает или задаёт уровень поля шаблона.
        /// </summary>
        public int Level { get; set; }


        /// <summary>
        /// Возвращает тип элемента для перекрестного отчета
        /// </summary>
        public int CrossTableRoleID { get; set; }



        public ListAttributeAggregation ListAttributeAggregation { get; set; }

        public int CompareTo(TemplateField field)
        {
            return Order - field.Order;
        }

        public Constraint Constraint { get; set; }

        
        /// <summary>
        /// Создает объект типа Teleform.Reporting.TemplateField.
        /// </summary>
        /// <param name="attribute"></param>
        public TemplateField(Attribute attribute)
        {
            Attribute = attribute;
#warning нужно исправить?
            Format = new Format(0, "Общий", "", "", null, "{0}", null);
            Name = string.Empty;
            //HashName = string.Empty;

            //Operation = "0";
            //Filter = string.Empty;
            Predicate = string.Empty;
            Aggregation = string.Empty;
            PredicateInfo = string.Empty;
            IsVisible = true;
            Level = 1;
            //CrossTableRoleID;
            ListAttributeAggregation = Attribute.IsListAttribute ? new ListAttributeAggregation(string.Empty, string.Empty) : null;
           //GetConstraintByAttributeFpath();
        }

        /// <summary>
        /// Инициализирует объект типа Teleform.Reporting.TemplateField.
        /// </summary>
        /// <param name="attribute">Атрибут, который представляет текущее поле шаблона.</param>
        /// <param name="format">Определяет формат вывода данных в текущее поле.</param>
        /// <param name="alias">Псевдоним текущего поля шаблона.</param>
        public TemplateField(object id, string nativeEntityName, string name, string hashName, Attribute attribute, Format format, 
            int level, int order, string aggregation, string predicate, string predicateInfo, 
            bool isVisible, ListAttributeAggregation listAttributeAggregation, int crossTableRoleID, string templateID) //object filter, string operation,
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            if (format == null)
                throw new ArgumentNullException("format");

            PredicateInfo = predicateInfo;

            Aggregation = aggregation;
            Attribute = attribute;


            AggregationFunction = GetAggregationFunctions();

            Format = format;

            ID = id;

            NativeEntityName = nativeEntityName;

            Name = name;
            HashName = hashName;

            Order = order;
            Level = level;            

          //  Operation = operation;
          //  Filter = filter;

            Predicate = predicate;

            IsVisible = isVisible;

            ListAttributeAggregation = listAttributeAggregation;

            CrossTableRoleID = crossTableRoleID;

            TemplateID = templateID;

            //GetConstraintByAttributeFpath();
        }

     

        private AggregateFunction GetAggregationFunctions()
        {
            var tmpType = Attribute.Type.Name;

            if (!string.IsNullOrWhiteSpace(Aggregation))
            {
                if (tmpType == "Table")
                {
                    var type = Type.GetType("int");

                    return type.GetAdmissableAggregateFunctions().FirstOrDefault(f => f.Lexem.ToUpper() == Aggregation.ToUpper());
                }
                else if (tmpType != "Table")
                {
                    return Attribute.Type.GetAdmissableAggregateFunctions().FirstOrDefault(f => f.Lexem.ToUpper() == Aggregation.ToUpper());
                }
            }

            return null;
        }

        public TemplateField Clone()
        {
            return (TemplateField)(this as ICloneable).Clone();
        }

        object ICloneable.Clone()
        {
            return new TemplateField(this.ID, this.NativeEntityName, this.Name, this.HashName, this.Attribute, this.Format,
                this.Level, this.Order, this.Aggregation, this.Predicate, this.PredicateInfo,
                this.IsVisible, this.ListAttributeAggregation, this.CrossTableRoleID, this.TemplateID); // this.Filter, this.Operation,
        }
    }
}