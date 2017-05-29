using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Teleform.ProjectMonitoring.NavigationDialogView
{
    public struct DataTreeNode
    {
        public string selectedEntityID { get; set; }
        public string selectedConstrID { get; set; }
        public string selectedInstanceID { get; set; }
        public int urlLevel { get; set; }
        public string currentPath { get; set; }
        public string getLeftUrl { get; set; }
        /// <summary>
        /// Если находимся на карточке - true
        /// </summary>
        public bool isDynamicCard { get; set; }
        /// <summary>
        /// Таблица котороя отображается в ReportViewControls(TableView) c  Objects and Title's
        /// </summary>
        public DataTable ReportTableView { get; set; }
    }
}