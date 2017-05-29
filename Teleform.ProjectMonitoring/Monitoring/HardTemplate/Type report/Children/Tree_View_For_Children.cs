#define alexj
#define ViktorWWW

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Teleform.ProjectMonitoring.admin.SeparationOfAccessRights;

namespace Teleform.ProjectMonitoring.HardTemplate
{
    public partial class HardTemplateView
    {
        private void StartBuildTreeView_Children()
        {
            maxlevel = template.Fields.Max(x => x.Level);
            var currentLevel = 1;

            var dt = dynamicQueryForChildren.FirstQuery();
            //var dt = dynamicQuery.GetData(currentLevel);
#if alexj
            dt = AuthorizationRules.EntityInstancesResolution(dt, template.Entity, Session["SystemUser.objID"].ToString());
            //dt.Columns.Remove("objID");
#endif

            //Если что то пошло не так!
            if (dt.Rows.Count == 0)
                return;

            tree.Nodes.Add(
                CreateNewHeaderNode(template.Fields.Where(x => x.Level == currentLevel).ToList()));

            foreach (DataRow row in dt.Rows)
            {
                var node = CreateFirstNode_Children(row, currentLevel);

                TreeNodeCollection collect = BuildChildNodesChildren(row);
                FillNode(collect, ref node);

                tree.Nodes.Add(node);
            }
            MainView.ActiveViewIndex = 2;
        }

        private TreeNodeCollection BuildChildNodesChildren(DataRow row)
        {
            var currentLevel = 1;

            dynamicQueryForChildren.parent = "parentID = " + row["objID"].ToString();
            var dt = dynamicQueryForChildren.GetData(currentLevel, row);

#if alexj
            dt = AuthorizationRules.EntityInstancesResolution(dt, template.Entity, Session["SystemUser.objID"].ToString());
            //dt.Columns.Remove("objID");
#endif

            TreeNode nodeContainer = new TreeNode();
            if (dt.Rows.Count != 0)
                nodeContainer.ChildNodes.Add(
                    CreateNewHeaderNode(template.Fields.Where(x => x.Level == currentLevel).ToList()));

            foreach (DataRow item in dt.Rows)
            {
                var node = CreateFirstNode_Children(item, currentLevel);
                TreeNodeCollection collection = BuildChildNodesChildren(item); //, currentLevel);
                FillNode(collection, ref node);
                nodeContainer.ChildNodes.Add(node);
            }
            return nodeContainer.ChildNodes;
        }

        private TreeNode CreateFirstNode_Children(DataRow row, int currentLevel)
        {

#if Viktor
            var fields = template.Fields.Where(x => x.Level == currentLevel && !x.IsDenied).OrderBy(x => x.Order).ToArray();

#else
            var fields = template.Fields.Where(x => x.Level == currentLevel).OrderBy(x => x.Order).ToArray();
#endif

            

            var sb = new StringBuilder();
            string text = null;

            for (int i = 0; i < fields.Count(); i++) // 1 по умолчанию колонка [objID]  row.ItemArray.Count(); 
            {
                text = string.Format
                           (
                               fields[i].Format.Provider,
                               fields[i].Format.FormatString,
                               row[fields[i].Name]
                            );
                if (fields[i].Attribute.SType == "bit")
                {
                    if (text.ToLower() == "true")
                        sb.Append(string.Concat("<textarea readonly='readonly' style='text-align: center; height: 20px; width: 225px;'>Да</textarea>"));
                    else if (text.ToLower() == "false")
                        sb.Append(string.Concat("<textarea readonly='readonly' style='text-align: center; height: 20px; width: 225px;'>Нет</textarea>"));
                    else
                        sb.Append(string.Concat("<textarea readonly='readonly' style='text-align: center; height: 20px; width: 225px;'> </textarea>"));
                }
                else
                    sb.Append(string.Concat("<textarea readonly='readonly' style='text-align: center; height: 20px; width: 225px;'>", text, "</textarea>"));
            }

            return new TreeNode { Text = sb.ToString(), SelectAction = TreeNodeSelectAction.None };
        }
    }
}