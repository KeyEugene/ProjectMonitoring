#define SelectedRow

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Teleform.ProjectMonitoring.HttpApplication;
using Teleform.Reporting;

namespace Teleform.ProjectMonitoring.NavigationDialogView
{
    public abstract class BottomTreeNodeBase : System.Web.UI.UserControl
    {
        /// <summary>
        /// Основная таблица по которой мы "бегаем"
        /// </summary>
        public DataTable MainTable { get; set; }
        /// <summary>
        /// Collection ConstrID InstanceID
        /// </summary>
        public Hashtable collection { get; set; }
        /// <summary>
        /// Все данные о странице на которой мы находимся
        /// </summary>
        public DataTreeNode dataTreeNode { get; set; }
        public Reporting.Entity entity { get; set; }

        public List<string> titles { get; set; }
        /// <summary>
        /// Если таблица IsHierarchic, тогда нам нужна только одна колонка name (т.е. hash). Которая по умолнчанию не может быть DbNull
        /// </summary>
        public string hashNameCol { get; set; }

        /// <summary>
        /// Название колонки для таблицы, которая имеет иерархический вид( ObjID идет по умолчанию)
        /// </summary>
        public string nameColParent;
        /// <summary>
        /// для навигации по объектам
        /// </summary>
        public string constrID { get; set; }

        /// <summary>
        /// Индекс на который говорит о глубине уровня (глубина вложенных node)
        /// </summary>
        public int totalIndexNode { get; set; }
        public int currentIndexNode = 0;

        public abstract TreeNodeCollection BuildChildNodes(DataRow row, string instanceID, int level = 0);
        public abstract TreeNode CreateNode(DataRow row, int level);

        protected string GetCountInstance(DataRow row, string instanceID)
        {
#if SelectedRow

            var query = string.Format("DECLARE @cnt int EXEC  [report].[getListAttributeCount] '{0}', {1}, @cnt out", row["childID"].ToString(), instanceID);

            var dt = Global.GetDataTable(query);

            if (dt.Rows.Count != 0)
                return dt.Rows[0][0].ToString();
            else
                return "";
#else
            var parentEntityID = row["parentID"].ToString();
            var childEntityID = row["childID"].ToString();
            var constName = row["const"].ToString();

            var parentTable = Storage.Select<UserFilter>(parentEntityID).FilteredBusinessContent();
            var childTable = Storage.Select<UserFilter>(childEntityID).FilteredBusinessContent();

            List<Column> listColumns = this.GetSchema().Entities.FirstOrDefault(x => x.ID.Equals(parentEntityID))
                .Lists.FirstOrDefault(x => x.ConstraintName.Equals(constName)).Columns.ToList();

            StringBuilder parentColString = new StringBuilder();

            for (int i = 0; i < parentTable.Rows.Count; i++)
            {
                if (parentTable.Rows[i]["objID"].ToString().Equals(instanceID))
                {
                    for (int j = 0; j < listColumns.Count(); j++)
                    {
                        var columns = parentTable.Columns;
                        parentColString.Append(listColumns[j].ParentColumn + " = " + parentTable.Rows[i][listColumns[j].RefColumn].ToString());

                        int y = j++;
                        if (listColumns.Count > y)
                            parentColString.Append(" AND ");

                    }
                    continue;
                }
            }

            childTable.Select(parentColString.ToString());

            return childTable.Rows.Count.ToString();
#endif
        }
        public void FillNode(TreeNodeCollection collect, ref TreeNode node)
        {
            TreeNode[] list = new TreeNode[collect.Count];
            collect.CopyTo(list, 0);

            if (collect.Count != 0)
                for (int i = 0; i < list.Count(); i++)
                    node.ChildNodes.Add(list[i]);
        }


    }
}