using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Excel = Microsoft.Office.Interop.Excel;
using Teleform.Reporting.WordExcelTemplateAddIns;
using System.Xml.Linq;

using Teleform.Reporting;
using System.Text;
using System.Security;
using Teleform.Reporting.Sequring;

namespace Teleform.Office.DBSchemeWordAddIn
{
    public partial class LoginForm : Form
    {
        Sequring SequringAddIn;
        public LoginForm()
        {            
            InitializeComponent();
            SequringAddIn = new Sequring(PasswordTextBox);
        }   

        public void SetErrorMessage(string message)
        {
            label1.Text = "Нет подключения, или неверный пароль/логин";
            label1.Height = 25;
        }

        public event EventHandler<LoginButtonClickEventArgs> LoginButtonClick;

        public class LoginButtonClickEventArgs : EventArgs
        {
            public string Login { get; private set; }

            public string Password { get; private set; }

            public LoginButtonClickEventArgs(string login, string password)
            {
                this.Login = login;
                this.Password = password;

            }
        }


        private void LoginButton_Click(object sender, EventArgs e)
        {

            //var notSecurtStrintg = Encryption.ConvertToUNString(SecureString);
            var notSecurtStrintg = Encryption.ConvertToUNString(SequringAddIn.SecureString);
            var password = Encryption.Encrypt(notSecurtStrintg);


            if (LoginButtonClick != null)
                LoginButtonClick(this, new LoginButtonClickEventArgs(LoginTextBox.Text, password));

        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

     

        private void PasswordTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            var xxx = PasswordTextBox.Text;

            if (e.KeyChar == (char)Keys.Back)
            {
               SequringAddIn.ProcessBackspace();
            }
            else
            {
                SequringAddIn.ProcessNewCharacter(e.KeyChar);
            }
            e.Handled = true;
        }

        //private char _passwordChar = '*';
        //private char PasswordChar
        //{
        //    get { return _passwordChar; }
        //    set { _passwordChar = value; }
        //}

        //SecureString _secureString = new SecureString();
        //private SecureString SecureString
        //{
        //    get { return _secureString; }
        //}
        //public void ResetDisplayCharacters(int currentPosition)
        //{          

        //    PasswordTextBox.Text = new string(_passwordChar, _secureString.Length);
        //    PasswordTextBox.SelectionStart = currentPosition;
        //} 

        //private void ProcessBackspace(int start, int length)
        //{
        //    if (length > 0)
        //    {
        //        RemoveSelectedCharacters(start, length);
        //        ResetDisplayCharacters(start);
        //    }
        //    else if (start > 0)
        //    {
        //        _secureString.RemoveAt(start - 1);
        //        ResetDisplayCharacters(start - 1);
        //    }
        //}
        //private void ProcessNewCharacter(int start, int length,  char character) 
        //{
        //    if (length > 0)
        //        RemoveSelectedCharacters(start, length);

        //    _secureString.InsertAt(start, character);
        //    ResetDisplayCharacters(start + 1);
        //}
        //private void RemoveSelectedCharacters(int start, int length)
        //{
        //    for (int i = 0; i < length; i++)
        //        _secureString.RemoveAt(start);

        //}
      
       
      
    }
}
