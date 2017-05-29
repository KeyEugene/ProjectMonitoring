#define Viktor

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DataReader = Teleform.Reporting.DynamicCard;

namespace Teleform.Reporting.DynamicCard
{
    public class DatabaseReader// : DataReader
    {
        public string ConnectionString { get; set; }

        public DatabaseReader(string connectionString)
        {
            ConnectionString = connectionString;
        }


        public DataTable GetRelations(CardRelationField relation)
        {
            var card = relation.Card;
            var constraint = relation.Constraint;

            bool isCreate = false;

            //если режим создания объекта
            if (card.EntityInstance == null)
            {
                isCreate = true;
                FillEntityInstance(card, "");
            }
            else if (string.IsNullOrEmpty(card.EntityInstance.EntityInstanceID))
                isCreate = true;

            card.DependencyRelations = new DependencyRelations(constraint, card.EntityInstance);

            var tableTitleAttributes = new Dictionary<string, string>();
            tableTitleAttributes.Add(constraint.ConstraintName, relation.Value);
            card.DependencyRelations.RelationTableTitleAttributes = tableTitleAttributes;

            DataTable ReferenceTableDataSource = card.DependencyRelations.SetRelationColumnsValue_GetReferenceTable(true, isCreate);

            return ReferenceTableDataSource;

        }


        public void FillEntityInstance(Card card, string entityInstanceID)
        {
            var entity = Storage.Select<Entity>(card.Entity.ID);

            EntityInstance entityInstance = new EntityInstance(entityInstanceID, entity.SystemName, true);

            entityInstance.Constraints = entity.Constraints;

            entityInstance.SetRelationColumnsValue();

            card.EntityInstance = entityInstance;

#if Viktor
            //заполнить констраинт, если создаем списковый объект из ссылочного объекта
            if (card.ConstraintID != null && card.InstanceID != null)
            {
                var constraint = entityInstance.Constraints.FirstOrDefault(con => con.ConstraintObjID == card.ConstraintID.ToString());

                if (constraint != null)
                {
                    CardRelationField relation = new CardRelationField(constraint, card);

                    var dt = GetRelations(relation);

                    var rows = dt.Select(string.Format("objID = {0}", Convert.ToInt32(card.InstanceID)));
                    var row = rows.First();
                    var titles = new StringBuilder();
                    for (int i = 1; i < row.ItemArray.Count(); i++)
                    {
                        var cell = row.ItemArray[i];
                        titles.Append(cell).Append(" ~ ");
                    }

                    card.DependencyRelations.RelationTableTitleAttributes[constraint.ConstraintName] = titles.ToString();

                    FillRelation(relation, card.InstanceID);

                }
            }

#endif



        }


        public void FillRelation(CardRelationField relation, int? instanceID)
        {
            var card = relation.Card;

            bool isCreate = false;
            if (string.IsNullOrEmpty(card.EntityInstance.EntityInstanceID))
                isCreate = true;

#if Viktor
            card.DependencyRelations.NewEntityInstanceID = instanceID.ToString();
#else
            if (instanceID == -1)
                card.DependencyRelations.NewEntityInstanceID = "";
            else
                card.DependencyRelations.NewEntityInstanceID = instanceID.ToString();
#endif






            card.DependencyRelations.SetRelationColumnsValue_GetReferenceTable(false, isCreate);



            foreach (var field in card.Fields.Where(f => f is CardRelationField).OfType<CardRelationField>())
            {

                var title = field.Card.EntityInstance.RelationColumnsValue.Where(columnVal => columnVal.ConstraintName == field.SystemName).FirstOrDefault().TitleAttribute;

                field.Value = title;

            }

        }



        public void FillFieldsValue(Card card, int instanceID)
        {
            var query = string.Format("EXEC [report].[getBObjectdata] '{0}', NULL, @cyr=0, @flTitle=2, @instances = {1}", card.Entity.SystemName, instanceID.ToString());

            var table = Storage.GetDataTable(query);

            if (table.Rows.Count == 1)
            {
                var row = table.Rows[0];

                foreach (var field in card.Fields)
                    field.Value = row[field.SystemName];
                
                card.Filled = true;
            }
            else throw new InvalidOperationException
            (
                string.Format("Объект предоставления данных вернул {0} стр. Ожидалась одна строка.", table.Rows.Count)
            );

            query = string.Format("SELECT * FROM [{0}] WHERE [objID] = {1}", card.Entity.SystemName, instanceID);
            table = Storage.GetDataTable(query);

            if (table.Rows.Count == 1)
            {
                var fieldsValuesFromDbDict = card.FieldsValuesFromDB;
                var dataRow = table.Rows[0];

                var columns = table.Columns.OfType<DataColumn>().Select(c => c.ColumnName);
                var fileColumns = new[] { "body", "modified", "mimeTypeID", "fileName" };

                foreach (var column in columns)
                    if (!fileColumns.Contains(column))
                        fieldsValuesFromDbDict[column] = dataRow[column];

                if (columns.Intersect(fileColumns).Count() == 4)
                    fieldsValuesFromDbDict["fileName"] = new File
                    {
                        FileName = dataRow["fileName"].ToString(),
                        MimeType = dataRow["mimeTypeID"].ToString(),
                        Content = dataRow["body"] is DBNull ? null : (byte[])dataRow["body"]
                    };

                FillEntityInstance(card, instanceID.ToString());
            }
            else throw new InvalidOperationException
            (
                string.Format("Объект предоставления данных вернул {0} стр. Ожидалась одна строка.", table.Rows.Count)
            );
        }




        public int GetInstanceID(CardRelationField filledRelation)
        {
            var card = filledRelation.Card;

            var constrant = card.EntityInstance.Constraints.First(con => con.ConstraintName == filledRelation.SystemName);

            var relationColValues = card.EntityInstance.RelationColumnsValue.Where(col => col.ConstraintName == constrant.ConstraintName);


            var colValList = new List<string>();

            foreach (var item in relationColValues)
            {
                if (item.Value != null && item.Value.ToString() != "")
                {
                    var colVal = string.Format("{0} = {1}", item.RefCol, item.Value);
                    colValList.Add(colVal);
                }
            }

            var columnsValue = string.Join(" and ", colValList);

            var query = new StringBuilder("SELECT [objID] FROM [").
               Append(filledRelation.Entity.SystemName).Append("] WHERE ").Append(columnsValue);

            var table = Storage.GetDataTable(query.ToString());

            //if (table.Rows.Count == 1)
                return Convert.ToInt32(table.Rows[0][0]);
            //else
            //    throw new NotImplementedException("Не знаю почему сработал метод OpenRelationHandler(object sender, EventArgs e), надо разобраться ");


        }




        public byte[] GetFileContent(Card card, int instanceID)
        {
            var query = string.Format("SELECT [A].[body], [MT].[mime] FROM [{0}] [A] LEFT JOIN [MimeType] [MT] ON [MT].[objID] = [A].[mimeTypeID] WHERE [A].[objID] = {1}", card.Entity.SystemName, instanceID);
            var table = Storage.GetDataTable(query);

            if (table.Rows.Count == 0)
                throw new Exception("Не удалось загрузить документ.");

            return (byte[])table.Rows[0]["body"];
        }
    }
}
