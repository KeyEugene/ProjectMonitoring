
#define Alex
#define SelectedRow
#define UserFilter

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using IEnumerable = System.Collections.IEnumerable;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;

using Constraint = Teleform.Reporting.constraint.Constraint;
using Teleform.Reporting.DynamicCard;
using System.Xml.Linq;
using System.Web;
using Microsoft.SqlServer;

namespace Teleform.Reporting.Web
{
    [ToolboxData("<{0}:TableView runat=\"server\" />")]
    public partial class TableViewControl : CompositeDataBoundControl, IPostBackEventHandler
    {

        public bool IsCardControl
        {
            get
            {
                var o = ViewState["IsCardControl"];
                if (o != null)
                    return (bool)o;
                else return false;
            }
            set { ViewState["IsCardControl"] = value; }
        }




        /// <summary>
        /// Выражение формируется из ID строк столбца objID
        /// </summary>
        public string NavigatFilterExpression// { get; set; }
        {
            get { return ViewState["NavigatFilterExpression"].ToString(); }
            set { ViewState["NavigatFilterExpression"] = value; }
        }




        public string UserFilterExpression { get; set; }

        private string ExpressFilterExpression { get; set; }

        private string DefaultFilterExpression { get; set; }

        private string ResultFilterExpression { get; set; }


        public object EntityFilterID//{get;set;}
        {
            get { return ViewState["EntityFilterID"]; }
            set { ViewState["EntityFilterID"] = value; }
        }

        public EntityFilter EntityFilter
        {
            get
            {
                var entityFilter = Storage.Select<EntityFilter>(EntityFilterID);
                return entityFilter;

            }
        }

        public Template Template
        {
            get
            {
                var template = Storage.Select<Template>(TemplateID);
                return template;
            }
        }

        public object TemplateID
        {
            get { return ViewState["TemplateID"]; }
            set { ViewState["TemplateID"] = value; }
        }

        private int TemplateCode
        {
            get
            {
                var o = ViewState["TemplateCode"];

                return o == null ? -1 : (int)o;
            }
            set { ViewState["TemplateCode"] = value; }
        }

        public object tempEFID { get; set; }


        protected string GetSortingExpression()
        {
            var sortTupleValues = SessionContent.SortingData.Where(dict => dict.Key.StartsWith(Template.ID.ToString())).Select(dict => (Tuple<string, string>)dict.Value); //new 

            var sortTupleItems1 = sortTupleValues.Select(t => t.Item1);

            var sortingExpression = string.Join(", ", sortTupleItems1);
            return sortingExpression;

        }

        private void childControls_SortingApplied(object sender, EventArgs e)
        {
            if (sender is SortingControl)
            {
                var control = sender as SortingControl;
                var sessionkey = MakeKey(control.AttributeID, control.GetType());

                SessionContent.SortingData[sessionkey] = control.SortingData;

                DataBind();


            }
        }




        /// <summary>
        /// Возвращает или задаёт уникальный ключ контекста данных.
        /// </summary>
        public string ContextKey
        {
            get
            {
                var o = ViewState["ContextKey"];

                return o != null ? o.ToString() : "test";
            }
            set
            {
                ViewState["ContextKey"] = value;
            }
        }

        private TableHeaderRow HeaderRow;

        private Table table;

        public List<IFilterControl> ListFilterControls = new List<IFilterControl>();

        public List<SortingControl> ListSortingControls = new List<SortingControl>();

        private bool IsInternalDataBinding { get; set; }

        public event EventHandler SelectedItemCreating;

        public void RaisePostBackEvent(string argument)
        {
            if (argument.StartsWith("s"))
            {
                SelectedRowIndex = int.Parse(argument.Substring(1));
            }
            IsInternalDataBinding = true;
            DataBind();
        }


#if true

        private IEnumerable<DataRow> DataRow
        {
            get
            {
                return Page.Session[string.Concat("DataRowFor", Template.ID.ToString())] as IEnumerable<DataRow>;
            }
            set
            {
                Page.Session[string.Concat("DataRowFor", Template.ID.ToString())] = value;
            }
        }

#endif


        public int EntityInstanceID
        {
            get
            {
                var o = ViewState["EntityInstanceID"];

                if (o != null)
                    return int.Parse(o.ToString());
                else
                    return -1;
            }
            set
            {
                ViewState["EntityInstanceID"] = value;
            }
        }


