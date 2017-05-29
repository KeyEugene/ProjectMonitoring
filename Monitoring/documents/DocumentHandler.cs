using System;
using System.Web;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace Teleform.ProjectMonitoring
{
    public class DocumentHandler : IHttpHandler
    {
        public DocumentHandler()
        { }

        public void ProcessRequest(HttpContext context)
        {
            var Request = context.Request;
            var Response = context.Response;

            int id = Convert.ToInt32(Path.GetFileNameWithoutExtension(VirtualPathUtility.GetFileName(Request.Path)));

            var adapter = new SqlDataAdapter(
@"
SELECT
    [A].[body],
    [MT].[mime]
FROM [_Application] [A] JOIN [MimeType] [MT] ON [A].[mimeTypeID] = [MT].[objID]
WHERE [A].[objID] = " + id, Kernel.ConnectionString);

            var t = new DataTable();

            adapter.Fill(t);

            if (t.Rows.Count > 0)
            {

                var o = t.Rows[0];

                Response.ContentType = o["mime"].ToString();
                Response.BinaryWrite((byte[])o["body"]);
                Response.Flush();
            }
            else
            {
                Response.ContentType = "text/html";

                Response.Write(string.Format(@"<html>
<body style=;font-size: 130%;>
<table>
    <tr>
        <td><img src='{0}nofile.png' /></td>
        <td style='color: RoyalBlue;font-size: 23pt'>Документ не загружен</td>
    </tr>
</table>
<hr />
Для того чтобы загрузить документ нажмите «Правка». В поле «Источник» выберите необходимый файл, затем сохраните текущие изменения.
</body></html>", VirtualPathUtility.ToAbsolute("~/images/")));
            }

            Response.End();
        }

        public bool IsReusable { get { return false; } }
    }
}