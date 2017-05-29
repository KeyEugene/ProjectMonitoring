#define alexj

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IEnumerable = System.Collections.IEnumerable;
using System.Data;

namespace Teleform.Reporting.Web
{
    partial class TableViewControl
    {
        private Style activePageStyle;
        public Style ActivePageStyle
        {
            get
            {
                if (activePageStyle == null)
                    activePageStyle = new Style();

                return activePageStyle;
            }
        }

        private Style pageStyle;
        public Style PageStyle
        {
            get
            {
                if (pageStyle == null)
                    pageStyle = new Style();

                return pageStyle;
            }
        }

        /// <summary>
        /// кол-во кнопок на странице
        /// </summary>
        private static readonly int pageInWindow = 5;

        /// <summary>
        /// Кол-во всех строк (Data.Table.Rows.Count;)
        /// </summary>
        private int itemCount = -1;

        /// <summary>
        /// Возвращает число представляемых текущим элементом управления объектов (кол-во всех строк в DataSource).
        /// </summary>
        public int ItemCount
        {
            get
            {
                itemCount = DataView.OfType<DataRowView>().Count();

                return itemCount;
            }
        }

        /// <summary>
        /// Возвращает или задаёт число, отображаемых на странице элементов (кол-во строк на одной странице).
        /// </summary>
        public int PageSize
        {
            get
            {
                var o = ViewState["pageSize"];
                if (o != null)
                    return (int)o;
                else
                    return ItemCount;
            }
            set
            {
                ViewState["pageSize"] = value;
            }

        }

        public bool AllowPaging
        {
            get
            {
                var o = ViewState["allowPaging"];
                if (o != null)
                    return (bool)o;
                else return false;
            }
            set
            {
                ViewState["allowPaging"] = value;
            }
        }

        /// <summary>
        /// Возвращает число страниц в рамках постраничного отображения (кол-во страниц).
        /// </summary>
        /// 
        public int PageCount
        {
            get
            {
                var pageCount = ItemCount / PageSize;

                if (ItemCount % PageSize != 0)
                    pageCount++;
                return pageCount;
            }
        }

        public int PageIndex
        {
            get
            {
                var o = ViewState["PageIndex"];
                return o == null ? 0 : (int)o;
            }
            set
            {
                SessionContent.CurrentPage = value;
                ViewState["PageIndex"] = value;
            }
        }

        private int LeftPageIndex
        {
            get
            {
                var o = ViewState["LeftPageIndex"];
                return o == null ? 0 : (int)o;
            }
            set
            {
                ViewState["LeftPageIndex"] = SessionContent.LeftPageIndex = value;
            }
        }

        public string PagingCommand
        {
            get
            {
                var o = ViewState["PagingCommand"];
                return o == null ? "" : (string)o;
            }
            set
            {
                ViewState["PagingCommand"] = value;
            }
        }

        protected void pageButton_Click(object sender, EventArgs e)
        {
            var control = (LinkButton)sender;
            ViewState["PagingCommand"] = control.ID;
            this.DataBind();
        }

        private void TrackPageIndex()
        {
            if (SessionContent.CurrentPage < PageCount)
            {
                PageIndex = SessionContent.CurrentPage;
                LeftPageIndex = SessionContent.LeftPageIndex;
            }
            else
                LeftPageIndex = PageIndex = PageCount - 1;

            var pagingCommand = ViewState["PagingCommand"];

            //двигать в право на 5 страниц
            if (pagingCommand == "NextView")
            {
                #region old
                //if (PageIndex + 5 <= PageCount)
                //    PageIndex = PageIndex + 5;
                //else
                //    PageIndex = (PageCount % PageIndex) + PageIndex;
                //LeftPageIndex = PageIndex;
                #endregion

                if ((PageIndex + 1) % 5 == 0)
                    LeftPageIndex = PageIndex = PageIndex + 1;
                else
                    LeftPageIndex = PageIndex = PageIndex + (5 - ((PageIndex + 1) % 5)) + 1;
                


            }
            //двигать в лево на 5 страниц
            else if (pagingCommand == "PrevView")
            {
#if !alexj
                if (PageIndex - 5 > 1)
                {
                    PageIndex = PageIndex - 5;
                    LeftPageIndex = LeftPageIndex - 5;
                }
                else
                {
                    PageIndex = 0;
                    LeftPageIndex = 0;
                }
#else
                var lpi = LeftPageIndex - (LeftPageIndex % pageInWindow);
                if (lpi != LeftPageIndex)
                {
                    LeftPageIndex = lpi += pageInWindow;
                }
                PageIndex = LeftPageIndex -= pageInWindow;
#endif
            }
            else if (pagingCommand != null && !pagingCommand.Equals("PrevView") && !pagingCommand.Equals("NextView"))
            {
                var command = pagingCommand.ToString().Substring(4);

                //двигаем в конец, в провао
                var rpi = Math.Min(LeftPageIndex + pageInWindow, PageCount);
                if (int.Parse(command) + 1 == rpi && rpi < PageCount)
                {
                    //ЗДЕСЬ!
                    PageIndex = int.Parse(command);
                    //LeftPageIndex += pageInWindow - 1;
                }
                //двигаем в начало, в лево
                else if (int.Parse(command) == LeftPageIndex && rpi < PageCount)
                {
                    PageIndex = int.Parse(command);
                    //ЗДЕСЬ!
                    //LeftPageIndex = Math.Max(LeftPageIndex -= pageInWindow - 1, 0);
                }
                else
                    PageIndex = int.Parse(command);
            }
        }

        private void AddPager(int columnCount)
        {
            var pageRow = new TableRow();
            var pageCell = new TableCell
            {
                ColumnSpan = columnCount,
                HorizontalAlign = HorizontalAlign.Center
            };
            pageCell.ApplyStyle(PageStyle);

            if (LeftPageIndex > 0)
            {
                var pageButton = new LinkButton
                {
                    ID = "PrevView",
                    Text = "<<<",
                };
                pageButton.Click += new EventHandler(pageButton_Click);
                pageButton.ApplyStyle(pageStyle);
                pageCell.Controls.Add(pageButton);

            }
            var RightPageIndex = Math.Min(LeftPageIndex + pageInWindow, PageCount);

            foreach (var index in Enumerable.Range(LeftPageIndex, Math.Min(pageInWindow, PageCount - LeftPageIndex)))
            {
                var text = index + 1;
                var pageButton = new LinkButton
                {
                    Text = text.ToString(),
                    ID = string.Concat("Page", index)
                };
                pageButton.Click += new EventHandler(pageButton_Click);

                if (index == PageIndex)
                    pageButton.ApplyStyle(activePageStyle);

                pageCell.Controls.Add(pageButton);
            }

            if (RightPageIndex < PageCount)
            {
                var pageButton = new LinkButton
                {
                    Text = ">>>",
                    ID = "NextView",
                };
                pageButton.Click += new EventHandler(pageButton_Click);
                pageButton.ApplyStyle(pageStyle);
                pageCell.Controls.Add(pageButton);
            }
            pageRow.Cells.Add(pageCell);
            pageRow.ApplyStyle(PageStyle);
            table.Rows.Add(pageRow);
        }

    }

}