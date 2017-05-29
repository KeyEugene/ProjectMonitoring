using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace Teleform.Reporting.Deprecated
{
    internal class ExcelReportBuilder : IGroupReportBuilder
    {
        // Временная папка для сгенерированного хранимой процедурой отчета.
        public string TemporaryDirectory { get; private set; }
        // Строка соединения.
        public string ConnectionString { get; private set; }

        public string DefaultExtension 
        {
            get 
            {
                return ".xlsx";
            }
        }

        public ExcelReportBuilder(string outputDirectory, string connectionString)
        {
            if (String.IsNullOrEmpty(outputDirectory))
                throw new ArgumentNullException("outputDirectory", string.Format("Параметр {0} не должен быть null или пустой строкой.", "outputDirectory"));
            if (String.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString", string.Format("Параметр {0} не должен быть null или пустой строкой.", "connectionString"));

            this.TemporaryDirectory = outputDirectory;
            this.ConnectionString = connectionString;
        }

        public void Create(Stream output, GroupReport report)
        {
            if (output == null)
                throw new ArgumentNullException("output", string.Format("Параметр {0} имеет значение null.", "output"));
            if (report == null)
                throw new ArgumentNullException("template", string.Format("Параметр {0} имеет значение null.", "template"));

            // Параметры хранимой процедуры.
            var fullPath = Path.Combine(TemporaryDirectory, Guid.NewGuid().ToString() + this.DefaultExtension);
#if f
            var sheet = (report as ExcelReport).Sheet;
#endif
            var templateID = report.Template.ID;

            // Создание или дополнение отчета зависит от наличия содержимого в шаблоне.
            var content = report.Template.Content;
            if (content != null)
            {
                try
                {
                    File.WriteAllBytes(fullPath, content);
                }
                catch (IOException ex)
                {
#warning TODO сообщения по I/O - нужны или нет?
                    throw new InvalidOperationException(String.Format("Возникла общая ошибка ввода-вывода:\n{0}.", ex.Message));
                }
            }

#if f
            // Выполняем хранимую процедуру.
            GenerateExcelReport(templateID, sheet, fullPath);
#endif

            // Результат - пишем в поток.
            try
            {
                using (Stream fileStream = File.OpenRead(fullPath))
                {
                    fileStream.CopyTo(output);
                }
            }
            catch (FileNotFoundException ex)
            {
                throw new InvalidOperationException(String.Format("Не найден файл {0}.", fullPath));
            }
            catch (NotSupportedException ex)
            {
                throw new InvalidOperationException(String.Format("Поток назначения не поддерживает запись:\n{0}.", ex.Message));
            }
            catch (ObjectDisposedException ex)
            {
                throw new InvalidOperationException(String.Format("Поток назначения был преждевременно закрыт:\n{0}.", ex.Message));
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException(String.Format("Возникла общая ошибка ввода-вывода:\n{0}.", ex.Message));
            }
        }

 
        private void GenerateExcelReport(object templateID, string sheet, string fullPath)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand())
                {
                    // Хардкод команды и списка ее параметров - не комильфо!
                    command.CommandText = "EXEC [report].[exportToExcel.Test] @templateID,@file,@list";
                    command.Parameters.AddWithValue("@templateID", templateID);
                    command.Parameters.AddWithValue("@file", fullPath);
                    command.Parameters.AddWithValue("@list", sheet);
                    command.Connection = connection;

                    var errorMessages = new StringBuilder();
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        //TODO Нужно адекватное исключение
                        for (int i = 0; i < ex.Errors.Count; i++)
                            errorMessages.Append("#" + i + "\n" +
                                "Сообщение: " + ex.Errors[i].Message + "\n" +
                                "Номер ошибки: " + ex.Errors[i].Number + "\n" +
                                "Строка: " + ex.Errors[i].LineNumber + "\n" +
                                "Источник: " + ex.Errors[i].Source + "\n" +
                                "Процедура: " + ex.Errors[i].Procedure + "\n");

                        throw new InvalidOperationException(errorMessages.ToString());
                    }
                }
            }
        }

    }
}
