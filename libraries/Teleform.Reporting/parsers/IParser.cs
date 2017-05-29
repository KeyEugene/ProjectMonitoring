using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Teleform.Reporting.Parsers
{
    /// <summary>
    /// Описывает интерфейс парсера для объектов определённого типа.
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Возвращает объект на основе предоставленного элемента xml.
        /// </summary>
        /// <param name="e">Элемент xml.</param>
        /// <exception cref="ArgumentNullException">Параметр e равен null.</exception>
        /// <exception cref="ArgumentException">Парсер не может обработать предоставленный элемент xml.</exception>
        object Parse(XElement e);
    }
}
