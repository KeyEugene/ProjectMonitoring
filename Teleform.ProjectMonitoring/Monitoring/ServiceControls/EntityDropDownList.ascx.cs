using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Teleform.Reporting;

namespace Teleform.ProjectMonitoring
{
    public partial class EntityDropDownList : System.Web.UI.UserControl
    {
        public IEnumerable<Entity> Entities { get; set; }



        protected void Page_Load(object sender, EventArgs e)
        {
            //this.Attributes.Add("class", "entityDropDownList");
        }

        public override void DataBind()
        {
            FillDropDownList();

            base.DataBind();
        }

        private void FillDropDownList()
        {
            var li_s = new StringBuilder();
            //li_s.Append("<li><a href='" + Session["getLeftUrl"].ToString() + "EntityListAttributeView.aspx?entity='>Не выбрано</a></li>");
            li_s.Append("<li><a href='" + Session["getLeftUrl"].ToString() + "EntityListAttributeView.aspx'>Не выбрано</a></li>");

            // var el = new StringBuilder();
            string a = "";

            foreach (Entity entity in Entities)
            {
                var el = new StringBuilder(
                string.Format("<a id='{2}' href='{0}'>{1}</a>",
                    string.Format("{3}EntityListAttributeView.aspx?entity={0}{1}{2}", entity.ID, entity.IsHierarchic ? "&parentID=-1" : "", (Request["checker"] != null ? "&checker=" + Request["checker"] : string.Empty), Session["getLeftUrl"].ToString()),
                    entity.Name,
                    entity.ID.ToString()
                    ));

                if (entity.IsHierarchic)
                {
                    el.Append("<div class='openChild' onclick='ShowChild(this)'>+</div>");
                    AddChilds(ref el, entity.SystemName, entity.ID);
                }
                var mainLi = string.Concat("<li title='", entity.Name, "' >", el.ToString(), "</li>");
                li_s.Append(mainLi);
            }
            var mainUl = string.Concat("<div class='containerEntityDropDownList'><ul class='entityNavi EntityDropDownList_scroll'>", li_s.ToString(), @"</ul></div>
             <h2 class='entityListOpen' onclick='openEntityNavi()'>+</h2>");

            Session["EntityDropDownList"] = mainUl;
        }

        private void AddChilds(ref StringBuilder el, string SystemName, object EntityID)
        {
            var dt = Storage.GetDataTable(string.Concat("EXEC [report].[getChildList] '", SystemName, "', null"));
            //var dt = Storage.GetDataTable("select * from _Bank where objID < 7");

            if (dt.Rows.Count == 0)
                return;

            var li_s = new StringBuilder();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var name = dt.Rows[i]["name"];
                var lefturl = Session["getLeftUrl"].ToString();
                //var nameObject = Request["nameObject"];
                var checker = Request["checker"] != null ? "&checker=" + Request["checker"] : string.Empty;


                li_s.Append(string.Concat("<li title='", name, "'>",
                     string.Format("<a href='{0}'>{1}</a>",
                    string.Format("{3}EntityListAttributeView.aspx?entity={0}&parentID={1}{2}&nameObject={4}", EntityID, dt.Rows[i]["objID"], checker, lefturl, name), name),
                    "</li>"));
            }
            var ulChild = string.Concat("<ul class='childContainer EntityDropDownList_scroll'>", li_s.ToString(), "</ul>");
            el.Append(ulChild);
        }
    }
}