using System;
using System.Data;
using Newtonsoft.Json.Linq;

namespace Empire.Shared.Business
{
    public static class Extensions
    {
        #region IDataReader
        public static string ReadColumn(this IDataReader oIDataReader, string sName)
        {
            return oIDataReader.ReadColumn(sName, String.Empty);
        }
        public static string ReadColumn(this IDataReader oIDataReader, string sName, string sDefault)
        {
            return oIDataReader.ReadColumn<string>(sName, sDefault);
        }
        public static int ReadColumn(this IDataReader oIDataReader, string sName, int iDefault)
        {
            return oIDataReader.ReadColumn<int>(sName, iDefault);
        }
        public static bool ReadColumn(this IDataReader oIDataReader, string sName, bool bDefault)
        {
            return oIDataReader.ReadColumn<bool>(sName, bDefault);
        }
        public static bool? ReadColumn(this IDataReader oIDataReader, string sName, bool? bDefault)
        {
            return oIDataReader.ReadColumn<bool?>(sName, bDefault);
        }
        public static DateTime ReadColumn(this IDataReader oIDataReader, string sName, DateTime dtDefault)
        {
            return oIDataReader.ReadColumn<DateTime>(sName, dtDefault);
        }
        public static decimal ReadColumn(this IDataReader oIDataReader, string sName, decimal dDefault)
        {
            return oIDataReader.ReadColumn<decimal>(sName, dDefault);
        }
        public static double ReadColumn(this IDataReader oIDataReader, string sName, double dDefault)
        {
            return oIDataReader.ReadColumn<double>(sName, dDefault);
        }
        public static float ReadColumn(this IDataReader oIDataReader, string sName, float fDefault)
        {
            return oIDataReader.ReadColumn<float>(sName, fDefault);
        }

        public static T ReadColumn<T>(this IDataReader oIDataReader, string sName, T oDefault)
        {
            if (!oIDataReader.HasColumn(sName))
            {
                return oDefault;
            }

            return Extensions.ReadColumnValue<T>(oIDataReader[sName], oDefault);
        }
        public static T ReadColumnValue<T>(object oColValue, T oDefault)
        {
            if (oColValue == null || oColValue == System.DBNull.Value)
            {
                return oDefault;
            }

            T oReturnValue;
            try
            {
                oReturnValue = (T)oColValue;
            }
            catch (Exception)
            {
                return oDefault;
            }

            return oReturnValue;
        }
        public static bool HasColumn(this IDataReader oIDataReader, string sName)
        {
            for (int iColumn = 0; iColumn < oIDataReader.FieldCount; iColumn++)
            {
                string sColumnName = oIDataReader.GetName(iColumn);
                if (String.Compare(sColumnName, sName, true) == 0)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region JToken
        public static string ReadValue(this JToken oJToken, string sName)
        {
            return oJToken.ReadValue(sName, String.Empty);
        }
        public static string ReadValue(this JToken oJToken, string sName, string sDefault)
        {
            JToken oValueJToken = oJToken.SelectToken(sName);
            return (oValueJToken == null) ? sDefault : oValueJToken.Value<string>();
        }
        public static int ReadValue(this JToken oJToken, string sName, int iDefault)
        {
            string sValue = oJToken.ReadValue(sName, iDefault.ToString());

            int iValue = iDefault;
            return int.TryParse(sValue, out iValue) ? iValue : iDefault;
        }
        public static DateTime ReadValue(this JToken oJToken, string sName, DateTime dtDefault)
        {
            string sValue = oJToken.ReadValue(sName, dtDefault.ToString());

            DateTime dtValue = dtDefault;
            return DateTime.TryParse(sValue, out dtValue) ? dtValue : dtDefault;
        }

        public static Guid ReadValue(this JToken oJToken, string sName, Guid guidDefault)
        {
            string sValue = oJToken.ReadValue(sName, guidDefault.ToString());

            Guid guidValue = guidDefault;
            return Guid.TryParse(sValue, out guidValue) ? guidValue : guidDefault;
        }

        #endregion
    }
}
