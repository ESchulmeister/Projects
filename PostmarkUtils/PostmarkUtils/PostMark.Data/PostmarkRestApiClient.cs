using System;
using System.Net.Http;
using Empire.Shared.Utilities;

namespace PostMark.Data
{
    public class PostmarkRestApiClient : RestApiClient
    {
        #region Constants
        #endregion

        #region Properties
        public Guid AppToken
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        public PostmarkRestApiClient(Guid guidAppToken) : base()
        {
            this.BaseUrl = AppSettings.Current.PostmarkBaseUrl;
            this.AppToken = guidAppToken;
        }
        #endregion

        #region Methods

        public string GetBounces(int iMaxValue)
        {
            const int MaxBounces = 500;

            if(iMaxValue == 0 || iMaxValue > MaxBounces)
            {
                throw new ArgumentException($"Invalid maxValue - {iMaxValue.ToString()}");
            }
            
            string sUrl = $"{this.BaseUrl}/bounces?count={iMaxValue.ToString()}&offset=0";

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

        protected override void SetHeaders(HttpClient oHttpClient)
        {
            base.SetHeaders(oHttpClient);

            oHttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            oHttpClient.DefaultRequestHeaders.Add("X-Postmark-Server-Token", this.AppToken.ToString());
        }

        #endregion
    }
}
