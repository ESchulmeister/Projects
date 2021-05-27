using System;
using System.Data;
using System.Threading.Tasks;
using Empire.DriverLog.Data;
using Empire.Shared.Business;
using Empire.Shared.Utilities;

namespace Empire.DriverLog.Business
{
    public class UserFactory
    {
        #region Constants
        public const string DriverApplicationCode = "DRV_RUN";
        #endregion

        #region Methods
        /// <summary>
        /// AuthenticationApiController/Get
        /// </summary>
        /// <param name="sUserID"></param>
        /// <param name="sPassword"></param>
        /// <returns></returns>
        public async Task<User> Authenticate(string sUserID, string sPassword)
        {
            var oUserRepository = new UserRepository();

   
            IDataReader oDataReader = null;
            try
            {
                oDataReader = await oUserRepository.Authenticate(sUserID, sPassword);
            }
            catch(Exception oException)
            {
                oException.Log();
                throw;
            }

            if(!oDataReader.Read())
            {
                return null;
            }

            bool bAdmin = false;

            bAdmin = oDataReader.ReadColumn("DLog_Super", false);
            string sDriverCode = oDataReader.ReadColumn("Drv_CD", string.Empty);
            string sLocationCode = oDataReader.ReadColumn("Loc_CD");


            User oUser = null;
            if (bAdmin)
            {
                oUser = new Administrator(oDataReader);
            }
            else
            {
                if(String.IsNullOrWhiteSpace(sDriverCode))
                {
                    throw new UnauthorizedUserException();
                }

                oUser =  new Driver(oDataReader);
            }

            if (oUser != null)
            {

                oUser.Location = await Location.Instance(sLocationCode);
            }

            oDataReader.Close();
            oDataReader.Dispose();

             
            return oUser;
        }

     
        /// <summary>
        /// Run info (#, truck no) per current driver
        /// </summary>
        /// <param name="oDriver"></param>
        /// <returns></returns>
        public async Task<Run> GetRunInfo(Driver oDriver)
        {
            var oUserRepository = new UserRepository();
            IDataReader oDataReader = null;
            try
            {
                oDataReader = await oUserRepository.GetRunInfo(oDriver.DriverCode);
            }
            catch (Exception)
            {
                throw;
            }

            if(oDataReader.Read())
            {
                oDriver.CurrentRun = new Run(oDataReader);
            }

            return oDriver.CurrentRun;
        }

        /// <summary>
        /// Record user Login
        /// </summary>
        /// <param name="sUserID">UserName/email</param>
        /// <param name="bSuccess">bool- success/failure</param>
        /// <returns></returns>
        public async Task RecordLogin(string sUserID, bool bSuccess)
        {
            var oUserRepository = new UserRepository();

            await oUserRepository.RecordLogin(sUserID, bSuccess);
        }


     
        #endregion
    }
}
