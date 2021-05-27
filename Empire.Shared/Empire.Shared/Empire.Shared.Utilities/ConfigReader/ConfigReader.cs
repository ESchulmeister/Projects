using System;
using System.Configuration;

namespace Empire.Shared.Utilities

{
    public class ConfigReader : IReader
    {
        #region Events
        #endregion

        #region Fields
        private static readonly ConfigReader s_oConfigReader = new ConfigReader();
        #endregion

        #region Properties
        public static ConfigReader Default
        {
            get
            {
                return s_oConfigReader;
            }
        }
        public static MandatoryConfigReader Mandatory
        {
            get
            {
                return MandatoryConfigReader.Current;
            }
        }

        #endregion

        #region Constructors
        protected ConfigReader()
        {
        }
        #endregion

        #region Event Procedures
        #endregion

        #region Methods

        #region IReader Members

        public virtual string Read(string sKey)
        {
            return this.Read(sKey, String.Empty);
        }
        public string Read(string sKey, string sDefault)
        {
            string sValue = ConfigurationManager.AppSettings[sKey];
            if (String.IsNullOrWhiteSpace(sValue))
            {
                sValue = this.HandleMissingEntry(sKey, sDefault);
            }
            return sValue;
        }
        public int Read(string sKey, int iDefault)
        {
            string sValue = this.Read(sKey, iDefault.ToString());
            int iValue = iDefault;

            return int.TryParse(sValue, out iValue) ? iValue : iDefault;
        }
        public bool Read(string sKey, bool bDefault)
        {
            string sValue = this.Read(sKey, bDefault.ToString());

            return sValue.ToBoolean(bDefault);
        }
        public double Read(string sKey, double dDefault)
        {
            string sValue = this.Read(sKey, dDefault.ToString());
            double dValue = dDefault;

            return double.TryParse(sValue, out dValue) ? dValue : dDefault;
        }

        public string[] Read(string sKey, params string[] aDelimiters)
        {
            string sValue = this.Read(sKey);

            if (String.IsNullOrWhiteSpace(sValue))
            {
                return new string[] { };
            }

            return sValue.Split(aDelimiters, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion

        protected virtual string HandleMissingEntry(string sKey, string sDefault = null)
        {
            return sDefault;
        }

        public string ReadConnectionString(string sKey)
        {
            var oConnectionString = ConfigurationManager.ConnectionStrings[sKey];
            if(oConnectionString == null)
            {
                this.HandleMissingEntry(sKey);
                return null;
            }

            return oConnectionString.ConnectionString; 
        }

        #endregion
    }

}
