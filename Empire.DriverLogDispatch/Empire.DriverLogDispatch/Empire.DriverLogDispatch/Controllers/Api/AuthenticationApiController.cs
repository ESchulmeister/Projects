using System;
using System.Threading.Tasks;
using System.Web.Http;
using Empire.DriverLog.Business;
using Empire.Shared.Utilities;

namespace Empire.DriverLog.Api.Controllers
{
    [RoutePrefix("api/authentication")]
    public class AuthenticationController : ApiController
    {
        // GET: api/Authentication
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Get([FromUri] string uid, [FromUri] string pwd, [FromUri] bool remember)
        {
            string sUserID = Encryption.TripleDes.Decrypt(uid, Encryption.Keys.UserName);
            string sPassword = Encryption.TripleDes.Decrypt(pwd, Encryption.Keys.Password);

            User oUser = null;
            var oUserFactory = new UserFactory();


            try
            {
                oUser = await oUserFactory.Authenticate(sUserID, sPassword);

                await oUserFactory.RecordLogin(sUserID, (oUser != null));

            }
            catch(UnauthorizedUserException oUnauthorizedUserException)
            {
                oUnauthorizedUserException.Log();
                return this.Unauthorized();
            }
            catch (Exception oException)
            {
                oException.Log("Authentication");
                return this.InternalServerError();
            }

            if (oUser == null)
            {
                return this.NotFound();
            }

            string sLocation = (oUser.Location == null) ? null : oUser.Location.Code;

            bool bAdmin = (oUser is Administrator);

            Driver oDriver = oUser as Driver;
            string sDriverCode = (oDriver == null) ? string.Empty : oDriver.DriverCode;

            string sUserContext = String.Format("{0}{1}{2}{1}{3}{1}{4}{1}{5}{1}{6}{1}{7}", 
                                                            oUser.UserID.ToString(), Constants.Delimiters.AuthCookie, oUser.UserName, oUser.FirstName, oUser.LastName, bAdmin.ToString(), sDriverCode, sLocation);

            return this.Ok(sUserContext);
        }


    }
}
