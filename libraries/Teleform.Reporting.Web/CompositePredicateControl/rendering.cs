//#define Alex
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Writer = System.Web.UI.HtmlTextWriter;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Teleform.Reporting.Web
{
    partial class CompositePredicateControl
    {       

        public object FilterData
        {
            get
            {
                EnsureChildControls();

                var tuple = new Tuple<string, string, string, string, string>
                (
                    TechPredicateBox.Text,
                    UserPredicateBox.Text,
                    JeysonBox.Text,
                    ValueBox.Text,
                    OperatorList.SelectedValue
                );

                return tuple;
            }

            set
            {
                EnsureChildControls();

                if(value is Tuple<string, string, string, string, string>)
                {
                    var tuple = value as Tuple<string, string, string, string, string>;

                    TechPredicateBox.Text = tuple.Item1;
                    UserPredicateBox.Text = tuple.Item2;
                    JeysonBox.Text = tuple.Item3;
                    ValueBox.Text = tuple.Item4;
                    OperatorList.SelectedValue = tuple.Item5;
                }
            }
        }

        protected override void Render(Writer writer)
        {
            var harvester = string.Format("harvester('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');",
            JeysonBox.ClientID, TechPredicateBox.ClientID, UserPredicateBox.ClientID, ApplyButton.ClientID, startBlockButton.ClientID, endBlockButton.ClientID, addExpressionButton.ClientID, addAndOperatorButton.ClientID, addOrOperatorButton.ClientID);

            //Page.ClientScript.RegisterStartupScript(this.GetType(), "", "alert('Загрузка страници')", true);
            
            Page.ClientScript.RegisterStartupScript(this.GetType(), ClientID, harvester, true);
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "ClientID", harvester, true);
            
            //ApplyButton.Attributes.Add("onclick", '');

            var x = JeysonBox.ClientID;

            addExpressionButton.Attributes.Add("onclick", string.Format("addExpression('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')", JeysonBox.ClientID,
                ApplyButton.ClientID, startBlockButton.ClientID, endBlockButton.ClientID, addExpressionButton.ClientID, addAndOperatorButton.ClientID, addOrOperatorButton.ClientID,
               TechPredicateBox.ClientID, UserPredicateBox.ClientID, OperatorList.ClientID, ValueBox.ClientID));

            startBlockButton.Attributes.Add("onclick", string.Format("startBlock('{0}','{1}','{2}')", JeysonBox.ClientID, TechPredicateBox.ClientID, UserPredicateBox.ClientID));

            endBlockButton.Attributes.Add("onclick", string.Format("endBlock('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", JeysonBox.ClientID, ApplyButton.ClientID,
                startBlockButton.ClientID, endBlockButton.ClientID, addExpressionButton.ClientID, addAndOperatorButton.ClientID, addOrOperatorButton.ClientID, 
                TechPredicateBox.ClientID, UserPredicateBox.ClientID));

            addAndOperatorButton.Attributes.Add("onclick", string.Format("addLogicOperator('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",JeysonBox.ClientID, ApplyButton.ClientID,
                startBlockButton.ClientID, endBlockButton.ClientID, addExpressionButton.ClientID, addAndOperatorButton.ClientID, addOrOperatorButton.ClientID,
                TechPredicateBox.ClientID, UserPredicateBox.ClientID, " AND "));

            addOrOperatorButton.Attributes.Add("onclick", string.Format("addLogicOperator('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", JeysonBox.ClientID, ApplyButton.ClientID,
                startBlockButton.ClientID, endBlockButton.ClientID, addExpressionButton.ClientID, addAndOperatorButton.ClientID, addOrOperatorButton.ClientID,
                TechPredicateBox.ClientID, UserPredicateBox.ClientID, " OR "));

           clearAllButton.Attributes.Add("onclick", string.Format("clearAllPredicate('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", JeysonBox.ClientID, ApplyButton.ClientID,
               startBlockButton.ClientID, endBlockButton.ClientID, addExpressionButton.ClientID, addAndOperatorButton.ClientID, addOrOperatorButton.ClientID,
               TechPredicateBox.ClientID, UserPredicateBox.ClientID));
                        
            clearButton.Attributes.Add("onclick", string.Format("clearPredicate('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", JeysonBox.ClientID, ApplyButton.ClientID,
               startBlockButton.ClientID, endBlockButton.ClientID, addExpressionButton.ClientID, addAndOperatorButton.ClientID, addOrOperatorButton.ClientID,
               TechPredicateBox.ClientID, UserPredicateBox.ClientID));
                      
            writer.AddAttribute("class", "PredicateControlDiv");           
            writer.RenderBeginTag("div");

            if (!string.IsNullOrWhiteSpace(ID))
                writer.AddAttribute("id", ClientID);
            
            if (!string.IsNullOrEmpty(CssClass))
                writer.AddAttribute("class", CssClass);

            writer.RenderBeginTag("details");

            if (ActiveStyle != null && Active)
                writer.AddAttribute("class", ActiveStyle.CssClass);

            writer.RenderBeginTag("summary");
            writer.RenderEndTag();

            writer.RenderBeginTag("div");

            Table.RenderControl(writer);         

            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
        }
    }
}
