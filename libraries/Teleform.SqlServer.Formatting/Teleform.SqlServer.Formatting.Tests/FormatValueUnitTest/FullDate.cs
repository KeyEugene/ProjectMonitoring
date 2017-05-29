using Teleform.SqlServer.Formatting; 
using System.Data.SqlTypes;
using System;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Teleform.SqlServer.Formatting.Tests
{
        partial class FormatValueUnitTest
    {  
        [TestMethod]
        [Description("Проверка дат")]
        [Owner("Виктор Гатаулин")]
        [TestCategory("FullDate")]
        public void FullDate_UsualDate()
        {
            var value = new DateTime(1,12,17);
            var expected = "17 декабря 1 года";

            var actual = FormatProvider.FormatExecutor(null, "{0:dd MMMMM yyyy года}", value);

            Assert.AreEqual(expected, actual, "Составитель - {0}.", "Виктор Гатаулин");            
        }

        [TestMethod]
        [Description("Проверка передачи null значения в дату.")]
        [Owner("Виктор Гатаулин")]
        [TestCategory("FullDate")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FullDate_NullValue()
        {
            FormatProvider.FormatExecutor(null, "{0:dd MMMMM yyyy года}", null);
        }

        [TestMethod]
        [Description("Проверка передачи DbNull значения в дату")]
        [Owner("Виктор Гатаулин")]
        [TestCategory("FullDate")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FullDate_DbNullValue()
        {
            FormatProvider.FormatExecutor(null,"{0:dd MMMMM yyyy года}", DBNull.Value);
        }

        [TestMethod]
        [Description("Проверка передачи значения даты вне допустимого диапазона")]
        [Owner("Виктор Гатаулин")]
        [TestCategory("FullDate")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FullDate_OutOfRangeValue()
        {
            FormatProvider.FormatExecutor(null,"{0:dd MMMMM yyyy года}", new DateTime(3013, 12, 17));
        }


        [TestMethod]
        [Description("Проверяем работоспасобность по таблице [Test].[Formatting].[FullDate].")]
        [Owner("Алексей Крайнов")]
        [TestCategory("FullDate")]
        [DataSource("System.Data.SqlClient", @"Initial Catalog = Test; Password = 345; User ID = sa; Data Source = STEND\SQLEXPRESS",
            "[Formatting].[FullDate]", DataAccessMethod.Sequential)]
        public void FullDate_CheckTableDiteTime() {

            var value = Convert.ToDateTime(TestContext.DataRow["date"]);
            var expected = TestContext.DataRow["expected"].ToString();

            var actual = FormatProvider.FormatExecutor(null, "{0:dd MMMMM yyyy года}", value);

            Assert.AreEqual(expected, actual, " Состовитель - Алексей Крайнов");
        }

        [TestMethod]
        [Description("Проверяем работоспасобность по таблице [Test].[Formatting].[FullDate].")]
        [Owner("Алексей Крайнов")]
        [TestCategory("FullDate")]
        [DataSource("System.Data.SqlClient", @"Initial Catalog = Test; Password = 345; User ID = sa; Data Source = STEND\SQLEXPRESS",
            "[Formatting].[FullDate]", DataAccessMethod.Sequential)]
        public void FullDate_CheckTableWithSqlDiteTime() 
        {
            var value = new SqlDateTime(Convert.ToDateTime(TestContext.DataRow["date"]));
            var expected = TestContext.DataRow["expected"].ToString();

            var actual = FormatProvider.FormatExecutor(null, "{0:dd MMMMM yyyy года}", value);

            Assert.AreEqual(expected, actual, " Состовитель - Алексей Крайнов");
        }


    }
}
