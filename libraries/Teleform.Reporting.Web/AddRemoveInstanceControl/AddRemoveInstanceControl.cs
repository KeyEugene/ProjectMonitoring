using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Teleform.Reporting.Web
{
    public partial class AddRemoveInstanceControl : CompositeControl
    {

        private LinkButton AddRemoveInstanceButton;

        public bool IsRemoveInstance { get; set; }


        protected override void CreateChildControls()
        {
            AddRemoveInstanceButton = new LinkButton
            {
                ID = "NewInstanceButton",
            };
           
            AddRemoveInstanceButton.Click += (AddRemoveInstanceButton_Click);
            
            //var rowID = -1;
            //var entityID = 111443571;
            //AddRemoveInstanceButton.Attributes.Add("onclick", "switchSelectedRow(" + rowID + "," + entityID + "," + "false" + " )");
      
            Controls.Add(AddRemoveInstanceButton);
            
            if (IsRemoveInstance == true)
            {
                AddRemoveInstanceButton.Attributes.Add("title", "Удалить объект");
                AddRemoveInstanceButton.Text = "Удалить объект";
            }
            else
            {
                AddRemoveInstanceButton.Attributes.Add("title", "Добавить объект");
                AddRemoveInstanceButton.Text = "Добавить объект";
            }
        }
    }
}
