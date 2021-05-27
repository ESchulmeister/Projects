using Empire.Shared.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postmark.Business
{
    public class Server
    {

        #region properties 

        public string Name
        {
            get;   set;
        }

        public Guid UserName
        {
            get; set;
        }

        public Guid Password
        {
            get; set;
        }

        public Guid APIToken
        {
            get; set;
        }
        public int MaxBounces
        {
            get; set;
        }

        #endregion

        #region Constructors
        public Server() 
        {

        }

        public Server(IDataReader oDataReader)
        {
            this.Name = oDataReader.ReadColumn("ServerName");
            this.UserName = oDataReader.ReadColumn<Guid>("UserName", Guid.NewGuid());
            this.Password = oDataReader.ReadColumn<Guid>("Password", Guid.NewGuid());
            this.APIToken = oDataReader.ReadColumn<Guid>("API_Token", Guid.NewGuid());
            this.MaxBounces = oDataReader.ReadColumn("BounceCheck_Param", 0);
        }
        #endregion
    }
}
