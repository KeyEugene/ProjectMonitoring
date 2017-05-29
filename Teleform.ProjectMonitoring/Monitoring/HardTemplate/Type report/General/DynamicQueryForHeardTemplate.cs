#define alexj
#define ViktorWWW

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Teleform.ProjectMonitoring.HardTemplate.Type_report;
using Teleform.ProjectMonitoring.HttpApplication;
using Teleform.Reporting;

namespace Teleform.ProjectMonitoring.HardTemplate
{
    public class DynamicQueryForHeardTemplate
    {

        protected string objID;

        protected string insideQuery;
        public string whereParentID;
        public string parent;

        public string SqlQuery
        {
            get
            {
                return insideQuery.Replace("parentID is null", parent);
            }

        }
        public string mainString
        {
            get
            {
#if alexj
                return string.Concat("set dateformat 'dmy'; SELECT ", select.ToString(),
                ", [objID] FROM (", SqlQuery, ") this ", string.IsNullOrEmpty(where.ToString()) ? "" : string.Concat(" WHERE ", where.ToString()),
                " GROUP BY ", groupBy.ToString(), " ", orderBy.ToString(),", [objID]");
#else
                return string.Concat("set dateformat 'dmy'; SELECT ", select.ToString(),
                " FROM (", SqlQuery, ") this ", string.IsNullOrEmpty(where.ToString()) ? "" : string.Concat(" WHERE ", where.ToString()),
                " GROUP BY ", groupBy.ToString(), " ", orderBy.ToString());
#endif
            }
        }
        public TemplateField mainAggregationField;

        protected StringBuilder select;
        protected string selectCol;
        protected StringBuilder groupBy;
        protected string groupCol;
        protected StringBuilder where;
        protected string whereCol;
        protected StringBuilder orderBy;
        protected string orderCol;
        protected readonly Template template;
        protected Hashtable sortHashTable;

        protected CollectionWhere collectionWhere;

        protected bool haveAggr;

        public DynamicQueryForHeardTemplate(string sqlQuery, Template template, Hashtable sortHashTable)
        {
            this.insideQuery = sqlQuery;
            this.template = template;
            this.sortHashTable = sortHashTable;

            select = new StringBuilder();
            groupBy = new StringBuilder();
            where = new StringBuilder();
            orderBy = new StringBuilder();

            collectionWhere = new CollectionWhere();
        }

        public virtual DataTable GetData(int currentLevel, DataRow rowWhere = null)
        {
            select.Clear(); groupBy.Clear(); where.Clear(); orderBy.Clear();

            haveAggr = false;
            
#if Viktor
            var fields = template.Fields.Where(x => x.Level == currentLevel && !x.IsDenied).ToList();
#else
            var fields = template.Fields.Where(x => x.Level == currentLevel).ToList();
#endif



            foreach (var field in fields)
                BuildStringFromField(field);

            if (!haveAggr)
                FindToFieldWithDefaultAggregation(fields); //Default Aggregation = 1 Level

            if (rowWhere != null)
                SetWhereFromRow(--currentLevel, rowWhere);
            else
                mainAggregationField = fields.FirstOrDefault(x => x.Aggregation != "");

            PreRequestToSql();

            return Global.GetDataTable(mainString); // Возвращаем DataTable
        }

        protected virtual void BuildStringFromField(TemplateField field)
        {
            var nameColumn = string.Concat("[", field.Name, "] "); // ?
            selectCol = groupCol = nameColumn;

            var sort = sortHashTable[field.Attribute.ID.ToString()];
            if (sort != null && !String.IsNullOrEmpty(sort.ToString()))
                orderCol = string.Concat(nameColumn, sort);

            whereCol = null;

            if (field.Predicate != "")
                SetFiler(field);

            if (field.Aggregation != "")
            {
                haveAggr = true;
                SetAggregation(field);
            }

            AppendString();
        }

        protected virtual void PreRequestToSql()
        {

        }

