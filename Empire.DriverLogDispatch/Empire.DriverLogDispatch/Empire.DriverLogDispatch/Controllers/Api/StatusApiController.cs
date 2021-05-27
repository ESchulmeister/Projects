using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Empire.DriverLog.Business;
using Empire.Shared.Utilities;

namespace Empire.DriverLog.Controllers.Api
{

    [RoutePrefix("api/status")]
    public class StatusApiController : ApiController
    {
        #region Methods
        [HttpGet]
        [Route("")]
        ///REST API  call - AJAX - List of statuses
        ////~/Scripts/Apps/HomeIndex.js - loadStatus()
        public async Task<IHttpActionResult> Get()
        {
            var oStatusFactory = new StandingDataFactory();

            List<StopStatus> lstStatuses = null;
            try
            {
                lstStatuses = await oStatusFactory.ListStopStatuses();

            }
            catch (Exception oException)
            {
                oException.Log("Status List");
                return this.InternalServerError();
            }

            return this.Ok(lstStatuses);
        }

        #endregion
    }
}
