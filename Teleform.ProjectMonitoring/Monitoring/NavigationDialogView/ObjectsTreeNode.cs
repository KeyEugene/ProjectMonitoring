using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace Teleform.ProjectMonitoring.NavigationDialogView
{
    public sealed class ObjectsTreeNode : BottomTreeNodeBase
    {
        public override TreeNodeCollection BuildChildNodes(System.Data.DataRow row, string instanceID, int level = 0)
        {
            var nodeContainer = new TreeNode();
            var node = new TreeNode();

            node = this.CreateNode(row, 0);

            if (this.entity.IsHierarchic)
                for (int i = 0; i < this.MainTable.Rows.Count; i++)
                {
                    byte hasOtherNodes = 0;
                    bool isExtendedMainNode = false;

                    if (this.MainTable.Rows[i][this.nameColParent].ToString() == row["objID"].ToString())
                    {
                        hasOtherNodes = 1;

                        currentIndexNode++;

                        var collecitonNode = BuildChildNodes(this.MainTable.Rows[i], null);
                        isExtendedMainNode = collecitonNode[0].Expanded == true ? true : false;
                        this.FillNode(collecitonNode, ref node);
                    }

                    if (hasOtherNodes == 1)
                    {
                        currentIndexNode--;

                        var tmpNode = this.CreateNode(row, isExtendedMainNode == false ? 0 : 1);


                        node.Expanded = tmpNode.Expanded;
                        node.Text = tmpNode.Text;
                    }

                    if (totalIndexNode == currentIndexNode)
                    {
                        nodeContainer.ChildNodes.Add(node);
                        return nodeContainer.ChildNodes;
                    }
                }

            nodeContainer.ChildNodes.Add(node);
            return nodeContainer.ChildNodes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="level">обозначает : 0 - переход на XDynamcicCard, 1 - переход на ListAttributeView.aspx </param>
        /// <returns></returns>
        public override TreeNode CreateNode(System.Data.DataRow row, int level)
        {
            var hrefCard = "";
            var hrefAttrView = "";
            var cssClass = "navigationTagA";
            var isExpanded = false;
            StringBuilder nameObject = new StringBuilder();

            BuildNameString(ref nameObject, row);
     


            hrefCard = string.Concat(this.dataTreeNode.getLeftUrl, "Dynamics/XDynamicCard.aspx?entity=", entity.ID, "&id=", row["objID"]);

            
            if (this.entity.IsHierarchic)
            {
                hrefAttrView = string.Concat(this.dataTreeNode.getLeftUrl,
                   "ListAttributeView.aspx?entity=", this.entity.ID, "&constraint=", this.constrID, "&id=", row["objID"], "&level=1", "&nameObject=", nameObject);

                if (level == 1) // 1 - открывать нод
                    isExpanded = true;
            }

            if (string.Concat(this.dataTreeNode.getLeftUrl, "Dynamics/XDynamicCard.aspx", this.dataTreeNode.currentPath) == hrefCard ||
                            string.Concat(this.dataTreeNode.getLeftUrl, "ListAttributeView.aspx", this.dataTreeNode.currentPath) == hrefAttrView)
            {
                cssClass = "item_selected";
                isExpanded = true;
            }

            return new TreeNode
            {
                Text = string.Concat("<a href='", hrefAttrView,
                "' style='color: lightgray;' class='", cssClass,
                "'>", !string.IsNullOrEmpty(hrefAttrView) ? "(Список)" : "",
                "</a><a href='", hrefCard,
                "' class='", cssClass,
                "'><span class='tooltipNavigation' title='", nameObject.ToString(),
                "'>", nameObject.ToString(), "</span></a>"),
                SelectAction = TreeNodeSelectAction.None,
                Expanded = isExpanded
            };
        }

        private void BuildNameString(ref StringBuilder nameObject, DataRow row)
        {
            if (this.entity.IsHierarchic)
            {
                nameObject.Append(row[hashNameCol].ToString());
            }
            else
            {
                nameObject.Append("  ");

                for (byte i = 0; i < this.titles.Count; i++)
                {
                    nameObject.Append(!String.IsNullOrEmpty(row[this.titles[i]].ToString()) ? row[this.titles[i]].ToString() + ", " : "  ");
                }
                nameObject.Remove(nameObject.Length - 2, 2);
            }
        }
    }
}