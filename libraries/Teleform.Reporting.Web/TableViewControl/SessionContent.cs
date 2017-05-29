
#define isNotDeprecated

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web;
using System.Data;

namespace Teleform.Reporting.Web
{
    public class SessionContent
    {
      

        //public Dictionary<string, DataView> DataView { get; set; }

        public Dictionary<string, object> SortingData { get; private set; }

        public Dictionary<string, object> FieldsSize { get; private set; }

        public Dictionary<string, object> FilterData { get; private set; }

        public Dictionary<string, string> ExpressionForSelectCheckBoxes { get; private set; }

        public Dictionary<string, EntityInstance> EntityInstances { get; set; }

        public Dictionary<string, DataTable> TemplateContent { get; set; }
                
        public int CurrentPage { get; set; }

        public int LeftPageIndex { get; set; }

        public bool IsRemoveInstance { get; set; }

        public bool IsInstanceAdded { get; set; }

        

        public SessionContent()
        {

            //DataView = new Dictionary<string, DataView>();

            SortingData = new Dictionary<string, object>();
 
            FieldsSize = new Dictionary<string, object>();

            FilterData = new Dictionary<string, object>();

            ExpressionForSelectCheckBoxes = new Dictionary<string, string>();

            EntityInstances = new Dictionary<string,  EntityInstance>();

            TemplateContent = new Dictionary<string, DataTable>();

        
        }
    }
}
