using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Report.Interop.Templates;
using Report.OXML.Templates;

namespace Report.Templates
{
    /// <summary>
    /// Класс-обертка, описывающий взаимодействие пользователя с шаблоном документа Word.
    /// </summary>
    /// <remarks>
    /// Позволяет обрабатывать шаблоны при помощи механизма Interop (требует наличия Office на целевой машине),
    /// либо при помощи OpenXML SDK (предпочтительный вариант).
    /// </remarks>
    public class WordTemplate : IDisposable
    {
        /// <summary>
        /// Хранит значение флага использования Interop.
        /// </summary>
        private bool _useInterop;

        /// <summary>
        /// Ссылка на класс, осуществляющий операции с шаблоном
        /// </summary>
        private BaseTemplate _template;

        /// <summary>
        /// Открывает документ шаблона для дальнейшей обработки.
        /// </summary>
        /// <param name="templatePath">Путь к шаблону.</param>
        /// <param name="useInterop">Задает режим создания отчета: Interop или OpenXML. См. Примечания к классу.</param>
        public WordTemplate(string templatePath, bool useInterop = false)
        {
            this._useInterop = useInterop;
            if (useInterop)
                _template = new Interop.Templates.WordTemplate( templatePath );
            else
                _template = new OXML.Templates.WordTemplate( templatePath );
        }

        /// <summary>
        /// Получает список закладок.
        /// </summary>
        /// <returns>Список найденных в документе закладок.</returns>
        public List<string> GetBookmarksList()
        {
            var result = _template.GetBookmarksList();
            return result;
        }

        /// <summary>
        /// Заполняет закладки значениями из словаря.
        /// </summary>
        /// <remarks>
        /// Значения не присваиваются, если в шаблоне отсутствует закладка с указанным именем.
        /// </remarks>
        /// <param name="values">Пары "имя закладки - значение"</param>
        public void EvaluateBookmarks( Dictionary<string, string> values )
        {
            _template.EvaluateBookmarks( values );
        }

        /// <summary>
        /// Заполняет закладки их именами.
        /// </summary>
        /// <remarks>
        /// Может использоваться для проверки правильности составления шаблона пользователем.
        /// </remarks>
        public void EvaluateBookmarks()
        {
            _template.EvaluateBookmarks();
        }

        /// <summary>
        /// Сохраняет сформированный отчет на диск.
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="documentName"></param>
        public void SaveToDocument( string destination, string documentName )
        {
            _template.SaveToDocument( destination, documentName );
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        public void Dispose()
        {
            _template.Dispose();
        }
    }
}
