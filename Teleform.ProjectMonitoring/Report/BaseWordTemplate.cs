using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Teleform.Office.Reporting.enums;
using Teleform.Office.Reporting.Placeholders;

namespace Teleform.Office.Reporting
{
    public abstract class BaseWordTemplate : IDisposable
    {
        public PlaceholderType PlaceholderType { get; private set; }

        /// <summary>
        /// Возвращает байт-массив, представляющий тело шаблона.
        /// </summary>
        protected byte[] Body { get; private set; }

        /// <summary>
        /// Возвращает путь к файлу с телом шаблона.
        /// </summary>
        protected string Path { get; private set; }

        protected IPlaceholder _placeholder;

        public BaseWordTemplate( string path ) 
        {
            this.Path = path;
            try
            {
                Body = File.ReadAllBytes( path );
            }
            catch ( Exception ex )
            {
                throw new Exception( ex.Message );
            }

            this.PlaceholderType = enums.PlaceholderType.ContentControl;
        }

        public BaseWordTemplate( byte[] body )
        {
            Body = body;

            this.PlaceholderType = enums.PlaceholderType.ContentControl;
        }

        protected abstract void AttachPlaceholders();

        /// <summary>
        /// Сохраняет заполненный шаблон по указанному пути.
        /// </summary>
        public abstract void Save(string path);

        /// <summary>
        /// Заполняет шаблон предоставленными данными.
        /// </summary>
        /// <param name="data"></param>
        public IEnumerable<PlaceholderData> GetPlaceholders()
        {
            return this._placeholder.GetPlaceholders();
        }

        /// <summary>
        /// Возвращает список закладок шаблона.
        /// </summary>
        public void FillPlaceholders( IDictionary<string, string> data )
        {
            this._placeholder.FillPlaceholders( data );
        }

        protected bool IsDisposed = false;

        public void Dispose()
        {
            Cleanup(true);

            GC.SuppressFinalize(this);
        }

        protected abstract void Cleanup(bool disposing);
    }
}
