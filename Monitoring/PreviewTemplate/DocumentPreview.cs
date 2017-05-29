

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Monitoring.PreviewTemplate
{
    public class DocumentPreview : IHttpHandler
    {
        public bool IsReusable { get { return false; } }

       

        public void ProcessRequest(HttpContext context)
        {
        }

     }
}
