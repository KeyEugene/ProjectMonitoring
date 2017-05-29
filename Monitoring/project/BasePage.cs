#define Viktor

using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web.UI.WebControls;

using Phoenix;

namespace Teleform.ProjectMonitoring
{
    [Serializable]
    public class PageContext
    {

        public string AbsoluteUri { get; set; }

        public List<string> UrlParamaters;

        public string Path { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string TargetName { get; set; }

        public bool Redirect { get; set; }

    }

    public class SystemObject
    {
        public string Name { get; set; }

        public int ID { get; set; }
    }

    public class BasePage : Phoenix.Web.UI.Page
    {
        private static Dictionary<string, List<SystemObject>> types;

        public object TransferingObject { get; set; }

        public string CurrentPageTitle { get; set; }






#if Viktor
        /// <summary>
        /// Возвращает или задает контекст предыдущих страниц.
        /// </summary>
        public List<PageContext> RecentPages
        {
            get { return Session["PageContexts"] as List<PageContext>; }
            set { Session["PageContexts"] = value; }
        }
#else
        /// <summary>
        /// Возвращает или задает контекст предыдущих страниц.
        /// </summary>
        public List<PageContext> RecentPages
        {
            get { return ViewState["PageContexts"] as List<PageContext>; }
            set { ViewState["PageContexts"] = value; }
        }
#endif

        /// <summary>
        /// Возвращает или задаёт имя отображаемого объекта.
        /// </summary>
        public string ObjectName
        {
            get
            {
                var o = ViewState["ObjectName"];
                return (o == null ? string.Empty : o.ToString());
            }
            set { ViewState["ObjectName"] = value; }
        }



#if Viktor


        protected override void OnPreLoad(EventArgs e)
        {
            //base.OnPreLoad(e);

            if (RecentPages == null)
                RecentPages = new List<PageContext>();


            var currentPageContext = new PageContext();

            currentPageContext.Path = AppRelativeVirtualPath;

            if (string.IsNullOrEmpty(CurrentPageTitle))
                currentPageContext.Title = this.Title;
            else
                currentPageContext.Title = CurrentPageTitle;
            currentPageContext.AbsoluteUri = Request.Url.AbsoluteUri;

            var currentPageName = System.IO.Path.GetFileNameWithoutExtension(VirtualPathUtility.GetFileName(Request.Path));
            currentPageContext.Name = currentPageName;

            var currentPageUrlParameters = new List<string>();

            foreach (string key in Request.QueryString.Keys)
                currentPageUrlParameters.Add(key + "=" + Request.QueryString[key]);

            currentPageContext.UrlParamaters = currentPageUrlParameters;

            var index = RecentPages.FindIndex(recPageContext => MatchesContext(recPageContext, currentPageName, currentPageUrlParameters));

            if (index == -1)
            {
                //Костылек
                var recentPageContext = RecentPages.FirstOrDefault(item => item.Name == "EntityListAttributeView");
                if (recentPageContext != null && currentPageName == "EntityListAttributeView")                
                {
                    RecentPages.Remove(recentPageContext);
                    RecentPages.Add(currentPageContext);
                }
                else
                    RecentPages.Add(currentPageContext);
            }
            else
                RecentPages.RemoveRange(index + 1);            
        }

        private bool MatchesContext(PageContext recetnPageContext, string currentPageName, List<string> currentPageUrlParameters)
        {
            return recetnPageContext.Name == currentPageName && recetnPageContext.UrlParamaters.SequenceEqual(currentPageUrlParameters);
        }
        /// <summary>
        /// Возвращает историю посещения относительно имени страницы и параметров запроса.
        /// </summary>
        public List<PageContext> GetPageHistory(IEnumerable<PageContext> recentPages, string absoluteUri)
        {
            var pageContextList = new List<PageContext>();

            foreach (var item in recentPages)
            {
                if (item.AbsoluteUri != absoluteUri)
                    pageContextList.Add(item);
            }

            return pageContextList;
        }

#else
        /// <summary>
        /// Возвращает историю посещения относительно имени страницы и параметров запроса.
        /// </summary>
        public List<PageContext> GetPageHistory(IEnumerable<PageContext> history, string name, IList<string> urlParameters, out bool me)
        {
            me = false;
            var pageContextList = new List<PageContext>();

            foreach (var item in history)
            {
                if (item.Name != name || !item.UrlParamaters.SequenceEqual(urlParameters))
                    pageContextList.Add(item);
                else
                {
                    pageContextList.Add(item);
                    me = true;
                    break;
                }
            }

            return pageContextList;
        }
        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);

