using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Data;
using Empire.Shared.Utilities;
using Empire.DriverLog.Data;

namespace Empire.DriverLog.Business
{
    /// <summary>
    /// Cached lists - Locations/Stops
    /// </summary>
    public class StandingDataFactory
    {
        #region Methods
       /// <summary>
       /// LocationApiController/GET || Locations.List
       /// </summary>
       /// <returns></returns>
        public async Task<List<Location>> ListLocations()
        {
            var oCacheAdapter = new CacheAdapter<Location>();
            List<Location> lstLocations = oCacheAdapter.GetCollection() as List<Location>;

            if(lstLocations != null)
            {
                return lstLocations;
            }

            var oStandingDataRepository = new StandingDataRepository();

            IDataReader oDataReader = null;
            try
            {
                oDataReader = await oStandingDataRepository.ListLocations();
            }
            catch (Exception)
            {
                throw;
            }

            lstLocations = new List<Location>();
            while (oDataReader.Read())
            {
                lstLocations.Add(new Location(oDataReader));
            }

            oDataReader.Close();

            oCacheAdapter.PutCollection(lstLocations);

            return lstLocations;
        }

        /// <summary>
        /// StatusApiController.Get || StopStatus.List
        /// </summary>
        /// <returns></returns>
        public async Task<List<StopStatus>> ListStopStatuses()
        {

            var oCacheAdapter = new CacheAdapter<StopStatus>();
            List<StopStatus> lstStatuses = oCacheAdapter.GetCollection() as List<StopStatus>;

            if (lstStatuses != null)
            {
                return lstStatuses;
            }

            var oStandingDataRepository = new StandingDataRepository();

            IDataReader oDataReader = null;
            try
            {
                oDataReader = await oStandingDataRepository.ListStatuses();
            }
            catch (Exception)
            {
                throw;
            }

            lstStatuses = new List<StopStatus>();
            while (oDataReader.Read())
            {
                lstStatuses.Add(new StopStatus(oDataReader));
            }

            oDataReader.Close();

            oCacheAdapter.PutCollection(lstStatuses);


            return lstStatuses;
        }

        #endregion
    }
}
