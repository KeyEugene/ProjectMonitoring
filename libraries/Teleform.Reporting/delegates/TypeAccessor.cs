namespace Teleform.Reporting
{
    /// <summary>
    /// Описывает правило получения типа по его имени.
    /// </summary>
    /// <param name="name">Имя типа.</param>
    /// <returns>Возвращает тип по его имени.</returns>
    public delegate Type TypeAccessor(string name);
}
