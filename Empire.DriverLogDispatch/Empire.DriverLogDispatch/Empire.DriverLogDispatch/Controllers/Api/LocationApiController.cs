using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Empire.DriverLog.Business;
using Empire.Shared.Utilities;

namespace Empire.DriverLog.Controllers
{
    [RoutePrefix("api/location")]
    public class LocationApiController : ApiController
    {
        #region Methods
        [HttpGet]
        [Route("")]
        ///REST API  call - AJAX - List of locations
        ////~/Scripts/Apps/HomeIndex.js - loadLocations()
        public async Task<IHttpActionResult> Get()
        {
            var oLocationFactory = new StandingDataFactory();

            List<Location> lstLocations = null;
            try
            {
                lstLocations = await oLocationFactory.ListLocations();

            }
            catch (Exception oException)
            {
                oException.Log("Location List");
                return this.InternalServerError();
            }

            return this.Ok(lstLocations);
        }

        #endregion
    }
}
