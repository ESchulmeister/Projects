using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Empire.Shared.Utilities
{
    public abstract class Encryption
    {
        #region Constants
        public static class Keys
        {
            public const string UserName = "Empire Auto Username";
            public const string Password = "Empire Password";
        }
        #endregion

        #region Properties
        public static TripleDesEncryption TripleDes
        {
            get
            {
                return new TripleDesEncryption();
            }
        }
        #endregion

        #region Methods
        public abstract string Encrypt(string sClearText, string sKey);
        public abstract string Decrypt(string sCipher, string sKey);

        protected virtual byte[] ToByteArray(string sText)
        {
            return Encoding.Unicode.GetBytes(sText);
        }
        protected virtual string ToEncodedString(byte[] aBytes)
        {
            return Encoding.Unicode.GetString(aBytes);
        }

        protected byte[] TruncateHash(string sKey, int iLength)
        {
            var oSHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();
            byte[] aKeyBytes = Encoding.Unicode.GetBytes(sKey);
            byte[] aHashBytes = oSHA1CryptoServiceProvider.ComputeHash(aKeyBytes);

            var lstBytes = new List<byte>();
            for (int i = 0; i < iLength; i++)
            {
                if(i > aHashBytes.Length - 1)
                {
                    lstBytes.Add((byte)0);
                }
                else
                {
                    lstBytes.Add(aHashBytes[i]);
                }
            }
            
            return lstBytes.ToArray();
        }

        #endregion
    }
}
