using Empire.Shared.Business;
using System.Data;
using System.Globalization;

namespace Empire.DriverLog.Business
{
    public abstract class User
    {
        #region Properties
        public int ID
        {
            get;
            set;
        }
        public decimal UserID
        {
            get;
            set;
        }
        public string LastName
        {
            get;
            set;
        }
        public string FirstName
        {
            get;
            set;
        }
        public string UserName
        {
            get;
            set;
        }
        public string FullName
        {
            get;
            set;
        }

        public string SatelliteCode
        {
            get;  set;
        }

        public string FullAddress { get; set; }


        public Location Location
        {
            get; set;
        }

        public bool IsAdmin
        {
            get; set;
        }


        public bool IsLocked
        {
            get;
            protected set;
        }
        #endregion

        #region Constructors
        public User()
        {

        }
        public User(IDataReader oDataReader)
        {
            this.Load(oDataReader);
        }
        #endregion

        #region Methods
        protected virtual void Load(IDataReader oDataReader)
        {
            this.ID = oDataReader.ReadColumn("PKey", 0);
            this.UserID = oDataReader.ReadColumn("User_ID", 0M);
            this.UserName = oDataReader.ReadColumn("User_Name");
            this.LastName = oDataReader.ReadColumn("Lname");
            this.FirstName = oDataReader.ReadColumn("Fname");
            this.SatelliteCode = oDataReader.ReadColumn("Sat_CD");

            this.FullName = $"{this.FirstName.Trim()} {this.LastName}".Trim();
            this.IsAdmin = oDataReader.ReadColumn("DLog_Super", false);
        }

        public override string ToString()
        {
            return this.FullName;
        }
        #endregion
    }
}
