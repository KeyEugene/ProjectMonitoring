using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Monitoring.documents
{
    public partial class preview : System.Web.UI.Page
    {
        protected string DocumentID { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PreviousPage != null)
            {
            }
        }
    }
}