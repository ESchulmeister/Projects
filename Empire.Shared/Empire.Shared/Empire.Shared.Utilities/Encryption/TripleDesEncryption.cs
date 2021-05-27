using System;
using System.IO;
using System.Security.Cryptography;

namespace Empire.Shared.Utilities
{
    public class TripleDesEncryption : Encryption
    {
        #region Methods
        public override string Decrypt(string sCipher, string sKey)
        {
            byte[] aTextBytes = Convert.FromBase64String(sCipher);

            var oTripleDESCryptoServiceProvider = this.CreateProvider(sKey);

            string sClearText = null;
            using (var oMemoryStream = new MemoryStream())
            {
                using (var oCryptoStream = new CryptoStream(oMemoryStream, oTripleDESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    oCryptoStream.Write(aTextBytes, 0, aTextBytes.Length);
                    oCryptoStream.FlushFinalBlock();

                    sClearText = this.ToEncodedString(oMemoryStream.ToArray());
                }
            }

            return sClearText;
        }

        public override string Encrypt(string sClearText, string sKey)
        {
            
            byte[] aTextBytes = this.ToByteArray(sClearText);

            var oTripleDESCryptoServiceProvider = this.CreateProvider(sKey);

            string sCipher = null;
            using (var oMemoryStream = new MemoryStream())
            {
                using(var oCryptoStream = new CryptoStream(oMemoryStream, oTripleDESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    oCryptoStream.Write(aTextBytes, 0, aTextBytes.Length);
                    oCryptoStream.FlushFinalBlock();

                    sCipher = Convert.ToBase64String(oMemoryStream.ToArray());
                }
            }

            return sCipher;
        }

        protected TripleDESCryptoServiceProvider CreateProvider(string sKey)
        {
            const int Factor = 8;

            var oTripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();

            oTripleDESCryptoServiceProvider.Key = this.TruncateHash(sKey, oTripleDESCryptoServiceProvider.KeySize / Factor);
            oTripleDESCryptoServiceProvider.IV = this.TruncateHash("", oTripleDESCryptoServiceProvider.BlockSize / Factor);

            return oTripleDESCryptoServiceProvider;
        }
        #endregion
    }
}
