using System;
using Newtonsoft.Json.Linq;
using Empire.Shared.Business;

namespace Postmark.Business
{
    public class BouncedMail
    {

        #region Properties
        
        public int ServerKey
        {
            get; set;
        }

        public Guid MessageID
        {
            get; set;
        }


        public string BounceType
        {
            get;   set;
        }

        public string Description
        {
            get; set;
        }

        public string Detail
        {
            get; set;
        }

        public string SendFrom
        {
            get; set;
        }

        public string SendTo
        {
            get; set;
        }

        public string Subject
        {
            get; set;
        }

        public DateTime BounceDate
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

            this.MessageID = oJToken.ReadValue("MessageID", Guid.Empty);
            this.BounceType = oJToken.ReadValue("BounceType");
            this.Description = oJToken.ReadValue("Description");
            this.Detail = oJToken.ReadValue("Details");
            this.SendTo = oJToken.ReadValue("Email");
            this.SendFrom = oJToken.ReadValue("From");
            this.BounceDate = oJToken.ReadValue("BouncedAt", DateTime.MinValue);
            this.Subject = oJToken.ReadValue("Subject");
        }
        #endregion
    }
}
