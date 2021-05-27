using System.Data;
using Empire.Shared.Business;
using Newtonsoft.Json.Linq;

namespace Empire.DriverLog.Business
{
    public class Run
    {
        #region Properties
        public int Number
        {
            get; set;
        }
        public string Note
        {
            get; set;
        }

        public byte[] Signature
        {
            get;set;
        }
        public string TruckNumber
        {
            get;
            set;
        }

        public bool? CanBeCompleted
        {
            get; set;
        }
        #endregion

        #region Constructors
        public Run()
        {

        }
        public Run(IDataReader oDataReader)
        {
            this.Number = oDataReader.ReadColumn("Run_No", 0);
            this.Note = oDataReader.ReadColumn("Dlog_Run_Note");
            this.TruckNumber = oDataReader.ReadColumn("Truck_No");
            this.CanBeCompleted = oDataReader.ReadColumn("CanBeCompleted", (bool?)null);
        }
        public Run(JObject oJObject)
        {
            this.Number = oJObject.ReadValue("Number", 0);
            this.Note = oJObject.ReadValue("Note");
            this.TruckNumber = oJObject.ReadValue("TruckNumber");
        }
        #endregion
    }
}
