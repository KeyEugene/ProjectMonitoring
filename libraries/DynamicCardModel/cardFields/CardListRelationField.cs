using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Teleform.Reporting.constraint;

namespace Teleform.Reporting.DynamicCard
{
    public class CardListRelationField : IRelationField
    {
        public CardListRelationField(ListConstraint listConstraint, Card card)
        {
            ListConstraint = listConstraint;
            Card = card;

            ListRelationColumns = new List<ListRelationColumn>();

            GetListRelationColumns();
        }


        private void GetListRelationColumns()
        {
            foreach (var column in ListConstraint.Columns)
            {
                var listRelationColumn = new ListRelationColumn(column.ParentColumn, column.RefColumn);
                ListRelationColumns.Add(listRelationColumn);
            }
        }


        public bool IsForbidden { get; set; }


        public List<ListRelationColumn> ListRelationColumns { get; private set; }

        public class ListRelationColumn
        {
            public string ParentColumn { get; private set; }

            public string RefColumn { get; private set; }

            public ListRelationColumn(string parentColumn, string refColumn)
            {
                ParentColumn = parentColumn;
                RefColumn = refColumn;
            }
        }



        public Card Card { get; private set; }
        private ListConstraint ListConstraint { get; set; }


        public int ID
        {
            get { return int.Parse(ListConstraint.ConstraintID); }
        }

        public string Name
        {
            get { return ListConstraint.Alias; }
        }

        public string SystemName
        {
            get { return ListConstraint.ConstraintName; }
        }

        public Entity Entity
        {
            get { return Storage.Select<Entity>(ListConstraint.ParentTblID); }
        }

        public string NativeEntityName
        {
            get
            {
                return Entity.SystemName;
            }
        }

        public bool IsNullable
        {
            get { return true; }
        }




        public int Value { get; set; }

        object IField.Value
        {
            get { return Value; }
            set
            {
                if (value != DBNull.Value && value != string.Empty)
                    Value = int.Parse(value.ToString());

            }
        }


        
    }
}
