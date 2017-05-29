using ArgumentException = System.ArgumentException;
using XElement = System.Xml.Linq.XElement;

namespace Teleform.Reporting.Parsers
{
    public class FormatParser : ObjectParser, IParser
    {
        private IParser parser;

        public FormatParser()
        {
            parser = this;
        }

        public Format Parse(XElement e)
        {
            return (Format) parser.Parse(e);
        }

        object IParser.Parse(XElement e)
        {
            if (e.Name != "format")
                throw new ArgumentException();

            string name;
            object id;

            ParseObject(e, out id, out name);

            var descriptionAttribute = e.Attribute("description");

            if (descriptionAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "description", e), "e");

            string example = null;
            var exampleAttribute = e.Attribute("example");
#if f
            if (exampleAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "example", e), "e");
#else
            if (exampleAttribute != null)
                example = exampleAttribute.Value;
#endif
            string providerType = null;
            var providerAttribute = e.Attribute("provider");

            if (providerAttribute != null)
                providerType = providerAttribute.Value;

            string formatString = "{0}";
            var formatStringAttribute = e.Attribute("formatString");

            if (formatStringAttribute != null)
                formatString = formatStringAttribute.Value;

            string assembly = null;
            var assemblyAttribute = e.Attribute("assembly");

            if (assemblyAttribute != null)
                assembly = assemblyAttribute.Value;

            return new Format(id, name, descriptionAttribute.Value, example, providerType, formatString, assembly);
        }
    }
}
