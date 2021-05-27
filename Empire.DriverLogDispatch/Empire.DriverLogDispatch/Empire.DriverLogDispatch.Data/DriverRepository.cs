using Empire.Shared.Data;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Empire.DriverLog.Data
{
    public class DriverRepository : Repository
    {
        #region Constructors

        public DriverRepository()
        {
            this.ConnectionString = "poyConnection";
        }

        #endregion

        #region Methods
        public async Task<IDataReader> List(string sLocationCode)
        {
            var lstParameters = new List<IDbDataParameter>();

            using (var oSqlDataAdapter = new SqlDataAdapter(this.ConnectionString))
            {
                var oSqlParameter = oSqlDataAdapter.AddParameter("@LocationCode", DbType.String, sLocationCode, ParameterDirection.Input, 2 );
                lstParameters.Add(oSqlParameter);

                return await oSqlDataAdapter.QueryAsync("[ES].[usp_selDLog_ActiveRuns]", lstParameters);

            }
        }

        public async Task<IDataReader> List()
        {

            using (var oSqlDataAdapter = new SqlDataAdapter(this.ConnectionString))
            {

                return await oSqlDataAdapter.QueryAsync("[ES].usp_selDrivers_impersonate");

            }
        }
        #endregion
    }
}
