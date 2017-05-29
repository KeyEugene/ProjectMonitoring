using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Teleform.DocumentModel
{
    public static class XElementExtensions
    {
        // Summary:
        //     Gets or sets the name of this element.
        //
        // Returns:
        //     An System.Xml.Linq.XName that contains the name of this element.
        public static void Expected(this XElement e, string expectedName)
        {
            if (e.Name != expectedName)
                throw new InvalidOperationException(
                    string.Format(
                        "Ожидался xml-элемент '{0}'. Вместо получили '{1}'.",
                        expectedName,
                        e.Name
                ));
        }
        //
        // Summary:
        //     Gets the first (in document order) child element with the specified System.Xml.Linq.XName.
        //
        // Parameters:
        //   name:
        //     The System.Xml.Linq.XName to match.
        //
        // Returns:
        //     A System.Xml.Linq.XElement that matches the specified System.Xml.Linq.XName,
        //     or null
        public static XElement ExpectedElement(this XElement e, string expectedElement)
        {
            var element = e.Element(expectedElement);

            if (element != null)
                return element;

            throw new InvalidOperationException(
                string.Format(
                    "В следующем xml-элементе отсутствует ожидаемый подэлемент '{0}':\n{1}.",
                    expectedElement,
                    e
            ));
        }

        public static string GetAttributeValue(this XElement e, string expectedAttribute)
        {
            var attribute = e.Attribute(expectedAttribute);

            if (attribute != null)
                return attribute.Value;

            throw new InvalidOperationException(
                string.Format(
                    "В следующем xml-элементе отсутствует ожидаемый атрибут '{0}':\n{1}.",
                    expectedAttribute,
                    e
            ));
        }
    }
}
