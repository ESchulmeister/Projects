using System.Data;

namespace Empire.DriverLog.Business
{
    public class Administrator : User
    {
        #region Constructors
        public Administrator() : base()
        {

        }
        public Administrator(IDataReader oDataReader) : base(oDataReader)
        {
        }
        #endregion

        #region Methods
        protected override void Load(IDataReader oDataReader)
        {
            base.Load(oDataReader);
       }

        public override string ToString()
        {
            return $"Administrator - {base.ToString()}";
        }
        #endregion
    }
}
