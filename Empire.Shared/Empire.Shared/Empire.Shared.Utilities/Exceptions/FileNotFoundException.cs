using System;

namespace Empire.Shared.Utilities
{
    public class FileNotFoundException : Exception
    {
        #region Constructors
        public FileNotFoundException() : base("File not found!")
        {

        }
        public FileNotFoundException(string sDirectoryName, string sFileName) : base($"Azure Share File Does Not Exist! [refDF32:{sDirectoryName}/{sFileName}]")
        {

        }
        #endregion
    }
}
