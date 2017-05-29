using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting.MicrosoftOffice
{
    /// <summary>
    /// Описывает шаблон для генерации Excel-отчёта.
    /// </summary>
    public class ExcelTemplate : Template
    {
        public ExcelTemplate(Template template)
            : base(template.ID, template.Name, template.Entity, template.FileName, template.Content, template.Fields)
        {
        }

        /// <summary>
        /// Возвращает имя листа Excel-книги, на которую необходимо осуществить выгрузку.
        /// </summary>
        public string Sheet
        {
            get {
                if (Parameters != null && Parameters.ContainsKey("sheet"))
                {
                    var v = Parameters["sheet"];

                    if (!string.IsNullOrWhiteSpace(v))
                        return v;
                }

                return string.Format("Выгрузка {0}", DateTime.Now.ToString().Replace(":", "."));
            }
        }
    }
}
