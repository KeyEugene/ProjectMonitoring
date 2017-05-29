
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using TableItemStyle = System.Web.UI.WebControls.TableItemStyle;
using System.Web.UI;

namespace Teleform.Reporting.Web
{
    partial class TableViewControl
    {
        private TableItemStyle selectedRowStyle;

        /// <summary>
        /// Снимает выделение в отображаемом списке.
        /// </summary>
        public void ClearSelection()
        {
            ViewState.Remove("SelectedIndex");
        }

        /// <summary>
        /// Индекс выделенной строки на странице
        /// </summary>
        public int SelectedRowIndex
        {
            get
            {
                var o = ViewState["SelectedRowIndex"];
                return o == null ? -1 : (int)o;
            }
            set
            {
                ViewState["SelectedRowIndex"] = value;
                //ViewState["SelectedIndex"] = SessionContent.SelectedRowIndex = value;
            }
        }

        /// <summary>
        /// Возвращает ссылку на объект System.Web.UI.WebControls.TableItemStyle, позволяющий
        /// настроить вид выделенной строки в данном элементе управления.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public TableItemStyle SelectedRowStyle
        {
            get
            {
                if (selectedRowStyle == null)
                    selectedRowStyle = new TableItemStyle();

                return selectedRowStyle;
            }
        }

        /// <summary>
        /// Получает или задает URL-адрес страницы, на которую осуществляется передача
        /// с текущей страницы при открытии элемента.
        /// </summary>
        public string OpenAction
        {
            get { return ViewState["OpenAction"] as string; }
            set { ViewState["OpenAction"] = value; }
        }

        public string[] Parameters { get; set; }
    }
}