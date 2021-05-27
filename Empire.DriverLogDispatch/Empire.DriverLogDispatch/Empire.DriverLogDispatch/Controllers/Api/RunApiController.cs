using System;
using System.Threading.Tasks;
using System.Web.Http;
using Empire.DriverLog.Business;
using Empire.Shared.Utilities;

namespace Empire.RunLog.Controllers
{
    [RoutePrefix("api/run")]
    public class RunApiController : ApiController
    {
        #region Methods
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Get([FromUri] int ID, [FromUri] string drvCd)
        {
            Run oRun = null;

            try
            {
                if (ID > 0)
                {
                    var oRunFactory = new RunFactory();
                    oRun = await oRunFactory.Instance(ID);

                    string sCanBeCompleted = oRun.CanBeCompleted.HasValue ? oRun.CanBeCompleted.ToString() : "unknown";
                    $" Run - {oRun.Number};  can complete ? - {sCanBeCompleted}".Log();
                }
                else
                { 
                    if(!String.IsNullOrWhiteSpace(drvCd))
                    {
                        var oUserFactory = new UserFactory();
                        var oDriver = new Driver()
                        {
                            DriverCode = drvCd
                        };

                        oRun = await oUserFactory.GetRunInfo(oDriver);

                   }
                
                }

                if (oRun == null)
                {
                    return this.NotFound();
                }
            }
            catch (Exception oException)
            {
                oException.Log("Run by id");
                return this.InternalServerError();
            }

            return this.Ok(oRun);

        }

        [HttpPut]
        [Route("")]
        //REST API  call - AJAX
        ///~/Scripts/Apps/HomeIndex.js -updateRun()
        public async Task<IHttpActionResult> Put([FromBody] RunDto oRunDto)
        {
            try
            {
                var oRun = new Run()
                {
                    Number = oRunDto.Key,
                    Note = oRunDto.Note,
                    Signature = String.IsNullOrWhiteSpace(oRunDto.Signature) ? null : Convert.FromBase64String(oRunDto.Signature)
                };
                var oRunFactory = new RunFactory();

                await oRunFactory.Update(oRun);
            }
            catch (Exception oException)
            {
                oException.Log("Update run");
                return this.InternalServerError();
            }

            return this.Ok();
        }

        #endregion

        #region nested


        public class RunDto
        {
            #region Properties
            public int Key
            {
                get; set;
            }
            public string Note
            {
                get; set;
            }
            public string Signature
            {
                get; set;
            }


            #endregion
        }


        #endregion
    }
}
