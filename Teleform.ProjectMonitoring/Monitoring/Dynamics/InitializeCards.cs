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

namespace Teleform.ProjectMonitoring.Dynamics
{
    using Reporting.DynamicCard;
    using Teleform.ProjectMonitoring.HttpApplication;
    using System.Text;

    public partial class XDynamicCard
    {

        private Dictionary<int, Card> InitializeDynamicCards(DynamicCardControl DCControl, int entityID, int? instanceID, int? constraintID)
        {
            var reader = new DatabaseReader(Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString);

            Dictionary<int, Card> cards;
            Card card;
            var table = new DataTable();
            var query = string.Empty;

            if (DCControl.CurrentMode == Mode.Create && instanceID != null)
                DCControl.ChangeMode(Mode.ReadOnly);


            if (constraintID != null || instanceID == null)
                DCControl.ChangeMode(Mode.Create);

            var mode = DCControl.CurrentMode;

            if (mode == Mode.ReadOnly)
            {
                query = string.Format("select [Permission].[isItPermissible] ('{0}', {1}, 1,0,0,0)", entityID, instanceID);
                table = CheckUserRights(DCControl, table, query);

                card = XmlCardSerializer.Deserialize(entityID);

                card.InstanceID = instanceID;
                reader.FillFieldsValue(card, instanceID.Value);

                cards = new Dictionary<int, Card>();
                cards.Add(entityID, card);
                SetAncestorCards(mode, card, cards, instanceID, reader);
                cards = cards.Reverse().ToDictionary(k => k.Key, v => v.Value);
            }
            else if (mode == Mode.Edit || mode == Mode.Create)
            {
                if (mode == Mode.Edit)
                {
                    query = string.Format("select [Permission].[isItPermissible] ('{0}', {1}, 0,0,1,0)", entityID, instanceID);
                    table = Storage.GetDataTable(query);

                    //проверяем имеет ли пользователь права на действие
                    if (Convert.ToBoolean(table.Rows[0][0]) == false)
                    {
                        DCControl.ChangeMode(Mode.ReadOnly);
                        DenayPermissionDialog.Show();
                        Reset(DCControl);                                              

                        //return null;
                    }
                }
                else if (mode == Mode.Create)
                {
                    query = string.Format("select [Permission].[isItPermissible] ('{0}', null, 0,1,0,0)", entityID);
                    table = CheckUserRights(DCControl, table, query);
                }
                AdditionalViews.SetActiveView(DefaultView);

                if (Session[DCControl.SessionKey] == null)
                {
                    card = XmlCardSerializer.Deserialize(entityID);

                    if (mode != Mode.Create)
                        reader.FillFieldsValue(card, instanceID.Value);

                    if (constraintID != null && instanceID != null)
                    {
                        card.ConstraintID = constraintID;
                        card.InstanceID = instanceID;
                    }
                    cards = new Dictionary<int, Card>();
                    cards.Add(entityID, card);
                    SetAncestorCards(mode, card, cards, instanceID, reader);
                    cards = cards.Reverse().ToDictionary(k => k.Key, v => v.Value);
                    Session[DCControl.SessionKey] = cards;
                }
                else
                    cards = Session[DCControl.SessionKey] as Dictionary<int, Card>;
            }
            else
            {
                throw new NotSupportedException("Не поддерживаемый режим.");
            }
            DCControl.DataBaseReader = reader;


            foreach (var item in cards)
                DCControl.SetFieldsTaboo(DCControl.userID, item.Value);

            DCControl.Cards = cards;
            return cards;
        }

        private void SetAncestorCards(Mode mode, Card card, Dictionary<int, Card> cards, int? instanceID, DatabaseReader reader)
        {
            if (card.Entity.AncestorID != -1)
            {
                var ancestorCard = XmlCardSerializer.Deserialize(card.Entity.AncestorID);
                if (mode != Mode.Create)
                    reader.FillFieldsValue(ancestorCard, instanceID.Value);
                ancestorCard.IsAncestor = true;
                cards.Add(card.Entity.AncestorID, ancestorCard);

                SetAncestorCards(mode, ancestorCard, cards, instanceID, reader);
            }
        }

        private DataTable CheckUserRights(DynamicCardControl control, DataTable table, string query)
        {
            
            table = Storage.GetDataTable(query);

            //проверяем на имеет пользователь права на действие
            if (Convert.ToBoolean(table.Rows[0][0]) == false)
            {
                Reset(control);
                Session["isPermission"] = true;

                DenayPermissionDialog.Show();
                Response.Redirect(string.Format("~/EntityListAttributeView.aspx?entity={0}", Request.QueryString["entity"]));

            }
            return table;
        }



    }
}