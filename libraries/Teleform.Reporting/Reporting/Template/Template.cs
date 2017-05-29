#define Viktor

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Teleform.Reporting.Reporting.Template;

namespace Teleform.Reporting
{
    [Serializable()]
    public class Template : IUniquelyDeterminedObject, ICloneable
    {


        protected IDictionary<string, string> parameters;
        private string fileName;

        public event EventHandler Changed;

        public void AcceptChanges()
        {
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }

        //public int UserID { get; set; }

        public Template Clone()
        {
            return (Template)(this as ICloneable).Clone();
        }

        object ICloneable.Clone()
        {

            var fields = Fields.Select(f => f.Clone());


            return new Template(this.ID, this.Name, this.Entity, this.fileName, this.TypeName, this.TypeCodeString, this.TemplateByDefault, 
                this.TreeTypeEnum, this.Content, fields, this.parameters);
        }


        public string GetFilterExpressionByAttrID()
        {
            List<string> filterExpression = new List<string>();

            foreach (var item in Fields)
            {
                if (!string.IsNullOrEmpty(item.Predicate))
                {
                    if (item.ListAttributeAggregation != null)
                    {
                        if (!string.IsNullOrEmpty(item.ListAttributeAggregation.AggregateLexem))
                            filterExpression.Add(item.Predicate.Replace("#a", item.HashName));
                        else
                            filterExpression.Add(item.Predicate.Replace("#a", item.Attribute.ID.ToString()));
                    }
                    else
                        filterExpression.Add(item.Predicate.Replace("#a", item.Attribute.ID.ToString()));
                }
            }
            return string.Join(" AND ", filterExpression);
        }


        public string GetFilterExpressionByAttrAlias()
        {
            List<string> filterExpression = new List<string>();

            foreach (var item in Fields)
            {
                if (!string.IsNullOrEmpty(item.Predicate))
                    filterExpression.Add(item.Predicate.Replace("#a", item.Attribute.Name));
            }
            return string.Join(" AND ", filterExpression);
        }

        public object ID { get; set; }

        [Obsolete("Может возвращать имя файла по умолчанию.")]
        public string FileName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    if (!string.IsNullOrWhiteSpace(Name))
                        fileName = Name;
                    else fileName = "отчет";
                }

                return fileName;
            }
            set { fileName = value; }
        }

        public string Name { get; set; }



        public string TypeName { get; set; }

        public EnumTypeCode TypeCode { get; set; }

        private string typeCodeString;

        public string TypeCodeString
        {
            get
            {
                return typeCodeString;
            }
            set
            {
                typeCodeString = value;

                if (typeCodeString == "screenTree")
                    TypeCode = EnumTypeCode.screenTree;
                else if (typeCodeString == "crossReport")
                    TypeCode = EnumTypeCode.crossReport;
                else if (typeCodeString == "WordBased")
                    TypeCode = EnumTypeCode.WordBased;
                else if (typeCodeString == "ExcelBased")
                    TypeCode = EnumTypeCode.ExcelBased;
                else if (typeCodeString == "TableBased")
                    TypeCode = EnumTypeCode.TableBased;
                else if (typeCodeString == "InputExcelBased")
                    TypeCode = EnumTypeCode.InputExcelBased;
                else
                    TypeCode = EnumTypeCode.Undefined;
            }
        }


        public byte[] Content { get; set; }

        public Entity Entity { get; private set; }

        /// <summary>
        /// Флаг который говорит, что этот шаблон по умолчанию 
        /// (Шаблон по умолчанию может быть только один)
        /// </summary>
        public bool TemplateByDefault { get; set; }

        /// <summary>
        /// Вид древовидного отчета (Обычный древовидный = 1 = "General", агрегация по ветке = 2 = "Branch", Агрегация по дочерним объектам = 3 = "Children"
        /// </summary>
        public EnumTreeType TreeTypeEnum { get; set; }


        public TemplateFieldCollection Fields { get; private set; }


        public IDictionary<string, string> Parameters
        {
            get
            {
                if (parameters == null)
                    parameters = new Dictionary<string, string>();

                return parameters;
            }
        }



        public Template(string name, Entity entity, string typeCode, byte[] content, IEnumerable<TemplateField> fields = null, object id = null)
        {
            if (name == null)
                throw new ArgumentNullException("name", Message.Get("Common.NullName"));

            Name = name;
            Entity = entity;
            TypeCodeString = typeCode;
            Content = content;

            if (id != null)
                ID = id;

            if (fields == null)
                Fields = new TemplateFieldCollection();
            else
                Fields = new TemplateFieldCollection(fields);
        }

#if Viktor
        public Template(object id, string name, Entity entity, string fileName, string typeName, string typeCode, bool templateByDefault,
             EnumTreeType treeType, byte[] content, IEnumerable<TemplateField> fields, IDictionary<string, string> parameters = null)
            : this(name, entity, typeCode, content, fields)
        {
            if (id == null)
                throw new ArgumentNullException("id", Message.Get("Common.NullIdentifier"));

            ID = id;

            //UserID = userID;

            FileName = fileName;
            TypeName = typeName;
            this.TemplateByDefault = templateByDefault;
            this.TreeTypeEnum = treeType;
            this.parameters = parameters;
        }
#else
        public Template(object id, string name, Entity entity, string fileName, string typeName, string typeCode, bool templateByDefault,
             EnumTreeType treeType, byte[] content, IEnumerable<TemplateField> fields, IDictionary<string, string> parameters = null)
            : this(name, entity, typeCode, content, fields)
        {
            if (id == null)
                throw new ArgumentNullException("id", Message.Get("Common.NullIdentifier"));

            ID = id;

            //UserID = userID;

            FileName = fileName;
            TypeName = typeName;
            this.TemplateByDefault = templateByDefault;
            this.TreeTypeEnum = treeType;
            this.parameters = parameters;
        }
#endif



    }
}
