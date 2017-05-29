using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Teleform.ProjectMonitoring.Templates
{
    public class DialogTemplateField : ITemplate
    {
        private Table table;

        public DialogTemplateField(Table table)
        {
            this.table = table;
        }

        void ITemplate.InstantiateIn(Control container)
        {
            container.Controls.Add(table);
        }
    }
}