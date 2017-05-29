using ArgumentNullException = System.ArgumentNullException;


namespace Teleform.Reporting.Providers
{  
    
    using SchemaParser = Parsers.SchemaParser;



    /// <summary>
    /// Представляет провайдер схемы на основе xml.
    /// </summary>
    public class XmlSchemaProvider : XmlDocumentInstanceProvider
    {
        private IProvider provider;

        /// <summary>
        /// Инициализирует объект типа Teleform.Reporting.Providers.XmlSchemaProvider.
        /// </summary>
        /// <param name="uri">
        /// Cтрока универсального кода ресурса (uri), который ссылается на файл, содержащий схему.
        /// </param>
        /// <exception cref="ArgumentNullException">Значение параметра uri равно null.</exception>
        public XmlSchemaProvider(string uri) : base(new SchemaParser(), uri)
        {
            provider = this;
        }

        //public XmlSchemaProvider(string uri) : base(new WordExcelTemplateAddIns.a)
        //{

        //}
        
        /// <summary>
        /// Возвращает экземпляр схемы.
        /// </summary>
        /// <returns>Возвращает экземпляр схемы.</returns>
        public Schema GetInstance()
        {
            return (Schema) provider.GetInstance();
        }

    }
}
