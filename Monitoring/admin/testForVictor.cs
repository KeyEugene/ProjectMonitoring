using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Teleform.Reporting;
using Teleform.ProjectMonitoring.Templates;

namespace Teleform.ProjectMonitoring.admin
{
    public class testForVictor : CompositeControl
    {

        public string EntityID
        {
            get { return ViewState["Eid"] == null ? null : ViewState["Eid"].ToString(); }
            set { ViewState["Eid"] = value; }
        }

        public Template template
        {
            get { return ViewState["_TemplateDesigner"] as Template; }
            set
            {
                ViewState["_TemplateDesigner"] = value;
            }
        }

        private Template Template
        {
            get
            {
                try
                {

                    if (template == null)
                    {
                        //if (!string.IsNullOrEmpty(TemplateID))
                        //{
                        //    //var t = Storage.Select<Template>(TemplateID);

                        //    template = t.Clone();
                        //} else
                        var entity = Storage.Select<Entity>(EntityID);
                        var content = new byte[0];
                        template = new Template(string.Empty, entity, "TableBased", content);
                    }

                    return template;
                } catch
                {
                    return null;
                }
            }
        }

        public Designer designer { get; set; }

        public List<FieldBox> FieldBoxList { get; set; }


        protected override void CreateChildControls()
        {
            designer = new Designer();
            designer.ID = "desigenr";

            MethodForCreateListFieldBox();

            var tbl = MethodForBuildTable();

            this.Controls.Add(tbl);
        }

        private Table MethodForBuildTable()
        {
            var td = new Table();

            return td;
        }

        private void MethodForCreateListFieldBox()
        {
            foreach (var field in Template.Fields)
            {
                FieldBoxList.Add(new FieldBox(field));
            }
        }

        public override void DataBind()
        {
            CreateChildControls();
        }

    }
}