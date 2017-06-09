using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Teleform.Reporting;

namespace Teleform.ProjectMonitoring.NavigationDialogView
{
    public partial class NavigationByObjects : System.Web.UI.UserControl
    {
        private DataTreeNode data;

        /// <summary>
        /// Вкл./Откл. навигации по объектам 
        /// </summary>
        private bool isOnObjectsNavigation
        {
            get
            {
                return Session["checkBoxObjectsNavigation"] == null ? true : Convert.ToBoolean(Session["checkBoxObjectsNavigation"]);
            }
        }

        public ObjectsTreeNode RightTreeNode { get; private set; }

        private IEnumerable<Entity> GetSchemaEntitys
        {
            get
            {
                return this.GetSchema().Entities;
            }
        }

        public string GetLeftUrl
        {
            get
            {
                return Session["getLeftUrl"] == null ? null : Session["getLeftUrl"].ToString();
            }
        }

        public string SelectedEntity
        {
            get { return Session["SelectedEntity"] == null ? "" : (string)Session["SelectedEntity"]; }
            set { Session["SelectedEntity"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //objectTreeView.Nodes.Clear();
            InitializationDataTree();

            // OpenMenuDialog_Click(null, null);
        }


        protected void OpenMenuDialog_Click(object sender, EventArgs e)
        {
            if (isOnObjectsNavigation)
            {
                NavigationObjects.Visible = true;
                //Если находимся в мониторинге и entity определен
                var entity = Request["entity"];

                if (!string.IsNullOrEmpty(entity))
                {
                    dialog_object.Show();

                    InitializationRightTreeNode(entity);
                    CreateObjectTree();
                }
                else
                {
                    NavigationObjects.Visible = false;
                }
            }
            else
            {
                NavigationObjects.Visible = false;
            }

        }

        protected void CloseDialog_Click(object sender, EventArgs e)
        {
            dialog_object.Close();
            objectTreeView.Nodes.Clear();
            NavigationObjects.Visible = false;
        }

        private void CreateObjectTree()
        {
            var node = new TreeNode();

            FillThisNode(RightTreeNode.entity, ref node, "");

            if (RightTreeNode.entity.IsHierarchic)
            {
                for (int i = 0; i < RightTreeNode.dataTreeNode.ReportTableView.Rows.Count; i++)
                {
                    RightTreeNode.currentIndexNode = 0;
                    var collectionNode = RightTreeNode.BuildChildNodes(RightTreeNode.dataTreeNode.ReportTableView.Rows[i], null);
                    RightTreeNode.FillNode(collectionNode, ref node);
                }
            }
            else
            {
                for (int i = 0; i < RightTreeNode.dataTreeNode.ReportTableView.Rows.Count; i++)
                {
                    //objectTreeView.CheckedNodes.Add(RightTreeNode.CreateNode(RightTreeNode.dataTreeNode.ReportTableView.Rows[i], 0));
                    node.ChildNodes.Add(RightTreeNode.CreateNode(RightTreeNode.dataTreeNode.ReportTableView.Rows[i], 0));
                }
            }

            objectTreeView.Nodes.Add(node);
        }

        private void InitializationRightTreeNode(string entity)
        {
            RightTreeNode = new ObjectsTreeNode();
            RightTreeNode.dataTreeNode = data;
            RightTreeNode.entity = GetSchemaEntitys.FirstOrDefault(x => (string)x.ID == entity);

            if (RightTreeNode.entity.IsHierarchic)
            {
                int i = 0;
                var Iresult = int.TryParse(IndexNodeTextBox.Text, out i);

                if (i == 0)
                {
                    IndexNodeTextBox.Text = "1";
                    i = 1;
                }
                RightTreeNode.totalIndexNode = i;

                //Получаем таблицу и применяем к ней фильтр (Разделения прав доступа)
                //objectsTreeNode.MainTable = Storage.Select<BusinessContent>(objectsTreeNode.entity.ID).Table;
                var userID = Convert.ToInt32(HttpContext.Current.Session["SystemUser.objID"]);
                RightTreeNode.MainTable = Storage.Select<BusinessContent>(RightTreeNode.entity.ID).GetTable(userID);

                GetContrID();
                RightTreeNode.nameColParent = RightTreeNode.entity.Attributes.FirstOrDefault(x => x.AppType == AppType.parentid).ID.ToString();
                RightTreeNode.hashNameCol = RightTreeNode.entity.Attributes.FirstOrDefault(x => x.FPath.ToLower() == "name").ID.ToString();

            }
            else
            {
                if (LevelContainer != null)
                {
                    LevelContainer.Visible = false;
                }
            }

            RightTreeNode.titles = RightTreeNode.entity.Attributes.Where(x => x.AppType == AppType.title).Select(x => x.ID.ToString()).ToList();
        }

        private void FillThisNode(Reporting.Entity entity, ref TreeNode node, string countInstance)
        {
            var entityID = entity.ID.ToString();
            bool isExpanded = false;
            var href = string.Concat(GetLeftUrl, string.Concat("EntityListAttributeView.aspx?entity=", entityID));

            if (entity.IsHierarchic)
            {
                if (Request.Path == "/monitoring/EntityListAttributeView.aspx") // && Request["entity"].ToString() == entityID)
                    Session["PathRightNavi"] = Request.Url.Query;

                if (Session["PathRightNavi"] != null)
                    href = string.Concat(GetLeftUrl, string.Concat("EntityListAttributeView.aspx", Session["PathRightNavi"].ToString()));
            }
            var cssClass = "navigationTagA";

            if (entityID == SelectedEntity && !string.IsNullOrEmpty(entityID)) // <-- Если мы хотим знать на какой находимся Entity , если зашли 'глубже' по TreeView
            {
                isExpanded = true;
                if ((Request["entity"] == null ? "" : Request["entity"].ToString()) == entityID)
                {
                    cssClass = "object_item_selected";
                }
            }

            cssClass = "object_item_selected";
            isExpanded = true;

            node.Text = string.Concat("<span title='", countInstance,
                "'><a href='", href,
                "' class='", cssClass,
                "'>", entity.Name, "</a></span>");
            node.SelectAction = TreeNodeSelectAction.None;
            node.Expanded = isExpanded;
        }

        private void InitializationDataTree()
        {
            data = new DataTreeNode();

            data.selectedEntityID = Request["entity"] == null ? "" : Request["entity"].ToString();
            data.selectedConstrID = Request["constraint"] == null ? null : Request["constraint"].ToString();

            data.urlLevel = Request["level"] == null ? 0 : Convert.ToInt16(Request["level"]);

            if (Page.Session["TableFromPage"] != null)
            {
                data.ReportTableView = Page.Session["TableFromPage"] as DataTable;
                data.selectedInstanceID = Page.Session["instanceIDNavigation"].ToString();
            }
            else
                data.ReportTableView = new DataTable();

            data.currentPath = Request.Url.Query;
            data.getLeftUrl = GetLeftUrl;

            //Что бы не менялся id когда мы находимся на карточке
            if (Request.RawUrl.IndexOf("XDynamicCard.aspx") != -1)
                data.isDynamicCard = true;
            else
                data.isDynamicCard = false;
        }

        private void GetContrID()
        {
            //for (int i = 0; i < LeftTreeNode.MainTable.Rows.Count; i++)
            //{
            //    if (LeftTreeNode.MainTable.Rows[i]["parentID"].ToString() == RightTreeNode.entity.ID.ToString() &&
            //       Convert.ToBoolean(LeftTreeNode.MainTable.Rows[i]["isHierarchic"]))
            //    {
            //        RightTreeNode.constrID = LeftTreeNode.MainTable.Rows[i]["constrID"].ToString();
            //    }
            //}
        }
    }
}