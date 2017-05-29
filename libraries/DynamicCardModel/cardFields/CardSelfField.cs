

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting.DynamicCard
{
    public class CardSelfField : ISelfField
    {

        public CardSelfField(Attribute attribute, Card card)
        {
            Attribute = attribute;
            Card = card;
        }


        public bool IsForbidden 
        {
            get
            { 
                return Name == SystemName; 
            }
            set
            {

            }
        }

        private Attribute Attribute { get; set; }


        public string NativeEntityName
        {
            get { return Storage.Select<Entity>(Attribute.EntityID).SystemName; }
        }

        public string ID
        {
            get { return Attribute.ID.ToString(); }
        }

        public string Name
        {
            get { return Attribute.Name; }
        }

        public string SystemName
        {
            get { return Attribute.FPath; }
        }

        private string UType
        {
            get { return Attribute.Type.Name; }
        }

        internal string SType
        {
            get { return Attribute.SType; }
        }

        internal bool IsComputed
        {
            get { return Attribute.IsComputed; }
        }

        internal bool IsIdentity
        {
            get { return Attribute.IsIdentity; }
        }

        public bool IsNullable
        {
            get { return Attribute.IsNullable; }
        }


      


        public object Value
        {
            get
            {
                object value;

                if (Card.FieldsValuesFromDB.TryGetValue(SystemName, out value))
                {
                    if (TypeCode == Type.Boolean && (value is DBNull || value == null))
                        return false;

                    if (TypeCode == Type.Float)
                        value = value != null ? value.ToString().Replace(",", ".") : string.Empty;

                    return value ?? DBNull.Value;
                }

                if (TypeCode == Type.Boolean)
                    return false;
                else return DBNull.Value;
            }
            set
            {
                if (Card.FieldsValuesFromDB.ContainsKey(SystemName))
                    Card.FieldsValuesFromDB[SystemName] = value;
                else
                    Card.FieldsValuesFromDB.Add(SystemName, value);
            }
        }


        public enum Type
        {
            Object,
            Boolean,
            FileName,
            DateTime,
            Numeric,
            ShortString,
            Float
        }

        public Type TypeCode
        {
            get
            {
                switch (UType)
                {
                    case "flag":
                    case "bit":
                        return Type.Boolean;
                    case "fileName":
                        return Type.FileName;
                    case "date":
                    case "datetime":
                    case "smalldatetime":
                    case "endTime":
                    case "startTime":
                        return Type.DateTime;
                    case "int":
                    case "tinyint":
                    case "tiny":
                    case "Year":
                        return Type.Numeric;
                    case "money":
                        return Type.Float;
                    case "code":
                        return Type.ShortString;
                }

                return Type.Object;
            }
        }

        public bool IsReadOnly(Mode mode)
        {
            if (mode == Mode.Create)
                return IsComputed || IsIdentity;
            else if (mode == Mode.Edit)
                return IsComputed || IsIdentity;
            //return Computed || IsIdentity || KeyMember;
            else if (mode == Mode.ReadOnly)
                return true;

            throw new NotImplementedException();
        }





        public Card Card { get; private set; }

        public bool ContainsNonEmptyValue()
        {
            return Value != null && !(Value is DBNull) && !string.IsNullOrWhiteSpace(Value.ToString());
        }

        public override string ToString()
        {
            return SystemName;
        }


    }
}
