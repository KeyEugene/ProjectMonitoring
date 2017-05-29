
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Teleform.Reporting
{
    [Serializable()]
    public class EntityFilterField : IField
    {
        public string AttributeID { get; set; }       
        public Attribute Attribute { get; private set; }

        public string PredicateInfo { get; set; }
        public string TechPredicate { get; set; }
        public string UserPredicate { get; set; }
        public int Sequence { get; set; }


        public EntityFilterField(string attributeID, Attribute attribute, string predicateInfo, string techPredicate, string userPredicate, int sequence)
        {
            AttributeID = attributeID;
            Attribute = attribute;

            PredicateInfo = predicateInfo;
            TechPredicate = techPredicate;
            UserPredicate = userPredicate;

            Sequence = sequence;
        }

        public EntityFilterField( Attribute attribute)
        {
            //AttributeID = string.Empty;
            Attribute = attribute;            
            //PredicateInfo = string.Empty;
            //TechPredicate = string.Empty;
            //UserPredicate = string.Empty;
        }
    
    }
}
