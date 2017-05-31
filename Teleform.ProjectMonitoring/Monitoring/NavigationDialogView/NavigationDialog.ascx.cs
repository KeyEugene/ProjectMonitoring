#define alexj

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Teleform.ProjectMonitoring.HttpApplication;
using Teleform.ProjectMonitoring.NavigationDialogView;
using Teleform.Reporting;

namespace Teleform.ProjectMonitoring
{
    public partial class NavigationDialog : System.Web.UI.UserControl
    {
        public string GetCurrentUrl
        {
            get
            {
                return Request.Url.AbsoluteUri;
            }
        }
        public string GetLeftUrl
        {
            get
            {
                return Session["getLeftUrl"] == null ? null : Session["getLeftUrl"].ToString();
            }
        }
        /// <summary>
        /// Collection ConstrID InstanceID
        /// </summary>
        public Hashtable HashCollection
        {
            get { return Session["NavigationPath"] == null ? new Hashtable() : (Hashtable)Session["NavigationPath"]; }
            set { Session["NavigationPath"] = value; }
        }
        public string SelectedEntity
        {
            get { return Session["SelectedEntity"] == null ? "" : (string)Session["SelectedEntity"]; }
            set { Session["SelectedEntity"] = value; }
        }
        /// <summary>
        /// Вкл./Откл. навигации
        /// </summary>
        private bool isOnNavigation
        {
            get
            {
                return Session["checkBoxMainNavigation"] == null ? true : Convert.ToBoolean(Session["checkBoxMainNavigation"]);
            }
        }
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

        private BottomTreeNodeBase LeftTreeNode;
        private BottomTreeNodeBase RightTreeNode;
        private DataTreeNode data;
        private IEnumerable<Entity> GetSchemaEntitys;

        #region Cycle life page
        protected override void OnInit(EventArgs e)
        {
            Session["TableNovigationPath"] = GetMetadata();

            #region Получение сегмента URL cтроки, для формирования link-кнопок
            //ToDo: Временно, исправить ...
            string monitorName = Request.Url.Segments.FirstOrDefault(x => x.StartsWith("monitoring"));

            if (string.IsNullOrEmpty(monitorName))
                monitorName = Request.Url.Segments[1];

            Session["getLeftUrl"] = string.Concat(Request.Url.GetLeftPart(UriPartial.Authority), "/", monitorName);
            #endregion

            ShowAllNavigation.Checked = Session["ShowAllNavigation"] == null ? false : Convert.ToBoolean(Session["ShowAllNavigation"]);
        }

        protected override void OnPreRender(EventArgs e)
        {
            GetSchemaEntitys = this.GetSchema().Entities;

            if (isOnNavigation)
            {
                //NavigationTreeDialog.Visible = true;
                Session["ShowAllNavigation"] = ShowAllNavigation.Checked;
                InitializationDataTree();
                InitializationLeftTreeNode();
                FillTreeView();
            }
            //else
                //NavigationTreeDialog.Visible = false;

            if (isOnObjectsNavigation)
            {
                NavigationObjects.Visible = true;
                //Если находимся в мониторинге и entity определен
                var entity = Request["entity"];

                if (!string.IsNullOrEmpty(entity))
                {
                    InitializationRightTreeNode(entity);
                    CreateObjectTree();
                }
                else
                {
                    NavigationObjects.Visible = false;
                }
            }
            else
                NavigationObjects.Visible = false;

            Page.Session["TableFromPage"] = null;
        }

        #endregion

