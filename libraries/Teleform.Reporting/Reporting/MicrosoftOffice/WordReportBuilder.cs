using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Teleform.Reporting
{
    public class WordReportBuilder : ISingleReportBuilder
    {
        public string TemporaryDirectory { get; private set; }

        private WordprocessingDocument Document { get; set; }


        public WordReportBuilder()
        {

        }


#warning Не используется.
        public WordReportBuilder(string temporaryDirectory)
        {
            this.TemporaryDirectory = temporaryDirectory;
        }

        public void Create(Stream output, SingleReport report)
        {
            if (output == null)
                throw new ArgumentNullException("output", Message.Get("Common.NullArgument", "output"));
            if (report == null)
                throw new ArgumentNullException("report", Message.Get("Common.NullArgument", "report"));

            // Открываем содержимое в памяти.
            var content = report.Template.Content;
            using (var memory = new MemoryStream())
            {
                memory.Write(content, 0, (int)content.Length);

                try
                {
                    Document = WordprocessingDocument.Open(memory, true);
                    Document.ChangeDocumentType(DocumentFormat.OpenXml.WordprocessingDocumentType.Document);
                }
#warning TODO Посмотреть актуальные исключения на операции.
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Возникла ошибка при чтении тела шаблона.\n", ex.InnerException);
                }

                var properties = report.Instance.Properties;
                var baseProperties = report.Instance.BaseProperties;

                bool useBaseProperties = baseProperties != null;

                // Получаем список закладок...
                var placeholders = Document.ContentControls();
                if (placeholders.Count() == 0)
                {
                    Document.Close();
                    throw new InvalidOperationException("В теле шаблона не найдено Structured Document Tag элементов.");
                }

                foreach (var placeholder in placeholders)
                {
                    var placeholderId = placeholder.GetSdtTagText();

                    object entityId, hashedAttributeId, formatId;

                    var uid = new UniqueIDCreator();
                    uid.Split(placeholderId, out entityId, out hashedAttributeId, out formatId);

                    // В случае отсутствия value - оставлять элемент с оригинальным текстом, но выделять красным шрифтом.
                    if (!useBaseProperties)
                    {
                        if (properties.First(x => x.Field.Attribute.ID.ToString() == hashedAttributeId.ToString()).Value == null)
                            (placeholder as SdtElement).SetSdtTextColor("FF0000");
                        else
                        {
                            // Заполняем
                            try
                            {
                                var value = properties
                                    .Where(
                                    x => (x.Field.Attribute.ID.ToString() == hashedAttributeId.ToString()) && (x.Field.Format.ID.ToString() == formatId.ToString())
                                    )
                                    .First().Value;

                                var element = placeholder as SdtElement;
                                element.SetSdtText(value.ToString());
                                element.SetSdtTextColor("000000");
                            }
                            catch (InvalidOperationException ex)
                            {
                                Document.Close();
                                throw new InvalidOperationException(String.Format("Не удалось найти атрибут.\n{0}.", ex.Message));
                            }
                        }
                    }
                    else
                    {
                        var property = baseProperties.First(x => x.Attribute.ID.ToString() == hashedAttributeId.ToString());

                        if (property.Value == null || property.Value is DBNull)
                            (placeholder as SdtElement).SetSdtTextColor("FF0000");
                        else
                        {
                            try
                            {
                                var element = placeholder as SdtElement;
                                var format = property.Attribute.Type.GetAccessibleFormats().
                                    First(o => o.ID.ToString() == formatId.ToString());

                                element.SetSdtText(string.Format(format.Provider, format.FormatString, property.Value));
                                element.SetSdtTextColor("000000");
                            }
                            catch (InvalidOperationException ex)
                            {
                                Document.Close();
                                throw new InvalidOperationException(String.Format("Не удалось найти атрибут.\n{0}.", ex.Message));
                            }
                        }
                    }
#warning Неплохо бы выводить сведения о хеше.
                }
                Document.Close();

                // Результат - пишем в поток.
                try
                {
                    memory.Seek(0, SeekOrigin.Begin);
                    memory.CopyTo(output);
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
        }
    }

    public static class ContentControlExtensions
    {
        public static IEnumerable<OpenXmlElement> ContentControls(this OpenXmlPart part)
        {
            return part.RootElement.
                Descendants().
                Where(e => e is SdtBlock || e is SdtRun || e is SdtCell);
        }

        public static IEnumerable<OpenXmlElement> ContentControls(this WordprocessingDocument document)
        {
            foreach (var cc in document.MainDocumentPart.ContentControls())
                yield return cc;
            foreach (var header in document.MainDocumentPart.HeaderParts)
                foreach (var cc in header.ContentControls())
                    yield return cc;
            foreach (var footer in document.MainDocumentPart.FooterParts)
                foreach (var cc in footer.ContentControls())
                    yield return cc;
            if (document.MainDocumentPart.FootnotesPart != null)
                foreach (var cc in document.MainDocumentPart.FootnotesPart.ContentControls())
                    yield return cc;
            if (document.MainDocumentPart.EndnotesPart != null)
                foreach (var cc in document.MainDocumentPart.EndnotesPart.ContentControls())
                    yield return cc;
        }

#warning Восходящее приведение! Нужна ли проверка?
        public static string GetSdtTagText(this OpenXmlElement element)
        {
            SdtProperties properties = element.Elements<SdtProperties>().FirstOrDefault();
            Tag tag = properties.Elements<Tag>().FirstOrDefault();
            if (tag != null)
                return tag.Val.ToString();
            else
                return null;
        }

        public static string GetSdtTextName(this OpenXmlElement element)
        {
            return element.InnerText;
        }

#warning Может ли отсутствовать текст в закладке?
        public static void SetSdtText(this SdtElement element, string text)
        {
            if (element.Descendants<Text>().FirstOrDefault().Text != null)
                element.Descendants<Text>().FirstOrDefault().Text = text;
        }

        public static void SetSdtTextColor(this SdtElement element, string color)
        {
            var runProperties = element.Descendants<RunProperties>().ToList();

            foreach (var rp in runProperties)
                try
                {
                    rp.Color = new Color() { Val = color };
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Не удалось установить цвет текста в Structured Document Tag элементе.", ex.InnerException);
                }
        }
    }
}
