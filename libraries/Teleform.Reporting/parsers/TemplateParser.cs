#define Alex

using Convert = System.Convert;
using ArithmeticException = System.ArithmeticException;
using ArgumentException = System.ArgumentException;
using XElement = System.Xml.Linq.XElement;
using System.Data;

using System.Collections.Generic;
using System.Linq;
using Teleform.Reporting.Reporting.Template;


namespace Teleform.Reporting.Parsers
{
    public class TemplateParser : ObjectParser, IParser
    {
        private IParser parser;
        private TemplateFieldParser templateFieldParser;

        public TemplateParser()
        {
            parser = this;
        }

        object IParser.Parse(XElement e)
        {
            string name;
            object id;
            byte[] content;
            Entity entity;
            bool templateByDefault;
            EnumTreeType treeType;

            ParseObject(e, out id, out name);

            var fileNameAttribute = e.Attribute("fileName");
            if (fileNameAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "fileName", e), "e");

            var typeNameAttribute = e.Attribute("typeName");
            if (typeNameAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "typeName", e), "e");

            var typeCodeAttribute = e.Attribute("typeCode");
            if (typeCodeAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "typeCode", e), "e");

            var entityIDAttribute = e.Attribute("entityID");
            if (entityIDAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "entityID", e), "e");

            entity = Storage.Select<Entity>(entityIDAttribute.Value);

            var templateByDefaultAttribute = e.Attribute("templateByDefault");
            if (templateByDefaultAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "entityID", e), "e");

            templateByDefault = templateByDefaultAttribute.Value == "1" ? true : false;


            var treeTypeIDAttribute = e.Attribute("templateByDefault");
            if (treeTypeIDAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "entityID", e), "e");

            treeType = GetTreeTypeID(e.Attribute("treeTypeID").Value);

            List<TemplateField> fields = new List<TemplateField>();

            var fieldsElement = e.Element("fields");

            if (fieldsElement != null)
            {
                var fieldElements = fieldsElement.Elements("field");

                if (fieldElements.Count() == 0)
                    throw new ArgumentException(Message.Get("Xml.NoElements", "field", e), "e");

                templateFieldParser = new TemplateFieldParser(entity);




                fields = fieldElements.Select(o => templateFieldParser.Parse(o)).ToList();


                


            }

            var contentNode = e.Element("content");

            if (contentNode == null)
                throw new ArgumentException(Message.Get("Xml.NoElement", "content", e), "content");

            try
            {
                content = Convert.FromBase64String(contentNode.Value);
            }
            catch
            {
#warning Использую ArithmeticException.
                throw new ArithmeticException("Не удалось преобразовать тело документа к массиву байт.");
            }

            var sheetAttribute = e.Attribute("sheet");
            Dictionary<string, string> parameters = null;

            if (sheetAttribute != null)
            {
                parameters = new System.Collections.Generic.Dictionary<string, string>();
                parameters["sheet"] = sheetAttribute.Value;
            }

            return new Template(id, name, entity, fileNameAttribute.Value, typeNameAttribute.Value, typeCodeAttribute.Value, templateByDefault, treeType, content, fields, parameters);
        }

        private EnumTreeType GetTreeTypeID(string value)
        {
            if (string.IsNullOrEmpty(value) || value == "0")
                return EnumTreeType.Undefined;
            else if (value == "1")
                return EnumTreeType.General;
            else if (value == "2")
                return EnumTreeType.Branch;
            else if (value == "3")
                return EnumTreeType.Children;

            return EnumTreeType.Undefined;
        }



        public Template Parse(XElement e)
        {
            return (Template)parser.Parse(e);
        }
    }
}
