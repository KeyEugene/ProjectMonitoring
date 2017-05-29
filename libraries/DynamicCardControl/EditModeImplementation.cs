

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Teleform.Reporting.constraint;

namespace Teleform.Reporting.DynamicCard
{
    partial class DynamicCardControl
    {

        private object OpenedRelationSystemName
        {
            get
            {
                return ViewState["OpenedRelationSystemName"];
            }
            set
            {
                ViewState["OpenedRelationSystemName"] = value;
            }
        }



        private object OpenedRelationEntityID
        {
            get
            {
                return ViewState["OpenedRelationEntityID"];
            }
            set
            {
                ViewState["OpenedRelationEntityID"] = value;
            }
        }


        private Type GetAttrTypeFromFieldID(string id, Card card)
        {
            var entity = Storage.Select<Entity>(card.Entity.ID);
            var attribute = entity.Attributes.Single(a => a.ID == id);

            //var entityType = Schema.Entities.First(e => e.ID.ToString() == card.Entity.ID.ToString());
            //var attribute = entityType.Attributes.Single(a => a.ID.ToString() == id);

            return attribute.Type;
        }

        private CardRelationField OpenedRelation
        {
            get
            {
                Card currentCard;

                Cards.TryGetValue(Convert.ToInt32(OpenedRelationEntityID), out currentCard);

                return OpenedRelationSystemName == null ? null : currentCard.GetRelation(OpenedRelationSystemName.ToString());
            }
        }

        private void FillDataCellWhenEditMode(IField field, TableCell cell)
        {

            Card currentCard;

            if (field is CardRelationField)
            {
                var f = field as CardRelationField;

                var label = new Label { Text = f.Value };

                cell.Controls.Add(label);

                Cards.TryGetValue(Convert.ToInt32(f.Card.Entity.ID), out currentCard);

                if (currentCard.EntityInstance == null)
                    DataBaseReader.FillEntityInstance(currentCard, "");

                Constraint constraint = currentCard.EntityInstance.Constraints.First(c => c.ConstraintName == field.SystemName);


                //если режим создания объекта
                if (string.IsNullOrEmpty(currentCard.EntityInstance.EntityInstanceID))
                {
                    var RelationEditedButton = CreateRelationEditedButton(f);
                    cell.Controls.Add(RelationEditedButton);

                    if (constraint.IsNullable)
                    {
                        var RelationClearedButton = CreateRelationClearedButton(f);
                        cell.Controls.Add(RelationClearedButton);
                    }

                }
                else if (!constraint.IsIdentified)
                {
                    var RelationEditedButton = CreateRelationEditedButton(f);
                    cell.Controls.Add(RelationEditedButton);

                    if (constraint.IsNullable)
                    {
                        var RelationClearedButton = CreateRelationClearedButton(f);
                        cell.Controls.Add(RelationClearedButton);
                    }

                }
            }
            else if (field is CardSelfField)
            {
                WebControl control = null;

                var f = field as CardSelfField;

                Cards.TryGetValue(Convert.ToInt32(f.Card.Entity.ID), out currentCard);

                if (f.IsReadOnly(CurrentMode))
                    control = new Label { Text = f.Value.ToString() };
                else
                    switch (f.TypeCode)
                    {
                        case CardSelfField.Type.Object:
                            control = new TextBox
                            {
                                TextMode = TextBoxMode.MultiLine,
                                Text = f.Value.ToString()
                            };
                            break;
                        case CardSelfField.Type.Boolean:
                            if (f.Value == "")
                                f.Value = false;     // очень явно, для случая очистки всех полей при создании      
                            control = new CheckBox { Checked = (bool)f.Value };
                            break;
                        case CardSelfField.Type.FileName:
                            control = new FileUpload();

                            if (f.ContainsNonEmptyValue())
                                cell.Controls.Add(new Literal { Text = (f.Value as File).FileName });
                            break;
                        case CardSelfField.Type.Float:
                        case CardSelfField.Type.ShortString:
                        case CardSelfField.Type.Numeric:
                            control = new TextBox { Text = f.Value.ToString() };
                            break;
                        case CardSelfField.Type.DateTime:
                            var textBox = new TextBox();
                            control = textBox;

                            if (f.ContainsNonEmptyValue())
                            {
                                var date = DateTime.Parse(f.Value.ToString());

                                textBox.Text = date.ToString("yyyy-MM-dd");
                            }
                            break;
                        default:
                            throw new NotImplementedException("Не реализована часть для варианта " + f.TypeCode.ToString());
                    }
                if (control != null)
                {
                    if (control is TextBox)
                    {
                        var textBox = control as TextBox;

                        var entityType = Schema.Entities.First(e => e.ID.ToString() == currentCard.Entity.ID.ToString());
                        var attribute = entityType.Attributes.Single(a => a.ID.ToString() == f.ID);

                        textBox.ApplyType(attribute.Type);
                    }

                    cell.Controls.Add(control);
                    WebControls.Add(field.SystemName, control);
                }
            }
        }



        private Button CreateRelationEditedButton(CardRelationField relation)
        {
            var b = new Button
            {
                Text = "\u21D3",
                ToolTip = "Выбрать значение",
                CommandArgument = relation.SystemName,
                CommandName = relation.Card.Entity.ID.ToString()
            };


            b.ApplyStyle(OpenRelatedListButtonStyle);

            b.Click += new EventHandler(RelationEditedButton_Click);

            return b;
        }





        private Button CreateRelationClearedButton(IField field)
        {
            var f = field as CardRelationField;

            var b = new Button
            {
                Text = "\u2613",
                ToolTip = "Очистить значение.",
                CommandArgument = f.SystemName,
                CommandName = f.Card.Entity.ID.ToString()
            };

            if (ClearReferenceButtonStyle != null)
                b.ApplyStyle(ClearReferenceButtonStyle);

            b.Click += new EventHandler(RelationClearedButton_Click);
            return b;
        }




    }
}