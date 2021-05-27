using System;
using System.Data;

namespace Empire.Shared.Business
{
    public class Location
    {
        #region Properties
        public string Code
        {
            get;
            protected set;
        }
        public string Description
        {
            get;
            protected set;
        }

        #endregion

        #region Constructors
        public Location()
        {

        }

        public Location(IDataReader oDataReader)
        {
            this.Load(oDataReader);
        }
        #endregion

        #region Methods
        protected virtual void Load(IDataReader oDataReader)
        {
            this.Code = oDataReader.ReadColumn("Loc_Cd", String.Empty);
            this.Description = oDataReader.ReadColumn("Loc_Desc", String.Empty);
        }

        public override string ToString()
        {
            return this.Code;
        }
        #endregion
    }
}
