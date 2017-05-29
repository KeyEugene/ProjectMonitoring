

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Teleform.Reporting;


namespace System.Web.UI.WebControls
{
    public static class TextBoxExtensions
    {

        public static void ApplyType(this TextBox box, Teleform.Reporting.Type type)
        {
            if (type.Description.HasFlag(TypeDescription.Number))
            {
                box.Attributes.Add("type", "number");
                box.Attributes.Add("min", type.MinValue.ToString());
                box.Attributes.Add("max", type.MaxValue.ToString());
            }
            if (type.Description.HasFlag(TypeDescription.Float))
            {
                box.Attributes.Add("type", "number");
                box.Attributes.Add("min", type.MinValue.ToString());
                box.Attributes.Add("max", type.MaxValue.ToString());
                box.Attributes.Add("step", "any");
            }
            else if (type.Description.HasFlag(TypeDescription.String) && !string.IsNullOrWhiteSpace(type.Pattern))
            {
                box.Attributes.Add("pattern", type.Pattern);
                box.Attributes.Add("title", type.PatternDescription);
            }
            else if (type.Description.HasFlag(TypeDescription.Date))
                box.Attributes.Add("type", "date");
            //else if (type.Description.HasFlag(TypeDescription.Logic))
            //    box.Attributes.Add("type", "checkbox");
        }
    }
}
