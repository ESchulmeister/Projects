using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Empire.Shared.Data;
using Empire.Shared.Utilities;

namespace Empire.DriverLog.Data
{
    public class RunRepository : Repository
    {
        #region Constructors

        public RunRepository()
        {
            this.ConnectionString = "poyConnection";
        }

        #endregion

        #region Methods
        public async Task<IDataReader> Update(int iRunNo, string sNote, byte[] aSignature)
        {
            var lstParameters = new List<IDbDataParameter>();

            using (var oSqlDataAdapter = new SqlDataAdapter(this.ConnectionString))
            {
                var oSqlParameter = oSqlDataAdapter.AddParameter("@Run_No", DbType.Int32, iRunNo, ParameterDirection.Input );
                lstParameters.Add(oSqlParameter);

                oSqlParameter = oSqlDataAdapter.AddParameter("@Note", DbType.String, sNote, ParameterDirection.Input );
                lstParameters.Add(oSqlParameter);

                oSqlParameter = oSqlDataAdapter.AddParameter("@Signature", DbType.Binary, aSignature, ParameterDirection.Input, -1);
                lstParameters.Add(oSqlParameter);


                return await oSqlDataAdapter.QueryAsync("[ES].usp_updDLog_RunHeader", lstParameters);
            }
        }

        public async Task<IDataReader> Get(int iRunNo)
        {
            var lstParameters = new List<IDbDataParameter>();

            using (var oSqlDataAdapter = new SqlDataAdapter(this.ConnectionString))
            {
                var oSqlParameter = oSqlDataAdapter.AddParameter("@Run_No", DbType.Int32, iRunNo, ParameterDirection.Input);
                lstParameters.Add(oSqlParameter);

                return await oSqlDataAdapter.QueryAsync("[ES].[usp_chkDLog_GetRunInfo]", lstParameters);
            }
        }
        #endregion
    }
}
