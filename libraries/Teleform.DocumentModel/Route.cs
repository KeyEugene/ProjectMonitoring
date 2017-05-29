using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.DocumentModel
{
    public partial class Route
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public int Number { get; set; }

        public DateTime Date { get; set; }

        public int ID { get; set; }

        public IEnumerable<Route.Point> Points { get; private set; }

        public Route(IEnumerable<Point> points)
        {
            Points = points ?? Enumerable.Empty<Point>();
        }
    }
}
