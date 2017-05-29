using ArgumentException = System.ArgumentException;
using System.Collections.Generic;
using System.Linq;
using XElement = System.Xml.Linq.XElement;
using System;

namespace Teleform.Reporting.Parsers
{
    public class ReportParser<T> : IParser where T : Report
    {
        private IParser parser;
        private TemplateParser templateParser;
        private InstanceParser instanceParser;
        private Schema schema;

        public ReportParser(Schema schema)
        {
            this.parser = this;
            this.schema = schema;
            templateParser = new TemplateParser();
        }


        private Entity GetEntity(object id)
        {
            foreach (var item in schema.Entities)
                if (item.ID.ToString() == id.ToString())
                    return item;

            return null;
        }
       
        object IParser.Parse(XElement e)
        {
            if (e == null)
                throw new ArgumentNullException("e", Message.Get("Common.NullArgument", "e"));

            var templateElement = e.Element("template");

            if (templateElement == null)
                throw new ArgumentException(Message.Get("Xml.NoElement", "template", e), "e");

            var template = templateParser.Parse(templateElement);
#if f
            // Проверка элемента инстанса.
            var instancesElement = e.Element("instances");

            if (instancesElement == null)
                throw new ArgumentException(Message.Get("Xml.NoElement", "instances", e), "e"); 


            // Получение элемента инстанса.
            var instanceElements = instancesElement.Elements("instance");

#endif

            var instanceElements = e.Elements("instance");
            if (instanceElements.Count() == 0)
                throw new ArgumentException(Message.Get("Xml.NoElements", "instance", e), "e");

            if (typeof(T) == typeof(GroupReport))
            {
                var instances = new List<Instance>();

                foreach (var instanceElement in instanceElements)
                {
                    instanceParser = new InstanceParser(template);
                    instances.Add(instanceParser.Parse(instanceElement));
                }

                return new GroupReport(template, instances);
            }
            else
#warning Неясное сообщение исключения.
                throw new InvalidOperationException("В ReportParser был передан неверный тип объекта.");
        }

        public T Parse(XElement e)
        {
            return (T) parser.Parse(e);
        }
    }
}
