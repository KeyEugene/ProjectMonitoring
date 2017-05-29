#define Viktor

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using Teleform.Reporting;
using Teleform.Reporting.Reporting.Template;
using Teleform.Reporting.Web;

using EmptyFieldCheckerEventArgs = Teleform.Reporting.DynamicCard.DynamicCardControl.EmptyFieldCheckerEventArgs;

namespace Teleform.ProjectMonitoring.Dynamics
{
    using Reporting.DynamicCard;
    using Teleform.ProjectMonitoring.HttpApplication;
    using System.Text;

    public partial class XDynamicCard : BasePage
    {

        //private string SessionKey
        //{
        //    get { return string.Concat("Key", Request["entity"]); }
        //}

        private string SessionKey
        {
            get { return string.Concat("entity=", Request["entity"]); }
        }

        private Dictionary<int, Card> Cards { get; set; }






        private void ControlParameters()
        {

            var entityIDParameter = Request["entity"];
            var instanceIDParameter = Request["id"];
            var constraintIDParameter = Request["constraintID"];

            if (string.IsNullOrEmpty(entityIDParameter))
                throw new Exception("Не указан идентификатор типа сущности.");

            int? constraintID = null;

            if (constraintIDParameter != null)
                constraintID = int.Parse(constraintIDParameter);

            int? instanceID = null;

            if (instanceIDParameter != null && !string.IsNullOrEmpty(instanceIDParameter))
                instanceID = int.Parse(instanceIDParameter);
            else
                instanceID = null;

            DCControl.userID = Convert.ToInt32(Session["SystemUser.objID"]);
            DCControl.SessionKey = SessionKey;
            DCControl.Schema = Teleform.ProjectMonitoring.HttpApplication.Global.Schema;

            Cards = InitializeDynamicCards(DCControl, int.Parse(entityIDParameter), instanceID, constraintID);


        }

        protected void RelationClickedHandler(object sender, DynamicCardControl.RelationClickedEventArgs e)
        {
            TeplatesFilters.Visible = false;
            SaveObjects.Visible = false;
            DeleteImage.Visible = false;
            DeleteInstance.Visible = false;
            ListEntity.Visible = false;
            RecordsNumberLabel.Visible = false;


            var relation = e.Relation;

            var reader = new DatabaseReader(Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString);

            var entID = int.Parse(e.Relation.Entity.ID.ToString());

            var instanceID = reader.GetInstanceID(relation);

            InitializeDynamicCards(HelpCard, entID, instanceID, null);

            HelpCard.Recreate();

            AdditionalViews.SetActiveView(ObjectView);
        }

        private string PostBackUrl
        {
            get { return Session["PostBackUrl"] as string; }
            set { Session["PostBackUrl"] = value; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsCrossPagePostBack)
            {
                ControlParameters();
                Card card;

                var entityID = Convert.ToInt32(Request["entity"]);

                Cards.TryGetValue(entityID, out card);

                if (card != null)
                    CurrentPageTitle = "Карточка " + card.Entity.Name;
                else
                    throw new Exception("Что то не так, карточка не найдена, обратитесь к разработчикам");

                //PageTitle = "Карточка " + Cards.FirstOrDefault(c => c.Key == entityID).Value.Entity.Name;

            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Request.UrlReferrer != null)
                PostBackUrl = Request.UrlReferrer.ToString();

            if (!IsCrossPagePostBack)
            {

                if (!string.IsNullOrEmpty(ResizableCardBox.Text))
                {
                    DCControl.SetFieldsSize(ResizableCardBox.Text);
                    DCControl.Recreate();
                }
            }
            else
                Reset(DCControl);

            if (EntityQueryString != Request.QueryString["entity"])
            {
                EntityQueryString = Request.QueryString["entity"];
                CardListRelationName = "";
            }
            else
            {
                if (!string.IsNullOrEmpty(TemplateList.SelectedValue) && !string.IsNullOrEmpty(CardListRelationName))
                {
                    var templateID = TemplateList.SelectedValue;
                    Draw_ReportViewControl();
                }
            }
        }



        protected void SelfFieldClickedHandler(object sender, DynamicCardControl.SelfFieldClickedEventAgrs e)
        {
            var selfField = e.SelfField;
            var instanceID = selfField.Card.EntityInstance.EntityInstanceID;
            var entityID = selfField.Card.Entity.ID.ToString();

            DocumentPreview.ID = instanceID;
            DocumentPreview.EntityID = entityID;
            DocumentPreview.InitializeAsUserControl(this);
            AdditionalViews.SetActiveView(DocumentPreviewView);
        }
        private string CardListRelationName
        {
            get { return ViewState["CardListRelationName"] as string; }
            set { ViewState["CardListRelationName"] = value; }
        }

