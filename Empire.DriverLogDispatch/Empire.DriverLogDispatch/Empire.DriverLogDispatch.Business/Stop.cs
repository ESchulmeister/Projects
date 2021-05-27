using Empire.Shared.Business;
using System.Data;

namespace Empire.DriverLog.Business
{
    public class Stop
    {
        #region Properties
        public int Key
        {
            get; set;
        }

        public bool  IsActive
        {
            get; set;
        }  
        public int SequenceNumber
        {
            get; set;
        }

        public string DisplaySequence
        {
            get; set;
        }

        public Customer Customer
        {
            get;
            set;
        }

        public StopStatus CurrentStatus
        {
            get; set;
        }

        #endregion

        #region Constructors
        public Stop()
        {

        }
        public Stop(IDataReader oDataReader)
        {
            this.Load(oDataReader);
        }
        #endregion

        #region Methods
        protected virtual void Load(IDataReader oDataReader)
        {
            this.Key = oDataReader.ReadColumn("Run_No", 0);
            this.SequenceNumber = oDataReader.ReadColumn("Stop_Seq", 0);
            this.DisplaySequence = this.SequenceNumber.ToString();
            this.IsActive = oDataReader.ReadColumn("Active", false);

        }

        public override string ToString()
        {
            if(this.Customer == null)
            {
                return base.ToString();
            }
            
            return $"Stop at {this.Customer.ToString()}";
        }
        #endregion
    }
}