        private void FillTreeView()
        {
            treeView.Nodes.Clear();
            objectTreeView.Nodes.Clear();

            //Вход в админку и "Шаблоны" только Типу Администратор
            //var typeID = Convert.ToInt32(Session["SystemUser.typeID"]);
            //var userID = Convert.ToInt32(Session["SystemUser.objID"]);

            FillMonitoring();
            //treeView.Nodes.Add(FillMonitoring());

            //var templatePermission = StorageUserObgects.Select<UserTemplatePermission>(userID, userID).Permission;

            //if (typeID == 1 || typeID == 0) //typeID == 0 - временно
            //    treeView.Nodes.Add(CreateMainNode("Шаблоны", "Templates/TemplateManager.aspx"));

            //if (typeID == 0 || typeID == 1)
            //{
            //    var reportsNode = CreateMainNode("Специальные отчеты", "Reporting/Reports.aspx");
            //    reportsNode.ChildNodes.Add(CreateMainNode("Древовидное представление", "HardTemplate/HardTemplateView.aspx"));
            //    reportsNode.ChildNodes.Add(CreateMainNode("Перекрестное представление", "CrossTemplate/CrossTemplateView.aspx"));
            //    reportsNode.Expanded = false;
            //    treeView.Nodes.Add(reportsNode);
            //}

            //if (typeID == 2 || typeID == 5 || typeID == 1 || typeID == 0)  //typeID == 0 - временно
            //    treeView.Nodes.Add(CreateMainNode("Маршруты документов", "Routes/Routes.aspx"));

            //treeView.Nodes.Add(CreateMainNode("Уведомления", "events.aspx"));
            //treeView.Nodes.Add(CreateMainNode("Личные настройки", "Settings.aspx"));

            //if (typeID == 1 || typeID == 0) //typeID == 0 - временно
            //    treeView.Nodes.Add(CreateMainNode("Администрирование", "admin/administration.aspx"));
            
        }

        #region  Fill navigation Tree Node
        private TreeNode FillMonitoring()
        {
            //var nodeMonitoring = CreateMainNode("Функционал АРМ", "EntityListAttributeView.aspx");// new TreeNode();//

#if alexj

            #region Отсеивание Entity без права доступа
            var dt = Storage.GetDataTable(string.Format(@"select iif(t.alias is NULL,'Шаблон',t.alias) typeAlias, p.* from Permission.UserPermission({0},NULL)p 
                                                left join model.BTables b on b.name=p.entity
                                                left join model.AppTypes t on t.object_id=b.appTypeID
                                                where  p.objID is NULL
                                                order by p.entityAlias", Session["SystemUser.objID"].ToString()));

            var permittedEntities = dt.AsEnumerable().Where(ent => Convert.ToBoolean(ent["read"]));
            var allEntities = GetSchemaEntitys.Where(o => o.IsMain && !o.IsEnumeration).OrderBy(o => o.Name);

            var Entities = allEntities.Where(ent => permittedEntities.Select(o => o["entity"].ToString()).Contains(ent.SystemName));
            #endregion


            foreach (var entity in Entities)
#else
            foreach (var entity in GetSchemaEntitys.Where(o => o.IsMain && !o.IsEnumeration).OrderBy(o => o.Name))
#endif
            {
                var node = new TreeNode();
                FillThisNode(entity, ref node, GetCountInstance(entity.SystemName));
                FilledNavigationNode(entity, ref node);
                treeView.Nodes.Add(node);
                //nodeMonitoring.ChildNodes.Add(node);

                //Делает для того что бы был открыт главный Node, если мы находимся внутри его
                node.Expanded = GetExpandedMainNode();
               //node.Expanded = GetExpandedMainNode();
            }
            return null;
        }

        private void FilledNavigationNode(Reporting.Entity entity, ref TreeNode node)
        {
            string parentID = entity.ID.ToString();

            foreach (DataRow row in LeftTreeNode.MainTable.Rows)
            {
                var parentID1 = row["parentID"];
                var parentID2 = parentID;
                var showAll = ShowAllNavigation.Checked == true ? true : Convert.ToBoolean(row["isIdentified"]); //row["isIdentified"].ToString() == "1"
                var isIdent = row["isIdentified"];

                if (row["parentID"].ToString() == parentID && (ShowAllNavigation.Checked == true ? true : Convert.ToBoolean(row["isIdentified"])))
                //Последняя проверка в if-е учитывает ChechBox(Показать все, показать в организациях подчиненные организации) т.е. не учитывать флаг  isIdentified или учитывать
                {
                    var instanceID = GetInstanceID(parentID, row["childID"].ToString());

                    var collectionNodes = LeftTreeNode.BuildChildNodes(row, instanceID);
                    LeftTreeNode.FillNode(collectionNodes, ref node);
                }
            }
            HashCollection = LeftTreeNode.collection;
        }
        #endregion

        #region Bild for FronEnd (Node)


        private TreeNode CreateMainNode(string namePage, string leftPath)
        {
            var href = string.Concat(GetLeftUrl, leftPath);
            var cssClass = "navigationMainTagA";

            if (href == GetCurrentUrl)
            {
                cssClass = "navigationTagASelected";
                SelectedEntity = "xyz";
                HashCollection = new Hashtable(); ;
            }
            var s = string.Concat("<a href='", href,
                "' class='", cssClass,
                "'>", namePage, "</a>");
            var treeNode = new TreeNode { Text = s, SelectAction = TreeNodeSelectAction.None };
            return treeNode;
        }

        private void FillThisNode(Reporting.Entity entity, ref TreeNode node, string countInstance, bool isObjects = false)
        {
            var entityID = entity.ID.ToString();
            bool isExpanded = false;
            var href = string.Concat(GetLeftUrl, string.Concat("EntityListAttributeView.aspx?entity=", entityID));

            if (entity.IsHierarchic && isObjects)
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
                    cssClass = "navigationTagASelected";

                    //Обнуляем  пару ConstrID and InstanceID, если ненаходимся в карточке и главном ноде Мониторинга
                    if (!data.isDynamicCard && Request.Path == "/monitoring/EntityListAttributeView.aspx" && !isObjects)
                        LeftTreeNode.collection = HashCollection = new Hashtable();
                }
            }

            //Отмечаем как выбранный Node в навигации по объектам
            if (isObjects)
            {
                cssClass = "navigationTagASelected";
                isExpanded = true;
            }

            node.Text = string.Concat("<span title='", countInstance,
                "'><a href='", href,
                "' class='", cssClass,
                "'>", entity.Name, "</a></span>");
            node.SelectAction = TreeNodeSelectAction.None;
            node.Expanded = isExpanded;
        }
        #endregion

