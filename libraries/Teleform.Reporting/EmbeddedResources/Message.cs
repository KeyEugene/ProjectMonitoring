using ResourceManager = System.Resources.ResourceManager;

namespace Teleform.Reporting
{
    /// <summary>
    /// Позволяет извлекать предназначенные для пользователя сообщения.
    /// </summary>
    internal static class Message
    {
        private static ResourceManager manager;

        static Message()
        {
            manager = new ResourceManager("Teleform.Reporting.EmbeddedResources.Messages", typeof(Message).Assembly);
        }

        /// <summary>
        /// Возвращает текст сообщения с указанным именем на основе предоставленных аргументов.
        /// </summary>
        /// <param name="name">Имя сообщения.</param>
        /// <param name="arguments">Аргументы заполения текста сообщения.</param>
        /// <returns>Возвращает текст сообщения.</returns>
        internal static string Get(string name, params object[] arguments)
        {
            return string.Format(manager.GetString(name), arguments);
        }

        internal static string Get(string name, UniquelyDeterminedObject instance, params object[] arguments)
        {
            return string.Format(
                string.Format(
                    "Объект\n\tтипа '{0}'\n\tс идентификатором '{1}'\n\tс именем '{2}' вызвал исключение.\n",
                    instance.GetType().Name,
                    instance.ID,
                    instance.Name) + Get(name, arguments));
        }
    }
}
