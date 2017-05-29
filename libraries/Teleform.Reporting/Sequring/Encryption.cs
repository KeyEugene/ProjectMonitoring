using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Security;
using System.Runtime.InteropServices;


//namespace Teleform.ProjectMonitoring
namespace Teleform.Reporting.Sequring
{
    public abstract  class Encryption 
    {

        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);            
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Decrypt(string encryptedText)
        {
            var z = Convert.FromBase64String(encryptedText);
            return Encoding.UTF8.GetString(z);
        }

        public static SecureString ConvertToUNSecureString(string plainText)
        {
            var secureStr = new SecureString();
            if (plainText.Length > 0)
            {
                foreach (var c in plainText.ToCharArray())
                    secureStr.AppendChar(c);
            }
            return secureStr;
        }

        public static string ConvertToUNString(SecureString secureString)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }


       
    }
}