        protected virtual void SetWhereFromRow(int currentLevel, DataRow rowWhere)
        {
            if (collectionWhere.list.Count != 0)
                collectionWhere.DeleteEverythingAfterTheLevel(currentLevel); // <-- (огромный костыль)

            //Предварительные пляски с бубном
            var ColWithAggr = template.Fields.FirstOrDefault(x => x.Level == currentLevel && x.Aggregation != "");

            string nameColWithAggr = string.Empty;

            if (ColWithAggr != null)
                nameColWithAggr = ColWithAggr.Name;
            else
            {
                ColWithAggr = template.Fields.FirstOrDefault(x => x.Level == 1 && x.Aggregation != "");

                if (ColWithAggr != null)
                    nameColWithAggr = ColWithAggr.Name;
            }

            var fields = template.Fields.Where(x => x.Level == currentLevel).ToList();

            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i].Name == nameColWithAggr)
                    continue;

                var row = rowWhere[fields[i].Name].ToString();

                row = string.IsNullOrEmpty(row) ? " is null" : string.Concat(" = '", row, "'");

                var nameField = string.Concat("[", fields[i].Name, "] ");

                collectionWhere.Add(currentLevel, nameField, row);
            }

            foreach (var item in collectionWhere.list)
            {
                if (string.IsNullOrEmpty(whereCol))
                    whereCol = string.Concat(item.v1, item.v2);
                else
                    whereCol = string.Concat(whereCol, " AND ", item.v1, " ", item.v2);
            }
            AppendString();
        }

        protected void FindToFieldWithDefaultAggregation(List<TemplateField> fields)
        {

#if Viktor
            var field = template.Fields.FirstOrDefault(x => x.Level == 1 && x.Aggregation != "" && !x.IsDenied);
#else
                   var field = template.Fields.FirstOrDefault(x => x.Level == 1 && x.Aggregation != "");
#endif



            if (field != null)
                BuildStringFromField(field);
        }

        protected void SetAggregation(TemplateField field)
        {
            var s = selectCol;
            selectCol = string.Concat(field.Aggregation, "(", s, ") AS ", s);
            groupCol = string.Empty;
        }


        protected void SetFiler(TemplateField field)
        {
            whereCol = field.Predicate.Replace("#a", field.Name);
        }

        protected void AppendString()
        {

            if (!string.IsNullOrEmpty(selectCol))
            {
                if (select.Length == 0)
                    select.Append(selectCol);
                else
                    select.Append(string.Concat(", ", selectCol));
            }

            if (!string.IsNullOrEmpty(whereCol))
            {
                if (where.Length == 0)
                    where.Append(string.Concat(whereCol, " "));
                else
                    where.Append(string.Concat(" AND ", whereCol, " "));
            }

            if (!string.IsNullOrEmpty(groupCol))
            {
                if (groupBy.Length == 0)
                    groupBy.Append(groupCol);
                else
                    groupBy.Append(string.Concat(", ", groupCol));
            }

            if (!string.IsNullOrEmpty(orderCol))
            {
                if (orderBy.Length == 0)
                    orderBy.Append(string.Concat("ORDER BY ", orderCol));
                else
                    orderBy.Append(string.Concat(", ", orderCol));
            }

            ZeroingStrings();
        }

        protected void ZeroingStrings()
        {
            selectCol = null;
            groupCol = null;
            whereCol = null;
            orderCol = null;
        }

    }

    #region Костыль еще тот

    public class CollectionWhere
    {
        public List<FunnyTypeWhere> list { get; protected set; }

        public CollectionWhere()
        {
            list = new List<FunnyTypeWhere>();
        }

        public void Add(int lvl, string v1, string v2)
        {
            list.Add(new FunnyTypeWhere(lvl, v1, v2));
        }

        public void DeleteEverythingAfterTheLevel(int lvl)
        {
            List<FunnyTypeWhere> container = list.Where(x => x.lvl < lvl).Select(o => new FunnyTypeWhere(o)).ToList();
            list = container;
        }

    }

    public class FunnyTypeWhere
    {
        public int lvl { get; protected set; }
        public string v1 { get; protected set; }
        public string v2 { get; protected set; }

        public FunnyTypeWhere(FunnyTypeWhere fun)
        {
            lvl = fun.lvl;
            v1 = fun.v1;
            v2 = fun.v2;
        }

        public FunnyTypeWhere(int lvl, string v1, string v2)
        {
            this.lvl = lvl;
            this.v1 = v1;
            this.v2 = v2;
        }
    }

    #endregion
}
