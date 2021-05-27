using System;
using System.Security.Cryptography;
using System.Text;

namespace Empire.Shared.Utilities
{
    public class Md5Hash : Lazy<Md5Hash>
    {
        #region Properties
        public static Md5Hash Current
        {
            get
            {
                return new Md5Hash();
            }
        }
        #endregion

        #region Methods
        public byte[] Compute(string sClearText)
        {
            var oEncoding = Encoding.UTF8;

            var oMd5 = MD5.Create();
            return oMd5.ComputeHash(oEncoding.GetBytes(sClearText));
        }
        #endregion
    }
}
