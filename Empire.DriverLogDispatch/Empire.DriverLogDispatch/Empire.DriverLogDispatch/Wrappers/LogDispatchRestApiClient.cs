using System;
using System.Text;
using System.Web;
using Empire.DriverLog.Business;
using Empire.Shared.Utilities;

namespace Empire.DriverLog
{
    public class LogDispatchRestApiClient : RestApiClient
    {
        #region Constants
        #endregion

        #region Properties
        protected string ApiUrl
        {
            get
            {
                var oUri = HttpContext.Current.Request.Url;

                var sbUrl = new StringBuilder(oUri.Scheme);
                sbUrl.AppendFormat("://{0}/{1}", oUri.Authority, AppSettings.Current.VirtualFolder);

                if(!String.IsNullOrWhiteSpace(AppSettings.Current.VirtualFolder))
                {
                    sbUrl.Append("/");
                }

                sbUrl.Append("api");

                return sbUrl.ToString();
            }
        }
        #endregion

        #region Constructors
        public LogDispatchRestApiClient()
        {
            this.BaseUrl = HttpContext.Current.Request.Url.BaseUrl();
        }
        #endregion

        #region Methods

        public string AuthenticateUser(string sUserID, string sPassword, bool bRemember)
        {
            sUserID = HttpUtility.UrlEncode(Encryption.TripleDes.Encrypt(sUserID, Encryption.Keys.UserName));
            sPassword = HttpUtility.UrlEncode(Encryption.TripleDes.Encrypt(sPassword, Encryption.Keys.Password));

            string sUrl = $"{HttpContext.Current.Request.Url.BaseUrl()}api/authentication?uid={sUserID}&pwd={sPassword}&remember={bRemember}";

            return this.Get(sUrl);
        }
        public string GetRun(Driver oDriver)
        {
            if(oDriver == null)
            {
                throw new ArgumentNullException("Driver");
            }
            string sUrl = $@"{this.ApiUrl}/run?ID=0&drvCd={oDriver.DriverCode}";

            $"GetRun URL: {sUrl}".Log();

            return this.Get(sUrl);
        }


        protected string FormatApiUrl(string sMethodUrl, params string[] aParams)
        {
            if (String.IsNullOrWhiteSpace(sMethodUrl))
            {
                return null;
            }

            string sApiUrl = null; // oConfigReader.Read(IntegrationApiKey, String.Empty);

            return String.Concat(this.BaseUrl, sApiUrl, sMethodUrl);
        }

        #endregion
    }
}
