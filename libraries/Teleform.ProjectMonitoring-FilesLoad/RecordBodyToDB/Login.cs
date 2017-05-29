using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecordBodyToDB
{
    public partial class Login : Form
    {
        private string login { get; set; }
        private string pwd { get; set; }

        public static string connString;
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            login = TextBoxLogin.Text;
            pwd = TextBoxPassword.Text;
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pwd))
            {
                label3.Text = "Введите логин и пароль.";
                return;
            }

            string s = string.Concat(Directory.GetCurrentDirectory(), "\\ConnectionString.txt");

            var di = Directory.Exists(s);


            if (File.Exists(s))
            connString = File.ReadAllText(s);
            else {
                label3.Text = "Отсутствует файл со строкой подключения, обратитесь к администратору.";
                return;
            }

            connString = string.Format(connString, login, pwd);

            if (!CheckConnectionString())
            {
                label3.Text = "Неверная пара логин/пароль.";
                return;
            }

            this.Close();
        }

        public static bool CheckConnectionString()
        {
            using (var conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            }
        }

        private void TextBoxPassword_TextChanged(object sender, EventArgs e)
        {
            label3.Text = string.Empty;
        }
    }
}
