using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelDialog
{
    public class ListItem
    {
        public readonly string Value;
        public readonly string Text;

        public ListItem(string value, string text)
        {
            this.Value = value;
            this.Text = text;
        }
    }
}
