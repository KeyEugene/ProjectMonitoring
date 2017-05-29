using Teleform.SqlServer.Formatting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlTypes;
using System;
using System.Data.SqlClient;

namespace Teleform.SqlServer.Formatting.Tests
{
   
    partial class FormatValueUnitTest
    {
        [TestMethod]
        [Description("Проверка передачи Null значения в аргументе для формата.")]
        [Owner("Виктор Гатаулин")]
        [TestCategory("FormatArgument")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FormatArgument_NullValue()
        {
            FormatProvider.FormatExecutor(null,null, 111);
        }

        [TestMethod]
        [Description("Проверка передачи DbNull значения в аргументе для формата.")]
        [Owner("Виктор Гатаулин")]
        [TestCategory("FormatArgument")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FormatArgument_DBNullValue()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        [Description("Проверка передачи недопустимого формата.")]
        [Owner("Виктор Гатаулин")]
        [TestCategory("FormatArgument")]
        [ExpectedException(typeof(ArgumentException))]
        public void FormatArgument_InvalidValue()
        {
            FormatProvider.FormatExecutor(null,"x", 111);            
        }
    }
}
