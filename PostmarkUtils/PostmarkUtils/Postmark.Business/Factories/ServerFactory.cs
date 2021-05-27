using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
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

        public async Task RecordBounces(Server oServer, StreamWriter oWriter)
        {
            var oPostmarkRestApiClient = new PostmarkRestApiClient(oServer.APIToken);

            var sBounceJson = oPostmarkRestApiClient.GetBounces(oServer.MaxBounces);

            JToken oJToken = JToken.Parse(sBounceJson);


            var lstBouncedMails = new List<BouncedMail>();
            foreach(var oChildJToken in oJToken.SelectToken("Bounces"))
            {
                lstBouncedMails.Add(new BouncedMail(oChildJToken));
            }

            int iTotalBounces = lstBouncedMails.Count();
            DateTime? dtLatestBounce = lstBouncedMails.Any() ? lstBouncedMails.Max(oBouncedMail => oBouncedMail.BounceDate) : (DateTime?)null;

            string sTimestamp = dtLatestBounce.HasValue ? $"; latest @ {dtLatestBounce.Value.ToDateFormatted()}" : String.Empty;
            string sLog = $"  -  Server: {oServer.Name}|| # {iTotalBounces.ToString("#,##0")} bounces{sTimestamp}";

            Console.WriteLine(sLog);

            oWriter.WriteLine(sLog);

            var oServerRepository = new ServerRepository();
            lstBouncedMails.ToList().ForEach(async oBouncedMail => await oServerRepository.RecordBounce(oServer.Key, oBouncedMail.MessageID, oBouncedMail.BounceType, oBouncedMail.Description, 
                                                                oBouncedMail.Detail, oBouncedMail.SendTo, oBouncedMail.SendFrom, oBouncedMail.BounceDate,oBouncedMail.Subject, oWriter));
        }

        public async Task UpdateLastRun(Server oServer, StreamWriter oWriter)
        {
            var oServerRepository = new ServerRepository();
            await oServerRepository.UpdateLastRun(oServer.Name, oWriter);

        }

    }
}
