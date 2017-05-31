#define Alex

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace Teleform.ProjectMonitoring
{
    using Reporting;
    using Reporting.MicrosoftOffice;
    using Teleform.ProjectMonitoring.HttpApplication;
    using Teleform.Reporting.Web;

    public partial class ReportView
    {


#if true
        private List<string> GetInstancesID()
        {
            var idList = new List<string>();

            foreach (DataRowView row in ReportViewControl.DataView)
                idList.Add(row["objID"].ToString());

            return idList;
        }
#else
        private string GetInstancesID()
        {
            if (ReportViewControl.DataView != null)
            {
                var s = new StringBuilder();

                foreach (DataRowView row in ReportViewControl.DataView)
                    s.Append(string.Concat(row["objID"], ","));
                s.Length--;

                return s.ToString();
            }

            return string.Empty;
        }
#endif


        public void CreateExcelReport(string templateID, int userID)
        {
            var template = Storage.Select<Template>(templateID);
            entityID = template.Entity.ID.ToString();
            var table = Storage.Select<BusinessContent>(entityID).GetTable(userID);

            if (!Frame.IsNeedAllInstances.Checked)
            {
                var instancesID = GetInstancesID();
                DataRow[] rows = table.AsEnumerable().Where(x => instancesID.Contains(x["objID"].ToString())).ToArray();
                table = rows.CopyToDataTable();
            }

            string file = template.FileName;

            using (var stream = new MemoryStream())
            {
                var builder = new ReportViewExcelBuilder();
                builder.Create(stream, Teleform.Reporting.GroupReport.Make(template, table));

                Response.Clear();
                Response.ContentType = "text/html";
                Response.AddHeader("content-disposition", string.Format("attachment;fileName={0}.xlsx", file));
                Response.ContentEncoding = Encoding.UTF8;
                Response.BinaryWrite(stream.ToArray());
                Response.Flush();
                Response.End();
            }

        }

    }
}