        private string CardListRelationSystemName
        {
            get { return ViewState["CardListRelationSystemName"] as string; }
            set { ViewState["CardListRelationSystemName"] = value; }
        }

        private string CardListRelationEntityID
        {
            get { return ViewState["CardListRelationEntityID"] as string; }
            set { ViewState["CardListRelationEntityID"] = value; }
        }

        private string EntityQueryString
        {
            get { return ViewState["EntityQueryString"] as string; }
            set { ViewState["EntityQueryString"] = value; }
        }



        protected void DynamicCard_EditionCanceled(object sender, EventArgs e)
        {
            Reset(DCControl);
        }


        public static string GetTransactionString(string query)
        {
            return string.Format(@"BEGIN TRANSACTION [Tran1]
            BEGIN TRY
            {0}
            COMMIT TRANSACTION [Tran1]
            END TRY
            BEGIN CATCH
              ROLLBACK TRANSACTION [Tran1]
            END CATCH  ", query);
        }

        protected void DynamicCardCommonContainer_CardNotSaving(object sender, EmptyFieldCheckerEventArgs e)
        {            
            ErrorMessageBox.Show();
            (ErrorMessageBox.FindControl("ErrorLabel") as Label).Text = string.Concat("Не заполнено обязательное поле ", e.FieldName);
        }


        protected void DynamicCardCommonContainer_CardSaving(object sender, EventArgs e)
        {
            var dynamicCardControl = sender as DynamicCardControl;

            if (dynamicCardControl.CurrentMode != Mode.ReadOnly)
            {
                var deletingObjects = new Dictionary<string, Int32>();

                foreach (Card card in dynamicCardControl.Cards.Values)
                {
                    var xml = XmlCardSerializer.Serialize(card, dynamicCardControl.CurrentMode, dynamicCardControl.LastInsertedInstanceID);

                    var query = string.Format("DECLARE @instanceID objID EXEC [model].[BObjectSave] @xml = '{0}', @instanceID = @instanceID OUTPUT SELECT @instanceID as '@instanceID'", xml);

#if trueWWW
                    var table = Storage.GetDataTable(query);
                    if (table.Rows[0][0] != DBNull.Value)
                        dynamicCardControl.LastInsertedInstanceID = Convert.ToInt32(table.Rows[0][0]);

#else
                    try
                    {
                        var table = Storage.GetDataTable(query);
                        if (table.Rows[0][0] != DBNull.Value)
                            dynamicCardControl.LastInsertedInstanceID = Convert.ToInt32(table.Rows[0][0]);
                    }
                    catch (Exception ex)
                    {
                        Reset(dynamicCardControl);
                        ErrorMessageBox.Show();                        
                        (ErrorMessageBox.FindControl("ErrorLabel") as Label).Text = ex.Message;
                        //CreateErroreDialog.Show();                      
                    }
#endif

                }

                Storage.ClearBusinessContents();

                Storage.BusinessContentIsChanged = true;

                if (dynamicCardControl.CurrentMode == Mode.Edit)
                {
                    Reset(dynamicCardControl);
                }
                else if (dynamicCardControl.CurrentMode == Mode.Create && dynamicCardControl.LastInsertedInstanceID != -1)
                {
                    dynamicCardControl.ChangeMode(Mode.ReadOnly);
                    Reset(dynamicCardControl);
                    reloadCard(dynamicCardControl.LastInsertedInstanceID);
                }
            }
        }

        private void reloadCard(int insertedID)
        {
#if true


            var currentPageContext = RecentPages.Last();

            var entityID = Request.QueryString["entity"];
            currentPageContext.UrlParamaters.Clear();
            currentPageContext.UrlParamaters.Add(string.Concat("entity=", entityID));
            currentPageContext.UrlParamaters.Add(string.Concat("id=", insertedID));

            var parameters = currentPageContext.UrlParamaters.Aggregate((ready, next) => ready + "&" + next);

            if (!string.IsNullOrEmpty(parameters))
                parameters = "?" + parameters;

            var redirectPath = string.Concat(currentPageContext.Path, parameters);
            //RecentPages.Remove(currentPageContext);
            

            Response.Redirect(redirectPath);


#else
                        
            var currentPage = RecentPages[RecentPages.Count - 1];
            RecentPages.Remove(currentPage);
            var currentPath = string.Format("{0}?{1}&id={2}", currentPage.Path, currentPage.UrlParamaters[0], insertedID);

            if (!string.IsNullOrEmpty(currentPath))
                Server.Transfer(currentPath, false);
#endif
        }


        private void goToPreviosPage()
        {
            //var page = Page as BasePage;

            var currentPageContext = RecentPages.Last();
            var index = RecentPages.FindIndex(item => item.AbsoluteUri == currentPageContext.AbsoluteUri);
            if (index > 0)
            {
                var previosPage = RecentPages[index - 1];
                Response.Redirect(previosPage.AbsoluteUri);
            }


            //var parameters = string.Empty;

            //if (previosPage.UrlParamaters.Count > 0)
            //{
            //    parameters = previosPage.UrlParamaters.Aggregate((ready, next) => ready + "&" + next);

            //    if (!string.IsNullOrEmpty(parameters))
            //        parameters = "?" + parameters;


            //    var goAweyPath = AppRelativeVirtualPath.Replace(@"~", "");
            //    if (!previosPage.AbsoluteUri.Contains(goAweyPath))
            //        ReturnControl(previosPage.AbsoluteUri);
            //    else
            //    {
            //        var previosPath = string.Concat(previosPage.Path, parameters);
            //        if (!string.IsNullOrEmpty(previosPath))
            //            Server.Transfer(previosPath);

            //    }

            //}
        }


        protected void DynamicCard_ControlReturning(object sender, EventArgs e)
        {
            var control = sender as DynamicCardControl;

            control.ChangeMode(Mode.ReadOnly);
            Reset(control);
            goToPreviosPage();

        }

        protected void ReturnControl(string previosPageUrl)
        {

            Page.ClientScript.RegisterStartupScript(GetType(), "blablabla",
            Page.ClientScript.GetPostBackEventReference(
                        new PostBackOptions(this, null, previosPageUrl, false, true, false, true, false, null)),
                    true);
        }

        private void Reset(DynamicCardControl dynamicCardControl)
        {

            var x = Session[dynamicCardControl.SessionKey];
            Session[dynamicCardControl.SessionKey] = null;
        }




        protected void ModeChangedHandler(object sender, EventArgs e)
        {
            if (sender is DynamicCardControl)
            {
                var control = sender as DynamicCardControl;

                ControlParameters();
            }
        }





        string GetBObjectDataCondition = string.Empty;

        protected void ListRelationClickedHandler(object sender, DynamicCardControl.ListRelationClickedEventArgs e)
        {
            DCControl.EntityName = e.ListRelation.Entity.SystemName;
            CardListRelationName = e.ListRelation.Name;
            CardListRelationSystemName = e.ListRelation.SystemName;

            CardListRelationEntityID = e.ListRelation.Entity.ID.ToString();

            ViewState["GetBObjectDataCondition"] = GetBObjectDataCondition = e.ListRelation.MakeQuery();
            FillDropDownList();
            Draw_ReportViewControl();
        }




        private string Path()
        {
            foreach (var item in Request.Url.Segments)
                if (item.StartsWith("monitoring")) return string.Concat(Request.Url.GetLeftPart(UriPartial.Authority), "/", item);

            return null;
        }


        protected void ReportViewControl_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        protected void ReportViewControl_DataReady(object sender, EventArgs e)
        {
            ListEntity.Text = string.Format("{0}{1}", ReportViewControl.Template.Entity.Name, ":");
            RecordsNumberLabel.Text = ReportViewControl.DataView.Count.ToString();

        }



        protected void DeleteMessage_Closed(object sender, Phoenix.Web.UI.Dialogs.MessageBoxEventArgs e)
        {
            if (e.Result == Phoenix.Web.UI.Dialogs.MessageBoxResult.Yes)
            {
                int instanceID;
                var isParse = Int32.TryParse(SelectedRowIDBox.Text, out instanceID);
                if (isParse)
                {
                    var templateID = TemplateList.SelectedValue;
                    var template = Storage.Select<Template>(templateID);

                    ReportViewControl.DeleteInstance(template.Entity.ID.ToString(), instanceID);

                    Draw_ReportViewControl();

                    ControlParameters();
                    DCControl.Recreate();
                }
                else
                {
                    throw new Exception("Объект не уадален, надо выбрать объект");
                }
            }
        }

        protected void DeleteInstance_Click(object sender, EventArgs e)
        {
            if (ReportViewControl.SelectedRowIndex != -1)
            {
                DeleteMessage.Show();
            }
        }

        protected void SaveObjects_OnClick(object sender, EventArgs e)
        {
             try
            {
                ReportViewControl.SaveInstances();
            }
             catch (Exception ex)
             {
                 ErrorMessageBox.Show();
                 (ErrorMessageBox.FindControl("ErrorLabel") as Label).Text = ex.Message;
             }
           
            SaveObjectsJeysonBox.Text = null;

            Draw_ReportViewControl();
        }

        protected void TemplateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var templateID = TemplateList.SelectedValue;

            var template = Storage.Select<Template>(templateID);

            var listEntityID = Storage.Select<Entity>(template.Entity.ID).ID.ToString();

            var templateSessionKey = string.Format("TemplateList_{0}", listEntityID);// MakeUniqueKey("TemplateList");
            Session[templateSessionKey] = TemplateList.SelectedValue;

            Draw_ReportViewControl();

        }
        private void FillDropDownList()
        {

            TemplateList.Items.Clear();
            TemplateList.Items.Add(new ListItem("Титульный шаблон", string.Format("titleAttributesTemplate_{0}", CardListRelationEntityID)));
            TemplateList.Items.Add(new ListItem("Создание и редактирование объектов", string.Format("requireAttributesTemplate_{0}", CardListRelationEntityID)));

            TemplateList.DataBind();

            var templateSessionKey = string.Format("TemplateList_{0}", CardListRelationEntityID);
            var templateID = Session[templateSessionKey];
            if (templateID != null)
                TemplateList.SelectedValue = templateID.ToString();
            else
                TemplateList.SelectedValue = string.Format("requireAttributesTemplate_{0}", CardListRelationEntityID);

        }


