using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Teleform.DocumentModel
{
    public class XmlSourceParser
    {
        public Source ParseSource(XElement e)
        {
            e.Expected("routeList");
          
            var routesElement = e.ExpectedElement("routes");

            return new Source
            {
              // Description = description,
               //Routes = e.Elements("route").Select(subElement => ParseProperty(subElement))
            };

        }

        private Route ParseRoute(XElement e)
        {
            e.Expected("route");

            throw new NotImplementedException();
        }

        private Route.Point ParsePoint(XElement e, Route.Point.Description description)
        {
            e.Expected("point");
           // e.Elements()
           
            throw new NotImplementedException();
        }

        private Route.Point.Description ParseDescription(XElement e)
        {
            e.Expected("description");

            return new Route.Point.Description
            (
                e.Elements("property").Select(subElement => ParseProperty(subElement))
            );
        }

        private Route.Point.Description.Property ParseProperty(XElement e)
        {
            e.Expected("property");

            return new Route.Point.Description.Property
            {
                Name = e.GetAttributeValue("name"),
                Value = e.GetAttributeValue("value")
            };
        }
    }
}
