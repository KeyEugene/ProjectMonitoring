using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;

namespace Teleform.Reporting.Web
{
    public static class CellControlNewAttributes
    {

        public static string GetDateTimeValue(string text, TemplateField field)
        {
            if (!string.IsNullOrEmpty(text))
            {
                DateTime result;
#if trueWWW

                if (DateTime.TryParseExact(text, "d/mm/yyyy hh:mm:ss tt", field.Format.Provider, DateTimeStyles.None, out result))
                {
                    DateTime theDateTime = DateTime.ParseExact(text, "dd/mm/yyyy h:mm:ss tt", field.Format.Provider);

                    text = theDateTime.ToString("g", new CultureInfo("ru-RU"));
                }

                
#else
                if (DateTime.TryParseExact(text, "dd.MM.yyyy H:mm:ss", field.Format.Provider, DateTimeStyles.None, out result))
                {
                    DateTime theDateTime = DateTime.ParseExact(text, "dd.MM.yyyy H:mm:ss", field.Format.Provider);                   

                    text = theDateTime.ToString("yyyy-MM-dd");
                }
#endif
                
            }
            return text;
        }

        public static string GetNumberValue(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                double num;
                bool res = double.TryParse(text, out num);
                if (res == true)
                    text = num.ToString(CultureInfo.CreateSpecificCulture("en-US"));
            }
            return text;
        }

        public static string GetNewText(EntityInstance entityInstance, TemplateField field, DataRowView rowInPage, IDictionary<TemplateField, int> fieldIndices)
        {
            string textNew = null;

            if (entityInstance.EntityInstanceID == rowInPage["objID"].ToString() && entityInstance.SelfColumnsValue != null)
            {   
                    foreach (var colValue in entityInstance.SelfColumnsValue)
                    {
                        if (colValue.Key == field.Attribute.FPath)
                        {
                            //if (!string.IsNullOrEmpty(colValue.Value))
                                textNew = string.Concat(colValue.Value);
                            //else textNew = string.Format(field.Format.Provider, field.Format.FormatString, rowInPage.Row.ItemArray[fieldIndices[field]]);
                        }
                    }
                }
            
            return textNew;
        }
    }
}
