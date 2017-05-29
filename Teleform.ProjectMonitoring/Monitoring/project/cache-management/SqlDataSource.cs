using System;
using Phoenix.Optimization;
using System.Web.UI;

namespace Teleform.ProjectMonitoring
{
    /// <summary>
    /// Представляет базу данных SQL для элементов управления с привязкой данных.
    /// Предоставляет информацию о числе выбранных элементов.
    /// </summary>
    public class SqlDataSource : System.Web.UI.WebControls.SqlDataSource
    {
        /// <summary>
        /// Инициализирует должным образом данный объект.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Selected += new System.Web.UI.WebControls.SqlDataSourceStatusEventHandler(DataSelected);
        }

        /// <summary>
        /// Возвращает количество выбранных элементов в результате выполнения последней команды.
        /// </summary>
        public int DataItemCount
        {
#if true
            get
            {
                var o = Page.Cache[Page.GetType().Name + ".DataItemCount" + Page.Request["entity"]];
                return (o == null ? 0 : (int) o);
            }
            private set
            { Page.Cache[Page.GetType().Name + ".DataItemCount" + Page.Request["entity"]] = value; }
#else
            get;
            private set;
#endif
        }

        public event EventHandler DataChanged;

        private void DataSelected(object sender, System.Web.UI.WebControls.SqlDataSourceStatusEventArgs e)
        {
            DataItemCount = e.AffectedRows;

            if (DataChanged != null)
                DataChanged(this, EventArgs.Empty);
        }
    }
}