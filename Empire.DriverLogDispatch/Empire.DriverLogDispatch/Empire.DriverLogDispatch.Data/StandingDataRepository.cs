using System.Data;
using System.Threading.Tasks;
using Empire.Shared.Data;

namespace Empire.DriverLog.Data
{
    public class StandingDataRepository : Repository
    {
        #region Constructors

        public StandingDataRepository()
        {
            this.ConnectionString = "poyConnection";
        }

        #endregion

        #region Methods
        public async Task<IDataReader> ListLocations()
        {
            using (var oSqlDataAdapter = new SqlDataAdapter(this.ConnectionString))
            {
                return await oSqlDataAdapter.QueryAsync("[ES].[usp_selLocations]");
            }
        }


        public async Task<IDataReader> ListStatuses()
        {
            using (var oSqlDataAdapter = new SqlDataAdapter(this.ConnectionString))
            {
                return await oSqlDataAdapter.QueryAsync("[ES].[usp_selStatuses]");
            }
        }
        #endregion
    }
}
