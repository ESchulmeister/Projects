using System;

namespace Empire.Shared.Utilities
{
    public class ShareNotFoundException : Exception
    {
        #region Constructors
        public ShareNotFoundException() : base("Share not found!")
        {

        }
        public ShareNotFoundException(string sShareName) : base($"Azure Share Does Not Exist! [refDF32: {sShareName}]")
        {

        }
        #endregion
    }
}
