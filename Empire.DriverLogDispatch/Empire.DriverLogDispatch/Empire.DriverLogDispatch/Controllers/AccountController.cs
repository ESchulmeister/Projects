using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Empire.DriverLog.Business;
using Empire.DriverLog.Models;
using Empire.Shared.Utilities;


namespace Empire.DriverLog.Controllers
{
    public class AccountController : Controller
    {
        #region Constants
        public const string LoginErrorKey = "LoginError";
        #endregion

        #region Methods
        [HttpGet]
        public ActionResult SignIn()
        {
            var oSignInModel = new SignInModel();
    

            //check Auth cookie, create one @ success login
            var oHttpCookie = Request.Cookies[Constants.Cookies.UserName];
            if(oHttpCookie != null)
            {
                oSignInModel.Username = oHttpCookie.Value;
            }

            return View(oSignInModel);
        }

        [HttpPost]
        [HandleError]
        public ActionResult SignIn(SignInModel oSignInModel)
        {
            string sUserID = oSignInModel.Username;
            string sPassword = oSignInModel.Password;
            bool bRemember = oSignInModel.WillRememberUser;

            string sUserContext = null;


            //POST - AccountController/SignIn
            try
            {
                sUserContext = this.AuthenticateUser(sUserID, sPassword, bRemember);

                $" #{sUserID} - logged in".Log();
            }
            catch (RestApiException oRestApiException)
            {
                this.HandleLoginRestApiException(oRestApiException, sUserID);
                return View();
            }
            catch (Exception oException)
            {
                this.ModelState.AddModelError(LoginErrorKey, oException);
                oException.Log("Login");
                return View();
            }
            
            //Set up Forms auth cookie w/ user context
            if (String.IsNullOrWhiteSpace(sUserContext))
            {
                this.ModelState.AddModelError(LoginErrorKey, "You are not a supervisor or driver");
                return View();
            }
            this.SetCookie(sUserContext, bRemember);

            return RedirectToAction("Index", "Home"); 
        }

        [HttpGet]
        public ActionResult SignOut()
        {
            HttpCookie oHttpAuthCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (oHttpAuthCookie != null)
            {
                oHttpAuthCookie.Expire();
            }

            return RedirectToAction("SignIn");
        }


        /// <summary>
        /// Set  FormsAuthentication - .ASPXAUTH cookie
        /// Cookie Name - username
        /// </summary>
        /// <param name="sUserContext"></param>
        /// <param name="bRemember"></param>
        protected void SetCookie(string sUserContext, bool bRemember)
        {
            sUserContext = sUserContext.Replace("\"", String.Empty).Trim();

            string[] aParts = sUserContext.Split(new string[] { Constants.Delimiters.AuthCookie }, StringSplitOptions.RemoveEmptyEntries);
            if(aParts.Length < 2)
            {
                $"Invalid User Context - {sUserContext}".Log();
                throw new ArgumentException($"Invalid User Context - {sUserContext}");
            }

            string sUserName = aParts[1];

            int iDefaultTimeout = (int)FormsAuthentication.Timeout.TotalMinutes;

            FormsAuthenticationTicket oFormsAuthenticationTicket =
                new FormsAuthenticationTicket(1, sUserName, DateTime.Now, DateTime.Now.AddMinutes(iDefaultTimeout), bRemember, sUserContext);

            string sCookieValue = FormsAuthentication.Encrypt(oFormsAuthenticationTicket);
            var oHttpCookie = new HttpCookie(FormsAuthentication.FormsCookieName, sCookieValue);
            if (bRemember)
            {
                oHttpCookie.Expires = oFormsAuthenticationTicket.Expiration;
            }
            oHttpCookie.Path = FormsAuthentication.FormsCookiePath;
            Response.Cookies.Add(oHttpCookie);

            if (bRemember)
            {
                var oHttpUserCookie = new HttpCookie(Constants.Cookies.UserName, sUserName)
                {
                    Path = "/"
                };
                Response.Cookies.Add(oHttpCookie);
            }

        }

        /// <summary>
        /// Messages @ Home/Index
        /// </summary>
        /// <param name="oRestApiException">Exception @ AccountController/Get</param>
        protected void HandleLoginRestApiException(RestApiException oRestApiException, string sUserID)
        {
            switch (oRestApiException.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    {
                        oRestApiException.Log($"Login - {sUserID}", bStackTrace: false);
                        this.ModelState.AddModelError(LoginErrorKey, "Invalid Username. Please try again.");
                        return;
                    }
                case HttpStatusCode.Unauthorized:
                    {
                        oRestApiException.Log($"Login - {sUserID}", bStackTrace: false);
                        this.ModelState.AddModelError(LoginErrorKey, "You are not a supervisor or driver");
                        return;
                    }
                case HttpStatusCode.BadRequest:
                    {
                        oRestApiException.Log($"Login - {sUserID}", bStackTrace: false);
                        this.ModelState.AddModelError(LoginErrorKey, "You are not a Driver - we will bring you to other Applications.");
                        return;
                    }
                default:
                    {
                        this.ModelState.AddModelError(LoginErrorKey, "A server error has occurred. Please contact System Support");
                        oRestApiException.Log();
                        return;
                    }
            }
        }

        /// <summary>
        /// REst API  - Authentication Api controller
        /// </summary>
        /// <param name="sUserID"></param>
        /// <param name="sPassword"></param>
        /// <param name="bRemember"></param>
        /// <returns></returns>
        protected string AuthenticateUser(string sUserID, string sPassword, bool bRemember)
        {
            string sContext = null;
            try
            {
                var oLogDispatchRestApiClient = new LogDispatchRestApiClient();

                sContext = oLogDispatchRestApiClient.AuthenticateUser(sUserID, sPassword, bRemember);

                return sContext;
            }
            catch (Exception oEx)
            {
                oEx.Log();
                throw;
            }

        }
        #endregion
    }
}