using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting.DynamicCard
{
    public class SessionContent
    {


        public Dictionary<string, object> SessionsCards { get; set; }

        public SessionContent()
        {
            SessionsCards = new Dictionary<string, object>();
        }
    }
}
