#define Alex
#define ShowNotSelectedItemsWWW //не скрывать не выбранные елемены FilterControl, отметить их как не выбранные
#define CheckBoxSelect


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IEnumerable = System.Collections.IEnumerable;
using System.Data;

using System.Globalization;


namespace Teleform.Reporting.Web
{
    [ToolboxData("<{0}:FilterControl runat=\"server\" />")]
    public partial class FilterControl : WebControl, INamingContainer, IFilterControl
    {

        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public ListItemCollection Items
        {
            get
            {
                EnsureChildControls();
                return CheckBoxList.Items;
            }
        }
#if true
        public string TechPredicate
        {
            get
            {
                if (CheckBoxList.SelectedIndex != -1)
                {
                    var itemsCollection = Items.OfType<ListItem>();

                    var selectedItems = itemsCollection.Where(item => item.Selected);
                    
                    var hasEmptyValue = selectedItems.Where(item => string.IsNullOrEmpty(item.Value));
                    var hasFullValue = selectedItems.Where(item => !string.IsNullOrEmpty(item.Value));

                    List<string> itemsValueList = new List<string>();
                    foreach (var item in selectedItems)
                    {
                        if (!string.IsNullOrEmpty(item.Value))
                            itemsValueList.Add("'" + item.Value + "'");
                    }

                    var result = string.Empty;
                    var nullValue = "[" + AttributeID + "] IS NULL";
                    var itemsValueStr = string.Join(", ", itemsValueList);

                    if (hasEmptyValue.Count() > 0 && hasFullValue.Count() > 0)
                    {                         
                        result = string.Format("[{0}] IN ({1}) or {2}", AttributeID, itemsValueStr, nullValue);
                    }
                    else if (hasEmptyValue.Count() > 0 && hasFullValue.Count() == 0)
                    {
                        result = nullValue;
                    }
                    else if (hasEmptyValue.Count() == 0 && hasFullValue.Count() > 0)
                    {
                        result = string.Format("[{0}] IN ({1})", AttributeID, itemsValueStr);
                    }

                    return result;
                }
                else return null;
            }
        }
#else
        public string TechPredicate
        {
            get
            {
                if (CheckBoxList.SelectedIndex != -1)
                {
                    var filterExpression = new System.Text.StringBuilder();

                    var itemsCollection = Items.OfType<ListItem>();
                    var selectedItems = itemsCollection.Where(item => item.Selected && !string.IsNullOrWhiteSpace(item.Value));

                    if (selectedItems.Any())
                       filterExpression.AppendFormat("[{0}] IN ({1})", AttributeID, string.Join(", ", selectedItems.Select(item => "'" + item.Value + "'")));
                    
                    var result = filterExpression.ToString();

                    return result;
                }
                else return null;
            }
        }
#endif
        public object FilterData
        {
            get
            {
                //если item is selected возвращаем его текст
                var elementsCollection = new List<string>();

                foreach (ListItem item in CheckBoxList.Items)
                    if (item.Selected)
                        elementsCollection.Add(item.Text);

                return elementsCollection;
            }
            set
            {
                if (value is List<string>)
                {
                    //если нашелся текст в CheckBoxList.Items, то item сделать Selected = true
                    var elementsCollection = value as List<string>;

                    foreach (ListItem item in CheckBoxList.Items)
                    {
                        if (elementsCollection.Contains(item.Text))
                            item.Selected = true;
                        else
                            item.Selected = false;
                    }
                    var sortedItems = CheckBoxList.Items.Cast<ListItem>().OrderBy(i => i.Selected.Equals(false)).ToArray();
                    CheckBoxList.Items.Clear();
                    CheckBoxList.Items.AddRange(sortedItems);
                }

            }
        }

        public TemplateField Field { get; set; }

        public CheckBoxList CheckBoxList;

        private Button ApplyButton;

        private Button CancelButton;

        private Style listStyle;



        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Style ListStyle { get; set; }

        public string AttributeID { get; set; }

        public string ColumnType { get; set; }


        private TextBox LiveFilterBox;


        protected override void CreateChildControls()
        {
            LiveFilterBox = new TextBox { ID = "LiveFilterBox" };
            LiveFilterBox.TextMode = TextBoxMode.MultiLine;
            LiveFilterBox.Style.Add("width", "180px");

            CheckBoxList = new CheckBoxList { ID = "ItemList" };

            ApplyButton = new Button { ID = "ApplyButton", Text = "Применить" };

            CancelButton = new Button { ID = "CancelButton", Text = "Сбросить" };
            CancelButton.Click += new EventHandler(CancelFilter_Click);

            ApplyButton.Click += new EventHandler(ApplyFilter_Click);

            Controls.Add(CheckBoxList);
            Controls.Add(LiveFilterBox);

            Controls.Add(ApplyButton);
            Controls.Add(CancelButton);
        }


        public void RejectFilter()
        {
            LiveFilterBox.Text = null;
            CheckBoxList.ClearSelection();
            if (FilterCanceled != null)
                FilterCanceled(this, EventArgs.Empty);

            //ApplyFilter_Click(this, EventArgs.Empty);
        }

        public event EventHandler FilterApplied;
        public event EventHandler FilterCanceled;

        protected void ApplyFilter_Click(object sender, System.EventArgs e)
        {
            if (FilterApplied != null)
                FilterApplied(this, EventArgs.Empty);
        }

        protected void CancelFilter_Click(object sender, EventArgs e)
        {
            //LiveFilterBox.Text = null;  
            RejectFilter();

            //if (FilterCanceled != null)
            //    FilterCanceled(this, EventArgs.Empty);

            ApplyFilter_Click(this, EventArgs.Empty);

        }
    }
}