using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting.Web
{
    partial class AddRemoveInstanceControl
    {

        public event EventHandler AddRemoveInstanceButtonClick;

        private void AddRemoveInstanceButton_Click(object sender, EventArgs e)
        {
            if (AddRemoveInstanceButtonClick != null)
                AddRemoveInstanceButtonClick(this, EventArgs.Empty);

        }

    }
}
