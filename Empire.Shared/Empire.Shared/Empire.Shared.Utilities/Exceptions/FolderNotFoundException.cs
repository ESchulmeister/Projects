using System;

namespace Empire.Shared.Utilities
{
    public class FolderNotFoundException : Exception
    {
        #region Constructors
        public FolderNotFoundException() : base("Folder not found!")
        {

        }
        public FolderNotFoundException(string sFolderName) : base($"Azure Folder Does Not Exist! [refDF32: {sFolderName}]")
        {

        }
        #endregion
    }
}
