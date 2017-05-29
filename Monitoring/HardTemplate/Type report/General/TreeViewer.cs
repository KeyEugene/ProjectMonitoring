#define alexj
#define ViktorWWW

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI.WebControls;
using System.Text;
using Teleform.ProjectMonitoring.admin.SeparationOfAccessRights;

namespace Teleform.ProjectMonitoring.HardTemplate
{
    public partial class HardTemplateView
    {
        private int maxlevel;

        private void StartBuildTreeView()
        {
            maxlevel = template.Fields.Max(x => x.Level);
            var currentLevel = 1;

            var dt = dynamicQueryForGeneral.GetData(currentLevel);

#if alexj
            dt = AuthorizationRules.EntityInstancesResolution(dt, template.Entity, Session["SystemUser.objID"].ToString());
            dt.Columns.Remove("objID");
#endif

            //Если что то пошло не так!
            if (dt.Rows.Count == 0)
                return;

            tree.Nodes.Add(
                CreateNewHeaderNode(template.Fields.Where(x => x.Level == currentLevel).ToList()));

            foreach (DataRow row in dt.Rows)
            {
                var node = CreateNewNode(row, currentLevel);

                TreeNodeCollection collect = BuildChildNodes(row, currentLevel);
                FillNode(collect, ref node);

                tree.Nodes.Add(node);
            }
            MainView.ActiveViewIndex = 2;
        }

        private TreeNodeCollection BuildChildNodes(DataRow row, int currentLevel)
        {
            if (maxlevel == currentLevel)
                return new TreeNodeCollection();

            var dt = dynamicQueryForGeneral.GetData(++currentLevel, row);

#if alexj
            //dt = AuthorizationRules.EntityInstancesResolution(dt, template.Entity, Session["SystemUser.objID"].ToString());
            dt.Columns.Remove("objID");
#endif

            TreeNode nodeContainer = new TreeNode();
            nodeContainer.ChildNodes.Add(
                CreateNewHeaderNode(template.Fields.Where(x => x.Level == currentLevel).ToList()));


            if (maxlevel != currentLevel)
            {
                foreach (DataRow item in dt.Rows)
                {
                    var node = CreateNewNode(item, currentLevel);
                    TreeNodeCollection collection = BuildChildNodes(item, currentLevel);
                    FillNode(collection, ref node);
                    nodeContainer.ChildNodes.Add(node);
                }
            }
            else
            {
                foreach (DataRow item in dt.Rows)
                    nodeContainer.ChildNodes.Add(CreateNewNode(item, currentLevel));
            }

            return nodeContainer.ChildNodes;
        }

        private void FillNode(TreeNodeCollection collect, ref TreeNode n)
        {
            TreeNode[] list = new TreeNode[collect.Count];
            collect.CopyTo(list, 0);

            if (collect.Count != 0)
                for (int i = 0; i < list.Count(); i++)
                    n.ChildNodes.Add(list[i]);
        }


        #region Create front-end node
        private TreeNode CreateNewHeaderNode(List<Reporting.TemplateField> fields)
        {
            var sb = new StringBuilder();
            string aggr = "";


#if Viktor
            foreach (var field in fields.Where(f => !f.IsDenied))
            {
                if (field.AggregationFunction != null)
                    aggr = string.Concat(" (", field.AggregationFunction.Name, ")");

                sb.Append(string.Concat("<textarea readonly='readonly' style='text-align: center; background-color: lightgray; height: 30px; width: 225px;'>", field.Name, aggr, "</textarea>"));
            }
#else
            foreach (var field in fields)
            {
                if (field.AggregationFunction != null)
                    aggr = string.Concat(" (", field.AggregationFunction.Name, ")");

                sb.Append(string.Concat("<textarea readonly='readonly' style='text-align: center; background-color: lightgray; height: 30px; width: 225px;'>", field.Name, aggr, "</textarea>"));
            }
#endif
            return new TreeNode { Text = sb.ToString(), SelectAction = TreeNodeSelectAction.None };
        }

        private TreeNode CreateNewNode(DataRow row, int currentLevel)
        {

#if Viktor
            var fields = template.Fields.Where(x => x.Level == currentLevel && !x.IsDenied).ToArray();
#else
                  var fields = template.Fields.Where(x => x.Level == currentLevel).ToArray();
#endif



            var sb = new StringBuilder();
            var count = fields.Count();
            string textWithFormat = null;

            for (int i = 0; i < row.ItemArray.Count(); i++)
            {
                if (i >= count)
                    textWithFormat = string.Format
                                   (
                                      dynamicQueryForGeneral.mainAggregationField.Format.Provider,
                                      dynamicQueryForGeneral.mainAggregationField.Format.FormatString,
                                       row.ItemArray[i]
                                    );
                else
                    textWithFormat = string.Format
                               (
                                   fields[i].Format.Provider,
                                   fields[i].Format.FormatString,
                                   row.ItemArray[i]
                                );
                    
                if (i >= count ? dynamicQueryForGeneral.mainAggregationField.Attribute.SType == "bit" : fields[i].Attribute.SType == "bit")
                {
                    if (textWithFormat.ToLower() == "true")
                        sb.Append(string.Concat("<textarea readonly='readonly' style='text-align: center; height: 20px; width: 225px;'>Да</textarea>"));
                    else if (textWithFormat.ToLower() == "false")
                        sb.Append(string.Concat("<textarea readonly='readonly' style='text-align: center; height: 20px; width: 225px;'>Нет</textarea>"));
                    else
                        sb.Append(string.Concat("<textarea readonly='readonly' style='text-align: center; height: 20px; width: 225px;'> </textarea>"));
                }
                else
                    sb.Append(string.Concat("<textarea readonly='readonly' style='text-align: center; height: 20px; width: 225px;'>", textWithFormat, "</textarea>"));
            }

            return new TreeNode { Text = sb.ToString(), SelectAction = TreeNodeSelectAction.None };
        }
        #endregion
    }
}