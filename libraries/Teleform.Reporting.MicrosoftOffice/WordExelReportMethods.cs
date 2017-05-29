using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;


namespace Teleform.Reporting
{
    public static class WordExelReportMethods
    {

        public static bool UploadDoc(out byte[] body, out string mimeTypeID, out string fileName, out string typeID, out string code, FileUpload upload, string connectionString)
        {
            code = typeID = mimeTypeID = fileName = string.Empty;
            body = null;

            var fullFileName = upload.PostedFile.FileName;

            fileName = Path.GetFileNameWithoutExtension(fullFileName);
            var extension = Path.GetExtension(fullFileName);

            var query = string.Format(@"SELECT [M].[objID] mimyTypeID, [TT].[objID] typeID, [TT].[code] FROM [MimeType] [M]
                                        join [R_TemplateType] [TT] on [TT].[mimeTypeID] = [M].[objID]
                                        WHERE [M].[extension] = '{0}'", extension);
            var adapter = new SqlDataAdapter(query, connectionString);
            var dt = new DataTable();

            adapter.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                mimeTypeID = ((DataRow)dt.Rows[0])["mimyTypeID"].ToString();
                typeID = ((DataRow)dt.Rows[0])["typeID"].ToString();
                code = ((DataRow)dt.Rows[0])["code"].ToString();
            }

            using (Stream fs = upload.PostedFile.InputStream)
            using (BinaryReader br = new BinaryReader(fs))
            {
                if (fs.Length != 0)
                {
                    body = br.ReadBytes((int)fs.Length);
                }
            }

            return true;
        }


        public static byte[] GetFileContent(int instanceID, out string mimeType, out string name, out string extension, string connectionString)
        {
            var adapter = new SqlDataAdapter
             (
                 string.Format(
                     "SELECT [A].[body], [MT].[mime] ,[A].[fileName], [MT].[extension] FROM [R_Template] [A] JOIN [MimeType] [MT] ON [MT].[objID] = [A].[mimeTypeID] WHERE [A].[objID] = {0}",
                     instanceID
                 ),
                 connectionString
             );

            var table = new DataTable();
            adapter.Fill(table);

            if (table.Rows.Count == 0)
                throw new Exception("Выберите файл.");

            mimeType = table.Rows[0]["mime"].ToString();
            name = table.Rows[0]["fileName"].ToString();
            extension = table.Rows[0]["extension"].ToString();

            return (byte[])table.Rows[0]["body"];
        }
    }
}
