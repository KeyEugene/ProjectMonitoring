using ArgumentNullException = System.ArgumentNullException;
using XDocument = System.Xml.Linq.XDocument;

#if truef
namespace Teleform.Reporting.Providers
{
    using IParser = Parsers.IParser;

    /// <summary>
    /// Представляет провайдер экземпляра объекта некоторого типа на основе xml.
    /// </summary>
    public abstract class XmlInstanceProvider : IProvider
    {
        private IParser parser;

        /// <summary>
        /// Возвращает строку универсального кода ресурса (uri), который ссылается на файл,
        /// содержащий объект.
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Инициализирует объект типа Teleform.Reporting.Providers.XmlInstanceProvider.
        /// </summary>
        /// <param name="parser">Парсер получаемого провайдером объекта.</param>
        /// <param name="uri">
        /// Cтрока универсального кода ресурса (uri), который ссылается на файл, содержащий объект.
        /// </param>
        /// <exception cref="ArgumentNullException">Значение параметра parser или uri равно null.</exception>
        public XmlInstanceProvider(IParser parser, string uri)
        {
            if (parser == null)
                throw new ArgumentNullException("parser",
                    string.Format("Параметр {0} имеет значение null.", "parser"));

            if (uri == null)
                throw new ArgumentNullException("uri",
                    string.Format("Параметр {0} имеет значение null.", "uri"));

            this.parser = parser;
            Content = uri;
        }

        /// <summary>
        /// Реализует интерфейс Teleform.Reporting.Providers.IProvider.
        /// </summary>
        /// <returns>Возвращает экземпляр объекта определённого типа.</returns>
        object IProvider.GetInstance()
        {
            var document = XDocument.Load(Content);

            

            return parser.Parse(document.Root);
        }
    }
}
#endif