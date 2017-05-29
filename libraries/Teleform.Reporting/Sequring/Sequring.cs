using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Windows.Forms;


namespace Teleform.Reporting.Sequring
{
    public class Sequring
    {      

        SecureString _secureString = new SecureString();
        public SecureString SecureString
        {
            get { return _secureString; }
        }

        char _passwordChar = '*';
        //char PasswordChar
        //{
        //    get { return _passwordChar; }
        //    set { _passwordChar = value; }
        //}

        int start
        {
            get { return _passwordTextBox.SelectionStart; }           
        }
        int length
        {
            get { return _passwordTextBox.SelectionLength; }           
        }

        TextBox _passwordTextBox;
        public Sequring(TextBox passwordTextBox)
        {
            this._passwordTextBox = passwordTextBox;
        }

        public void ProcessBackspace()
        {
            if (length > 0)
            {
                RemoveSelectedCharacters();
                ResetDisplayCharacters(start);
            }
            else if (start > 0)
            {
                _secureString.RemoveAt(start - 1);
                ResetDisplayCharacters(start - 1);
            }
        }
        public void ProcessNewCharacter(char character)
        {
            if (length > 0)
                RemoveSelectedCharacters();

            _secureString.InsertAt(start, character);
            ResetDisplayCharacters(start + 1);
        }
        private void RemoveSelectedCharacters()
        {
            for (int i = 0; i < length; i++)
                _secureString.RemoveAt(start);

        }

        public void ResetDisplayCharacters(int currentPosition)
        {
            _passwordTextBox.Text = new string(_passwordChar, SecureString.Length);
            _passwordTextBox.SelectionStart = currentPosition;
        }

    }
}
