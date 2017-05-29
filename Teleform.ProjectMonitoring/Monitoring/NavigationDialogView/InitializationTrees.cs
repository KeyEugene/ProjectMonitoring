using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Teleform.ProjectMonitoring.HttpApplication;
using Teleform.ProjectMonitoring.NavigationDialogView;
using Teleform.Reporting;

namespace Teleform.ProjectMonitoring
{
    public partial class NavigationDialog
    {
        #region Initialization

        public DataTable GetMetadata()
        {
            return Global.GetDataTable("SELECT * FROM [model].[vo_ParentChildEntity] order by parentAlias, isHierarchic");
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

        private void InitializationLeftTreeNode()
        {
            LeftTreeNode = new NavigationTreeNode();

            if (Session["TableNovigationPath"] == null)
            {
                throw new Exception("Не подгрузилась таблица [model].[vo_ParentChildEntity] ");
            }
            else
            {
                LeftTreeNode.MainTable = (DataTable)Session["TableNovigationPath"];
            }

            LeftTreeNode.dataTreeNode = data;
            LeftTreeNode.collection = HashCollection;

            //Костыль, переписать потом по-русски
            UpdateHashCollectionWithID();

            //получаем EntityID на котором находимся, в дальнейшем будем открывать TreeNode с этим ID
            if (Request.RawUrl.IndexOf("EntityListAttributeView.aspx") != -1)
                SelectedEntity = Request["entity"];
        }

        //Обновляем InstanceID у HashTable
        private void UpdateHashCollectionWithID()
        {
            if (Session["entityID"] == null) //|| Request["id"] == null)
            {
                Session["entityID"] = Request["entity"];
                return;
            }
            var newInstanceID = Request["id"];
            var oldEntityID = Session["entityID"].ToString();
            Session["entityID"] = Request["entity"];
            var level = Request["level"];


            for (int i = 0; i < LeftTreeNode.MainTable.Rows.Count; i++)
            {
                if (LeftTreeNode.MainTable.Rows[i]["parentID"].ToString() == oldEntityID &&
                    (ShowAllNavigation.Checked == true ? true : Convert.ToBoolean(LeftTreeNode.MainTable.Rows[i]["isIdentified"]) == true))
                {
                    if (LeftTreeNode.collection.ContainsKey(LeftTreeNode.MainTable.Rows[i]["constrID"] + level))
                    {
                        LeftTreeNode.collection[LeftTreeNode.MainTable.Rows[i]["constrID"] + level] = newInstanceID;
                    }
                }
            }
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
                LevelContainer.Visible = false;
            }

            RightTreeNode.titles = RightTreeNode.entity.Attributes.Where(x => x.AppType == AppType.title).Select(x => x.ID.ToString()).ToList();
        }

        private void GetContrID()
        {
            for (int i = 0; i < LeftTreeNode.MainTable.Rows.Count; i++)
            {
                if (LeftTreeNode.MainTable.Rows[i]["parentID"].ToString() == RightTreeNode.entity.ID.ToString() &&
                   Convert.ToBoolean(LeftTreeNode.MainTable.Rows[i]["isHierarchic"]))
                {
                    RightTreeNode.constrID = LeftTreeNode.MainTable.Rows[i]["constrID"].ToString();
                }
            }
        }

        #endregion
    }
}