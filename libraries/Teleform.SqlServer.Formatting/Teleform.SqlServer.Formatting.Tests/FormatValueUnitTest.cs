using Teleform.SqlServer.Formatting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlTypes;
using System;
using System.Data.SqlClient;

namespace Teleform.SqlServer.Formatting.Tests
{
    
    [TestClass]
    public class FormatValueUnitTest
    {
        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestMethod]
        [Description("Проверка одного фиксированного значения в виде SqlInt32.")]
        [Owner("Гончаров Василий")]
        [TestCategory("MoneyInWords")]
        public void FixValue1_MoneyInWords()
        {
            var actual = FormatProvider.FormatValue("MoneyInWords", new SqlInt32(1765101));
            Assert.AreEqual<string>("один миллион семьсот шестьдесят пять тысяч сто один рубль", actual);
        }

        [TestMethod]
        [Description("Проверка одного фиксированного значения в виде SqlInt32.")]
        [Owner("Гончаров Василий")]
        [TestCategory("MoneyInWords")]
        public void FixValue2_MoneyInWords()
        {
            var actual = FormatProvider.FormatValue("MoneyInWords", new SqlInt32(1));
            Assert.AreEqual<string>("один рубль", actual);
        }

        [TestMethod]
        [Description("Проверка одного фиксированного значения в виде Int ")]
        [Owner("Гатаулин Виктор")]
        [TestCategory("MoneyInWords")]
        public void FixValue3_MoneyInWords()
        {
            var value = 6041000;
            string expected = "шесть миллионов сорок одна тысяча рублей";
            var actual =  FormatProvider.FormatValue("MoneyInWords", value);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [Description("Проверка одного фиксированного значения в виде Date ")]
        [Owner("Гатаулин Виктор")]
        [TestCategory("FullDate")]
        public void FixValue1_DateInWords()
        {
            string expected = "14 декабря 2013 года";
            var actual = FormatProvider.FormatValue("FullDate", new SqlDateTime(DateTime.Now));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [Description("Проверка работоспособности по таблице [Test].[Formatting].[MoneyInWords.View].")]
        [Owner("Виктор Гатаулин, Василий Гончаров")]
        [TestCategory("MoneyInWords")]
        [DataSource(
            "System.Data.SqlClient", @"Initial Catalog = Test; Password = 345; User ID = sa; Data Source = STEND\SQLEXPRESS",
            "[Formatting].[MoneyInWords.View]",
            DataAccessMethod.Sequential)]
        public void MoneyInWords_FromTable()
        {
            var value = Convert.ToInt32(TestContext.DataRow["input"]);
            var expected = TestContext.DataRow["expected"].ToString();

            var actual = FormatProvider.FormatValue("MoneyInWords", value);

            Assert.AreEqual(expected, actual, "Составитель - {0}.", TestContext.DataRow["author"]);
        }

    }
}
