using System;
using System.Drawing.Imaging;
using System.Globalization;
using System.Net;
using System.Text;
using System.Web;

namespace Empire.Shared.Utilities
{
    public static class Extensions
    {
        #region Methods

        #region Logging
    
        private static void LogError(this Exception oException, string sMethod, string sComponent = null)
        {
            oException = oException.Innermost();

            if (!String.IsNullOrWhiteSpace(sComponent))
            {
                sComponent = $"[{sComponent}]";
            }
            
            string sLogMessage = $"EXCEPTION {sComponent} {sMethod}: {oException.Message}; Stack Trace - {oException.StackTrace}";

            WriteEventLogEntry(sLogMessage);
        }


        public static void Log(this Exception oException,  string sMethod = "", bool bStackTrace = true)
        {
            string sStackTrace = bStackTrace ? String.Format("\n {0}", oException.StackTrace) : String.Empty;

            AppLogger.info($"{sMethod} - {oException.Message.XssSanitize()};{sStackTrace.XssSanitize()}");

            //event log
            LogError(oException, sMethod);
        }

        public static void Log(this string sValue)
        {
            AppLogger.info(sValue.XssSanitize());
        }

        public static string XssSanitize(this string sInput)
        {
            if (String.IsNullOrWhiteSpace(sInput))
            {
                return String.Empty;
            }

            sInput = sInput.Replace(@"\0", " ");
            sInput = sInput.Replace(@"\r", String.Empty);
            sInput = sInput.Replace(@"\n", String.Empty);

            return WebUtility.HtmlEncode(sInput);
        }

        private static void WriteEventLogEntry(string sLogMessage)
        {
            var oEventLogAdapter = new EventLogAdapter();
            oEventLogAdapter.WriteErrorEntry(sLogMessage);
        }
        #endregion

        public static Exception Innermost(this Exception oException)
        {
            Exception oInnerException = oException.InnerException;

            return (oInnerException == null) ? oException : oInnerException.Innermost();
        }

        #region Type Conversions
        public static DateTime ToDateTime(this string sValue, DateTime dtDefault)
        {
            if (String.IsNullOrWhiteSpace(sValue))
            {
                return dtDefault;
            }

            DateTime dtOut = dtDefault;

            DateTimeStyles eDateTimeStyles = DateTimeStyles.AssumeLocal;

            bool bDate = DateTime.TryParse(sValue, CultureInfo.GetCultureInfo("en-US"), eDateTimeStyles, out dtOut);
            if (!bDate)
            {
                bDate = DateTime.TryParse(sValue, CultureInfo.GetCultureInfo("en-GB"), eDateTimeStyles, out dtOut);
            }
            return bDate ? dtOut : dtDefault;

        }
        public static bool ToBoolean(this string sValue, bool bDefault)
        {
            if (String.IsNullOrWhiteSpace(sValue))
            {
                return bDefault;
            }

            bool bValue = bDefault;
            return bool.TryParse(sValue, out bValue) ? bValue : bDefault;
        }
        public static int ToInt32(this string sValue, int iDefault)
        {
            if (String.IsNullOrWhiteSpace(sValue))
            {
                return iDefault;
            }

            int iValue = iDefault;
            return int.TryParse(sValue, out iValue) ? iValue : iDefault;
        }
        public static decimal ToDecimal(this string sValue, decimal dDefault)
        {
            if (String.IsNullOrWhiteSpace(sValue))
            {
                return dDefault;
            }

            decimal dValue = dDefault;
            return decimal.TryParse(sValue, out dValue) ? dValue : dDefault;
        }
        #endregion

        public static string BaseUrl(this Uri oUri)
        {
            return $"{oUri.Scheme}://{oUri.Authority}{oUri.LocalPath}";
        }

        public static void Expire(this HttpCookie oHttpCookie)
        {
            var oHttpContext = HttpContext.Current;
            if(oHttpContext  == null)
            {
                return;
            }
            
            oHttpCookie.Expires = DateTime.Now.AddDays(-1d);
            oHttpContext.Response.Cookies.Add(oHttpCookie);
        }

        public static string Base64Encode(this string sPlainText)
        {
            var aPlainTextBytes = Encoding.UTF8.GetBytes(sPlainText);
            return Convert.ToBase64String(aPlainTextBytes);
        }
        public static string Base64Decode(this string sBase64EncodedData)
        {
            byte[] aBase64EncodedBytes = Convert.FromBase64String(sBase64EncodedData);
            return Encoding.UTF8.GetString(aBase64EncodedBytes);
        }
        public static void SendEmail(this string sEmailBody, bool bSubject = true)
        {
            var oSmtpAdapter = new SmtpAdapter();

            string sSubject = AppSettings.Current.Mail.Subject;

            if (String.IsNullOrWhiteSpace(sSubject))
            {
                sSubject = "Application Error";
            }

            oSmtpAdapter.SendEmail(sEmailBody, sSubject);
        }

        public static void Process(this Exception oException , string sMethod = null, bool bStackTrace = true, bool bLog = true)
        {
            const string cBodyPrefix = "Application Error";

            if (bLog)
            {
                oException.Log(sMethod);
            }

            string sStackTrace = bStackTrace ? $"<br> {oException.StackTrace}" : String.Empty;

            string sBody = $"{cBodyPrefix}:<br>{oException.Message} {sStackTrace}";

            sBody.SendEmail();

        }

        public static byte[] ToPngBytes(this byte[] aBytes)
        {
            var oImageAdapter = new ImageAdapter(ImageFormat.Png);
            return oImageAdapter.Convert(aBytes);
        }
        #endregion

    }
}
