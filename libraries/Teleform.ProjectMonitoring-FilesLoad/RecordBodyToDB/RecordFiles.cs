using RecordBodyToDB.ModelName;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecordBodyToDB
{
    public class RecordFiles
    {
        public List<DataFile> dataFile { get; set; }
        public static List<DataFile> CreateFile { get; set; }
        public string connString;
        public RecordFiles(List<DataFile> d, string c)
        {
            dataFile = d;
            connString = c;
            CreateFile = new List<DataFile>();
            Record();

            if (Form1.create && CreateFile.Count != 0) CreateNewApplication();
        }

        #region Search and store application
        public void Record()
        {
            foreach (var item in dataFile)
            {
                string numberApplication = CreateQueryForNumberApplication(item.NameApplication.NumberApplication).ToString();

                string query = string.Format(@"SELECT [a].[objID], [a].[fileName] FROM [_Application] AS [a]
                   JOIN [_Work] AS [w] ON [w].[objID] = [a].[_workID]
                   JOIN [_ApplicationType] AS [at] ON [at].[objID] = [a].[_ApplicationTypeID]
                   WHERE [w].[name] = '{0}' AND  [w].[number] = '{1}' AND [at].[code] = '{2}' {3}",
                                            item.NameWork,
                                            item.NumberContract,
                                            item.NameApplication.ApplicationType,
                                            numberApplication);
                using (var adapter = new SqlDataAdapter(query, connString))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        if (!Form1.create) Form1.NotFound.Add(item.Path);
                        CreateFile.Add(item);
                    }
                    else
                    {
                        Form1.Found.Add(item.Path);
                        SaveBody(dt.Rows, item);
                    }
                }
            }

        }

        private void SaveBody(DataRowCollection dataRowCollection, DataFile file)
        {

            foreach (DataRow item in dataRowCollection)
            {
                if (item.IsNull("fileName"))
                {
                    PreRecordToDb(item, file);
                }
                else
                {
                    if (Form1.edit) PreRecordToDb(item, file);
                    else
                    {
                        var dialogResult = MessageBox.Show(string.Format("В базе данных тело документа с параметрами {0} не пусто, заменить ?", file.Path), "Замена файла ", MessageBoxButtons.OKCancel);
                        if (dialogResult == DialogResult.OK)
                            PreRecordToDb(item, file);
                        else if (dialogResult == DialogResult.Cancel)
                            return;
                    }
                }
            }
        }



        private StringBuilder CreateQueryForNumberApplication(string p)
        {
            StringBuilder numberApplication = new StringBuilder();

            if (p != null)
                numberApplication.Append(string.Format(" AND [a].[number] = '{0}' ", p)); //foreach (var i in p) 
            else
                numberApplication.Append(" ");

            return numberApplication;
        }

        #endregion
        private void PreRecordToDb(DataRow item, DataFile file, bool createNewDoc = false)
        {
            string body = Convert.ToBase64String(File.ReadAllBytes(file.Path));

            string objID = item["objID"].ToString();

            string MimeTypeID = string.Empty;

            var adapter = new SqlDataAdapter(string.Format(" SELECT [objID] FROM [MimeType] WHERE [extension] = '{0}' ", file.Extention), connString);
            var dt = new DataTable();
            adapter.Fill(dt);
            if (dt.Rows.Count != 0)
                MimeTypeID = ((DataRow)dt.Rows[0])["objID"].ToString();

            string query = null;

            if (!createNewDoc)
                query = string.Format(@"UPDATE [_Application] SET [body] = CONVERT(varbinary(max), '{0}') , [fileName] = '{1}', [mimeTypeID] = '{2}',[modified] = GETDATE() WHERE [objID] = '{3}'",
                    body,
                    Path.GetFileName(file.Path),
                    MimeTypeID,
                    objID);
            else
            {
                query = string.Format(CreateQueryString(file),
                                        objID,
                                        file.NameApplication.ApplicationType,
                                        Path.GetFileName(file.Path),
                                        body,
                                        MimeTypeID);
                //            INSERT INTO [_Application] (_workID, _ApplicationTypeID, number, fileName, body, mimeTypeID, modified) 
                //VALUES ('{0}',(SELECT [objID] FROM [_ApplicationType] WHERE code = '{1}'), '{2}', '{3}', CONVERT(varbinary(max), '{4}'), '{5}', GETDATE())
            }

            RecordToDb(query, file);
        }

        private void RecordToDb(string query, DataFile file)
        {
            using (var conn = new SqlConnection(connString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    throw new Exception(string.Format("Не удалось сохранить файл {0} \n {1}", file.Path, ex.Message));
                }

            }
        }

        #region CreateNew
        private void CreateNewApplication()
        {
            foreach (var item in CreateFile)
            {
                string query = string.Format("SELECT [objID] FROM [_Work] WHERE [number] = '{0}'", item.NumberContract);

                using (var adapter = new SqlDataAdapter(query, connString))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count == 0)
                         Form1.NotFound.Add(item.Path);
                    else
                    {
                        Form1.NewCreate.Add(item.Path);
                        PreRecordToDb(dt.Rows[0], item, true);
                    }
                }
            }

        }
        private string CreateQueryString(DataFile file)
        {
            if(file.NameApplication.NumberApplication == null)
            {
                return @"INSERT INTO [_Application] (_workID, _ApplicationTypeID, fileName, body, mimeTypeID, modified) 
                   VALUES ('{0}',(SELECT [objID] FROM [_ApplicationType] WHERE code = '{1}'), '{2}', CONVERT(varbinary(max), '{3}'), '{4}', GETDATE())";
            }
            else
            {
                return @"INSERT INTO [_Application] (_workID, _ApplicationTypeID, number, fileName, body, mimeTypeID, modified) 
                   VALUES ('{0}',(SELECT [objID] FROM [_ApplicationType] WHERE code = '{1}'), '"+ file.NameApplication.NumberApplication +"', '{2}', CONVERT(varbinary(max), '{3}'), '{4}', GETDATE())";
            }
        }

        #endregion
    }
}
