using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Teleform.ProjectMonitoring.HttpApplication;

namespace Teleform.ProjectMonitoring.NavigationDialogView
{
    public class Class1
    {
        public DataTable mainTable;
        private TreeNode RepeateNode = new TreeNode();
        public TreeNodeCollection BuildChildNodes(string parentID  ,string childID, string instanceID)
        {
            var nodeContainer = new TreeNode();
            var isChildOnly = true;
            for (int i = 0; i < mainTable.Rows.Count; i++)
            {
                if (mainTable.Rows[i]["parentID"].ToString() == childID && mainTable.Rows[i]["isIndexed"].ToString() != "0" && mainTable.Rows[i]["isDependent"].ToString() != "1")
                {
                    isChildOnly = false;
                    var child = mainTable.Rows[i]["childID"].ToString();
                    var node = ChreateNewNode(mainTable.Rows[i], instanceID, false);
                    var collectionNode = BuildChildNodes(mainTable.Rows[i]["childID"].ToString(), child, null);
                    FillNode(collectionNode, ref node);
                    nodeContainer.ChildNodes.Add(node);
                }
            }

            //Проверяем относится Тип объекта к 'base'
            if (isChildOnly)
                foreach (DataRow row in mainTable.Rows)
                {
                    if (row["childID"].ToString().Equals(childID) ) //&& row["parentID"].ToString().Equals(parentID))
                    {
                        var node = ChreateNewNode(row, instanceID, true);
                        nodeContainer.ChildNodes.Add(node);
                        return nodeContainer.ChildNodes;
                    }
                }

            return nodeContainer.ChildNodes;
        }

        private TreeNode ChreateNewNode(DataRow row, string instanceID, bool isChild)
        {
            var NamePath = "";
            if (isChild)
                NamePath = row["childAlias"].ToString();
            else
                NamePath = row["parentAlias"].ToString();

            var childID = row["childID"].ToString();
            var constrID = row["constrID"].ToString();


            return new TreeNode
            {
                Text = "<a href='#' >" + NamePath + "(entity=" + childID + "&constraint=" + constrID + "&id=" + instanceID + "</a>",
                SelectAction = TreeNodeSelectAction.None,
                Expanded = false
            };
        }
        private void FillNode(TreeNodeCollection collect, ref TreeNode n)
        {
            TreeNode[] list = new TreeNode[collect.Count];
            collect.CopyTo(list, 0);

            if (collect.Count != 0)
                for (int i = 0; i < list.Count(); i++)
                    n.ChildNodes.Add(list[i]);
        }

        public DataTable GetTable()
        {
            var da = new SqlDataAdapter(@"SELECT  [parent]
      ,[parentAlias]
      ,[child]
      ,[childAlias]
      ,[const]
      ,[constrAlias]
      ,[isIndexed]
      ,[isDependent]
      ,[parentID]
      ,[childID]
      ,[constrID]
  FROM [model].[ParentChildEntity]", Global.ConnectionString);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}