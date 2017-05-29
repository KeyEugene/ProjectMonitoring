using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SimpleSHA
{
    public static class ShaHash
    {
        public static byte[] GetHash( this string inputString )
        {
            HashAlgorithm algorithm = SHA1.Create();  // SHA1.Create()
            return algorithm.ComputeHash( Encoding.UTF8.GetBytes( inputString ) );
        }

        public static string GetHashString( this string inputString )
        {
            StringBuilder sb = new StringBuilder();

            foreach ( byte b in inputString.GetHash() )
                sb.Append( b.ToString( "X2" ) );

            return sb.ToString();
        }
    }
}
