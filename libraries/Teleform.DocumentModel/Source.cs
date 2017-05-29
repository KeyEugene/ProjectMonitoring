using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.DocumentModel
{
   public class Source
    {
       public int MaxPoints { get; set; }

       public Route.Point.Description Description { get; set; }

       //public RoutesElement RoutesElement { get; set; }

       public IEnumerable<Route> Routes { get; set; }      
    }
}
