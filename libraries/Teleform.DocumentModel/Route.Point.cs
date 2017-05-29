using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Teleform.DocumentModel
{
    partial class Route
    {
        public partial class Point
        {
            private readonly StringDictionary index;

            public Point()
            {
                index = new StringDictionary();
            }

            public string this[string key]
            {
                get
                {
                    return index[key];
                }
                set
                {
                    index[key] = value;
                }
            }
        }
    }
}
