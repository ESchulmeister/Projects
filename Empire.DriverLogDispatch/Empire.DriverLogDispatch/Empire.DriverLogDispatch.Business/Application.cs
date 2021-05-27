using System;
using System.Web;
using System.Web.Security;
using Empire.Shared.Utilities;

namespace Empire.DriverLog.Business
{
    public class Application
    {
        #region Constants
        public const string UserKey = "Usr";
        #endregion

        #region Fields
        protected static readonly Lazy<Application> s_oApplication = new Lazy<Application>(() => new Application());
        #endregion

        #region Properties
        public static Application Current
        {
            get
            {
                return s_oApplication.Value;
            }
        }

        /// <summary>
        /// Currently Logged In user
        /// </summary>
        public User User   
        {
            get
            {
                var oHttpContext = HttpContext.Current;
                if(oHttpContext == null)
                {
                    return null;
                }

                User oUser = oHttpContext.Items[Application.UserKey] as User;
                if(oUser != null)
                {
                    return oUser;
                }

                HttpCookie oHttpAuthCookie = oHttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (oHttpAuthCookie == null)
                {
                    return null;
                }
                var oFormsAuthenticationTicket = FormsAuthentication.Decrypt(oHttpAuthCookie.Value);

                string[] aParts = oFormsAuthenticationTicket.UserData.Split(new string[] { Constants.Delimiters.AuthCookie }, StringSplitOptions.None);

                bool bAdmin = aParts[4].ToLower().ToBoolean(false);
                oUser = (bAdmin) ? new Administrator() as User : new Driver() as User;

                oUser.UserID = aParts[0].ToDecimal(0M);
                oUser.UserName = aParts[1].Trim();
                oUser.FirstName = aParts[2].Trim();
                oUser.LastName = aParts[3].Trim();
                oUser.Location = (aParts.Length < 7) ? null : new Location() { Code = aParts[6].Trim() };

                Driver oDriver = oUser as Driver;
                if(oDriver != null)
                {
                    oDriver.DriverCode = (aParts.Length < 6) ? "0" : aParts[5];
                }

                oHttpContext.Items[Application.UserKey] = oUser;
                
                return oUser;
            }
        }
        #endregion

        #region Constructors
        protected Application()
        {
        }
        #endregion
    }
}