            if (RecentPages == null)
                RecentPages = new List<PageContext>();

            var currentPageName = System.IO.Path.GetFileNameWithoutExtension(VirtualPathUtility.GetFileName(Request.Path));
            var pageUrlParameters = new List<string>();

            foreach (string key in Request.QueryString.Keys)
                pageUrlParameters.Add(key + "=" + Request.QueryString[key]);

            string target = string.Empty;

            bool me = false;


            if (RecentPages.Count == 0 && currentPageName != "environment")
            {
                RecentPages.Add(new PageContext
                {
                    Path = "~/environment.aspx",
                    Title = this.Title,// "Система",
                    UrlParamaters = new List<string>(),
                    Redirect = true
                });
            }

            if (PreviousPage != null && PreviousPage is BasePage)
            {
                var previousPage = PreviousPage as BasePage;
                var recentPages = previousPage.RecentPages;


                RecentPages = GetPageHistory(recentPages, currentPageName, pageUrlParameters, out me);

                if (!string.IsNullOrEmpty(previousPage.ObjectName))
                    target = previousPage.ObjectName;
                else if (Request["target"] != null)
                    target = Request["target"];

                ObjectName = string.Empty;
            }

            if (!me)
            {
                if (!IsPostBack)
                {
                    var context = new PageContext();
                    context.Name = currentPageName;
                    context.Path = AppRelativeVirtualPath;
                    context.TargetName = target;
                    context.UrlParamaters = pageUrlParameters;
                    context.AbsoluteUri = Request.Url.AbsoluteUri;

                    
                    //Костыль
                    if (context.Path == "~/environment.aspx")
                        context.Redirect = true;


                    if (string.IsNullOrEmpty(PageTitle))
                    {
                        if (!string.IsNullOrEmpty(Title))
                            context.Title = Title + " " + target;
                        else
                            context.Title = currentPageName;
                    }
                    else
                        context.Title = PageTitle;


                    var AbsPuth = RecentPages.FirstOrDefault(ap => ap.AbsoluteUri == context.AbsoluteUri);

                    if (AbsPuth == null)
                        RecentPages.Add(context);                    
                }

                else if (!IsCrossPagePostBack)
                {
                    var index = RecentPages.FindIndex(item => MatchesContext(item, currentPageName, pageUrlParameters));

                    if (index == -1)
                    {
                        var context = new PageContext();
                        context.Name = currentPageName;
                        context.Path = AppRelativeVirtualPath;
                        context.UrlParamaters = pageUrlParameters;
                        context.Title = PageTitle;

                        RecentPages.Add(context);

                        context.AbsoluteUri = Request.Url.AbsoluteUri;

                    }
                    else
                    {
                        RecentPages.RemoveRange(index + 1);
                    }
                    ObjectName = string.Empty;
                }
            }

        }

      
#endif


        //public override void VerifyRenderingInServerForm(System.Web.UI.Control control)
        //{
        //}

        /// <summary>
        /// Передаёт данные об объекте из гиперссылки на новую страницу.
        /// </summary>
        //protected void TransferData(object o, EventArgs e)
        //{
        //    if (o is IButtonControl)
        //    {
        //        var text = (o as IButtonControl).Text;

        //        if (!IsCrossPagePostBack)
        //            RecentPages.Last().Title = text;
        //        else ObjectName = text;
        //    }
        //}
    }
}