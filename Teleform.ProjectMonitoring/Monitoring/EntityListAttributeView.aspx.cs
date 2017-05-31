#define Alex
#define alexj

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Text;
using Teleform.Reporting;

using Teleform.ProjectMonitoring;
using System.IO;
using Teleform.ProjectMonitoring.HttpApplication;

namespace Teleform.ProjectMonitoring
{
    using Phoenix.Web.UI.Dialogs;

    public partial class EntityListAttributeView : BasePage
    {
        private Dictionary<string, int> check = new Dictionary<string, int>();

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var entityID = Request.QueryString["entity"];
            var nameObject = Request.QueryString["nameObject"];

            if (entityID != null)
            {

                // CurrentPageTitle = string.Format("{0} {1}", entityName, nameObject);

                var entityName = Storage.Select<Entity>(entityID).Name;
                if (!string.IsNullOrEmpty(entityName))
                    CurrentPageTitle = string.Format("{0} {1}", entityName, nameObject);
                else
                    CurrentPageTitle = string.Concat("Функционал АРМ");

            }


        }

        private bool EnumMode(Entity entity)
        {
            return entity.IsEnumeration;
        }

        private bool DefaultMode(Entity entity)
        {
            //return  !entity.IsEnumeration;
            return entity.IsMain && !entity.IsEnumeration;
        }



        protected void Page_Init(object sender, EventArgs e)
        {
            Predicate<Entity> predicate;

            //if (isClassifier)
            //    predicate = EnumMode;
            //else
                predicate = DefaultMode;

            FillEntityList( predicate);
        }

        private void FillEntityList( Predicate<Entity> predicate)
        {
            var dt = Storage.GetDataTable(string.Format(@"select iif(t.alias is NULL,'Шаблон',t.alias) typeAlias, p.* from Permission.UserPermission({0},NULL)p 
                                                left join model.BTables b on b.name=p.entity
                                                left join model.AppTypes t on t.object_id=b.appTypeID
                                                where  p.objID is NULL
                                                order by p.entityAlias", Session["SystemUser.objID"].ToString()));

            var permittedEntities = dt.AsEnumerable().Where(ent => Convert.ToBoolean(ent["read"]));
            var Entities = this.GetSchema().Entities.Where(ent => predicate(ent)).OrderBy(ent => ent.Name);

            Session["ReportCondition"] = "null";
        }

        protected void reCreateEntityTriggersButton_Click(object sender, EventArgs e)
        {
            var entityList = this.GetSchema().Entities.Where(ent => ent.IsEnumeration == false).ToList();

            var con = new SqlConnection(Kernel.ConnectionString);
            var dt = new DataTable();
            var da = new SqlDataAdapter("select tbl from model.Entity('base') where tbl <> '_User' and tbl <> '__Empty'", con);
            da.Fill(dt);

            IEnumerable<DataRow> rows = dt.AsEnumerable();

            foreach (var row in rows)
            {
                var entName = row[0];

                using (var cmd = new SqlCommand("EXEC [CreateEntityTriggerAfterInsert] @entName", con))
                {
                    con.Open();
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "@entName", DbType = System.Data.DbType.String, Value = entName });
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                using (var cmd = new SqlCommand("EXEC [CreateEntityTriggerAfterUpdate] @entName", con))
                {
                    con.Open();
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "@entName", DbType = System.Data.DbType.String, Value = entName });
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                using (var cmd = new SqlCommand("EXEC [CreateEntityTriggerAftrerDelete] @entName", con))
                {
                    con.Open();
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "@entName", DbType = System.Data.DbType.String, Value = entName });
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }
    }
}