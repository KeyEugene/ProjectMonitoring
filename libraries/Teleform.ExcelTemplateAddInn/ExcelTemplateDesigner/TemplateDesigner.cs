using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Excel = Microsoft.Office.Interop.Excel;
using Teleform.Office.DBSchemeWordAddIn;
using Teleform.Reporting;
using Teleform.Reporting.Providers;

using Teleform.Reporting.WordExcelTemplateAddIns;

using System.Xml.Linq;

using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

using LoginButtonClickEventArgs = Teleform.Office.DBSchemeWordAddIn.LoginForm.LoginButtonClickEventArgs;
using System.Security;

namespace ExcelTemplateDesigner
{
    public class TemplateDesigner
    {
        private static SchemeForm Schemeform;

        private static LoginForm LoginForm;

        public static bool IsHidden { get; private set; }
        public static bool IsInsertModeEnable { get; set; }

        //public static Schema Schema { get; private set; }

        public static AddInSchema AddInSchema { get; private set; }

        public static Excel.Application ExcelApplication { get; set; }

        public static void Show(Excel.Application excelApplication)
        {
            if (Schemeform == null)
            {
                ExcelApplication = excelApplication;
                LoginForm = new LoginForm();
                LoginForm.LoginButtonClick += new EventHandler<LoginButtonClickEventArgs>(loginButton_Click);

                LoginForm.ShowDialog();
                //form = new SchemeForm(excelApplication, Schema);
            }
        }


        protected static void loginButton_Click(object sender, LoginButtonClickEventArgs e)
        {
            if (FillSchema(e.Login, e.Password))
            {
                Schemeform = new SchemeForm(ExcelApplication, AddInSchema);
                LoginForm.Close();
                Schemeform.Show();
            }
            else
            {
                LoginForm.SetErrorMessage("Не подключения, или неверная пара логин-пароль.");

            }

        }


        public static bool FillSchema(string login, string password)
        {
        connectToServer:
            //var webQuery = Environment.GetEnvironmentVariable("officeaddinserver");
            //var xmlSchemeProvider = new XmlSchemaProvider(webQuery);


            var environmentVariable = Environment.GetEnvironmentVariable("officeaddinserver");

            var webQuery = string.Format("{0}?login={1}&password={2}", environmentVariable, login, password);

            var webQuery1 = string.Format("http://localHost:25000/monitoring/get.schema.aspx?login={0}&password={1}", login, password);

            if (webQuery.Equals(webQuery1))
            {
                
            }
            
            
            var parser = new AddInSchemaParser();

            try
            {
                var document = XDocument.Load(webQuery);
                AddInSchema = parser.Parse(document.Root);
                return true;
                //Schema = xmlSchemeProvider.GetInstance();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return false;
            }
        }

    }
}
