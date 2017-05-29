using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Report.enums;

namespace Report
{
    public abstract class BaseTemplate : IDisposable
    {
        /// <summary>
        /// Тип шаблона.
        /// </summary>
        public TemplateType Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal string Location { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal string DocumentName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal string FullPath
        {
            get
            {
                return Path.Combine(Location, DocumentName);
            }
        }

        internal string TemplatePath { get; set; }
        protected bool IsDisposed = false;

        public abstract List<string> GetBookmarksList();
        public abstract void EvaluateBookmarks();
        public abstract void EvaluateBookmarks( Dictionary<string, string> values );
        public abstract void SaveToDocument( string destination, string documentName );

        public abstract void Dispose();
    }
}
