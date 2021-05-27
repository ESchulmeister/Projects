using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Empire.Shared.Data;

namespace PostMark.Data
{
    public class ServerRepository : Repository
    {
        #region Constructors

        public ServerRepository()
        {
            this.ConnectionString = "_Connection";
        }

        #endregion

        #region Methods
        public async Task<IDataReader> List()
        {

            using (var oSqlDataAdapter = new SqlDataAdapter(this.ConnectionString))
            {
                return await oSqlDataAdapter.QueryAsync("[EmailFax].[uspPM_Get_Servers]");
            }
        }


        public async Task UpdateLastRun (string sServerKey, StreamWriter oLogStreamWriter)
        {
            var lstParameters = new List<IDbDataParameter>();

            using (var oSqlDataAdapter = new SqlDataAdapter(this.ConnectionString))
            {
                var oSqlParameter = oSqlDataAdapter.AddParameter("@sync_Type", DbType.StringFixedLength, sServerKey, ParameterDirection.Input, 10);
                lstParameters.Add(oSqlParameter);


                string sProc = "[Sync].[uspMonitor_Update_LastRun]";
                try
                {
                    await oSqlDataAdapter.ExecuteNonQueryAsync("[Sync].[uspMonitor_Update_LastRun]", lstParameters);
                }
                catch (Exception oException)
                {
                    oLogStreamWriter.WriteLine($"{sProc} - {oException.Message}");
                }

            }

        }
        /// <summary>
        /// Execute [EmailFax].[uspBounce_Insert]
        /// </summary>
        /// <param name="iServerKey"></param>
        /// <param name="guidMessageID"></param>
        /// <param name="sBounceType"></param>
        /// <param name="sBounceDescription"></param>
        /// <param name="sBounceDetails"></param>
        /// <param name="sSendTo"></param>
        /// <param name="sSendFrom"></param>
        /// <param name="dtBounce"></param>
        /// <param name="sSubject"></param>
        /// <returns></returns>

        public async Task RecordBounce(int iServerKey, Guid guidMessageID, string sBounceType, string sBounceDescription, string sBounceDetails,
                        string sSendTo, string sSendFrom, DateTime dtBounce, string sSubject, StreamWriter oLogStreamWriter)
        {

            var lstParameters = new List<IDbDataParameter>();

            using (var oSqlDataAdapter = new SqlDataAdapter(this.ConnectionString))
            {
                var oSqlParameter = oSqlDataAdapter.AddParameter("@PM_Server_FK", DbType.Int32, iServerKey, ParameterDirection.Input);
                lstParameters.Add(oSqlParameter);

                oSqlParameter = oSqlDataAdapter.AddParameter("@MessageID", DbType.Guid, guidMessageID, ParameterDirection.Input);
                lstParameters.Add(oSqlParameter);

                oSqlParameter = oSqlDataAdapter.AddParameter("@BounceType", DbType.String, sBounceType, ParameterDirection.Input);
                lstParameters.Add(oSqlParameter);

                oSqlParameter = oSqlDataAdapter.AddParameter("@BounceDescription", DbType.String, sBounceDescription, ParameterDirection.Input);
                lstParameters.Add(oSqlParameter);

                oSqlParameter = oSqlDataAdapter.AddParameter("@BounceDetails", DbType.String, sBounceDetails, ParameterDirection.Input);
                lstParameters.Add(oSqlParameter);

                oSqlParameter = oSqlDataAdapter.AddParameter("@SendTo", DbType.StringFixedLength, sSendTo, ParameterDirection.Input, 255);
                lstParameters.Add(oSqlParameter);

                oSqlParameter = oSqlDataAdapter.AddParameter("@SendFrom", DbType.StringFixedLength, sSendFrom, ParameterDirection.Input, 255);
                lstParameters.Add(oSqlParameter);

                oSqlParameter = oSqlDataAdapter.AddParameter("@Bounce_DT", DbType.DateTime, dtBounce, ParameterDirection.Input);
                lstParameters.Add(oSqlParameter);

                oSqlParameter = oSqlDataAdapter.AddParameter("@BounceSubject", DbType.String, sSubject, ParameterDirection.Input);
                lstParameters.Add(oSqlParameter);

                string sProc = "[EmailFax].[uspPMBounce_Insert]";
                try
                {
                    await oSqlDataAdapter.ExecuteNonQueryAsync("[EmailFax].[uspPM_Bounce_Insert]", lstParameters);

                    sProc = "[EmailFax].[uspPM_Update_Server_LastBounceCheck_DT]";
                    await UpdateServer(iServerKey);
                }
                catch(Exception oException)
                {
                    oLogStreamWriter.WriteLine($"{sProc} - {oException.Message}");
                }

            }

        }

        /// <summary>
        /// Exec [EmailFax].[uspUpdate_PM_Server_LastBounceCheck_DT]
        /// </summary>
        /// <param name="iServerKey"></param>
        private async Task UpdateServer(int iServerKey)
        {

            var lstParameters = new List<IDbDataParameter>();

            using (var oSqlDataAdapter = new SqlDataAdapter(this.ConnectionString))
            {
                var oSqlParameter = oSqlDataAdapter.AddParameter("@Server_PK", DbType.Int32, iServerKey, ParameterDirection.Input);
                lstParameters.Add(oSqlParameter);

                await oSqlDataAdapter.ExecuteNonQueryAsync("[EmailFax].[uspPM_Update_Server_LastBounceCheck_DT]", lstParameters);
            }
        }

        #endregion

    }

}