        public DataView DataView
        {

            get
            {
                //string path = HttpContext.Current.Request.Url.AbsolutePath;
                return Page.Session[string.Concat("DataViewFor", Template.Entity.SystemName, EntityInstanceID)] as DataView;
            }
            set
            {
                var val = value;
                Page.Session[string.Concat("DataViewFor", Template.Entity.SystemName, EntityInstanceID)] = value;
            }
        }

        //public DataView DataView { get; set; }


        public string MakeKey(string attributeID, System.Type type)
        {
            return string.Concat
            (
                Template.ID.ToString(),
                 "_",
                type.Name,
                attributeID,
                Template.Entity.SystemName

            );
        }

        public void RejectAllSortings()
        {
            if (Template != null)
                SessionContent.SortingData.Clear();

            DataBind();
        }


        public void RejectAllSortings(object sender, EventArgs e)
        {
            if (Template != null)
                SessionContent.SortingData.Clear();

            DataBind();
        }


        private SortingControl CreateSortingControl(string attributeID, Style style, TemplateField field)
        {
            //var control = Activator.CreateInstance<SortingControl>();

            var control = new SortingControl();

            control.AttributeID = attributeID;

            control.SortingApplied += childControls_SortingApplied;

            control.ID = Template.ID.ToString() + typeof(SortingControl).Name + attributeID + field.Order.ToString() + EntityFilterID;

            control.FieldName = field.Name;

            control.ApplyStyle(style);

            if (activeStyle != null)
                control.ActiveStyle = activeStyle;

            var twiceControl = ListSortingControls.Find(c => c.ID == control.ID);
            if (twiceControl == null)
                ListSortingControls.Add(control);

            return control;
        }

        private T CreateFilterControl<T>(string attributeID, Style style, TemplateField field) where T : WebControl, IFilterControl
        {
            var control = Activator.CreateInstance<T>();

            control.AttributeID = attributeID;
            control.FilterApplied += childControls_FilterApplied;
            control.FilterCanceled += childControls_FilterCanceled;
            control.Field = field;
            control.ID = Template.ID.ToString() + typeof(T).Name + attributeID + field.Order.ToString() + EntityFilterID;
            //control.ID = Template.ID.ToString() + typeof(T).Name + field.Attribute.ID.ToString() + field.Order.ToString() + EntityFilterID;

            control.ApplyStyle(style);

            if (activeStyle != null)
                control.ActiveStyle = activeStyle;

            var twiceControl = ListFilterControls.Find(c => c.ID == control.ID);

            if (twiceControl == null)
                ListFilterControls.Add(control);

            return control;
        }
        private void childControls_FilterCanceled(object sender, EventArgs e)
        {
            if (sender is FilterControl)
            {
                foreach (var item in ListFilterControls)
                {
                    if (item.ID.Contains(sender.GetType().Name))
                    {
                        //var sessionKey = MakeKey(item.Field.Attribute.ID.ToString(), item.GetType());
                        var sessionKey = MakeKey(item.AttributeID, item.GetType());

                        SessionContent.FilterData.Remove(sessionKey);
                        SessionContent.ExpressionForSelectCheckBoxes.Remove(sessionKey);

                    }
                }
            }
        }
        private void childControls_FilterApplied(object sender, EventArgs e)
        {
            if (sender is IFilterControl)
            {
                var control = sender as IFilterControl;

                //var sessionKey = MakeKey(control.Field.Attribute.ID.ToString(), control.GetType());
                var sessionKey = MakeKey(control.AttributeID, control.GetType());
                SessionContent.FilterData[sessionKey] = control.FilterData;

                var typeName = typeof(FilterControl).Name;

#if true

                if (control.ID.Contains(typeName))
                {
                    var itemsForSelectCheckBoxes = (List<string>)control.FilterData;

                    var hasEmptyValue = itemsForSelectCheckBoxes.Where(item => string.IsNullOrEmpty(item));
                    var hasFullValue = itemsForSelectCheckBoxes.Where(item => !string.IsNullOrEmpty(item));

                    var itemsWithQuotes = itemsForSelectCheckBoxes.Select(el => string.Format("'{0}'", (string)el)).Where(el => el != "''");


                    if (itemsForSelectCheckBoxes.Count() > 0)
                    {
                        var expressionForSelectCheckBoxes = string.Empty;

                        var nullValue = "[" + control.AttributeID + "] IS NULL";
                        var itemsWithQuotesWithComma = string.Join(",", itemsWithQuotes.ToArray());

                        if (hasEmptyValue.Count() > 0 && hasFullValue.Count() > 0)
                        {
                            expressionForSelectCheckBoxes = string.Format("[{0}] IN ({1}) OR {2}", control.AttributeID, itemsWithQuotesWithComma, nullValue);
                        }
                        else if (hasEmptyValue.Count() > 0 && hasFullValue.Count() == 0)
                        {
                            expressionForSelectCheckBoxes = nullValue;
                        }
                        else if (hasEmptyValue.Count() == 0 && hasFullValue.Count() > 0)
                        {
                            expressionForSelectCheckBoxes = string.Format("[{0}] IN ({1})", control.AttributeID, itemsWithQuotesWithComma);
                        }

                        SessionContent.ExpressionForSelectCheckBoxes[string.Concat(Template.ID.ToString(), typeName)] = expressionForSelectCheckBoxes;
                    }
                }
#else
                 if (control.ID.Contains(typeName))
                {
                    var itemsForSelectCheckBoxes = (List<string>)control.FilterData;

                    var itemsWithQuotes = itemsForSelectCheckBoxes.Select(el => string.Format("'{0}'", (string)el));


                    
                    if (itemsWithQuotes.Count() > 0)
                    {
                        var itemsWithQuotesWithComma = string.Join(",", itemsWithQuotes.ToArray());

                        var expressionForSelectCheckBoxes = string.Format("[{0}] in ({1})", control.AttributeID, itemsWithQuotesWithComma);

                        SessionContent.ExpressionForSelectCheckBoxes[string.Concat(Template.ID.ToString(), typeName)] = string.Format("[{0}] in ({1})", control.AttributeID, itemsWithQuotesWithComma);
                    }
                }
#endif

                Reset();
                IsInternalDataBinding = false;
                DataBind();
            }
        }

