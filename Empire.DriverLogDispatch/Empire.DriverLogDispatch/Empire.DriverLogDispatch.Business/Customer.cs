using Empire.Shared.Business;
using Empire.Shared.Utilities;
using System.Data;
using System.Globalization;
using System.Web;

namespace Empire.DriverLog.Business
{
    public class Customer
    {
        #region Properties

        public string ID
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }
        public string Address
        {
            get; set;
        }
        public string City
        {
            get;set;
        }

        public string State
        {
            get;set;
        }

        public string ZipCode
        {
            get;  set;
        }

        public string FullAddress
        {
            get; set; 
        }

        public string MapUrl
        {
            get; set;
        }
        #endregion

        #region Constructors
        public Customer()
        {

        }
        public Customer(IDataReader oDataReader)
        {
            this.Load(oDataReader);
        }
        #endregion

        #region Methods
        protected virtual void Load(IDataReader oDataReader)
        {
            this.ID = oDataReader.ReadColumn("Cust_ID");
            this.Name = oDataReader.ReadColumn("Cust_Name").Trim();

            string sAddress1 = oDataReader.ReadColumn("Cust_Addr1").Trim();
            this.Address = sAddress1;

            this.City = oDataReader.ReadColumn("City").Trim();
            this.State = oDataReader.ReadColumn("State").Trim();
            this.ZipCode = oDataReader.ReadColumn("Zip").Trim();
            this.FullAddress = $"{this.Address}, {this.City}, {this.State} {this.ZipCode}";
            this.MapUrl = $"{AppSettings.Current.MapUrl}&query={HttpUtility.UrlEncode(this.FullAddress)}";
        }

        public override string ToString()
        {
            return $"{this.ID} - {this.Name}";
        }
        #endregion
    }
}
