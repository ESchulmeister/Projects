using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Empire.Shared.Business;

namespace Empire.DriverLog.Business
{
    public class StopStatus
    {


        #region properties



        public int Key { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public bool WillDisplay { get; set; }

        public double SortOrder { get; set; }

        #endregion

        #region constructor
        public StopStatus(IDataReader oDataReader)
        {
            this.Load(oDataReader);
        }
        #endregion

        #region Methods
        protected virtual void Load(IDataReader oDataReader)
        {
            this.Key = oDataReader.ReadColumn("PKey", 0);
            this.Code = oDataReader.ReadColumn("Status_CD").Trim();
            this.WillDisplay = oDataReader.ReadColumn("Display_YN", false);

            this.Description = oDataReader.ReadColumn("Status_Desc").Trim().ToUpper();

            this.SortOrder= oDataReader.ReadColumn("Sort_Order", 0d);
        }



        #endregion


        public override string ToString()
        {
            return this.Code;
        }

        public static async Task<List<StopStatus>> List()
        {
            var oStandingDataFactory = new StandingDataFactory();
            return await oStandingDataFactory.ListStopStatuses();
        }
        public static async Task<StopStatus> Instance(string sStatus)
        {
            var lstStopStatuses = await StopStatus.List();
            return lstStopStatuses.FirstOrDefault(oStopStatus => String.Compare(oStopStatus.Code, sStatus, false) == 0);
        }
    }
}