        //ставит галочки в CheckBoxList item
        private void SelectFilterBoxItems(IEnumerable<DataRow> dataRows, string column, FilterControl control)
        {
            var elementsForCheck = dataRows.Select(row => row[column].ToString()).Distinct().ToList<string>();
            if (elementsForCheck is List<string>)
                control.FilterData = elementsForCheck;
        }


        //Заполняет CheckBoxList коллекцией всех строк из DataRow без учета фильтра
        private void FillFilterBox(string column, FilterControl control)
        {
            var rowFilter = GetFilterExpression(true);

            DataView.RowFilter = rowFilter;

            var dataRowsView = DataView.OfType<DataRowView>();

            var rowViewCollection = dataRowsView.Select(row => row[column].ToString()).Distinct();
            control.Items.Clear();
            foreach (var item in rowViewCollection)
                control.Items.Add(item);

        }

        protected string GetFilterExpression(bool noExpressFilterExpression)
        {
            if (EntityFilterID != null)
            {
                DefaultFilterExpression = EntityFilter.GetFilterExpression();

                ResultFilterExpression = CombineAllExpression(noExpressFilterExpression);

                return ResultFilterExpression;
            }
            else
            {
                DefaultFilterExpression = Template.GetFilterExpressionByAttrID();

                ResultFilterExpression = CombineAllExpression(noExpressFilterExpression);

                return ResultFilterExpression;
            }
        }


        protected string CombineAllExpression(bool noExpressFilterExpression)
        {
            var expressFilterExpression = string.Empty;

            if (noExpressFilterExpression == false)
            {
                ExpressFilterExpression = string.Join(" AND ", ListFilterControls.Where(control => !string.IsNullOrEmpty(control.TechPredicate)).Select(control => control.TechPredicate));
            }
            else
            {
                ExpressFilterExpression = "";
            }
            string[] numbersFilters = { NavigatFilterExpression, DefaultFilterExpression, ExpressFilterExpression };


            List<string> listFilters = new List<string>();

            foreach (var item in numbersFilters)
            {
                if (!string.IsNullOrEmpty(item))
                    listFilters.Add(item);
            }

            var resultFilterExpression = string.Join(" AND ", listFilters);

            if (SessionContent.IsInstanceAdded && !string.IsNullOrEmpty(resultFilterExpression))
            {
                resultFilterExpression = string.Format("{0} or (objID = '-1')", resultFilterExpression);
            }

            return resultFilterExpression;
            //return resultFilterExpression;
        }



