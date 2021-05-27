using System;
using System.Net;
using System.Net.Http;

namespace Empire.Shared.Utilities
{
    public abstract class RestApiClient
    {
        #region Properties
        public string BaseUrl
        {
            get;
            set;
        }

        protected string UserID
        {
            get;
            set;
        }
        protected string Password
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        protected RestApiClient()
        {
        }
        protected RestApiClient(string sBaseUrlConfigKey)
        {
            this.BaseUrl = ConfigReader.Default.Read(sBaseUrlConfigKey);
        }
        #endregion

        #region Methods
        protected string Get(string sUrl)
        {
            string sResponse = null;

            using (var oHttpClientHandler = new HttpClientHandler()
            {
                Credentials = (String.IsNullOrWhiteSpace(this.UserID)) ? null : new NetworkCredential(this.UserID, this.Password)
            })
            {
                using (var oHttpClient = new HttpClient(oHttpClientHandler))
                {
                    this.SetHeaders(oHttpClient);

                    using (HttpResponseMessage oHttpResponseMessage = oHttpClient.GetAsync(sUrl).Result)
                    {
                        if (!oHttpResponseMessage.IsSuccessStatusCode)
                        {
                            throw new RestApiException(oHttpResponseMessage.StatusCode, oHttpResponseMessage.ReasonPhrase);
                        }


                        // by calling .Result you are performing a synchronous call
                        using (HttpContent oResponseHttpContent = oHttpResponseMessage.Content)
                        {
                            sResponse = oResponseHttpContent.ReadAsStringAsync().Result;
                        }
                    }
                }
            }

            return sResponse;
        }

        protected virtual void SetHeaders(HttpClient oHttpClient)
        {
            // nothing to do
        }
        #endregion

    }
}
