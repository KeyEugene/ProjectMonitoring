


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Teleform.Reporting;
using Teleform.Reporting.constraint;

namespace Teleform.Reporting.DynamicCard
{
    public class CardRelationField : IRelationField
    {
        public CardRelationField(Constraint constraint, Card card)
        {
            Constraint = constraint;
            Card = card;
        }


        public bool IsForbidden { get; set; }

        public Constraint Constraint { get; set; }

        public int ID
        {
            get
            {
                return int.Parse(Constraint.ConstraintObjID);
            }
        }

        public string Name
        {
            get
            {
                return Constraint.Alias;
            }
        }

        public string SystemName 
        {
            get
            {
                return Constraint.ConstraintName;
            }
        }

        string RefTblID
        {
            get
            {
                return Constraint.RefTblID;
            }
        }

        public Card Card { get; private set; }       


        public bool IsNullable
        {
            get 
            {
                return Constraint.IsNullable;
            }
        }


        public Entity Entity
        {
            get
            {                
                return Storage.Select<Entity>(RefTblID);
            }
        }

        public string NativeEntityName
        {
            get
            {
                return Entity.SystemName;
            }
        }

        public string Value { get; set; }
        object IField.Value
        {
            get { return Value; }
            set
            {
                if (value != null)
                    Value = value.ToString();
                else
                {
                    Value = string.Empty;
                }
            }
        }


        private void Clear()
        {
            Value = string.Empty;
        }

       
    }
}