        //сопосталяет поле шаблона с полем в таблице по attributeID (hash)
        private IDictionary<TemplateField, int> TemplateFieldIndices;

        public bool isColResizable { get; set; }

        public event EventHandler SortingClick;

        protected void Sorting_Click(object sender, System.EventArgs e)
        {
            if (SortingClick != null)
                SortingClick(this, EventArgs.Empty);
        }


        public void Reset(bool resetSessionInfo = true)
        {
            if (resetSessionInfo)
            {
                DefaultFilterExpression = "";
                ExpressFilterExpression = "";
                ResultFilterExpression = "";

                PageIndex = 0;
                SelectedRowIndex = 0;
                LeftPageIndex = 0;
            }
            itemCount = -1;
        }

        void ApplyFiltration()
        {
            Reset();
            IsInternalDataBinding = true;
        }



        public void RejectAllFiltration()
        {
            foreach (var filterControl in ListFilterControls)
                filterControl.RejectFilter();
            SessionContent.FilterData.Clear();
            SessionContent.ExpressionForSelectCheckBoxes.Clear();
            Reset();
            IsInternalDataBinding = false;
            DataBind();


            //if (Template != null)
            //    foreach (var key in UsageContext.Filters.Keys.Where(k => k.StartsWith(Template.ID.ToString())).ToList())
            //        UsageContext.Filters.Remove(key); 
        }
        protected void ResetFiltration()
        {
            //if (Template != null)
            foreach (var key in SessionContent.FilterData.Keys.SkipWhile(k => k.StartsWith(Template.ID.ToString())).ToList())
            {
                if (!key.StartsWith(Template.ID.ToString()))
                {
                    SessionContent.FilterData.Remove(key);
                    SessionContent.ExpressionForSelectCheckBoxes.Remove(key);
                }
            }

            //foreach (var filterControl in ListFilterControls)
            //    filterControl.RejectFilter();
        }



        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        public void DeleteInstance(string entityID, int instanceID)
        {
            var query = string.Format("EXEC [model].[BObjectDelete] @entityID= {0}, @objID = {1}", entityID, instanceID);

            try
            {
                Storage.GetDataTable(query);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            Storage.ClearBusinessContents();
            SessionContent.EntityInstances.Clear();
            Storage.BusinessContentIsChanged = true;

        }

        private void addRemoveInstanceControl_addRemoveInstanceButtonClick(object sender, EventArgs e)
        {
            if (sender is AddRemoveInstanceControl)
            {
                var control = sender as AddRemoveInstanceControl;

                if (control.IsRemoveInstance)
                    SessionContent.IsRemoveInstance = false;
                else
                {
                    SessionContent.IsRemoveInstance = true;

                }

                DataBind();
            }
        }
        public void SaveInstances()
        {
            if (IsEditMode && SessionContent.EntityInstances.Count > 0)
            {
                foreach (var entityInstance in SessionContent.EntityInstances.Values)
                {

                    var o = Page.Session["SystemUser.objID"];

                    var xml = Serialize(Page.Session["SystemUser.objID"].ToString(), entityInstance);
#if true
                    
                    var query = "EXEC [model].[BObjectSave] @xml";
                    Storage.ExecuteNonQueryXML(query, xml);
#else
                    var query = string.Format("EXEC [model].[BObjectSave] @xml = '{0}'", xml);
                    Storage.GetDataTable(query);
#endif
                    //Storage.SaveInstances(query);
                }


                Storage.ClearBusinessContents();

                SessionContent.EntityInstances.Clear();

                Storage.BusinessContentIsChanged = true;


            }
        }
        private void ChangeColumnType()
        {
            var dt = DataView.ToTable();
            DataTable dtCloned = dt.Clone();
            dtCloned.Columns[0].DataType = typeof(string);

            foreach (DataRow row in dt.Rows)
                dtCloned.ImportRow(row);

            DataView = new DataView(dtCloned);
        }

        private void AddRemoveRunTimeInstance()
        {
            if (SessionContent.IsRemoveInstance && SessionContent.IsInstanceAdded == false)
            {
                ChangeColumnType();

                var lastRow = DataView.ToTable().AsEnumerable().Last();

                var newRow = DataView.AddNew();

                newRow["objID"] = "-1";

                var entityInstance = new EntityInstance(lastRow[0].ToString(), Template.Entity.SystemName, false);
                entityInstance.Constraints = Template.Entity.Constraints;
                entityInstance.DisplayReferenceTableControl = true;
                entityInstance.SetRelationColumnsValue();
                entityInstance.SelfColumnsValue = entityInstance.GetInstanceColumnsValue();
                entityInstance.EntityInstanceID = "-1";

                var sessionKey = string.Concat("entityInstance", Template.Entity.SystemName, "_", "-1");
                SessionContent.EntityInstances[sessionKey] = entityInstance;

                SessionContent.IsInstanceAdded = true;
            }
            else if (!SessionContent.IsRemoveInstance && SessionContent.IsInstanceAdded == true)
            {
                var index = DataView.Count;

                DataView.Delete(index - 1);

                var sessionKey = string.Concat("entityInstance", Template.Entity.SystemName, "_", "-1");

                SessionContent.EntityInstances.Remove(sessionKey);

                SessionContent.IsInstanceAdded = false;

            }
        }


        public bool IsEditMode//{get;set;}
        {
            get
            {
                var o = ViewState["IsEnableEditMode"];
                if (o != null)
                    return (bool)o;
                else return false;

            }
            set { ViewState["IsEnableEditMode"] = value; }
            //set { ViewState["IsEnableEditMode"] = true; }
        }



        protected override int CreateChildControls(IEnumerable collection, bool dataBinding)
        {
            bool badSituation = false;


            if (collection != null && collection is DataView)
            {

                var bcont = Storage.BusinessContentIsChanged;
                var dtview = DataView;


                //
                //if (Storage.BusinessContentIsChanged || DataView == null || EntityInstanceID == -1)
                if (!IsEditMode || DataView == null || Storage.BusinessContentIsChanged)
                {

                    DataView = collection as DataView;

                    Storage.BusinessContentIsChanged = false;

                    SessionContent.IsRemoveInstance = false;
                    SessionContent.IsInstanceAdded = false;
                }

                AddRemoveRunTimeInstance();


                if (DataRow == null || !IsInternalDataBinding)
                {
                    ListFilterControls.Clear();
                    HeaderRow = null;
                    Reset(false);
                    DataRow = DataView.Table.Rows.OfType<DataRow>();
                }




                IsInternalDataBinding = false;

                table = new Table { CellSpacing = 0, EnableViewState = false };
#if true
            
#else
             
                table.Width = Unit.Parse("100%");
#endif

                if (!string.IsNullOrEmpty(CssClass))
                {
                    table.CssClass = CssClass;
                }

                if (!badSituation)
                {
                    int tableWidth = 0;

                    table.Rows.Add(CreateHeaderRow(ref tableWidth));

                    table.Width = tableWidth;                                   

                }
                var columnCount = Template.Fields.Count;

                var postBack = Page.IsPostBack;

                TrackPageIndex();

                DataView.RowFilter = GetFilterExpression(false);

                DataView.Sort = GetSortingExpression();

                if (DataView.RowFilter != "")
                {
                    foreach (var control in ListFilterControls)
                    {
                        var typeName = typeof(FilterControl).Name;

                        if (control.ID.Contains(typeName))
                        {
                            foreach (var item in SessionContent.ExpressionForSelectCheckBoxes)
                            {
                                if (item.Key.StartsWith(Template.ID.ToString()))
                                {
                                    var expression = SessionContent.ExpressionForSelectCheckBoxes[item.Key];

                                    var rows = DataView.Table.Select(expression.ToString());

                                    SelectFilterBoxItems(rows, control.AttributeID, (FilterControl)control);

                                    var filter4 = GetFilterExpression(false);

                                    DataView.RowFilter = GetFilterExpression(false);
                                }
                            }
                        }

                    }
                }


                IEnumerable<DataRowView> rowsInPage;

                if (AllowPaging)
                    rowsInPage = DataView.OfType<DataRowView>().Skip(PageSize * PageIndex).Take(PageSize);
                else rowsInPage = DataView.OfType<DataRowView>();

                int index = 0;

                if (DataReady != null)
                    DataReady(this, EventArgs.Empty);


                var InstancesID = new StringBuilder();

                foreach (DataRowView rowInPage in rowsInPage)
                {
                    var rowID = rowInPage.Row["objID"];

                    var type1 = rowID.GetType();

                



                    if (rowID != DBNull.Value)
                    {
                        InstancesID.Append(string.Format("'{0}'{1}",rowID, ","));
                        //InstancesID.Append(string.Concat(rowID, ","));
                    }

                    var row = new TableRow
                    {
                        ClientIDMode = ClientIDMode.Static,
                        ID = rowInPage.Row["objID"].ToString()
                    };
                    var entityID = Template.Entity.ID;

                    if (IsCardControl)
                        row.Attributes.Add("onclick", "switchSelectedRow(" + rowID + "," + entityID + "," + "null" + " )");
                    else
                        row.Attributes.Add("onclick", "switchSelectedRow(" + rowID + "," + entityID + "," + "false" + " )");


                    row.Attributes.Add("class", "AlternativeRow");


                    foreach (var field in Template.Fields.Where(f => f.IsForbidden == false))
                    //foreach (var field in Template.Fields)
                    {
                        if (field.IsVisible)
                        {

                            var cell = new TableCell();

                            WebControl TableCellContorl;
                            #region EditModeImplimetation
                            if (IsEditMode)
                            {
                                var sessionKey = string.Concat("entityInstance", Template.Entity.SystemName, "_", rowInPage["objID"].ToString());
                                var isFoundEntityInstance = SessionContent.EntityInstances.FirstOrDefault(id => id.Key == sessionKey).Key;

                                if (!field.Attribute.IsComputed && field.Format.ID.ToString() == "0" && field.Attribute.SType != "Table" && !field.Attribute.FPath.Contains("/"))
                                {
                                    if (field.Attribute.Type.Description.HasFlag(TypeDescription.Logic))
                                    {
                                        var checkBoxControl = new CheckBoxControl();

                                        if (isFoundEntityInstance != null)
                                            checkBoxControl.EntityInstance = SessionContent.EntityInstances[sessionKey];

                                        TableCellContorl = (CheckBox)checkBoxControl.CreateControl(field, rowInPage, TemplateFieldIndices);
                                    }
                                    else if (field.Attribute.Type.Description.HasFlag(TypeDescription.Date))
                                    {
                                        var dateControl = new DateControl();

                                        if (isFoundEntityInstance != null)
                                            dateControl.EntityInstance = SessionContent.EntityInstances[sessionKey];

                                        TableCellContorl = (TextBox)dateControl.CreateControl(field, rowInPage, TemplateFieldIndices);
                                    }
                                    else if (field.Attribute.Type.Description.HasFlag(TypeDescription.Number) || field.Attribute.Type.Description.HasFlag(TypeDescription.Float))
                                    {
                                        var numberControl = new NumberControl();

                                        if (isFoundEntityInstance != null)
                                            numberControl.EntityInstance = SessionContent.EntityInstances[sessionKey];

                                        TableCellContorl = (TextBox)numberControl.CreateControl(field, rowInPage, TemplateFieldIndices);
                                    }
                                    else
                                    {
                                        var textAreaControl = new TextAreaControl();

                                        if (isFoundEntityInstance != null)
                                            textAreaControl.EntityInstance = SessionContent.EntityInstances[sessionKey];

                                        TableCellContorl = (TextBox)textAreaControl.CreateControl(field, rowInPage, TemplateFieldIndices);
                                    }

                                }
                                else if (field.Attribute.FPath.Contains("/"))
                                {
                                    //найти констраин, по имени поля 
                                    var fPath = field.Attribute.FPath;
                                    var indexOf = fPath.IndexOf("/");
                                    var constrName = fPath.Substring(0, indexOf);
                                    var constraint = Template.Entity.Constraints.First(c => c.ConstraintName == constrName);

                                    if (!constraint.IsIdentified)
                                    {
                                        var buttonControl = new ButtonControl();

                                        if (isFoundEntityInstance != null)
                                            buttonControl.EntityInstance = SessionContent.EntityInstances[sessionKey];

                                        TableCellContorl = (Button)buttonControl.CreateControl(field, rowInPage, TemplateFieldIndices);
                                        buttonControl.ButtonClick += new EventHandler<CreateReferenceTableControlEventArgs>(CreateReferenceTableControl);
                                    }
                                    else
                                    {
                                        var textAreaReadOnlyControl = new TextAreaReadOnlyControl();
                                        TableCellContorl = (TextBox)textAreaReadOnlyControl.CreateControl(field, rowInPage, TemplateFieldIndices);
                                    }
                                }
                                else
                                {
                                    var textAreaReadOnlyControl = new TextAreaReadOnlyControl();
                                    TableCellContorl = (TextBox)textAreaReadOnlyControl.CreateControl(field, rowInPage, TemplateFieldIndices);
                                }
                            }
                            #endregion
                            else
                            {
                                var textAreaReadOnlyControl = new TextAreaReadOnlyControl();
                                TableCellContorl = (TextBox)textAreaReadOnlyControl.CreateControl(field, rowInPage, TemplateFieldIndices);
                            }
                            cell.Controls.Add(TableCellContorl);

                            row.Cells.Add(cell);
                        }
                    }
                    table.Rows.Add(row);
                    index++;
                }
                #region таблица для навигации
                //таблица для навигационного контрола в которой присутствуют только титульные атрибуты
                if (DataView.Table.Rows.Count > 0)
                {
                    var filterExpression = string.Concat("objID in (", InstancesID, ")");

                   


                    if (InstancesID.IsEmpty() || InstancesID.ToString() == "-1,")
                        filterExpression = "";

                    var dv = new DataView(DataView.Table);

                    //dv.RowFilter = "objID = '/174864732726964.178450254734968.3800770055/'";

                    dv.RowFilter = filterExpression;

                    DataTable TableWhitTitlesAttr = dv.ToTable();

                    var noTitlesIDAttr = Template.Entity.Attributes.Where(x => x.AppType != AppType.title && x.AppType != AppType.objid).Select(x => x.ID.ToString()).ToList();

                    foreach (var id in noTitlesIDAttr)
                        TableWhitTitlesAttr.Columns.Remove(id);
                    if (Page.Request.Path.IndexOf("XDynamicCard.aspx") > 0)
                    {
                        Page.Session["TableFromPage"] = null;
                        Page.Session["instanceIDNavigation"] = null;
                    }
                    else
                    {

                        Page.Session["TableFromPage"] = TableWhitTitlesAttr;
                        Page.Session["instanceIDNavigation"] = TableWhitTitlesAttr.Rows[0]["objID"];
                        var instID = TableWhitTitlesAttr.Rows[0]["objID"];
                    }
                }
                #endregion


                #region Кнопка для создания,удаления объектов

                var templateID = Template.ID.ToString();

                if (IsEditMode && PageIndex == PageCount - 1 && templateID.Contains("requireAttributesTemplate"))
                {
                    var newInstanceCell = new TableCell
                    {
                        ColumnSpan = columnCount,
                        HorizontalAlign = HorizontalAlign.Left
                    };
                    var newInstanceRow = new TableRow();

                    var addRemoveInstanceControl = new AddRemoveInstanceControl();

                    addRemoveInstanceControl.IsRemoveInstance = SessionContent.IsRemoveInstance;

                    addRemoveInstanceControl.AddRemoveInstanceButtonClick += new EventHandler(addRemoveInstanceControl_addRemoveInstanceButtonClick);

                    newInstanceCell.Controls.Add(addRemoveInstanceControl);

                    newInstanceRow.Cells.Add(newInstanceCell);

                    this.table.Rows.Add(newInstanceRow);

                }


                #endregion

                CreateAggregationRow();

                if (AllowPaging && PageCount > 1)
                    AddPager(columnCount);

                Controls.Add(table);

                #region отображает выпадающий список из инстансов ссылки для текущего Entity

                if (DisplayReferenceTableControl)
                {

                    var field = (TemplateField)Page.Session["fieldForRefTableControl"];

                    var rowInPage = (DataRowView)Page.Session["rowInPageForRefTableControl"];

                    var constraints = Template.Entity.Constraints;

                    //Получить Instance parent объектa
                    var entityInstanceID = rowInPage["objID"].ToString();


                    var sessionKey = string.Concat("entityInstance", Template.Entity.SystemName, "_", entityInstanceID);

                    EntityInstance entityInstance;

                    var entityInstanceIsFound = SessionContent.EntityInstances.FirstOrDefault(key => key.Key == sessionKey).Key;

                    if (entityInstanceIsFound == null)
                    {
                        entityInstance = new EntityInstance(entityInstanceID, Template.Entity.SystemName, true);
                        entityInstance.Constraints = constraints;
                        entityInstance.SetRelationColumnsValue();
                    }
                    //если instance был создан вне ReferenceTableControl и не имеет constrain & RelationColumnsValue
                    else if (entityInstanceIsFound != null && SessionContent.EntityInstances[sessionKey].DisplayReferenceTableControl == false)
                    {
                        entityInstance = new EntityInstance(entityInstanceID, Template.Entity.SystemName, true);
                        entityInstance.Constraints = constraints;
                        entityInstance.SetRelationColumnsValue();
                        entityInstance.SelfColumnsValue = SessionContent.EntityInstances[sessionKey].SelfColumnsValue;
                    }
                    else
                        entityInstance = SessionContent.EntityInstances[sessionKey];



                    //найти констраин, по имени поля 
                    var fPath = field.Attribute.FPath;
                    var indexOf = fPath.IndexOf("/");
                    var constrName = fPath.Substring(0, indexOf);
                    var constraint = Template.Entity.Constraints.First(c => c.ConstraintName == constrName);

                    //Получить ссылочную таблцу
                    DependencyRelations dependencyRelation = new DependencyRelations(constraint, entityInstance);

                    bool isCreate = false;

                    if (entityInstance.EntityInstanceID == "-1")
                        isCreate = true;

                    DataTable ReferenceTableDataSource = dependencyRelation.SetRelationColumnsValue_GetReferenceTable(true, isCreate);

                    var ReferenceTableControl = new ReferenceTableControl(Template.Entity, field, rowInPage);

                    ReferenceTableControl.DataSource = ReferenceTableDataSource;

                    ReferenceTableControl.SelectedIndexChanged += new EventHandler(ReferenceTableControl_SelectedIndexChanged);

                    ReferenceTableControl.CloseButtonClick += new EventHandler(ReferenceTableControl_CloseButtonClick);

                    Controls.Add(ReferenceTableControl);

                    //выбрать объект из ссылочной таблицы 
                    if (ReferenceTableControl.EntityInstanceID != "")
                    {
                        dependencyRelation.NewEntityInstanceID = ReferenceTableControl.EntityInstanceID;
                        dependencyRelation.RelationTableTitleAttributes = ReferenceTableControl.TableTitleAttributes;
                        dependencyRelation.SetRelationColumnsValue_GetReferenceTable(false);

                        SessionContent.EntityInstances[sessionKey] = entityInstance;

                        //закрыть конрол
                        Page.Session["fieldForRefTableControl"] = null;
                        Page.Session["rowInPageForRefTableControl"] = null;
                        DisplayReferenceTableControl = false;
                        ReferenceTableControl.EntityInstanceID = "";

                        DataBind();
                    }
                }
                #endregion
            }

            return 0;
        }




        void CreateReferenceTableControl(object sendet, CreateReferenceTableControlEventArgs e)
        {
            DisplayReferenceTableControl = true;
            Page.Session["fieldForRefTableControl"] = e.Field;
            Page.Session["rowInPageForRefTableControl"] = e.RowInPage;

            DataBind();
        }

        void ReferenceTableControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        void ReferenceTableControl_CloseButtonClick(object sender, EventArgs e)
        {
            Page.Session["fieldForRefTableControl"] = null;
            Page.Session["rowInPageForRefTableControl"] = null;
            DisplayReferenceTableControl = false;

            DataBind();
        }

        bool DisplayReferenceTableControl
        {
            get
            {
                return Page.Session["DisplayReferenceTableControl"] == null ? false : (bool)Page.Session["DisplayReferenceTableControl"];
            }
            set
            {
                Page.Session["DisplayReferenceTableControl"] = value;
            }
        }



        public SessionContent SessionContent
        {
            get
            {
                var contexts = Page.Session["contexts"] as Dictionary<string, SessionContent>;

                if (contexts == null)
                    Page.Session["contexts"] = contexts = new Dictionary<string, SessionContent>();

                var sBuilder = new System.Text.StringBuilder(Page.Request.CurrentExecutionFilePath);

                var queryString = Page.Request.QueryString;

                foreach (var queryKey in queryString.Keys)
                    sBuilder.Append(queryKey.ToString()).Append(queryString[queryKey.ToString()].ToString());

                var key = sBuilder.ToString();

                SessionContent sessionContent;

                if (!contexts.TryGetValue(key, out sessionContent))
                {
                    sessionContent = new SessionContent();
                    contexts.Add(key, sessionContent);
                }

                return sessionContent;
            }
        }


        public string resultFilter { get; set; }





    }

}

