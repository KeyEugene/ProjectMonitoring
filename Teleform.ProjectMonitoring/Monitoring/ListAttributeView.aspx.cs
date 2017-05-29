using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using Teleform.Reporting;

namespace Teleform.ProjectMonitoring
{
    public partial class ListAttributeView : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var entityID = Request["entity"];
            var nameObject = Request["nameObject"];

            if (entityID != null)
            {
                var entityName = this.GetSchema().Entities.FirstOrDefault(o => o.ID.ToString() == entityID).Name;

               // var entityName = Storage.Select<Entity>(entityID).Name;

                if (entityName != null)
                    CurrentPageTitle = string.Format("{0} {1}", entityName, nameObject);
                    //CurrentPageTitle = string.Concat(entityName, nameObject);                
            }
        }


        private string ParentEntityID
        {
            get { return Request["entity"] == null ? null : (string)Request["entity"]; }
        }

        private string EntityName
        {
            get { return Request["entityname"] == null ? null : (string)Request["entityname"]; }
        }

        private string EntityID
        {
            get { return Request["id"] == null ? null : (string)Request["id"]; }
        }
    }
}