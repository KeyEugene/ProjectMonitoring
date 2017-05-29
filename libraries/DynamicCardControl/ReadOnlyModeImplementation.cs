#define Viktor


using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Teleform.Reporting.DynamicCard
{
    partial class DynamicCardControl
    {
        public int userID { get; set; }
       
        private void FillDataCellWhenReadOnlyMode(IField field, TableCell cell)
        {
            if (field is CardRelationField)
            {
                if (AllowManagement)
                {
                    var relation = field as CardRelationField;
                                        

                    var linkButton = new LinkButton 
                    {                        
                        ID = string.Concat(relation.Entity.ID, "_", relation.ID),
                        Text = relation.Value 
                    };
                    linkButton.CommandArgument = relation.ID.ToString();
                    linkButton.CommandName = relation.Card.Entity.ID.ToString();
                    linkButton.Click += new EventHandler(RelationClickedHandler);
                    cell.Controls.Add(linkButton);
                }
                else cell.Text = field.Value.ToString();
            }
            else if (field is CardListRelationField)
            {
                var cardListRelation = field as CardListRelationField;


                if (cardListRelation.Value > 0)
                {
                    if (AllowManagement)
                    {
                        var linkButton = new LinkButton
                        {
                            ID = string.Concat(cardListRelation.ID, "_", cardListRelation.Entity.ID),
                            Text = cardListRelation.Value.ToString()
                        };
                        linkButton.CommandArgument = cardListRelation.ID.ToString();
                        linkButton.CommandName = cardListRelation.Card.Entity.ID.ToString();
                        linkButton.Click += new EventHandler(ListRelationClickedHandler);

                        cell.Controls.Add(linkButton);
                    }
                    else                    
                        cell.Text = field.Value.ToString();
                    
                }
                else if (cardListRelation.Value == 0)
                {
                    if (AllowManagement)
                    {
                        var f = field as CardListRelationField;
                        var linkButton = new LinkButton
                        {                                                    
                            ID = string.Concat(cardListRelation.ID, "_", cardListRelation.Entity.ID),                          
                            Text = "Создать списковый объект"
                        };

                        cell.Controls.Add(linkButton);

                        var referenceEntity = cardListRelation.Card.Entity.ID.ToString();

                        var entity = Storage.Select<Entity>(cardListRelation.Entity.ID);

                        var constraintID = cardListRelation.ID;

                        var instanceID = cardListRelation.Card.InstanceID;

                        var path = string.Format("~/Dynamics/XDynamicCard.aspx?entity={0}&id={1}&constraintID={2}", entity.ID.ToString(), instanceID, constraintID);
                        linkButton.PostBackUrl = path;
                    }
                    else                    
                        cell.Text = field.Value.ToString();
                    
                }
            }
            else if (field is CardSelfField)
            {
                var f = field as CardSelfField;

                if (f.TypeCode == CardSelfField.Type.Boolean)
                {
                    cell.Controls.Add(
                        new CheckBox
                        {
                            Enabled = false,
                            Checked = f.ContainsNonEmptyValue() && Convert.ToBoolean(f.Value)
                        });
                }
                else
                {
                    cell.Controls.Add(new Label { Text = field.Value.ToString() });

                    if (f.TypeCode == CardSelfField.Type.FileName && f.ContainsNonEmptyValue() && AllowManagement)
                    {
                        var downloadButton = new Button
                        {
                            CommandArgument = f.SystemName,
                            CommandName = f.Card.Entity.ID.ToString(),
                            Text = "Скачать"
                        };

                        var displayDocumentButton = new Button
                        {
                            Text = "Просмотреть",

                            CommandArgument = f.SystemName,
                            CommandName = f.Card.Entity.ID.ToString()
                        };

                        if (DownloadButtonStyle != null)
                        {
                            downloadButton.ApplyStyle(DownloadButtonStyle);
                            displayDocumentButton.ApplyStyle(DownloadButtonStyle);
                        }

                        downloadButton.Click += new EventHandler(DownloadFile);


                        displayDocumentButton.Click += new EventHandler(SelfFieldClickedHandler);

                        cell.Controls.Add(downloadButton);
                        cell.Controls.Add(displayDocumentButton);
                    }
                }
            }
        }


    }
}
