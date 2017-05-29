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
        [Description("Проверка работоспособности по таблице [Test].[Formatting].[MoneyInWords.View].")]
        [Owner("Виктор Гатаулин, Василий Гончаров")]
        [TestCategory("MoneyInWords")]
        [DataSource(
            "System.Data.SqlClient", @"Initial Catalog = Test; Password = 345; User ID = sa; Data Source = STEND\SQLEXPRESS",
            "[Formatting].[MoneyInWords.View]",
            DataAccessMethod.Sequential)]
        public void MoneyInWords_FromTable_Int32()
        {
            var value = Convert.ToInt32(TestContext.DataRow["input"]);
            var expected = TestContext.DataRow["expected"].ToString();

            var actual = FormatProvider.FormatExecutor("Teleform.SqlServer.Formatting.MoneyFormat","{0:W}", value);

            Assert.AreEqual(expected, actual, "Составитель - {0}.", TestContext.DataRow["author"]);
        }

        [TestMethod]
        [Description("Проверка работоспособности по таблице [Test].[Formatting].[MoneyInWords.View].")]
        [Owner("Виктор Гатаулин, Василий Гончаров")]
        [TestCategory("MoneyInWords")]
        [DataSource(
            "System.Data.SqlClient", @"Initial Catalog = Test; Password = 345; User ID = sa; Data Source = STEND\SQLEXPRESS",
            "[Formatting].[MoneyInWords.View]",
            DataAccessMethod.Sequential)]
        public void MoneyInWords_FromTable_SqlInt32()
        {
            var value = new SqlInt32(Convert.ToInt32(TestContext.DataRow["input"]));
            var expected = TestContext.DataRow["expected"].ToString();

            var actual = FormatProvider.FormatExecutor("Teleform.SqlServer.Formatting.MoneyFormat", "{0:W}", value);

            Assert.AreEqual(expected, actual, "Составитель - {0}.", TestContext.DataRow["author"]);           
            
        }

        [TestMethod]
        [Description("Проверка передачи отрицательных сумм.")]
        [Owner("Виктор Гатаулин")]
        [TestCategory("MoneyInWords")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MoneyInWords_NegativeValue()
        {
            FormatProvider.FormatExecutor("Teleform.SqlServer.Formatting.MoneyFormat", "{0:W}", -2);
        }

        [TestMethod]
        [Description("Проверка передачи null значения.")]
        [Owner("Виктор Гатаулин")]
        [TestCategory("MoneyInWords")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MoneyInWords_NullValue()
        {
            FormatProvider.FormatExecutor("Teleform.SqlServer.Formatting.MoneyFormat", "{0:W}", null);
        }

        [TestMethod]
        [Description("Проверка передачи DBNull значения.")]
        [Owner("Виктор Гатаулин")]
        [TestCategory("MoneyInWords")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MoneyInWords_DBNullValue()
        {
            FormatProvider.FormatExecutor("Teleform.SqlServer.Formatting.MoneyFormat", "{0:W}", DBNull.Value);
        }

        [TestMethod]
        [Description("Проверка передачи верхней граници диапазона допустимых значений.")]
        [Owner("Виктор Гатаулин")]
        [TestCategory("MoneyInWords")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MoneyInWords_OutOfRangeValue()
        {
            FormatProvider.FormatExecutor("Teleform.SqlServer.Formatting.MoneyFormat", "{0:W}", 1500000001);
        }

        [TestMethod]
        [Description("Проверка передачи 0 значения")]
        [Owner("Алексей Крайнов")]
        [TestCategory("MoneyInWords")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MoneyInWords_ZiroValue() {
            Assert.Inconclusive();
        }

    }
}
