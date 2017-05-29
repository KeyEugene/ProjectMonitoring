using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Collections.Generic;

namespace Teleform.Reporting
{
    using Archivator = Teleform.IO.Compression.Archivator;

    /// <summary>
    /// Представляет функциональность подготовки архивированного группового отчёта.
    /// </summary>
    public class ArchiveReportBuilder : GroupReportBuilder
    {
        /// <summary>
        /// Возвращает объект, управляющий процессом архивирования.
        /// </summary>
        public Archivator Archivator { get; private set; }

        /// <summary>
        /// Возвращает временную директорию, в которой осуществляется подготовка отчёта.
        /// </summary>
        public string TemporaryDirectory { get; private set; }

        public ArchiveReportBuilder(IGroupReportBuilder reportBuilder, Archivator archivator, string temporaryDirectory)
            : base(reportBuilder)
        {
            //см. Teleform.Reporting.MicrosoftOffice.ReportType ReportType.ZipWord атрибут Parameters
#if true || dasha
            if (!Path.IsPathRooted(temporaryDirectory))
                temporaryDirectory = HttpContext.Current.Server.MapPath(temporaryDirectory);
#endif
            TemporaryDirectory = temporaryDirectory;
            Archivator = archivator;
        }

        public override void Create(Stream output, GroupReport report)
        {
            GroupReport singleReport;
            List<Instance> instanceList;

            var contentFolder = Path.Combine(TemporaryDirectory, "files");
            var archive = Path.Combine(TemporaryDirectory, "report.zip");

            if (!Directory.Exists(TemporaryDirectory))
                Directory.CreateDirectory(TemporaryDirectory);

            if (File.Exists(archive))
                File.Delete(archive);

            if(!Directory.Exists(contentFolder))
                Directory.CreateDirectory(contentFolder);
            else
                foreach(var file in Directory.GetFiles(contentFolder))
                    File.Delete(file);

            var i = 1;

            foreach (var instance in report.Instances)
            {
#if f
                var property = instance.Properties.FirstOrDefault(item => item.Field.Attribute.Naming);
                string name;

                if(property != null && property.Value != null) name = property.Value.ToString();
                else name = instance.GetHashCode().ToString("X8");

#warning Путь может соодержать недопустимые символы (ArgumentException).
                using (var stream = File.Create(Path.Combine(contentFolder, name + ".docx")))
                {
                    singleReport = new SingleReport(report.Template, instance);
                    ReportBuilder.Create(stream, singleReport);
                }
#else
                var validName = instance.FullName.Replace('\\', ' ').Replace('/', ' ').Replace('"', ' ').
                    Replace('*', ' ').Replace(':', ' ').Replace('<', ' ').Replace('>', ' ').Replace('?', ' ')
                    .Replace('|', ' ').Replace('\n',' ');

                validName = validName.Trim();
                var path = Path.Combine(contentFolder, "№" + i + ". " + validName.Substring(0, Math.Min(120, validName.Length)) + ".docx");

                using (var stream = File.Create(path))
                {
                    instanceList = new List<Instance>();
                    instanceList.Add(instance);
                    singleReport = new GroupReport(report.Template, instanceList);
                    ReportBuilder.Create(stream, singleReport);
                }
#endif
                i++;
            }

            Archivator.Create(contentFolder, archive);

            using (var stream = File.OpenRead(archive))
                stream.CopyTo(output);
        }
    }
}