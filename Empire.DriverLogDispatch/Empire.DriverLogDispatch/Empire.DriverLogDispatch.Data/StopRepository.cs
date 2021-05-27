using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Empire.Shared.Data;

namespace Empire.DriverLog.Data
{
    public class StopRepository : Repository
    {
        #region Constructors

        public StopRepository()
        {
            this.ConnectionString = "poyConnection";
        }

        #endregion

        #region Methods
        public async Task<IDataReader> List(int iRunID)
        {
            var lstParameters = new List<IDbDataParameter>();

            using (var oSqlDataAdapter = new SqlDataAdapter(this.ConnectionString))
            {
                var oSqlParameter = oSqlDataAdapter.AddParameter("@RunNumber", DbType.Int32, iRunID, ParameterDirection.Input);
                lstParameters.Add(oSqlParameter);

                return await oSqlDataAdapter.QueryAsync("[ES].[usp_selDriverStops]", lstParameters);
            }
        }


       public async Task Update(int iRunID, string sCustomerID, string sStatus)
       {
            var lstParameters = new List<IDbDataParameter>();

            using (var oSqlDataAdapter = new SqlDataAdapter(this.ConnectionString))
            {
                var oSqlParameter = oSqlDataAdapter.AddParameter("@Key", DbType.Int32, iRunID, ParameterDirection.Input);
                lstParameters.Add(oSqlParameter);

                oSqlParameter = oSqlDataAdapter.AddParameter("@Cust_ID", DbType.StringFixedLength, sCustomerID, ParameterDirection.Input, 8);
                lstParameters.Add(oSqlParameter);

                oSqlParameter = oSqlDataAdapter.AddParameter("@Status", DbType.StringFixedLength, sStatus, ParameterDirection.Input, 15);
                lstParameters.Add(oSqlParameter);

                await oSqlDataAdapter.ExecuteNonQueryAsync("[ES].[usp_updStop]", lstParameters);
            }
        }
        #endregion

    }
}
