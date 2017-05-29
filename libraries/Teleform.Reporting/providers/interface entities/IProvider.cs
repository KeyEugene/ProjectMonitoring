namespace Teleform.Reporting.Providers
{
    /// <summary>
    /// Предоставляет механизм получения экземпляра объекта.
    /// </summary>
    public interface IProvider
    {
        /// <summary>
        /// Возвращает экземпляр объекта.
        /// </summary>
        /// <returns></returns>
        object GetInstance();
    }

    public interface ISchemaProvider : IProvider
    {
        new Schema GetInstance();
    }
}
