using System;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Web;

namespace Salo
{
    public class SimulatorWebActionHandler : IActionHandler
    {
        private string _endpoint;
        private string _gameId;
        private string _playerId;
        public string Endpoint { get { return _endpoint; } }
        public string GameId { get { return _gameId; } }
        public string PlayerId { get { return _playerId; } }
        public SimulatorWebActionHandler(string endpoint, string gameId, string playerId)
        {
            _endpoint = endpoint;
            _gameId = gameId;
            _playerId = playerId;
        }

        private string ToQueryString(NameValueCollection nvc)
        {
            if (nvc == null)
            {
                return String.Empty;
            }

            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();
            return "?" + string.Join("&", array);
        }

        private const string StarIdParam = "starId";
        private const string UpgradeTypeParam = "upgradeType";
        private const string PlayerIdParam = "playerId";
        private const string OriginStarIdParam = "originStarId";
        private const string DestinationStarIdParam = "destinationStarId";
        private const string ShipsParam = "ships";
        private const string TechnologyParam = "technology";
        private const string HttpGet = "GET";
        private const string HttpPost = "POST";

        private T DoAction<T>(String action, NameValueCollection paramsCollection)
        {
            string fullUrl = string.Format("{0}/{1}/{2}{3}", Endpoint, GameId, action, ToQueryString(paramsCollection));

            System.Net.WebRequest oRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(fullUrl);
            oRequest.Method = HttpGet;
            T result = default(T);

            using (System.Net.HttpWebResponse oResponse = (System.Net.HttpWebResponse)oRequest.GetResponse())
            {
                var s = new DataContractJsonSerializer(typeof(T));
                var stream = oResponse.GetResponseStream();
                if (stream != null)
                {
                    result = (T) s.ReadObject(stream);
                }
            }

            return result;
        }

        private void DoAction(String action, NameValueCollection paramsCollection)
        {
            string fullUrl = string.Format("{0}/{1}/{2}{3}", Endpoint, GameId, action, ToQueryString(paramsCollection));

            System.Net.WebRequest oRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(fullUrl);
            oRequest.Method = HttpPost;

            System.Net.HttpWebResponse oResponse = (System.Net.HttpWebResponse) oRequest.GetResponse();
        }

        public void Upgrade(int starId, string upgradeType)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(StarIdParam, starId.ToString());
            nvc.Add(UpgradeTypeParam, upgradeType);
            DoAction("Upgrade", nvc);
        }

        public void BuildFleet(int starId)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(StarIdParam, starId.ToString());
            DoAction("BuildFleet", nvc);
        }

        public void Move(int originStarId, int destinationStarId, int ships)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(OriginStarIdParam, originStarId.ToString());
            nvc.Add(DestinationStarIdParam, destinationStarId.ToString());
            nvc.Add(ShipsParam, ships.ToString());
            DoAction("Move", nvc);
        }

        public void SetCurrentResearch(string research)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(PlayerIdParam, PlayerId);
            nvc.Add(TechnologyParam, research);
            DoAction("SetCurrentResearch", nvc);
        }

        public void SetNextResearch(string research)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(PlayerIdParam, PlayerId);
            nvc.Add(TechnologyParam, research);
            DoAction("SetNextResearch", nvc);
        }
    }
}
