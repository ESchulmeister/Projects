using Empire.DriverLog.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire.DriverLog.Business
{
    public class DriverFactory
    {
        
        /// <summary>
        /// DriverApiController.Get
        /// </summary>
        /// <param name="sLocationCode">ddlLocation.val()</param>
        /// <returns></returns>
        public async Task<List<Driver>> ListDrivers(string sLocationCode)
        {
            var oDriverRepository = new DriverRepository();

            IDataReader oDataReader = null;
            try
            {
                oDataReader = await oDriverRepository.List(sLocationCode);
            }
            catch (Exception)
            {
                throw;
            }

            var lstDrivers = new List<Driver>();
            while (oDataReader.Read())
            {
                var oDriver = new Driver(oDataReader);
                lstDrivers.Add(oDriver);
            }

            oDataReader.Close();

            return lstDrivers;
        }

        public async Task<List<Driver>> ListDrivers()
        {
            var oDriverRepository = new DriverRepository();

            IDataReader oDataReader = null;
            try
            {
                oDataReader = await oDriverRepository.List();
            }
            catch (Exception)
            {
                throw;
            }

            var lstDrivers = new List<Driver>();
            while (oDataReader.Read())
            {
                var oDriver = new Driver(oDataReader);
                lstDrivers.Add(oDriver);
            }

            oDataReader.Close();

            return lstDrivers;
        }
    }
}
