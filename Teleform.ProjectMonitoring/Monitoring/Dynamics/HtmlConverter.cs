using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Teleform.Office.TemplatePreview;

namespace Teleform.ProjectMonitoring.Dynamics
{
    public class HtmlConverter
    {
        private const string
           ExcelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
           WordMimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

        private static readonly Dictionary<string, dynamic> admissableTypes;

        static HtmlConverter()
        {
            admissableTypes = new Dictionary<string, dynamic>();

            admissableTypes.Add(ExcelMimeType,
                new
                {
                    Extension = ".xlsx",
                    ConverterType = typeof(ExcelInterop)
                });
            admissableTypes.Add(WordMimeType,
                new
                {
                    Extension = ".docx",
                    ConverterType = typeof(WordInterop)
                });
        }

        private static void DeleteDirectorySafely(string path)
        {
            foreach (var f in Directory.GetFiles(path))
                File.Delete(f);

            foreach (var d in Directory.GetDirectories(path))
            {
                DeleteDirectorySafely(d);
                Directory.Delete(d);
            }
        }

        public static bool CanConvert(string mimeType)
        {
            return admissableTypes.ContainsKey(mimeType);
        }

        public static string Convert(string mimeType, byte[] content, string storagePath, string section, DateTime modified)
        {
            dynamic converterInfo;

            if (!admissableTypes.TryGetValue(mimeType, out converterInfo))
                throw new Exception("Указанный тип документа не поддерживается.");

            var path = Path.Combine(storagePath, section);

            if (Directory.Exists(path))
            {
                var fileInfo = new FileInfo(Path.Combine(path, "index.html"));

                if (fileInfo.LastWriteTime < modified)
                    DeleteDirectorySafely(path);
                else goto ok;
            }

            Directory.CreateDirectory(path);

            var fileName = Path.Combine(path, string.Concat("index", converterInfo.Extension));

            File.WriteAllBytes(fileName, content);

            using (IPreview converter = Activator.CreateInstance(converterInfo.ConverterType))
                converter.SaveWithHtmlExtension(fileName);

            File.Delete(fileName);

        ok:
            return Path.Combine(path, "index.html");
        }
    }
}