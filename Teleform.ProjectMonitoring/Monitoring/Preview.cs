using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;

using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using Teleform.Office.TemplatePreview;
using System.IO;
using System.Reflection;
using Teleform.ProjectMonitoring.HttpApplication;

namespace Monitoring
{
    public class Preview
    {
        public int TemplateID { get; private set; }

        public string DirectoryPath { get; private set; }

        public string TemplateType { get; private set; }

        public Preview(int templateID, string directoryPath, string templateType)
        {
            if (templateType == "screentree")
                return;

            if (templateID <= 0)
                throw new ArgumentOutOfRangeException("templateID", "Идентификатор шаблона может быть только положительным числом.");

            if (string.IsNullOrEmpty(directoryPath))
                throw new ArgumentNullException("directoryPath", "Не задана директория.");

            if (templateType != "wordbased" && templateType != "excelbased")
                throw new EvaluateException("шаблоны могут быть только word и excel формата");

            this.TemplateID = templateID;
            this.DirectoryPath = directoryPath;
            this.TemplateType = templateType;
        }

        public string GetPreviewTemplate()
        {
            var tempData = Path.GetDirectoryName(DirectoryPath);
            if (tempData == null)
                return null;

            if (!Directory.Exists(tempData))
                Directory.CreateDirectory(tempData);

            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);

            var path = new StringBuilder();
            
            var dir = new DirectoryInfo(DirectoryPath);

            DirectoryInfo[] dirArr = dir.GetDirectories(TemplateID.ToString(), SearchOption.TopDirectoryOnly);

            //добавляем в путь имя папки
            DirectoryPath += string.Format(@"\{0}", TemplateID.ToString());

            if (dirArr.Count() == 0)
            {
                Directory.CreateDirectory(DirectoryPath);
                
                path.Append(SavePreviewTemplate());
            }
            else 
            {
                FileInfo[] fileArr = dirArr[0].GetFiles(string.Format("{0}.html", TemplateID.ToString()));
                if (fileArr.Count() == 0)
                    path.Append(SavePreviewTemplate());
                else
                    path.AppendFormat("{0}.html", TemplateID.ToString());
            }

            if (!string.IsNullOrEmpty(path.ToString()))
            {
                path.Insert(0, TemplateID.ToString() + "/");
                path.Insert(0, "temp_data/cache/");
            }
           
            return path.ToString();
        }

        private string SavePreviewTemplate()
        {
            string extension = string.Empty;

            byte[] body = null;

            DownLoadTemplateFromBase(out extension, out body);
            //вложить в cache папку

            if (body == null || string.IsNullOrEmpty(extension))
                return string.Empty;

            var pathForFormat = DirectoryPath + string.Format(@"\{0}{1}", TemplateID.ToString(), extension);
            try
            {
                File.WriteAllBytes(pathForFormat, body);
            }
            catch
            {
                throw new Exception(string.Format("Файл с расширением {0} не загрузился", extension));
            }

            try
            {
                if (TemplateType == "wordbased")
                    using (var wi = new WordInterop())
                        wi.SaveWithHtmlExtension(pathForFormat);
                else if (TemplateType == "excelbased")
                    using (var ei = new ExcelInterop())
                        ei.SaveWithHtmlExtension(pathForFormat);
            }

            catch
            {
#if EXCLUDED
                throw new Exception("Не сохранился файл с расширением html");
#endif
                return string.Empty;
            }

            try
            {
                File.Delete(pathForFormat);
            }
            catch
            {
                throw new Exception(string.Format("Файл с расширением {0} не удалился", extension));
            }

            return TemplateID.ToString() + ".html";
        }


        private void DownLoadTemplateFromBase(out string extension, out byte[] body)
        {
            var c = new SqlConnection(Global.ConnectionString);
#warning расширение может бть NULL
            var ad = new SqlDataAdapter(@"SELECT [MT].[extension], [RT].[body]
                                              FROM [model].[R$Template] [RT]
                                              JOIN [MimeType] [MT] ON [MT].[objID] = [RT].[mimeTypeID]
                                              WHERE [RT].[objID]=@template", c);

            ad.SelectCommand.Parameters.Add("template", SqlDbType.Int).Value = TemplateID;

            var table = new DataTable();
            ad.Fill(table);

            try
            {
                extension = Convert.ToString(table.Rows[0]["extension"]);
                body = (byte[])table.Rows[0]["body"];
            }
            catch 
            {
                extension = null;
                body = null;
            }
            /*catch (InvalidCastException)
            {
                extension = null;
                body = null;
            }
            catch
            {
                throw new Exception("Произошла ошибка при чтении файла шаблона.");
            }*/
        }
    }
}