using System;
using System.Collections.Generic;
using System.Linq;

namespace Teleform.Reporting
{
    public sealed class Property : IProperty
    {
        public object Value { get; private set; }

        public TemplateField Field { get; private set; }

        public Property(TemplateField field, object value)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            Field = field;

            if (!string.IsNullOrEmpty(value.ToString()))
                this.Value = value;
        }
    }
}