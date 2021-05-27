using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Empire.DriverLog.Business;
using Empire.Shared.Utilities;

namespace Empire.DriverLog.Controllers.Api
{
    [RoutePrefix("api/driver")]
    public class DriverApiController : ApiController
    {
        #region Methods
        [HttpGet]
        [Route("")]

        //REST API  call - AJAX
        ///~/Scripts/Apps/HomeIndex.js - loadDrivers()/loadImpersonate()
        //Param - Location_CD
        public async Task<IHttpActionResult> Get([FromUri] string locCode = null)
        {
            var oDriverFactory = new DriverFactory();

            List<Driver> lstDrivers = null;
            try
            {

                if (!string.IsNullOrWhiteSpace(locCode))
                {
                    lstDrivers = await oDriverFactory.ListDrivers(locCode);
                }
                else
                {
                    lstDrivers = await oDriverFactory.ListDrivers();
                }

            }
            catch (Exception oException)
            {
                oException.Log("Driver List");
                return this.InternalServerError();
            }

            return this.Ok(lstDrivers);
        }



  
        #endregion
    }
}
