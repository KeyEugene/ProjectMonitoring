using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;


namespace Teleform.Reporting.MicrosoftOffice.ImportFile
{
    

    public enum TypeFile
    {
        Excel,
        Word,
        None
    }
    public class BaseParseFile
    {
        public string fileName;
        private FileUpload importFileUpload;
        private readonly string Word = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        private readonly string Excel = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public BaseParseFile(FileUpload importFileUpload)
        {
            this.importFileUpload = importFileUpload;
        }

        public TypeFile GetTypeFile()
        {
            fileName = importFileUpload.FileName;
            var contentType = importFileUpload.PostedFile.ContentType;

            if (contentType == Word)
                return TypeFile.Word;
            else if (contentType == Excel)
                return TypeFile.Excel;
            else
                new Exception("Расширение файла не поддерживается.");

            return TypeFile.None;
        }

        public static DataTable GetDataTable(string query)
        {
            try
            {
                var da = new SqlDataAdapter(query, Storage.ConnectionString);
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (SqlException ex)
            {
                throw new Exception("Не удалось сохранить значения в таблицу", ex.InnerException);
            }
        }

    }
}
