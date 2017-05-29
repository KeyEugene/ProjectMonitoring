using Schema = Teleform.Reporting.Schema;
using Page = System.Web.UI.Page;
using UserControl = System.Web.UI.UserControl;

namespace Teleform.ProjectMonitoring
{
    using HttpApplication;

    [System.Obsolete("", true)]
    public static class UIExtensions
    {
        /// <summary>
        /// Возвращает основную схему, на которую опирается приложение.
        /// </summary>
        public static Schema GetSchema(this Page page)
        { return Global.Schema; }

        /// <summary>
        /// Возвращает основную схему, на которую опирается приложение.
        /// </summary>
        public static Schema GetSchema(this UserControl control)
        { return Global.Schema; }

        /// <summary>
        /// Возвращает условие поиска, возвращающее нуль элементов.
        /// </summary>
        public static string GetInvalidSearchCondition(this Page page)
        {
            return string.Empty;
        }

        /// <summary>
        /// Возвращает условие поиска, возвращающее нуль элементов.
        /// </summary>
        public static string GetInvalidSearchCondition(this UserControl control)
        {
            return string.Empty;
        }
    }
}