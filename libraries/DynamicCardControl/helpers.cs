
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataReader = Teleform.Reporting.DynamicCard;

namespace Teleform.Reporting.DynamicCard
{
    partial class DynamicCardControl
    {

        private void DownloadFile(object sender, EventArgs e)
        {
            if (sender is IButtonControl)
            {
                var button = sender as IButtonControl;

                Card card;
                var entityID = Convert.ToInt32(button.CommandName);
                Cards.TryGetValue(entityID, out card);
                var field = card.Fields.First(f => f.SystemName == button.CommandArgument);
                var content = DataBaseReader.GetFileContent(card, int.Parse(card.EntityInstance.EntityInstanceID));


                var response = Page.Response;
                response.Clear();
                response.AddHeader("content-disposition", string.Concat("attachment;fileName=", field.Value));
                response.BinaryWrite(content);
                response.Flush();
                response.End();
            }
        }

        public void Recreate()
        {
            RecreateChildControls();
        }

        public void ResetKeyAccessorDependents()
        {
            Page.Session[SessionKey + "CurrentMode"] = null;
        }

        public Mode CurrentMode
        {
            get
            {
                var o = Page.Session[SessionKey + "CurrentMode"];
                return o == null ? Mode.ReadOnly : (Mode)o;
            }
            private set
            {
                Page.Session[SessionKey + "CurrentMode"] = value;
            }
        }

        public void ChangeMode(Mode mode)
        {
            CurrentMode = mode;
        }

        private void ChangeMode(object sender, EventArgs e)
        {
            if (sender is IButtonControl)
            {
                var button = sender as IButtonControl;

                CurrentMode = (Mode)Enum.Parse(typeof(Mode), button.CommandArgument);

                if (ModeChanged != null)
                    ModeChanged(this, EventArgs.Empty);

                RecreateChildControls();
            }
        }

        private void AcceptChanges(object sender, EventArgs e)
        {

            foreach (var card in Cards.Values)
            {
                SaveState(card, true);

                var emptySelfColumnValue = card.Fields.Where(f => f is CardSelfField).OfType<CardSelfField>().FirstOrDefault(f => !f.IsForbidden && !f.IsNullable && !f.ContainsNonEmptyValue());

                if (emptySelfColumnValue != null)
                {
                    if (CardNotSaving != null)
                    {
                        CardNotSaving(this, new EmptyFieldCheckerEventArgs(emptySelfColumnValue.Name));
                        return;
                    }
                }
                if (card.EntityInstance != null)
                {
                    if (card.Entity.AncestorID == -1)
                    {
                        var emptyRelationColumnValue = card.EntityInstance.RelationColumnsValue.FirstOrDefault(c => !c.ConstraintIsNullable && c.Value == "");

                        if (emptyRelationColumnValue != null)
                        {
                            var constraint = card.EntityInstance.Constraints.First(con => con.ConstraintName == emptyRelationColumnValue.ConstraintName);
                            if (CardNotSaving != null)
                            {
                                CardNotSaving(this, new EmptyFieldCheckerEventArgs(constraint.Alias));
                                return;
                            }
                        }
                    }
                    else
                    {
                        foreach (var columnValue in card.EntityInstance.RelationColumnsValue)
                        {
                            if (string.IsNullOrEmpty(columnValue.Value.ToString()) && !columnValue.ConstraintIsNullable)
                            {
                                var constraint = card.EntityInstance.Constraints.First(con => con.ConstraintName == columnValue.ConstraintName);
                                if (!constraint.IsIdentified)
                                {
                                    if (CardNotSaving != null)
                                    {
                                        CardNotSaving(this, new EmptyFieldCheckerEventArgs(constraint.Alias));
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    card.EntityInstance.IsChanged = true;
                }
            }
            if (CardSaving != null)
                CardSaving(this, EventArgs.Empty);
        }


        public SessionContent SessionContent
        {
            get
            {
                var contexts = Page.Session["contexts"] as Dictionary<string, SessionContent>;

                if (contexts == null)
                    Page.Session["contexts"] = contexts = new Dictionary<string, SessionContent>();

                var sBuilder = new System.Text.StringBuilder(Page.Request.CurrentExecutionFilePath);

                var queryString = Page.Request.QueryString;

                foreach (var queryKey in queryString.Keys)
                    sBuilder.Append(queryKey.ToString()).Append(queryString[queryKey.ToString()].ToString());

                var key = sBuilder.ToString();

                SessionContent sessionContent;

                if (!contexts.TryGetValue(key, out sessionContent))
                {
                    sessionContent = new SessionContent();
                    contexts.Add(key, sessionContent);
                }

                return sessionContent;
            }
        }


    }
}
