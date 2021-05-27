using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Empire.Shared.Data;
using Empire.Shared.Utilities;

namespace Empire.DriverLog.Data
{
    public class UserRepository : Repository
    {

        #region Constructors

        public UserRepository()
        {
            this.ConnectionString = "poyConnection";
        }

        #endregion

        #region Methods
        public async Task<IDataReader> Authenticate(string sUserID, string sPassword)
        {
            var lstParameters = new List<IDbDataParameter>();

            using (var oSqlDataAdapter = new SqlDataAdapter(this.ConnectionString))
            {
                var oSqlParameter = oSqlDataAdapter.AddParameter("@User_name", DbType.String, sUserID, ParameterDirection.Input, 25);
                lstParameters.Add(oSqlParameter);

                byte[] aPasswordBytes = Md5Hash.Current.Compute(sPassword);
                oSqlParameter = oSqlDataAdapter.AddParameter("@Password", DbType.Binary, aPasswordBytes, ParameterDirection.Input);
                lstParameters.Add(oSqlParameter);

                return await oSqlDataAdapter.QueryAsync("[ES].[usp_sel_GetAuthenticatedUser]", lstParameters);
            }
        }

        public async Task RecordLogin(string sUserID, bool bSuccess)
        {
            var lstParameters = new List<IDbDataParameter>();
            using (var oSqlDataAdapter = new SqlDataAdapter(this.ConnectionString))
            {
                var oSqlParameter = oSqlDataAdapter.AddParameter("@User_Name", DbType.String, sUserID, ParameterDirection.Input, 25);
                lstParameters.Add(oSqlParameter);

                oSqlParameter = oSqlDataAdapter.AddParameter("@Status", DbType.Boolean, bSuccess, ParameterDirection.Input);
                lstParameters.Add(oSqlParameter);

                await oSqlDataAdapter.ExecuteNonQueryAsync("[ES].[usp_ins_LoginHistory]", lstParameters);
            }
        }

        public async Task<IDataReader> GetRunInfo(string sDriverCode)
        {
            var lstParameters = new List<IDbDataParameter>();
            using (var oSqlDataAdapter = new SqlDataAdapter(this.ConnectionString))
            {
                var oSqlParameter = oSqlDataAdapter.AddParameter("@Drv_CD", DbType.String, sDriverCode, ParameterDirection.Input, 4);
                lstParameters.Add(oSqlParameter);

                return await oSqlDataAdapter.QueryAsync("[ES].[usp_sel_GetDriverInfo]", lstParameters);
            }
        }


        #endregion
    }
}
