using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Teleform.ImportExcelAddIn;
using Teleform.ProjectMonitoring;

namespace ExcelDialog
{
    public partial class LoginForm : Form
    {
        private string ConnectionString;

        public string login { get; private set; }
        public string password { get; private set; }
        public string DataSource { get; private set; }
        public string InitialCatalog { get; private set; }

        public bool isAuthorized = false;


        public LoginForm(string ConnectionString)
        {
            InitializeComponent();
            // TODO: Complete member initialization
            this.ConnectionString = ConnectionString;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxLogin.Text) || string.IsNullOrEmpty(textBoxPassowrd.Text))
                ErrorMassage.Text = "Введите логин и пароль.";

            GetConnectionData();

            this.login = textBoxLogin.Text;
            this.password = Encryption.Encrypt(textBoxPassowrd.Text);

            try
            {
                var da = new SqlDataAdapter("SELECT CURRENT_USER", string.Format(ConnectionString, DataSource, InitialCatalog, login, password));
                var dt = new DataTable();

                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    ErrorMassage.Text = "Предоставлена неверная пара логин-пароль.";
                    return;
                }

                isAuthorized = true;
            }
            catch (System.Data.SqlClient.SqlException)
            {
                ErrorMassage.Text = "Предоставлена неверная пара логин-пароль.";
                throw new Exception();
            }

            this.Visible = false;
            return;
        }

        private void GetConnectionData()
        {
            try
            {
                DataSource = Globals.ThisAddIn.Application.Sheets[2].Cells(2, 1).Value;
                InitialCatalog = Globals.ThisAddIn.Application.Sheets[2].Cells(2, 2).Value;
            }
            catch (Exception)
            {
                ErrorMassage.Text = "Не могу прочитать данных для подключения к Sql-базе.";
                throw new Exception();

            }

            if (string.IsNullOrEmpty(DataSource) && string.IsNullOrEmpty(InitialCatalog))
            {
                MessageBox.Show("Нету данных для подключения к Sql-базе.");
                throw new Exception("");
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void textBoxLogin_TextChanged(object sender, EventArgs e)
        {
            ErrorMassage.Text = "";
        }


    }
}
