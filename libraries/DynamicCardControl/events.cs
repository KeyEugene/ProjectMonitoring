

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Teleform.Reporting.DynamicCard
{
    partial class DynamicCardControl
    {




        public event EventHandler DocumentDisplaying;

        public event EventHandler EditionCanceled;

        public event EventHandler ControlReturning;

        public event EventHandler ModeChanged;

        public event EventHandler CardSaving;

        public event EventHandler<EmptyFieldCheckerEventArgs> CardNotSaving;


        public class EmptyFieldCheckerEventArgs : EventArgs
        {
            public string FieldName { get; set; }

            public EmptyFieldCheckerEventArgs(string fieldName)
            {
                this.FieldName = fieldName;
            }
        }


        public event EventHandler<ListRelationClickedEventArgs> ListRelationClicked;
      
        public class ListRelationClickedEventArgs : EventArgs
        {
            public CardListRelationField ListRelation { get; private set; }

            public int InstanceID { get; private set; }

            public ListRelationClickedEventArgs(int instanceID, CardListRelationField listRelation)
            {
                InstanceID = instanceID;
                ListRelation = listRelation;
            }
        }






        public event EventHandler<SelfFieldClickedEventAgrs> SelfFieldClicked;
        private void SelfFieldClickedHandler(object sender, EventArgs e)
        {
            if (sender is IButtonControl)
            {
                var button = sender as IButtonControl;
                var fieldSysName = button.CommandArgument;
                var entityID = int.Parse(button.CommandName);

                Card card;
                Cards.TryGetValue(entityID, out card);

                CardSelfField selfField = card.GetSelfField(fieldSysName);
                if (SelfFieldClicked != null)
                    SelfFieldClicked(this, new SelfFieldClickedEventAgrs(selfField));
            }


        }
        public class SelfFieldClickedEventAgrs : EventArgs
        {
            public CardSelfField SelfField { get; private set; }

            public SelfFieldClickedEventAgrs(CardSelfField selfField)
            {
                SelfField = selfField;
            }
        }


        public event EventHandler<RelationClickedEventArgs> RelationClicked;
        public class RelationClickedEventArgs : EventArgs
        {
            public CardRelationField Relation { get; private set; }

            public RelationClickedEventArgs(CardRelationField relation)
            {
                Relation = relation;
            }
        }

        private void ListRelationClickedHandler(object sender, EventArgs e)
        {
            if (sender is IButtonControl && ListRelationClicked != null)
            {
                var button = sender as IButtonControl;

                var listRelationID = int.Parse(button.CommandArgument);

                var entityID = int.Parse(button.CommandName);

                Card card;

                Cards.TryGetValue(entityID, out card);

                var listRelation = card.GetCardListRelation(listRelationID);
                if (ListRelationClicked != null)
                {
                    RecreateChildControls();
                    ListRelationClicked(this, new ListRelationClickedEventArgs(int.Parse(card.EntityInstance.EntityInstanceID), listRelation));
                }
            }
        }
        public void RelationClickedHandler(object sender, EventArgs e)
        {
            if (sender is IButtonControl && RelationClicked != null)
            {
                var button = sender as IButtonControl;

                var relationID = int.Parse(button.CommandArgument);

                var entityID = int.Parse(button.CommandName);

                Card card;

                Cards.TryGetValue(entityID, out card);

                var relation = card.GetRelation(relationID);
                             
                if (RelationClicked != null)
                {
                    RecreateChildControls();
                    RelationClicked(this, new RelationClickedEventArgs(relation));
                }
            }
        }        
        public void RelationEditedButton_Click(object sender, EventArgs e)
        {
            if (sender is IButtonControl)
            {
                DisplayRelatedList = true;

                OpenedRelationSystemName = (sender as IButtonControl).CommandArgument;

                foreach (var card in Cards.Values)
                    SaveState(card);

                OpenedRelationEntityID = (sender as IButtonControl).CommandName;

                RecreateChildControls();
            }
        }





        private void RelationClearedButton_Click(object sender, EventArgs e)
        {
            if (sender is IButtonControl)
            {

                var constraintName = (sender as IButtonControl).CommandArgument;

                OpenedRelationSystemName = constraintName;

                foreach (var card in Cards.Values)
                    SaveState(card);
                OpenedRelationEntityID = (sender as IButtonControl).CommandName;

                DataBaseReader.GetRelations(OpenedRelation);

                OpenedRelation.Card.DependencyRelations.RelationTableTitleAttributes[constraintName] = "";

                OpenedRelation.Value = "";

                DataBaseReader.FillRelation(OpenedRelation, -1);

                OpenedRelationSystemName = null;

                OpenedRelationEntityID = null;

                RecreateChildControls();
            }

            RecreateChildControls();
        }

        public void DisplayDocumentHandler(object sender, EventArgs e)
        {
            if (DocumentDisplaying != null)
                DocumentDisplaying(this, EventArgs.Empty);
        }

        public void ClearButton_Click(object sender, EventArgs e)
        {

            foreach (var card in Cards.Values)
                card.Clear(CurrentMode);

            RecreateChildControls();
        }

        public void CancelButton_Click(object sender, EventArgs e)
        {
            if (EditionCanceled != null)
                EditionCanceled(this, EventArgs.Empty);
        }

        public void BackwardButton_Click(object sender, EventArgs e)
        {
            if (ControlReturning != null)
                ControlReturning(this, EventArgs.Empty);
        }


        public class ModelChanged : EventArgs
        {
            public Mode PreviousMode { get; private set; }

            public ModelChanged(Mode previousMode)
            {
                PreviousMode = previousMode;
            }
        }

    }
}
