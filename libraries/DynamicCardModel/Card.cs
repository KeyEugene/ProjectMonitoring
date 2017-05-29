using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


namespace Teleform.Reporting.DynamicCard
{
    public class Card
    {

        public Card(Entity entity)
        {


            Entity = entity;

            FieldsValuesFromDB = new Dictionary<string, object>();

            Fields = new List<IField>();
        }
        /// <summary>
        /// Указывает на то что у типа есть дети
        /// </summary>
        public bool IsAncestor { get; set; }

        public int? ConstraintID { get; set; }

        public int? InstanceID { get; set; }


        public Entity Entity { get; private set; }

        public EntityInstance EntityInstance { get; set; }

        public DependencyRelations DependencyRelations { get; set; }

        public List<IField> Fields { get; private set; }

        public Dictionary<string, object> FieldsValuesFromDB { get; internal set; }


        public bool Filled { get; internal set; }

        public void Clear(Mode mode)
        {
            foreach (var columnValue in EntityInstance.RelationColumnsValue)
            {
                columnValue.TitleAttribute = "";
                columnValue.Value = "";
            }
            foreach (var field in Fields)
                field.Value = "";    
        }

        //public object GetValuesFromDB(string systemName)
        //{
        //    object value;

        //    if (FieldsValuesFromDB.TryGetValue(systemName, out value))
        //        return value;

        //    return DBNull.Value;
        //}

        public CardRelationField GetRelation(int relationID)
        {
            var relation = Fields.FirstOrDefault(o => o is CardRelationField && (o as CardRelationField).ID == relationID);

            if (relation == null)
                throw new KeyNotFoundException();

            return relation as CardRelationField;
        }


        public CardRelationField GetRelation(string systemName)
        {
            var relation = Fields.FirstOrDefault(o => o is CardRelationField && (o as CardRelationField).SystemName == systemName);

            if (relation == null)
                throw new KeyNotFoundException();

            return relation as CardRelationField;
        }

        public CardListRelationField GetCardListRelation(int relationID)
        {
            var listRelation = Fields.FirstOrDefault(o => o is CardListRelationField && (o as CardListRelationField).ID == relationID);

            if (listRelation == null)
                throw new KeyNotFoundException();

            return listRelation as CardListRelationField;
        }

        public IField GetAnyField(string systemName)
        {
            var field = Fields.FirstOrDefault(o => o is CardSelfField && o.SystemName.ToLower() == systemName.ToLower());

            if (field == null)
                throw new KeyNotFoundException(systemName);

            return field;
        }

        public CardSelfField GetSelfField(string systemName)
        {
            var selfField = Fields.FirstOrDefault(o => o is CardSelfField && (o as CardSelfField).SystemName.ToLower() == systemName.ToLower());

            if (selfField == null)
                throw new KeyNotFoundException(systemName);

            return selfField as CardSelfField;
        }


    }
}
