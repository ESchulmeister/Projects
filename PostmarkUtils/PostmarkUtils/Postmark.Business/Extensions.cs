using System;

namespace Postmark.Business
{
    public static class Extensions
    {
        public static string ToDateFormatted(this DateTime dtArg)
        {
            return $"{dtArg.ToShortDateString()} {dtArg.ToLongTimeString()}";
        }
    }
}
