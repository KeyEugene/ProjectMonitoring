#define Viktor

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Teleform.ProjectMonitoring.NavigationFrame
{
    /// <summary>
    /// "Хлебные крошки" - отображение навигационнной цепочки
    /// </summary>
    public partial class Breadcrumb : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page is BasePage)
            {
                var page = Page as BasePage;
                LinkButton button;
#if Viktor
                for (int i = 0; i < page.RecentPages.Count; i++)
                {
                    var recent = page.RecentPages[i];

                    button = new LinkButton
                    {
                        ID = i.ToString(),
                        //CommandArgument = recent.AbsoluteUri
                        PostBackUrl = recent.AbsoluteUri                        
                    };
                    //button.Click += new EventHandler(button_Click);

                    breadcrumbs.Controls.Add(button);

                }
#else

                for (int i = 0; i < page.RecentPages.Count; i++)
                {
                    var recent = page.RecentPages[i];
                    string parameters = string.Empty;

                    if (recent.UrlParamaters.Count > 0)
                    {
                        parameters = recent.UrlParamaters.Aggregate((ready, next) => ready + "&" + next);

                        if (!string.IsNullOrEmpty(parameters))
                            parameters = "?" + parameters;
                    }

                    if (!recent.Redirect)
                    {
                        button = new LinkButton
                        {
                            ID = i.ToString(),
                            PostBackUrl = string.Format("{0}{1}", recent.Path, parameters)
                        };
                    }
                    else
                    {
                        button = new LinkButton
                        {
                            ID = i.ToString(),
                            CommandArgument = recent.Path
                        };
                        button.Click += new EventHandler(RedirectPathItemHandler);
                    }

                    breadcrumbs.Controls.Add(button);

                }
#endif

            }

        }


        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Page is BasePage)
            {
                var page = Page as BasePage;
                var labels = page.RecentPages.Select(item => item.Title);
                var i = 0;

                foreach (var c in breadcrumbs.Controls)
                {
                    if (c is IButtonControl)
                        (c as IButtonControl).Text = labels.ElementAt(i++) + " \\ ";
                }
            }
        }

    }
}