using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Empire.Shared.Data
{
    public interface ISqlDataAdapter : IDisposable
    {
        string ConnectionString { get; set; }
        Task<IDataReader> QueryAsync(string sSql, List<IDbDataParameter> lstParameters = null);
        Task ExecuteNonQueryAsync(string sSql, List<IDbDataParameter> lstParameters = null);

        List<IDbDataParameter> CreateParameterList();
        IDbDataParameter AddParameter(string sName, DbType enumDbType, object oValue = null, ParameterDirection enumDirection = ParameterDirection.Input, int iSize = 0);
    }
}
