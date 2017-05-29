#define Viktor

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Teleform.Reporting.constraint;

namespace Teleform.Reporting.DynamicCard
{
    public partial class DynamicCardControl : CompositeControl
    {

        public void SetFieldsTaboo(int userID, Card card)
        {
            if (userID == null)
                userID = 0;

            var deniedEntity = StorageUserObgects.Select<UserEntityPermission>(userID, userID).getReadDeniedEntities().AsEnumerable().Select(x => x["entity"].ToString()).ToList<string>();
            
            foreach (var field in card.Fields)
            {
                var entityName = deniedEntity.FirstOrDefault(e => e == field.NativeEntityName);
                if (entityName != null)
                    field.IsForbidden = true;
                else
                    field.IsForbidden = false;

            }
        }


        public void SetFieldsSize(string ColResizableDynamicCardBox)
        {
            if (!string.IsNullOrEmpty(ColResizableDynamicCardBox))
            {
                Dictionary<string, string> fieldsSizeDict = ColResizableDynamicCardBox.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(part => part.Split('='))
                    .ToDictionary(split => split[0], split => split[1]);


                this.FieldsSizeDict = fieldsSizeDict;

            }
        }

        public Dictionary<string, string> FieldsSizeDict { get; set; }

        private Dictionary<string, WebControl> WebControls;

        private Table Table;

        public string SessionKey
        { get; set; }

        public string EntityName { get; set; }


        public Dictionary<int, Card> Cards { get; set; }

        public DatabaseReader DataBaseReader { get; set; }

        public Schema Schema { get; set; }

        public bool AllowManagement { get; set; }

        public DynamicCardControl()
        {
            WebControls = new Dictionary<string, WebControl>();
            AllowManagement = true;
        }

        public string ClientTableID { get; set; }

        private void CreateButtons()
        {
            var row = new TableRow();
            var cell = new TableCell { ColumnSpan = 2 };

            row.Cells.Add(cell);
            Table.Rows.Add(row);

            if (AllowManagement) switch (CurrentMode)
                {
                    case Mode.ReadOnly:
                        var button = new Button { ID = "EditButton", Text = "Редактировать", CommandArgument = "Edit" };
                        button.Click += ChangeMode;
                        cell.Controls.Add(button);


                        var readOnlyBackwardButton = new Button { ID = "BackwardButton", Text = "Назад", CommandArgument = "ReadOnly" };
                        readOnlyBackwardButton.Click += BackwardButton_Click;
                        cell.Controls.Add(readOnlyBackwardButton);


                        break;
                    case Mode.Edit:
                        var acceptButton = new Button { ID = "AcceptButton", Text = "Сохранить", CommandArgument = "ReadOnly" };
                        acceptButton.Click += AcceptChanges;
                        acceptButton.Click += ChangeMode;

                        var cancelButton = new Button { ID = "CancelButton", Text = "Отмена", CommandArgument = "ReadOnly" };
                        cancelButton.Click += CancelButton_Click;
                        cancelButton.Click += ChangeMode;


                        cell.Controls.Add(acceptButton);
                        cell.Controls.Add(cancelButton);


                        var editBackwardButton = new Button { ID = "BackwardButton", Text = "Назад", CommandArgument = "ReadOnly" };
                        editBackwardButton.Click += BackwardButton_Click;
                        cell.Controls.Add(editBackwardButton);


                        break;
                    case Mode.Create:

                        var createButton = new Button { ID = "CreateButton", Text = "Создать", CommandArgument = "ReadOnly" };
                        createButton.Click += AcceptChanges;

                        var clearButton = new Button { ID = "ClearButton", Text = "Очистить все поля" };
                        clearButton.Click += new EventHandler(ClearButton_Click);


                        cell.Controls.Add(clearButton);
                        cell.Controls.Add(createButton);

                        var createBackwardButton = new Button { ID = "BackwardButton", Text = "Назад", CommandArgument = "ReadOnly" };
                        createBackwardButton.Click += BackwardButton_Click;
                        cell.Controls.Add(createBackwardButton);

                        break;
                }
        }



        private void SaveState(Card card, bool validation = false)
        {
            foreach (var field in card.Fields.Where(f => f is CardSelfField).OfType<CardSelfField>().Where(f => !f.IsReadOnly(CurrentMode)).Where(f => !f.IsForbidden))
            {
                var control = WebControls[field.SystemName];

                if (control is TextBox)
                {
                    var value = (control as TextBox).Text;

                    if (validation)
                    {
                        var type = GetAttrTypeFromFieldID(field.ID, card);

                        if (field.TypeCode == CardSelfField.Type.Object)
                        {
                            if (type.Length != null && value.Length > type.MaxValue)
                                throw new InvalidOperationException(string.Format
                                (
                                    "Поле '{0}' не может содержать более {1} символов.",
                                    field.Name,
                                    type.Length
                                ));
                        }
                    }

                    field.Value = value;
                }
                else if (control is CheckBox)
                    field.Value = (control as CheckBox).Checked;
                else if (control is FileUpload)
                {
                    var u = control as FileUpload;

                    if (u.HasFile)
                        field.Value = new File
                        {
                            Content = u.FileBytes,
                            FileName = u.FileName,
                            MimeType = System.IO.Path.GetExtension(u.FileName),
                            IsModified = true
                        };
                }
            }

        }


