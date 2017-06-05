using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teleform.ProjectMonitoring
{
    public static class PageExtensions
    {
        public static int GetSystemUser(this System.Web.UI.Page page)
        {
            var o = page.Session["SystemUser.ID"];
            return o == null ? 0 : Convert.ToInt32(o);
        }
    }
}