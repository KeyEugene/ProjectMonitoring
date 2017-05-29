using System;
using StringBuilder = System.Text.StringBuilder;
using Stream = System.IO.Stream;
using System.IO;
using System.Text;
using System.Linq;

#warning Следует ли использовать DateTimeOffset.
#warning Учитываются только кириллические и обычные кавычки.

namespace Teleform.Reporting.MicrosoftOffice
{
    /// <summary>
    /// Представляет алгоритм построения кириллического CSV-отчёта совместимого с Microsoft Excel.
    /// </summary>
    public class CsvReportBuilder : IGroupReportBuilder
    {
        /// <summary>
        /// Создаёт CSV-отчёт в указанном потоке на основе предоставленных данных.
        /// </summary>
        /// <param name="stream">Поток, в котором создаётся CSV-отчёт.</param>
        /// <param name="report">Данные для подготовки группового отчёта.</param>
        /// <exception cref="System.ArgumentNullException">Параметр stream или report равен null.</exception>
        public void Create(Stream stream, GroupReport report)
        {
            if (report == null)
                throw new ArgumentNullException("report", "Параметр report имеет значение null.");

            if (stream == null)
                throw new ArgumentNullException("stream", "Параметр stream имеет значение null.");

            var writer = new StreamWriter(stream, UTF8Encoding.Default);

            foreach (var field in report.Template.Fields)
                writer.Write(string.Concat(field.Name, ";"));

            foreach (var instance in report.Instances)
            {
                Instance.Property property;

                writer.WriteLine();

                foreach (TemplateField field in report.Template.Fields)
                {
                    property = instance.OwnProperties.First(o => o.Attribute.ID.ToString() == field.Attribute.ID.ToString());

                    var stringValue = property.Value.ToString();

                    if (string.IsNullOrWhiteSpace(stringValue))
                        writer.Write(";");
                    else if (property.Value is DateTime || property.Value is DateTimeOffset)
                        writer.Write("{0:dd.MM.yyyy};", property.Value);
                    else if (field.Attribute.Type.Name.Contains("money"))
                    {
                        switch (field.Format.FormatString)
                        {
                            case "{0:### ### ### ### ### p}.":
                                writer.Write("{0:### ### ### ### ###};", property.Value);
                                break;
                            case "{0}":
                                writer.Write(string.Concat(stringValue, "р.;"));
                                break;
                            default:
                                writer.Write(
                                    string.Concat(
                                        string.Format(field.Format.Provider, field.Format.FormatString, property.Value),
                                        ";"));
                                break;
                        }
                    }
                    else if (stringValue.Contains(';'))
                    {
                        foreach (var c in "«»\"")
                        {
                            var s = c.ToString();
                            stringValue = stringValue.Replace(s, string.Concat(s, s));
                        }
                        
                        writer.Write(string.Concat("\"", stringValue, "\";"));
                    }
                    else
                        writer.Write(string.Concat(stringValue, ";"));
                }
            }
        }
    }
}
