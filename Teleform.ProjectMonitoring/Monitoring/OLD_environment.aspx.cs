#define debug

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Teleform.ProjectMonitoring.HttpApplication;
using Teleform.Reporting;

namespace Teleform.ProjectMonitoring
{
    public partial class EnvironmentPage : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            #region Переход в обучающий проект "Рупетитор"

            var objID = Session["SystemUser.objID"];

            if (objID != Session[Session.SessionID])
            {
                var dt = Global.GetDataTable("SELECT * FROM INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'Param' ");

                if (dt.Rows.Count == 0)
                {
                    A6.Visible = false;
                    return;
                }
#if debug123
                dt = Global.GetDataTable("select [value] from [Param] where [code] = 'RepetitorSystem(Debug)'");
#else
                dt = Global.GetDataTable("select [value] from [Param] where [code] = 'RepetitorSystem'");
#endif

                if (dt.Rows.Count == 0)
                {
                    A6.Visible = false;
                    return;
                }
                string urlValue = dt.Rows[0]["value"].ToString();

                #region Проверка существовании в таблице [permission].[Acce$$] имеющегося sessionID
                dt = Global.GetDataTable(string.Concat("SELECT * FROM [permission].[Acce$$] WHERE [sId] = '" + Session.SessionID + "'"));
                #endregion

                objID = objID == null ? "0" : objID;

                if (dt.Rows.Count == 0)
                {
                    Global.GetDataTable(
                        string.Format("insert into [permission].[Acce$$] ([sId], [monitoringUserID]) values ('{0}', '{1}')",
                        Session.SessionID, objID));
                }
                else
                {
                    Global.GetDataTable(
                       string.Format("UPDATE [permission].[Acce$$] set [monitoringUserID] = '{0}' where [sId] = '{1}'",
                       objID, Session.SessionID));
                }

                Session[Session.SessionID] = objID ?? "0";
                Session["urlValue"] = urlValue;
            }


            A6.OnClientClick = string.Concat("window.open('", Session["urlValue"], "', '_blank')");

            #endregion
        }


       


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SystemUser.typeID"] == null)
                return;
            else
            {
                var userTypeID = Convert.ToInt32(Session["SystemUser.typeID"]);

                //"Администрирование" только для администратора
                A5.Visible = userTypeID == 1 ? true : false;

                //"Шаблоны" только для администратора
                Templates.Visible = userTypeID == 1 ? true : false;

                //"Специальные отчеты" только для администратора
                Reports.Visible = userTypeID == 1 ? true : false;

                //"Управление занятиями" только для Преподавателя
                A6.Visible = userTypeID == 3 ? true : false;

                //"Маршруты документов" только для администратора, отдела кадров и методиста
                Routes.Visible = userTypeID == 1 || userTypeID == 5 || userTypeID == 2 ? true : false;

            }

        }

    }
}