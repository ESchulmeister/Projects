using System.Data;
using Empire.Shared.Business;

namespace Empire.DriverLog.Business
{
    public class Driver : User
    {
        #region Properties
        public string DriverCode
        {
            get;
            set;
        }

        public Run CurrentRun
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        public Driver() : base()
        {

        }
        public Driver(IDataReader oDataReader) : base(oDataReader)
        {
        }
        #endregion

        #region Methods
        protected override void Load(IDataReader oDataReader)
        {
            base.Load(oDataReader);
            this.DriverCode = oDataReader.ReadColumn("DRV_CD");
            int iRunNumber = oDataReader.ReadColumn("Run_No", 0);

            if(iRunNumber != 0)
            {
                this.CurrentRun = new Run(oDataReader);
            }
        }
        public override string ToString()
        {
            return $"Driver - {base.ToString()}";
        }
        #endregion
    }
}
