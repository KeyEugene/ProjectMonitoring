using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using System.Collections;

using System.IO;
using System.Data;

namespace Monitoring
{
    using Teleform.ProjectMonitoring;

    public partial class _Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.Redirect("~/environment.aspx");
            Response.Redirect("~/Person_Page/Home.aspx");
        }
    }
}
