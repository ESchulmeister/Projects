using Empire.DriverLog.Business;
using Empire.DriverLog.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire.DriverLogDispatch.Business
{
    public class StatusFactory
    {

        public async Task<List<StopStatus>> List()
        {
            var oStatusRepository = new StatusRepository();

            IDataReader oDataReader = null;
            try
            {
                oDataReader = await oStatusRepository.List();
            }
            catch (Exception)
            {
                throw;
            }

            var lstStatuses = new List<StopStatus>();
            while (oDataReader.Read())
            {
                var oStatus = new StopStatus(oDataReader);
                lstStatuses.Add(oStatus);
            }

            oDataReader.Close();

            return lstStatuses;
        }
    }
}
