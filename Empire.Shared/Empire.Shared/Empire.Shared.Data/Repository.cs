namespace Empire.Shared.Data
{
    public abstract class Repository
    {
        #region Properties
        protected string ConnectionString
        {
            get;
            set;
        }
        #endregion
    }
}
