using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Empire.Shared.Data
{
    public class SqlDataAdapter : ISqlDataAdapter
    {
        #region Constants
        #endregion

        #region Events
        #endregion

        #region Fields
        protected SqlConnection m_oSqlConnection;
        #endregion

        #region Delegates
        #endregion

        #region Properties
        public string ConnectionString
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        public SqlDataAdapter(string sConnectionString)
        {
            this.ConnectionString = sConnectionString;
        }
        #endregion

        #region Methods
        public List<IDbDataParameter> CreateParameterList()
        {
            return new List<IDbDataParameter>();
        }

        public async Task<IDataReader> QueryAsync(string sSql, List<IDbDataParameter> lstParameters = null)
        {
            IDataReader oDataReader = null;

            SqlConnection oConnection = this.InitConnection();                                          // since IDataReader is returned, cannot close the connection here
            using (SqlCommand oSqlCommand = await this.InitCommand(oConnection, sSql, lstParameters))
            {
                oDataReader = await oSqlCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }
            return oDataReader;
        }

        public async Task ExecuteNonQueryAsync(string sSql, List<IDbDataParameter> lstParameters = null)
        {
            try
            {
                using (SqlConnection oConnection = this.InitConnection())
                {
                    using (SqlCommand oSqlCommand = await this.InitCommand(oConnection, sSql, lstParameters))
                    {
                        await oSqlCommand.ExecuteNonQueryAsync();
                    }
                }

            }
            catch (Exception)
            {
                // logging
                throw;
            }
        }

        private async Task<SqlCommand> InitCommand(SqlConnection oConnection, string sSql, List<IDbDataParameter> lstParameters)
        {
            SqlCommand oSqlCommand = new SqlCommand(sSql, oConnection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            if (lstParameters != null)
            {
                oSqlCommand.Parameters.AddRange(lstParameters.ToArray());
            }

            await oConnection.OpenAsync();

            return oSqlCommand;
        }

        private SqlConnection InitConnection()
        {
            if (String.IsNullOrWhiteSpace(this.ConnectionString))
            {
                throw new MissingMemberException("Data Factory", "Connection String");
            }
            ConnectionStringSettings oConnectionStringSettings = ConfigurationManager.ConnectionStrings[this.ConnectionString];
            if (oConnectionStringSettings == null || String.IsNullOrWhiteSpace(oConnectionStringSettings.ConnectionString))
            {
                throw new ConfigurationErrorsException(String.Format("Connection string '{0}' not found", this.ConnectionString));
            }

            return new SqlConnection(oConnectionStringSettings.ConnectionString);
       }

        public void Dispose()
        {
        }

        public IDbDataParameter AddParameter(string sName, DbType enumDbType, object oValue = null, ParameterDirection enumDirection = ParameterDirection.Input, int iSize = 0)
        {
            SqlParameter oSqlParameter = new SqlParameter(sName, oValue)
            {
                DbType = enumDbType,
                Direction = enumDirection
            };

            if (iSize > 0)
            {
                oSqlParameter.Size = iSize;
            }

            return oSqlParameter;
        }

        #endregion

        #region Nested Classes
        #endregion

    }
}