        protected void ResetAllFilters_OnClick(object sender, EventArgs e)
        {
            ReportViewControl.RejectAllFiltration();
        }

        protected void ResetAllSortings_OnClick(object sender, EventArgs e)
        {
            ReportViewControl.RejectAllSortings();
        }

        private void Draw_ReportViewControl()
        {
            var o = Request.QueryString["id"];

            if (o != null)
            {
                var entityInstanceID = int.Parse(o);

                if (string.IsNullOrEmpty(TemplateList.SelectedValue))
                    throw new Exception(string.Format("Нет шаблона для '{0}'", CardListRelationName));
                else
                {
                    TeplatesFilters.Visible = true;
                    SaveObjects.Visible = true;
                    DeleteImage.Visible = true;
                    DeleteInstance.Visible = true;
                    ListEntity.Visible = true;
                    RecordsNumberLabel.Visible = true;
                }


                var templateID = TemplateList.SelectedValue;

                var template = Storage.Select<Template>(templateID);

                UserTemlatePermission.SetFieldsTaboo(DCControl.userID, template);

                var listEntity = Storage.Select<Entity>(template.Entity.ID);

                var filteredListTable = GetFilteredListTable(template, listEntity);

                //Временно, отключить навигацию по объектам (надо разбираться)                
                Page.Session["checkBoxObjectsNavigation"] = false;

                initialiseTableContorl(listEntity, filteredListTable, templateID, entityInstanceID);

                AdditionalViews.SetActiveView(ReportView);

            }
        }
        private DataTable GetFilteredListTable(Template template, Entity listEntity)
        {
            var businessContent = Storage.Select<BusinessContent>(template.Entity.ID);

            var userID = Convert.ToInt32(HttpContext.Current.Session["SystemUser.objID"]);

            var listTable = businessContent.GetTable(userID);

            var referenceEntityID = Request.QueryString["entity"];

            var referenceEntity = Storage.Select<Entity>(referenceEntityID);
            var referenceEntityInstanseID = Request.QueryString["id"].ToString();

            if (referenceEntity.Lists.Count() == 0)
                return new DataTable();

            var key = referenceEntity.Lists.First(l => l.ConstraintName == CardListRelationSystemName).Key;

            var query = string.Format("SELECT objID FROM {0} WHERE {1} = {2}", listEntity.SystemName, key, referenceEntityInstanseID);

            var tableObjID = Storage.GetDataTable(query);

            DataRow[] rowsObjID = tableObjID.Select();

            if (rowsObjID.Count() == 0)
                return new DataTable();

            StringBuilder listInstancesObjID = new StringBuilder();

            foreach (var row in rowsObjID)
                listInstancesObjID.Append(string.Concat(row["objID"], ","));
            listInstancesObjID.Length--;


            var filterExpression = string.Format("objID in ({0})", listInstancesObjID.ToString());

            DataRow[] rows = listTable.Select(filterExpression);

            DataTable filteredListTable = new DataTable();

            if (rows != null && rows.Count() > 0)
                filteredListTable = rows.CopyToDataTable();

            return filteredListTable;
        }


        private void initialiseTableContorl(Entity entity, DataTable filteredListTable, string templateID, int entityInstanceID)
        {
            ReportViewControl.IsCardControl = true;

            if (!string.IsNullOrEmpty(SelectedRowIDBox.Text))
                ReportViewControl.SelectedRowIndex = int.Parse(SelectedRowIDBox.Text);

            ReportViewControl.EntityInstanceID = entityInstanceID;
            ReportViewControl.SetSelfColumnsValue(SaveObjectsJeysonBox.Text);
            ReportViewControl.SetTemplateFieldsSize(ResizableTableControlBox.Text);
            ReportViewControl.DataSource = filteredListTable;
            ReportViewControl.NavigatFilterExpression = "";
            ReportViewControl.IsEditMode = true;
            ReportViewControl.TemplateID = templateID;
            ReportViewControl.AllowPaging = true;
            ReportViewControl.DataBind();
            ReportViewControl.Visible = true;
            ReportViewControl.Enabled = true;
        }



    }
}