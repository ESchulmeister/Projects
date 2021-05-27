using System;

namespace Empire.Shared.Utilities
{
    public class DuplicateFileException : Exception
    {
        #region Constructors
        public DuplicateFileException() : base("File already exists!")
        {

        }
        public DuplicateFileException(string sFileName) : base($"File [{sFileName}] already exists!")
        {

        }
        #endregion
    }
}
