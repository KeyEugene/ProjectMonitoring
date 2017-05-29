using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Teleform.ProjectMonitoring.HttpApplication;

namespace Teleform.ProjectMonitoring.NavigationDialogView
{
    public class LowerTreeNode
    {
        /// <summary>
        /// тут храниться запрос(SELECT * FROM [model].[vo_ParentChildEntity]), берется из Sesstion 
        /// </summary>
        public DataTable ParentChildEntityTable;
        /// <summary>
        /// Collection ConstrID InstanceID
        /// </summary>
        public Hashtable collection;
        public object selectedEntityID;
        public object selectedConstrID;
        public string selectedInstanceID;
        public int urlLevel;
        public string currentPath;
        /// <summary>
        /// Если находимся на карточке true
        /// </summary>
        public bool isDynamicCard;
        public string getLeftUrl { get; set; }
        public TreeNodeCollection BuildChildNodes(DataRow row, string instanceID, int level = 0)
        {
            var nodeContainer = new TreeNode();
            var node = new TreeNode();
            var isTerminal = Convert.ToBoolean(row["isTerminal"]);
            var childID = row["childID"].ToString();

            if (row["parentID"].ToString().Equals(childID))
            {
                level++;
                if (!String.IsNullOrEmpty(instanceID))
                {
                    var count = GetCountInstance(row["const"].ToString(), instanceID);
                    if (count.Equals("0"))
                    {
                        return new TreeNodeCollection();
                    }
                }
                else
                {
                    return new TreeNodeCollection();
                }
            }

            //Задаем id на будущую ступень
            if (selectedConstrID == null && !string.IsNullOrEmpty(instanceID) && !isDynamicCard)
            {
                SavePath(row["constrID"].ToString(), selectedInstanceID, level);
            }

            if (!isTerminal)
            {
                node = ChreateNode(row, level);

                for (int i = 0; i < ParentChildEntityTable.Rows.Count; i++)
                {
                    if (ParentChildEntityTable.Rows[i]["parentID"].ToString() == childID && Convert.ToBoolean(ParentChildEntityTable.Rows[i]["isIdentified"]) != false)
                    {
                        //Задаем id на будущую ступень
                        if (childID.Equals(selectedEntityID) && row["constrID"].ToString().Equals(selectedConstrID) && urlLevel == level)
                            SavePath(ParentChildEntityTable.Rows[i]["constrID"].ToString(), selectedInstanceID, level);

                        var collectionNode = BuildChildNodes(ParentChildEntityTable.Rows[i], null, level);
                        FillNode(collectionNode, ref node);
                    }
                }
            }
            else
            {
                //var count = Global.GetDataTable(""); // Если нужно отображать кол-во объектов. Раскомментировать тут и заполнить запросом к бд. 
                nodeContainer.ChildNodes.Add(ChreateNode(row, level));
                return nodeContainer.ChildNodes;
            }
            nodeContainer.ChildNodes.Add(node);
            return nodeContainer.ChildNodes;
        }

        private TreeNode ChreateNode(DataRow row, int level)
        {
            string NamePath = row["childAlias"].ToString();
            string constrID = row["constrID"].ToString();
            string href = String.Empty;
            bool isExpanded = false;
            string cssClass = "navigationTagA";
            string tooltopCount = "";

            if (collection.ContainsKey(constrID + level))
            {
                href = string.Format("?entity={0}&constraint={1}&id={2}&level={3}",
                    row["childID"].ToString(),
                    constrID,
                    collection[constrID + level].ToString(),
                    level.ToString());

                //Определяем на каком TreeNode мы находимся 
                if (href.Equals(this.currentPath))
                    cssClass = "navigationTagASelected";

                href = string.Concat(getLeftUrl, "ListAttributeView.aspx", href);
                isExpanded = true;
                tooltopCount = GetCountInstance(row["const"].ToString(), collection[constrID + level].ToString());
            }
            else
            {
                href = "";
                cssClass = "navigationTagANotSelecte";
            }

            return new TreeNode
            {
                Text = string.Format("<span title='{3}'><a href='{0}' class='{1}'>{2} </a></span>", href, cssClass, NamePath, tooltopCount),
                SelectAction = TreeNodeSelectAction.None,
                Expanded = isExpanded
            };
        }

        private string GetCountInstance(string constraintID, string instanceID)
        {
            var query = string.Format("DECLARE @cnt int EXEC  [report].[getListAttributeCount] '{0}', {1}, @cnt out", constraintID, instanceID);
            var dt = Global.GetDataTable(query);

            if (dt.Rows.Count != 0)
                return dt.Rows[0][0].ToString();
            else
                return "";
        }


        public void FillNode(TreeNodeCollection collect, ref TreeNode n)
        {
            TreeNode[] list = new TreeNode[collect.Count];
            collect.CopyTo(list, 0);

            if (collect.Count != 0)
                for (int i = 0; i < list.Count(); i++)
                    n.ChildNodes.Add(list[i]);
        }

        private void SavePath(string p, string j, int level)
        {
            if (collection.ContainsKey(p + level))
                collection[p + level] = j;
            else
                collection.Add(p + level, j);
        }


    }
}