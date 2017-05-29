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
    public sealed class NavigationTreeNode : BottomTreeNodeBase
    {
        /// <summary>
        /// MainTable храниться запрос(SELECT * FROM [model].[vo_ParentChildEntity]), берется из Sesstion 
        /// </summary>

        public override TreeNodeCollection BuildChildNodes(DataRow row, string instanceID, int level = 0)
        {

            var nodeContainer = new TreeNode();
            var node = new TreeNode();
            var isTerminal = Convert.ToBoolean(row["isTerminal"]);
            var childID = row["childID"].ToString();

            if (row["parentID"].ToString() == childID)
            {
                level++;
                if (String.IsNullOrEmpty(instanceID))
                    return new TreeNodeCollection();
            }

            //Задаем id на будущую ступень
            if (string.IsNullOrEmpty(dataTreeNode.selectedConstrID) && !string.IsNullOrEmpty(instanceID) && !dataTreeNode.isDynamicCard)
                SavePath(row["constrID"].ToString(), dataTreeNode.selectedInstanceID, level);

            if (!isTerminal)
            {
                node = CreateNode(row, level);

                for (int i = 0; i < MainTable.Rows.Count; i++)
                {
                    if (MainTable.Rows[i]["parentID"].ToString() == childID && Convert.ToBoolean( MainTable.Rows[i]["isIdentified"]) != false)
                    {
                        //Задаем id на будущую ступень
                        if (childID == dataTreeNode.selectedEntityID && row["constrID"].ToString() == dataTreeNode.selectedConstrID
                            && dataTreeNode.urlLevel == level && !String.IsNullOrEmpty(this.dataTreeNode.selectedInstanceID))
                        {
                            SavePath(MainTable.Rows[i]["constrID"].ToString(), dataTreeNode.selectedInstanceID, level);
                        }

                        var collectionNode = BuildChildNodes(MainTable.Rows[i], null, level);
                        FillNode(collectionNode, ref node);
                    }
                }
            }
            else
            {
                nodeContainer.ChildNodes.Add(CreateNode(row, level));
                return nodeContainer.ChildNodes;
            }

            nodeContainer.ChildNodes.Add(node);
            return nodeContainer.ChildNodes;
        }

        public override TreeNode CreateNode(DataRow row, int level)
        {
            var NamePath = row["childAlias"];
            string constrID = row["constrID"].ToString();
            string href = String.Empty;
            bool isExpanded = false;
            string cssClass = "navigationTagA";
           // var tooltipCount = "";

            if (collection.ContainsKey(constrID + level))
            {
href = string.Concat(dataTreeNode.getLeftUrl, "ListAttributeView.aspx1?entity=", row["childID"], "&constraint=", constrID, "&id=", collection[constrID + level],
                    "&level=", level);

                //Определяем на каком TreeNode мы находимся 
                if (href == (string.Concat(dataTreeNode.getLeftUrl, "ListAttributeView.aspx", dataTreeNode.currentPath)) ||
                    row["childID"].ToString() == dataTreeNode.selectedEntityID)
                {
                    cssClass = "navigationTagASelected";
                }

                isExpanded = true;
               // tooltipCount = GetCountInstance(row, collection[constrID + level].ToString());
            }
            else
            {
                href = "";
                cssClass = "navigationTagANotSelecte";
            }

            return new TreeNode
            {
#if ToolTip // WithToolTip(count)
                Text = string.Concat("<span title='", tooltipCount,
                "'><a href='", href,
                "' class='", cssClass," ent", row["parentID"].ToString(),
                "'>", NamePath, " </a></span>"),
                SelectAction = TreeNodeSelectAction.None,
                Expanded = isExpanded
#else
                Text = string.Concat("<a href='", href,
                "' class='", cssClass," ent", row["parentID"].ToString(),
                "'>", NamePath, " </a>"),
                SelectAction = TreeNodeSelectAction.None,
                Expanded = isExpanded
#endif
            };
        }

        private void SavePath(string entityID, string instanceID, int level)
        {
            if (collection.ContainsKey(entityID + level))
                collection[entityID + level] = instanceID;
            else
                collection.Add(entityID + level, instanceID);
        }


    }
}