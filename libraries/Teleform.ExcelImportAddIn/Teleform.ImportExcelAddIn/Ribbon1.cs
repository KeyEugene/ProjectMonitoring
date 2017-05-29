using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using ExcelDialog;
using Teleform.Reporting;

namespace Teleform.ImportExcelAddIn
{
    public partial class Ribbon1
    {
        private EntityForm form;
        private LoginForm loginForm;
        private bool isAuthorized = false;
        private string ConnectionString = @"data source={0}; Initial Catalog = {1}; User Id={2}; Password={3}; Asynchronous Processing=true";

        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {
            
        }

        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
            if (loginForm == null)
                loginForm = new LoginForm(ConnectionString);

            if (!isAuthorized)
            {
                if (loginForm.isAuthorized)
                {
                    Storage.ConnectionString = string.Format(
                        ConnectionString, loginForm.DataSource, loginForm.InitialCatalog, loginForm.login, loginForm.password);
                    isAuthorized = true;

                    if (!loginForm.IsDisposed)
                        loginForm.Close();
                }
                else
                {
                    if (!loginForm.IsDisposed)
                        loginForm.Show();
                    else
                    {
                        loginForm = new LoginForm(ConnectionString);
                        loginForm.Show();
                    }
                    return;
                }
            }

            if (form != null)
                form.Close();

            form = new EntityForm(Globals.ThisAddIn.Application);
            if (!form.isClose)
                form.Show(); 
        }
    }
}
