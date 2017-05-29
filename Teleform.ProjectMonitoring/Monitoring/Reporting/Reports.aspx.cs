#define alexj

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Teleform.ProjectMonitoring
{
    public partial class Reports : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

#if alexj
            CurrentPageTitle = "Специальные отчеты";
#else
            PageTitle = "Отчеты";
#endif
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var userTypeID = Session["SystemUser.typeID"].ToString();
                //Если пользователь случайно забрел на страницу спец.отчетов - выкинуть его на страницу 404 и вернуть в систему
                if (string.IsNullOrEmpty(userTypeID) || userTypeID != "1")
                    Server.Transfer("~/ErrorPage2.aspx");                
            }
        }
    }
}