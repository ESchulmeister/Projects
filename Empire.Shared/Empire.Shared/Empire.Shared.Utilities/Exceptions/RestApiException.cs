using System;
using System.Net;

namespace Empire.Shared.Utilities
{
    public class RestApiException : Exception
    {
        #region Properties
        public HttpStatusCode StatusCode
        {
            get;
            protected set;
        }
        #endregion



        #region Constructors
        public RestApiException() : this(HttpStatusCode.InternalServerError, "A REST API Exception has occurred")
        {

        }
        public RestApiException(HttpStatusCode eHttpStatusCode, string sMessage) : base(sMessage)
        {
            this.StatusCode = eHttpStatusCode;
        }
        #endregion
    }
}
