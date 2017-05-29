using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting
{
    [Serializable()]
    public class AggregateFunction : UniquelyDeterminedObject
    {
        /// <summary>
        /// Возвращает лексему текущего оператора.
        /// </summary>
        public string Lexem { get; private set; }

        public AggregateFunction(object id, string name, string lexem): base(id, name)
        {
            if (string.IsNullOrWhiteSpace(lexem))
                throw new ArgumentException("lexem", Message.Get("Common.NullArgument", this, "lexem"));

            Lexem = lexem;
        }
    }
}