        private void FillDataCell(IField field, TableCell cell)
        {
            switch (CurrentMode)
            {
                case Mode.Create:
                case Mode.Edit:
                    FillDataCellWhenEditMode(field, cell);
                    break;
                case Mode.ReadOnly:
                    FillDataCellWhenReadOnlyMode(field, cell);
                    break;
            }
        }


        private int lastInsertedInstanceID { get; set; }

        public int LastInsertedInstanceID
        {
            get
            {
                if (lastInsertedInstanceID == 0)
                    return -1;
                else
                    return lastInsertedInstanceID;
            }

            set
            {
                lastInsertedInstanceID = value;
            }
        }


        protected override void CreateChildControls()
        {
            //AllowManagement = true;

            WebControls.Clear();
            if (Cards != null)
            {

                Table = new Table { ID = ClientTableID };
                if (GeneralStyle != null)
                    Table.ApplyStyle(GeneralStyle);
                Table.ClientIDMode = ClientIDMode.Static;
                Controls.Add(Table);
                if (!string.IsNullOrEmpty(CssClass))
                    Table.CssClass = CssClass;
                Table.Rows.Add(CreateHeaderRow());



                foreach (var card in Cards)
                {
                    foreach (var field in card.Value.Fields.Where(f => f.IsForbidden == false))
                    {
                        CreateRow(field);

                        //if (field is CardSelfField)
                        //{
                        //    var selfField = field as CardSelfField;
                        //    if (selfField.Visible)
                        //        CreateRow(selfField);
                        //}
                        //else
                        //{                            
                        //        CreateRow(field);                               
                        //}
                    }

                }


                CreateButtons();

                Controls.Add(CreateRelatedList());
            }

        }

        private void CreateRow(IField field)
        {

            if (CurrentMode == Mode.ReadOnly || !(field is CardListRelationField))
            {
                if (field is CardListRelationField)
                {
                    var listRelation = field as CardListRelationField;

                    if (listRelation.Card.IsAncestor && listRelation.Entity.AncestorID == Convert.ToInt32(listRelation.Card.Entity.ID))
                        return;
                }

                if (field is CardRelationField)
                {
                    var f = field as CardRelationField;

                    if (f.SystemName == "Person_erp_Executive")
                    {

                    }


                    Card currentCard;
                    Cards.TryGetValue(Convert.ToInt32(f.Card.Entity.ID), out currentCard);

                    if (currentCard.EntityInstance == null)
                        DataBaseReader.FillEntityInstance(currentCard, "");

                    var constraint = currentCard.EntityInstance.Constraints.First(c => c.ConstraintName == field.SystemName);

                    if (constraint.IsIdentified && currentCard.Entity.AncestorID != -1)
                    {
                        return;
                    }
                }



                var row = new TableRow();

                var headerCell = new TableHeaderCell();

                headerCell.Controls.Add(new Literal { Text = field.Name });


                if (CurrentMode != Mode.ReadOnly && !field.IsNullable)
                    headerCell.Controls.Add(new Literal { Text = "<span style='color:red'>*</span>" });


                row.Cells.Add(headerCell);

                var dataCell = new TableCell();

                row.Cells.Add(dataCell);

                FillDataCell(field, dataCell);

                Table.Rows.Add(row);
            }
        }

        private TableRow CreateHeaderRow()
        {
            var row = new TableRow();
            var cell1 = new TableHeaderCell { ID = "CardCell1" };
            var cell2 = new TableHeaderCell { ID = "CardCell2" };

            cell1.Attributes.Add("ColumnName", "CardCell1");
            cell2.Attributes.Add("ColumnName", "CardCell2");

            cell1.Attributes.Add("class", "CardCells");
            cell2.Attributes.Add("class", "CardCells");

            row.Cells.Add(cell1);
            row.Cells.Add(cell2);


            foreach (var item in row.Cells)
            {
                if (this.FieldsSizeDict != null && this.FieldsSizeDict.Count > 0)
                {
                    var cell = item as TableHeaderCell;
                    string s = this.FieldsSizeDict.First(k => k.Key == cell.ID).Value;

                    var cellWidth = s;

                    if (s.Contains('.'))
                        cellWidth = s.Remove(s.IndexOf('.'), s.Length - s.IndexOf('.'));

                    cell.Width = Unit.Parse(cellWidth);
                }
            }

            return row;
        }


    }
}
