using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PostMark.Data;

namespace Postmark.Business
{
    public class ServerFactory
    {
        public async Task<List<Server>> List()
        {
            var oServerRepository = new ServerRepository();

            IDataReader oDataReader = null;
            try
            {
                oDataReader = await oServerRepository.List();
            }
            catch (Exception)
            {
                throw;
            }

            var lstServers = new List<Server>();
            while (oDataReader.Read())
            {
                var oServer = new Server(oDataReader);
                lstServers.Add(oServer);
            }
            oDataReader.Close();

            return lstServers;
        }

        public async Task RecordBounces(Server oServer)
        {
            var oPostmarkRestApiClient = new PostmarkRestApiClient(oServer.APIToken);

            var sBounceJson = oPostmarkRestApiClient.GetBounces(oServer.MaxBounces);

            JToken oJToken = JToken.Parse(sBounceJson);

            var lstBouncedMails = new List<BouncedMail>();
            foreach(var oChildJToken in oJToken.SelectToken("Bounces"))
            {
                lstBouncedMails.Add(new BouncedMail(oChildJToken));
            }

            var oServerRepository = new ServerRepository();
            lstBouncedMails.ToList().ForEach(async oBouncedMail => await oServerRepository.RecordBounce(oBouncedMail.To));
        }
    }
}
