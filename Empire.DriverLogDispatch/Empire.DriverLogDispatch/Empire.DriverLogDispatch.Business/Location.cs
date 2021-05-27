using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Empire.Shared.Business;

namespace Empire.DriverLog.Business
{
    public class Location
    {
        #region Constructors
        public Location()
        {
        }
        public Location(IDataReader oDataReader)
        {
            this.Code = oDataReader.ReadColumn("Loc_CD");
        }
        #endregion

        #region Properties


        public string Code {get; set; }

        #endregion

        #region Methods
        public static async Task<List<Location>> List()
        {
            var oStandingDataFactory = new StandingDataFactory();
            return await oStandingDataFactory.ListLocations();
        }

        //currently selected location
        public static async Task<Location> Instance(string sCode)
        {
            var lstLocations = await Location.List();
            return lstLocations.FirstOrDefault(oLocation => String.Compare(oLocation.Code, sCode, false) == 0);
        }
        #endregion
    }
}
