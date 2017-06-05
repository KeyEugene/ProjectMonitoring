using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI;

using System.Globalization;

namespace Teleform.Reporting.Web
{
    public interface ICellControl
    {
        WebControl CreateControl(TemplateField field, DataRowView rowInPage, IDictionary<TemplateField, int> fieldIndices);
    }

    class TextAreaReadOnlyControl : ICellControl
    {
        public WebControl CreateControl(TemplateField field, DataRowView rowInPage, IDictionary<TemplateField, int> fieldIndices)
        {
            TextBox TextBox;
            TextBox = new TextBox
            {
                ID = field.Attribute.FPath + "TableCellContorl" + field.Order.ToString() + rowInPage.Row.ItemArray[0],
                ReadOnly = true,
                TextMode = TextBoxMode.MultiLine,
                //Height = Unit.Pixel(15),
                BorderWidth = Unit.Pixel(0),
                Text = string.Format(field.Format.Provider, field.Format.FormatString, rowInPage.Row.ItemArray[fieldIndices[field]])
            };
            //TextBox.Columns = 10;
            var text = string.Format(field.Format.Provider, field.Format.FormatString, rowInPage.Row.ItemArray[fieldIndices[field]]);
            TextBox.Attributes.Add("attributeName", field.Attribute.FPath);
            TextBox.Attributes.Add("class", "AlternativeRow");
            return TextBox;
        }
    }

    class CheckBoxControl : ICellControl
    {
        public EntityInstance EntityInstance { get; set; }

        public WebControl CreateControl(TemplateField field, DataRowView rowInPage, IDictionary<TemplateField, int> fieldIndices)
        {
            var isChecked = rowInPage.Row.ItemArray[fieldIndices[field]];
            if (isChecked.Equals(System.DBNull.Value))
                isChecked = false;

            if (EntityInstance != null)
            {
                if (EntityInstance.EntityInstanceID == rowInPage["objID"].ToString() && EntityInstance.SelfColumnsValue != null)
                {
                    foreach (var colValue in EntityInstance.SelfColumnsValue)
                    {
                        if (colValue.Key == field.Attribute.FPath)
                        {
                            if (string.IsNullOrEmpty(colValue.Value))
                                isChecked = false;
                            else
                            {
                                isChecked = Convert.ToBoolean(colValue.Value);
                                if (isChecked.Equals(System.DBNull.Value))
                                    isChecked = false;
                            }
                        }
                    }
                }
            }

            CheckBox CheckBox;
            CheckBox = new CheckBox
            {
                ID = field.Attribute.FPath + "TableCellContorl" + field.Order.ToString() + rowInPage.Row.ItemArray[0],
                Checked = (bool)isChecked,
            };
            CheckBox.InputAttributes.Add("onclick", "getTableCellControlsData()");
            CheckBox.InputAttributes.Add("attributeName", field.Attribute.FPath);
            CheckBox.InputAttributes.Add("class", "AlternativeRow");

            return CheckBox;
        }
    }


    class DateControl : ICellControl
    {
        public EntityInstance EntityInstance { get; set; }

        public WebControl CreateControl(TemplateField field, DataRowView rowInPage, IDictionary<TemplateField, int> fieldIndices)
        {
            

#if trueWWW

            DateTime data = (DateTime)rowInPage.Row.ItemArray[fieldIndices[field]];
            var text = data.ToString(new CultureInfo("ru-RU"));
#else
            var text = string.Format(field.Format.Provider, field.Format.FormatString, rowInPage.Row.ItemArray[fieldIndices[field]]);
            text = CellControlNewAttributes.GetDateTimeValue(text, field);

#endif

            if (EntityInstance != null)
            {
                if (EntityInstance.EntityInstanceID == rowInPage["objID"].ToString() && EntityInstance.SelfColumnsValue != null)
                {
                    foreach (var colValue in EntityInstance.SelfColumnsValue)
                    {
                        if (colValue.Key == field.Attribute.FPath)
                        {
                            //text = colValue.Value.ToString("g", new CultureInfo("ru-RU"));
                            text = CellControlNewAttributes.GetDateTimeValue(colValue.Value, field);
                        }
                    }
                }
            }

            TextBox TextBox;
            TextBox = new TextBox
            {
                ID = field.Attribute.FPath + "TableCellDateTextBox" + field.Order.ToString() + rowInPage.Row.ItemArray[0],
                ReadOnly = false,
                //Height = Unit.Pixel(15),
                BorderWidth = Unit.Pixel(0),
                Text = text
            };
            TextBox.Attributes.Add("onchange", "getTableCellControlsData()");
            TextBox.Attributes.Add("attributeName", field.Attribute.FPath);
            TextBox.Attributes.Add("class", "AlternativeRow");
            TextBox.Attributes.Add("type", "date");
            return TextBox;
        }
    }
    
    class TextAreaControl : ICellControl
    {
        public EntityInstance EntityInstance { get; set; }

