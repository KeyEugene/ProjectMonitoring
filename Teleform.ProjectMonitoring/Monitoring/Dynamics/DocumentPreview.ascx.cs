
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Teleform.ProjectMonitoring.Dynamics
{
    public partial class DocumentPreview : System.Web.UI.UserControl
    {
        public string EntityID { get; set; }

        public string ID { get; set; }
        
        public void InitializeAsUserControl(Page page)
        {
            if (EntityID != null && ID != null)
                docFiles.Attributes["src"] = string.Format("DocPreview.aspx?entity={0}&id={1}", EntityID, ID);
        }

    }
}