using System;
using Newtonsoft.Json.Linq;
using Empire.Shared.Business;

namespace Postmark.Business
{
    public class BouncedMail
    {

        #region Properties 

        public string Name
        {
            get;   set;
        }

        public string From
        {
            get; set;
        }

        public string To
        {
            get; set;
        }


        #endregion

        #region Constructors
        public BouncedMail() 
        {

        }

        public BouncedMail(JToken oJToken)
        {
            this.To = oJToken.ReadValue("Email", String.Empty);

            //this.UserName = oDataReader.ReadColumn<Guid>("UserName", Guid.NewGuid());
            //this.Password = oDataReader.ReadColumn<Guid>("Password", Guid.NewGuid());
            //this.APIToken = oDataReader.ReadColumn<Guid>("API_Token", Guid.NewGuid());
            //this.MaxBounces = oDataReader.ReadColumn("BounceCheck_Param", 0);
        }
        #endregion
    }
}
