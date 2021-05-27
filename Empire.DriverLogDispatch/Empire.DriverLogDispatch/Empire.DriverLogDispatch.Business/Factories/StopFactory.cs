using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Empire.DriverLog.Data;
using Empire.Shared.Business;

namespace Empire.DriverLog.Business
{
    public class StopFactory
    {

        /// <summary>
        /// List of stop per Run
        /// SttopApiController/Get
        /// </summary>
        /// <param name="iRunID"></param>
        /// <returns></returns>
        public async Task<List<Stop>> List(int iRunID)
        {
            var oStopRepository = new StopRepository();

            IDataReader oDataReader = null;
            try
            {
                oDataReader = await oStopRepository.List(iRunID);
            }
            catch (Exception)
            {
                throw;
            }

            var lstStops = new List<Stop>();
            while (oDataReader.Read())
            {
                var oStop = new Stop(oDataReader);
                oStop.Customer = new Customer(oDataReader); 

                string sStatus = oDataReader.ReadColumn("Status_CD").Trim();
                oStop.CurrentStatus = await StopStatus.Instance(sStatus);
                lstStops.Add(oStop);
            }

            oDataReader.Close();

            return lstStops;
        }

        /// <summary>
        /// StopApiController/PUT
        /// </summary>
        /// <param name="oStop"></param>
        /// <returns></returns>
        public async Task Update(Stop oStop)
        {
            var oCurrentStatus = oStop.CurrentStatus;
            if(oCurrentStatus == null)
            {
                throw new MissingMemberException("Stop", "Status");
            }

            var oCustomerRepository = new StopRepository();

            var oCustomer = oStop.Customer;
            if(oCustomer == null)
            {
                throw new MissingMemberException("Stop", "Customer");
            }
            
            try //update status
            {
               await oCustomerRepository.Update(oStop.Key, oCustomer.ID, oCurrentStatus.Code);
            }
            catch (Exception)
            {
                throw;
            }

        
        }

    }
}
