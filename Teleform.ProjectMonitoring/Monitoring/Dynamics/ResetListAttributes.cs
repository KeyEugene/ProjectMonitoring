using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using DynamicCardModel;

namespace MonitoringMinProm.DynamicCard
{
    public partial class DynamicCard : CompositeControl
    {
        private Button FormResetButton(Field f, IEnumerable<Relation> relationList)
        {
            var button = new Button { Text = "\u2613" };
            
            button.Attributes.Add("data-field", f.Name);
            button.Attributes.Add("title", "Очистить значение");
            button.Attributes.Add("class", "resetButton");
            button.Click += new EventHandler(ResetButtonClick);
            return button;
        }

        void ResetButtonClick(object o, EventArgs e)
        {
            var button = o as Button;
            var field = this.Card.FieldList.First(f => f.Name == button.Attributes["data-field"]);
            field.Value = string.Empty;
            field.Title = string.Empty;

            RecreateChildControls();
        }
    }
}