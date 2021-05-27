using System;
using System.Net;
using System.Web.Mvc;
using Empire.DriverLog.Business;
using Empire.Shared.Utilities;
using Newtonsoft.Json.Linq;

namespace Empire.DriverLog.Controllers
{
    public class HomeController : Controller
    {
        
        
        //Set Up base Url
        //VirtualFolder - app config setting, e.g DriverLog
        [Authorize]
        [HandleError]
        public ActionResult Index()
        {
            var oCurrUser = Application.Current.User;

            var oDriver = (oCurrUser as Driver);
            
            if(oDriver != null)
            {
                this.LoadRunNumber(oDriver);
            }

            ViewBag.BaseUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}/{AppSettings.Current.VirtualFolder}/";
            
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        private void LoadRunNumber(Driver oDriver)
        {
            const string ErrorKey = "LoadRunNumber";
            
            try
            {
                var oLogDispatchRestApiClient = new LogDispatchRestApiClient();

                string sRun = oLogDispatchRestApiClient.GetRun(oDriver);

                JObject oRunJObject = JObject.Parse(sRun);

                oDriver.CurrentRun = new Run(oRunJObject);

            }
            catch (RestApiException oRestApiException)
            {
                if(oRestApiException.StatusCode == HttpStatusCode.NotFound)
                {
                    return;
                }

                this.ModelState.AddModelError(ErrorKey, oRestApiException.Message);
            }
            catch(Exception oException)
            {
                this.ModelState.AddModelError(ErrorKey, oException.Message);
            }
        }

    }
}