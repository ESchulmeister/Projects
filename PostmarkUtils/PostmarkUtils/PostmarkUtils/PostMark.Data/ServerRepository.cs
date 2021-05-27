using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Empire.Shared.Data;

namespace PostMark.Data
{
    public class ServerRepository : Repository
    {
        #region Constructors

        public ServerRepository()
        {
            this.ConnectionString = "DevDefaultConnection";
        }

        #endregion

        #region Methods
        public async Task<IDataReader> List()
        {

            using (var oSqlDataAdapter = new SqlDataAdapter(this.ConnectionString))
            {
                return await oSqlDataAdapter.QueryAsync("[EmailFax].[uspGet_PM_Servers]");
            }
        }

        public async Task RecordBounce(string sTo)
        {
        }
        #endregion

    }
}
