namespace Teleform.Reporting
{
    /// <summary>
    /// Описывает однозначно определяемый именованный объект.
    /// </summary>
    public interface IUniquelyDeterminedObject
    {
        /// <summary>
        /// Возвращает уникальный идентификатор объекта.
        /// </summary>
        object ID { get; }

    }

    public interface IUserDeterminedObject
    {

        object ID { get; }

        int UserID { get; }
    }
}
