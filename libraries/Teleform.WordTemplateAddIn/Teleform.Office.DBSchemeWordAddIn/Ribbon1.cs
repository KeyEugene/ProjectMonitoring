using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using MSOffice = Microsoft.Office.Core;
using Word = Microsoft.Office.Interop.Word;
using System.Windows.Forms;
using Microsoft.Office.Tools.Word;
using System.Security.Cryptography;
using Teleform.Office.DBSchemeWordAddIn.XmlWebService;
using System.Xml;
using Teleform.Reporting;
using Teleform.Reporting.Providers;
using System.Net;
using System.Diagnostics;
using System.Xml.Linq;

using Teleform.Reporting.WordExcelTemplateAddIns;

// TODO:  Выполните эти шаги, чтобы активировать элемент XML ленты:

// 1: Скопируйте следующий блок кода в класс ThisAddin, ThisWorkbook или ThisDocument.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new Ribbon1();
//  }

// 2. Создайте методы обратного вызова в области "Обратные вызовы ленты" этого класса, чтобы обрабатывать действия
//    пользователя, например нажатие кнопки. Примечание: если эта лента экспортирована из конструктора ленты,
//    переместите свой код из обработчиков событий в методы обратного вызова и модифицируйте этот код, чтобы работать с
//    моделью программирования расширения ленты (RibbonX).

// 3. Назначьте атрибуты тегам элементов управления в XML-файле ленты, чтобы идентифицировать соответствующие методы обратного вызова в своем коде.  

// Дополнительные сведения можно найти в XML-документации для ленты в справке набора средств Visual Studio для Office.

namespace Teleform.Office.DBSchemeWordAddIn
{
    [ComVisible(true)]
    public class Ribbon1 : MSOffice.IRibbonExtensibility
    {
        private MSOffice.IRibbonUI ribbon;

        public static readonly string _envVarServer = "officeaddinserver";
        //public static readonly string ConnecionString = @"Current Language = Russian; data source=stend\SQLEXPRESS; Initial Catalog = MinProm2; User Id=sa; Password=345;";
        //public static readonly string ConnecionString = @"Current Language = Russian; data source=(localdb)\vault; Initial Catalog = MinProm2; Integrated Security = SSPI;";

        //private Schema schema;

        private AddInSchema addInSchema;

        //private List<AddInEntity> entities;

        //private List<Teleform.Reporting.Type> types;

        private System.Data.DataTable Table { get; set; }

        private SchemeForm scform { get; set; }

        public Ribbon1()
        {
#if false
        connectToServer:
            var webQuery = Environment.GetEnvironmentVariable(_envVarServer);
            var provider = new XmlSchemaProvider(webQuery);

            try
            {
                schema = provider.GetInstance();
            }
            catch (WebException)
            {
                var result = MessageBox.Show("Не удаётся подключиться к серверу мониторинга проектов.\nПроверьте подключение и войдите в систему", 
                                             "Уведомление", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Retry)
                    goto connectToServer;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //schema = SchemeWork.CreateSchema();
#endif
        }

        public void OnTableButton(MSOffice.IRibbonControl control)
        {
            MessageBox.Show("Функция не реализована на данный момент.", "Нет реализации", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
            object missing = System.Type.Missing;
            Word.Range currentRange = Globals.ThisAddIn.Application.Selection.Range;
            Word.Table newTable = Globals.ThisAddIn.Application.ActiveDocument.Tables.Add(
            currentRange, 3, 4, ref missing, ref missing);

            // Get all of the borders except for the diagonal borders.
            Word.Border[] borders = new Word.Border[6];
            borders[0] = newTable.Borders[Word.WdBorderType.wdBorderLeft];
            borders[1] = newTable.Borders[Word.WdBorderType.wdBorderRight];
            borders[2] = newTable.Borders[Word.WdBorderType.wdBorderTop];
            borders[3] = newTable.Borders[Word.WdBorderType.wdBorderBottom];
            borders[4] = newTable.Borders[Word.WdBorderType.wdBorderHorizontal];
            borders[5] = newTable.Borders[Word.WdBorderType.wdBorderVertical];

            // Format each of the borders.
            foreach (Word.Border border in borders)
            {
                border.LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                border.Color = Word.WdColor.wdColorBlue;
            }
        }

        public void OnTextButton(MSOffice.IRibbonControl control)
        {

            if (addInSchema == null)
                if (!FillSchema())
                    return;
            
            if (scform == null)
                scform = new SchemeForm(addInSchema);

            scform.Show();
            scform.Activate();
            
        }

        //public bool FillSchema(string login, string passw)
        public bool FillSchema()
        {

        connectToServer:
            //var webQuery = Environment.GetEnvironmentVariable(_envVarServer);
            //var provider = new XmlSchemaProvider(webQuery);



            byte[] plainTextBytes = Encoding.UTF8.GetBytes("admin");
            var password = Convert.ToBase64String(plainTextBytes);

            var webQuery = string.Format(@"http://localhost:25000/monitoring/get.schema.aspx?login=admin&password={0}", password); ;

            try
            {

                var parser = new AddInSchemaParser();
                var document = XDocument.Load(webQuery);
                addInSchema = parser.Parse(document.Root);
                return true;
                //schema = provider.GetInstance();
            }
            catch (WebException)
            {
                var result = MessageBox.Show("Не удаётся подключиться к серверу.\nНет соединения или не правильная пара логин/пароль",
                                             "Уведомление", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Retry)
                    goto connectToServer;
                else
                    return false;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                var result = MessageBox.Show("Не удаётся подключиться к серверу.\nНет соединения или не правильная пара логин/пароль",
                                             "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (result == DialogResult.Retry)
                    goto connectToServer;
                else
                    return false;
            }


        }

        #region Члены IRibbonExtensibility

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("Teleform.Office.DBSchemeWordAddIn.Ribbon1.xml");
        }

        #endregion

        #region Обратные вызовы ленты
        //Создавайте функции обратного вызова здесь. Для получения дополнительной информации о создании функций обратного вызова выберите элемент XML ленты в обозревателе решений и нажмите клавишу F1

        public void Ribbon_Load(MSOffice.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
        }

        #endregion

        #region Вспомогательные методы

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
