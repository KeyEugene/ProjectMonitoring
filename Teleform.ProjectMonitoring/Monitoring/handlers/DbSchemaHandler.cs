using System.Web;
using Teleform.Reporting;

namespace Teleform.ProjectMonitoring
{
    using HttpApplication;

    public class DbSchemaHandler : System.Web.UI.Page, IHttpHandler
    { 

        public bool IsReusable
        {
            get { return true; } 
        }

        public void ProcessRequest(HttpContext context)
        {            
                var query = "SELECT [model].[xmlPermitedEntities] ()";

                var xml = Storage.ExecuteScalarString(query);

                context.Response.Write(xml);

                //Response.Write(Global.SchemaXML);
            }

        }       
    }
