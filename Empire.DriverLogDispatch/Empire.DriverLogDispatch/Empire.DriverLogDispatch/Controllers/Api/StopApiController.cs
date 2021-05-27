using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Empire.DriverLog.Business;
using Empire.Shared.Utilities;

namespace Empire.DriverLog.Controllers
{
    [RoutePrefix("api/stop")]
    public class StopApiController : ApiController
    {
        #region Methods
        [HttpGet]
        [Route("")]
        // List of Stops
        //  REST API  - AJAX
        // ~/Scripts/Apps/HomeIndex.js - loadStops()
        public async Task<IHttpActionResult> Get([FromUri] int rid)
        {
            var oStopFactory = new StopFactory();

            List<Stop> lstStops = null;
            try
            {
                lstStops = await oStopFactory.List(rid);

            }
            catch (Exception oException)
            {
                oException.Log("Stop List");
                return this.InternalServerError();
            }

            return this.Ok(lstStops);
        }

        [HttpPut]
        [Route("")]
        // Update selected stops status
        // StopDto -  nested class - properties set up  @ ajax
        //  REST API - AJAX
        // ~/Scripts/Apps/HomeIndex.js - updateStop()
        public async Task Put([FromBody] StopDto oStopDto)
        {
            var oStop = new Stop()
            {
                Key = oStopDto.Key,
                Customer = new Customer() { ID = oStopDto.CustomerID },
                CurrentStatus = await StopStatus.Instance(oStopDto.Status)
            };
            
            var oStopFactory = new StopFactory();

            try
            {
               
                await oStopFactory.Update(oStop);

            }
            catch (Exception oException)
            {
                oException.Log("Update Stop");
                this.InternalServerError();
            }

            this.Ok();
        }

        #endregion

        #region Nested Classes

        /// <summary>
        /// Data Transformation Object - Stop re.
        /// </summary>
        public class StopDto
        {
            #region Properties
            public int Key
            {
                get; set;
            }
            public string CustomerID
            {
                get; set;
            }
            public string Status
            {
                get; set;
            }

            #endregion
        }



           
        }
        #endregion
    }