        public WebControl CreateControl(TemplateField field, DataRowView rowInPage, IDictionary<TemplateField, int> fieldIndices)
        {
            var text = string.Format(field.Format.Provider, field.Format.FormatString, rowInPage.Row.ItemArray[fieldIndices[field]]);

            if (EntityInstance != null)
            {
                if (EntityInstance.EntityInstanceID == rowInPage["objID"].ToString() && EntityInstance.SelfColumnsValue != null)
                {
                    foreach (var colValue in EntityInstance.SelfColumnsValue)
                    {
                        if (colValue.Key == field.Attribute.FPath)
                            text = colValue.Value;

                    }
                }
            }

            TextBox TextBox;
            TextBox = new TextBox
            {
                ID = field.Attribute.FPath + "TableCellContorl" + field.Order.ToString() + rowInPage.Row.ItemArray[0],
                ReadOnly = false,
                TextMode = TextBoxMode.MultiLine,
                //Height = Unit.Pixel(15),
                BorderWidth = Unit.Pixel(0),
                Text = text
            };
            TextBox.Attributes.Add("onkeyup", "getTableCellControlsData()");
            TextBox.Attributes.Add("attributeName", field.Attribute.FPath);
            TextBox.Attributes.Add("class", "AlternativeRow");
            return TextBox;
        }
    }

    class NumberControl : ICellControl
    {
        public EntityInstance EntityInstance { get; set; }

        public WebControl CreateControl(TemplateField field, DataRowView rowInPage, IDictionary<TemplateField, int> fieldIndices)
        {
            var text = string.Format(field.Format.Provider, field.Format.FormatString, rowInPage.Row.ItemArray[fieldIndices[field]]);

            text = CellControlNewAttributes.GetNumberValue(text);

            if (EntityInstance != null)
            {
                if (EntityInstance.EntityInstanceID == rowInPage["objID"].ToString() && EntityInstance.SelfColumnsValue != null)
                {
                    foreach (var colValue in EntityInstance.SelfColumnsValue)
                    {
                        if (colValue.Key == field.Attribute.FPath)
                            text = CellControlNewAttributes.GetNumberValue(colValue.Value);

                    }
                }
            }

            TextBox TextBox;
            TextBox = new TextBox
            {
                ID = field.Attribute.FPath + "TableCellContorl" + field.Order.ToString() + rowInPage.Row.ItemArray[0],
                ReadOnly = false,
                //Height = Unit.Pixel(15),
                BorderWidth = Unit.Pixel(0),
                Text = text
            };
            TextBox.Attributes.Add("onkeyup", "getTableCellControlsData()");
            TextBox.Attributes.Add("attributeName", field.Attribute.FPath);
            TextBox.Attributes.Add("class", "AlternativeRow");
            TextBox.Attributes.Add("type", "number");
            return TextBox;

        }

    }
    class ButtonControl : ICellControl
    {
        public EntityInstance EntityInstance { get; set; }

        public WebControl CreateControl(TemplateField field, DataRowView rowInPage, IDictionary<TemplateField, int> fieldIndices)
        {
            string text = string.Format(field.Format.Provider, field.Format.FormatString, rowInPage.Row.ItemArray[fieldIndices[field]]);

            if (EntityInstance != null)
            {
                if (EntityInstance.EntityInstanceID == rowInPage["objID"].ToString() && EntityInstance.RelationColumnsValue != null)
                {
                    //найти констраин, по имени поля 
                    var fPath = field.Attribute.FPath;
                    var indexOf = fPath.IndexOf("/");
                    var constrName = fPath.Substring(0, indexOf);
                    //Teleform.Reporting.constraint.Constraint constraint = EntityInstance.Constraints.First(c => c.ConstraintName == constrName);

                    foreach (var colValue in EntityInstance.RelationColumnsValue)
                    {
                        if (colValue.ConstraintName == constrName)
                        {
                            if (!string.IsNullOrEmpty(colValue.TitleAttribute))
                                text = string.Concat(colValue.TitleAttribute);
                            else if (!string.IsNullOrEmpty(colValue.Value.ToString()))
                                text = colValue.Value.ToString();
                        }
                    }
                }
            }


            Button Button;
            Button = new Button
            {
                ID = field.Attribute.FPath + "TableCellContorl" + field.Order.ToString() + rowInPage.Row.ItemArray[0],
                Text = text
            };
            Button.Click += new EventHandler(Button_Click);
            Button.Attributes.Add("attributeName", field.Name);
            Button.Attributes.Add("class", "AlternativeRow");
            _field = field;
            _rowInPage = rowInPage;
            return Button;
        }

        private TemplateField _field;

        private DataRowView _rowInPage;

        public event EventHandler<CreateReferenceTableControlEventArgs> ButtonClick;

        private void Button_Click(object sender, EventArgs e)
        {
            if (ButtonClick != null)
                ButtonClick(this, new CreateReferenceTableControlEventArgs(_field, _rowInPage));
        }
    }


    public class CreateReferenceTableControlEventArgs : EventArgs
    {
        public TemplateField Field { get; private set; }

        public DataRowView RowInPage { get; private set; }

        public CreateReferenceTableControlEventArgs(TemplateField field, DataRowView rowInPage)
        {
            this.Field = field;
            this.RowInPage = rowInPage;
        }
    }



}