        private bool GetExpandedMainNode()
        {
            var count = Request.Url.Segments.FirstOrDefault(x => x == "ListAttributeView.aspx"
                || x == "EntityListAttributeView.aspx"
                || x == "XDynamicCard.aspx");

            return String.IsNullOrEmpty(count) ? false : true;
        }

        private void CreateObjectTree()
        {
            var node = new TreeNode();

            FillThisNode(RightTreeNode.entity, ref node, "", true);

            if (!RightTreeNode.entity.IsHierarchic)
                for (int i = 0; i < RightTreeNode.dataTreeNode.ReportTableView.Rows.Count; i++)
                    node.ChildNodes.Add(RightTreeNode.CreateNode(RightTreeNode.dataTreeNode.ReportTableView.Rows[i], 0));
            else
                for (int i = 0; i < RightTreeNode.dataTreeNode.ReportTableView.Rows.Count; i++)
                {
                    RightTreeNode.currentIndexNode = 0;
                    var collectionNode = RightTreeNode.BuildChildNodes(RightTreeNode.dataTreeNode.ReportTableView.Rows[i], null);
                    RightTreeNode.FillNode(collectionNode, ref node);
                }

            objectTreeView.Nodes.Add(node);
        }

        private string GetInstanceID(string parentID, string childID)
        {
            string instanceID = String.Empty;

            if (parentID == data.selectedEntityID || childID == data.selectedEntityID)
                instanceID = data.selectedInstanceID;

            this.HashCollection = HashCollection;
            LeftTreeNode.collection = HashCollection;

            return instanceID;
        }

        private string GetCountInstance(string p)
        {
            // ToDo: Count...
            //var dt = Global.GetDataTable(string.Concat(" select count(*) from ", p));
            //return dt.Rows[0][0].ToString();
            return "";
        }
    }
}

