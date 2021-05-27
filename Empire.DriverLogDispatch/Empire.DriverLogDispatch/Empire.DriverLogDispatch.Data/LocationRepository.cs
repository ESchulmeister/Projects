using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Empire.Shared.Data;

namespace Empire.DriverLog.Data
{
    public class LocationrRepository : Repository
    {
        #region Constructors

        public LocationrRepository()
        {
            this.ConnectionString = "poyConnection";
        }

        #endregion

        #region Methods
        //public async Task<IDataReader> List()
        //{
        //    using (var oSqlDataAdapter = new SqlDataAdapter(this.ConnectionString))
        //    {
        //        return await oSqlDataAdapter.QueryAsync("[ES].[usp_selLocations]");
        //    }
        //}
        #endregion

    }
}
