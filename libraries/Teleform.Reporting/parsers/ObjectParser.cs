using ArgumentException = System.ArgumentException;
using ArgumentNullException = System.ArgumentNullException;
using XElement = System.Xml.Linq.XElement;

namespace Teleform.Reporting.Parsers
{
    public class ObjectParser
    {
        protected void ParseObject(XElement e, out object id, out string name)
        {
            if (e == null)
                throw new ArgumentNullException("e", Message.Get("Common.NullArgument", "e"));

            var idAttribute = e.Attribute("id");

            if (idAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "id", e), "e");

            var nameAttribute = e.Attribute("name");

            if (nameAttribute == null)
                throw new ArgumentException(Message.Get("Xml.NoAttribute", "name", e), "e");

            id = idAttribute.Value;
            name = nameAttribute.Value;
        }
    }
}
