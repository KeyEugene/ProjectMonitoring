

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting
{
    [Serializable()]
    public sealed class Operator : UniquelyDeterminedObject
    {
        
        /// <summary>
        /// Возвращает набор операторов для null-значения.
        /// </summary>
        public static IEnumerable<Operator> NullOperators
        { get; private set; }

        public int Order { get; set; }

        static Operator()
        {
            Operator emptyOperator = new Operator(-1, "пусто", "IS NULL"),
               nonEmptyOperator = new Operator(-2, "непусто", "IS NOT NULL");

            emptyOperator.NullValue = nonEmptyOperator.NullValue = true;

            NullOperators = new[] { emptyOperator, nonEmptyOperator };
        }

        /// <summary>
        /// Возвращает лексему текущего оператора.
        /// </summary>
        public string Lexem { get; private set; }

        /// <summary>
        /// Возвращает значение, указывающее, что одним из аргументов текущего
        /// оператора является null-значение.
        /// </summary>
        public bool NullValue { get; private set; }

        public Operator(object id, string name, string lexem, int order = 255)
            : base(id, name)
        {
#warning NULL
            if (string.IsNullOrWhiteSpace(lexem))
                throw new ArgumentException("lexem", Message.Get("Common.NullArgument", this, "lexem"));

            Lexem = lexem;
            Order = order;
        }
    }
}
